using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    // EventModel is per-type and per-object; there is one EventModel for static events of a type 
    // and one EventModel for each instance of an object.  This seems wasteful and I would be open
    // to finding a resolution that allows one EventModel for all instances, but I can't wrap my head around
    // how to go from a static invokee (InvokeEvent) to instance-event without multiple sets of constants, 
    // which just seems really involved.

    internal class EventModel
    {
        private static volatile int CurrentCookie;
        private static Dictionary<int, WeakReference<EventModel>> EventModels;

        private class EventRegistration
        {
            public EventRegistration(JavaScriptFunction invokee)
                : this(invokee, SynchronizationContext.Current)
            {

            }

            public EventRegistration(JavaScriptFunction invokee, SynchronizationContext syncContext)
            {
                if (invokee == null)
                    throw new ArgumentNullException(nameof(invokee));

                //Cookie = Interlocked.Increment(ref CurrentCookie);
                Invokee = new WeakReference<JavaScriptFunction>(invokee);
                TargetContext = syncContext;
            }

            //public readonly int Cookie;
            public readonly WeakReference<JavaScriptFunction> Invokee;
            public readonly SynchronizationContext TargetContext;
        }

        /*
        .addEventListener('eventname', function() { ... });
        function() { ... } has a unique handle, identified by Invokee.Target.handle_

        .removeEventListener('eventname', function() { ... })
        The handle will be there.  Do we even need a cookie?
    */
        private int cookie_;
        private object target_;
        private bool static_;
        private Dictionary<string, EventInfo> events_;
        private Dictionary<string, List<EventRegistration>> registrations_;
        private Dictionary<string, Delegate> marshalers_;

        private JavaScriptCallableFunction add_, remove_, set_;

        public EventModel(EventInfo[] events, object target)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            events_ = events.ToDictionary(e => e.Name.ToLower());
            static_ = (target == null);
            target_ = target;
            registrations_ = new Dictionary<string, List<EventRegistration>>();
            foreach (var key in events_.Keys)
            {
                registrations_.Add(key.ToLower(), new List<EventRegistration>());
            }

            Initialize();

            cookie_ = Interlocked.Increment(ref CurrentCookie);
            EventModels.Add(cookie_, new WeakReference<EventModel>(this));
        }

        private void Initialize()
        {
            add_ = (engine, construct, thisObj, arguments) =>
            {
                // todo: support inheritance

                // if there aren't enough args, return undefined
                var args = arguments.ToArray();
                if (args.Length < 2)
                    return engine.UndefinedValue;

                // if there's no matching event, return undefined
                string eventName = args[0].ToString().ToLower();
                List<EventRegistration> registeredHandlers;
                if (!registrations_.TryGetValue(eventName, out registeredHandlers))
                    return engine.UndefinedValue;

                // if args[1] isn't a function, return undefined
                JavaScriptFunction fn = args[1] as JavaScriptFunction;
                if (fn == null)
                    return engine.UndefinedValue;

                // if we don't know about the event
                EventInfo targetEvent;
                if (!events_.TryGetValue(eventName, out targetEvent))
                    return engine.UndefinedValue;

                // if 'this' isn't an object with an external pointer of the same type as the event's declaring type, return undefined
                JavaScriptObject jsObj = null;
                object external = null;
                if (!static_)
                {
                    jsObj = thisObj as JavaScriptObject;
                    if (jsObj == null)
                        return engine.UndefinedValue;
                    external = jsObj.ExternalObject;
                    if (external == null)
                        return engine.UndefinedValue;
                    if (!targetEvent.DeclaringType.IsAssignableFrom(external.GetType()))
                        return engine.UndefinedValue;
                }

                Delegate marshaler;
                if (!marshalers_.TryGetValue(eventName, out marshaler))
                {
                    lock (marshalers_)
                    {
                        if (!marshalers_.TryGetValue(eventName, out marshaler))
                        {
                            var invoke = targetEvent.EventHandlerType.GetMethod("Invoke");
                            Debug.Assert(invoke != null);
                            var paramsExpr = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

                            var marshalExpr = Expression.Lambda(targetEvent.EventHandlerType, Expression.Block(
                                Expression.Call(
                                    typeof(EventModel).GetMethod(nameof(EventModel.InvokeEvent)),
                                    Expression.Constant(cookie_),
                                    Expression.Constant(eventName),
                                    Expression.NewArrayInit(typeof(string), invoke.GetParameters().Select(p => Expression.Constant(p.Name))),
                                    Expression.NewArrayInit(typeof(object), paramsExpr))
                                )
                            );
                            marshaler = marshalExpr.Compile();
                            marshalers_.Add(eventName, marshaler);
                        }
                    }
                }

                lock (registeredHandlers)
                {
                    if (registeredHandlers.Count == 0)
                    {
                        targetEvent.AddMethod.Invoke(external, new object[] { marshaler });
                    }

                    var reg = new EventRegistration(fn);
                    registeredHandlers.Add(reg);
                }

                return engine.UndefinedValue;
            };

            remove_ = (engine, construct, thisVal, arguments) =>
            {
                // todo: support inheritance

                // if there aren't enough args, return undefined
                var args = arguments.ToArray();
                if (args.Length < 2)
                    return engine.UndefinedValue;

                // if there's no matching event, return undefined
                string eventName = args[0].ToString().ToLower();
                List<EventRegistration> registeredHandlers;
                if (!registrations_.TryGetValue(eventName, out registeredHandlers))
                    return engine.UndefinedValue;

                // if args[1] isn't a function, return undefined
                JavaScriptFunction fn = args[1] as JavaScriptFunction;
                if (fn == null)
                    return engine.UndefinedValue;

                // if we don't know about the event
                EventInfo targetEvent;
                if (!events_.TryGetValue(eventName, out targetEvent))
                    return engine.UndefinedValue;

                // if 'this' isn't an object with an external pointer of the same type as the event's declaring type, return undefined
                JavaScriptObject jsObj = null;
                object external = null;
                if (!static_)
                {
                    jsObj = thisVal as JavaScriptObject;
                    if (jsObj == null)
                        return engine.UndefinedValue;
                    external = jsObj.ExternalObject;
                    if (external == null)
                        return engine.UndefinedValue;
                    if (!targetEvent.DeclaringType.IsAssignableFrom(external.GetType()))
                        return engine.UndefinedValue;
                }

                // if there isn't a corresponding marshaler, bail
                Delegate marshaler;
                if (!marshalers_.TryGetValue(eventName, out marshaler))
                    return engine.UndefinedValue;

                lock (registeredHandlers)
                {
                    // todo: Remove from registered handlers

                    if (registeredHandlers.Count == 0)
                    {
                        targetEvent.RemoveMethod.Invoke(external, new object[] { marshaler });
                    }
                }

                return engine.UndefinedValue;
            };

            set_ = (engine, construct, thisVal, arguments) =>
            {

                // todo: support inheritance

                // if there aren't enough args, return undefined
                var args = arguments.ToArray();
                if (args.Length < 2)
                    return engine.UndefinedValue;

                // if there's no matching event, return undefined
                string eventName = args[0].ToString().ToLower();
                List<EventRegistration> registeredHandlers;
                if (!registrations_.TryGetValue(eventName, out registeredHandlers))
                    return engine.UndefinedValue;

                // if args[1] isn't a function, return undefined
                JavaScriptFunction fn = args[1] as JavaScriptFunction;
                if (fn == null)
                    return engine.UndefinedValue;

                // if we don't know about the event
                EventInfo targetEvent;
                if (!events_.TryGetValue(eventName, out targetEvent))
                    return engine.UndefinedValue;

                // if 'this' isn't an object with an external pointer of the same type as the event's declaring type, return undefined
                JavaScriptObject jsObj = null;
                object external = null;
                if (!static_)
                {
                    jsObj = thisVal as JavaScriptObject;
                    if (jsObj == null)
                        return engine.UndefinedValue;
                    external = jsObj.ExternalObject;
                    if (external == null)
                        return engine.UndefinedValue;
                    if (!targetEvent.DeclaringType.IsAssignableFrom(external.GetType()))
                        return engine.UndefinedValue;
                }

                // if there isn't a corresponding marshaler, bail
                Delegate marshaler;
                if (!marshalers_.TryGetValue(eventName, out marshaler))
                    return engine.UndefinedValue;

                lock (registeredHandlers)
                {
                    // todo: If we have a registered handler, don't re-add it, otherwise add it
                    // Then clear registered handlers for this item, and persist the set one.
                    if (registeredHandlers.Count == 0)
                    {
                        targetEvent.RemoveMethod.Invoke(external, new object[] { marshaler });
                    }
                }
                return engine.UndefinedValue;
            };
        }

        public static void InvokeEvent(int cookie, string eventName, string[] names, object[] values)
        {
            Debug.Assert(names != null);
            Debug.Assert(values != null);
            Debug.Assert(names.Length == values.Length);

            WeakReference<EventModel> model;
            if (!EventModels.TryGetValue(cookie, out model))
            {
                Debug.Assert(false);
                return;
            }

            EventModel eventModel;
            if (!model.TryGetTarget(out eventModel))
            {
                Debug.Assert(false);
                return;
            }

            List<EventRegistration> registeredHandlers;
            if (!eventModel.registrations_.TryGetValue(eventName, out registeredHandlers))
                return;

            foreach (var handler in registeredHandlers)
            {
                JavaScriptFunction invokee;
                if (!handler.Invokee.TryGetTarget(out invokee))
                    continue;

                JavaScriptEngine engine;
                try
                {
                    engine = invokee.GetEngine();
                    if (engine == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                if (handler.TargetContext != null)
                {
                    handler.TargetContext.Send((s) =>
                    {
                        using (var context = engine.AcquireContext())
                        {
                            var jsObj = engine.CreateObject();
                            for (int i = 0; i < names.Length; i++)
                            {
                                jsObj.SetPropertyByName(names[i], engine.Converter.FromObject(values[i]));
                            }

                            invokee.Invoke(new[] { jsObj });
                        }
                    }, null);
                }
                else
                {
                    using (var context = engine.AcquireContext())
                    {
                        var jsObj = engine.CreateObject();
                        for (int i = 0; i < names.Length; i++)
                        {
                            jsObj.SetPropertyByName(names[i], engine.Converter.FromObject(values[i]));
                        }

                        invokee.Invoke(new[] { jsObj });
                    }
                }
            }
        }

        public JavaScriptCallableFunction AddEventListener
        {
            get { return add_; }
        }

        public JavaScriptCallableFunction RemoveEventListener
        {
            get { return remove_; }
        }
    }
}
