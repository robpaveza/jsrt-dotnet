using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptFunction : JavaScriptObject
    {
        internal JavaScriptFunction(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {

        }

        public JavaScriptValue Invoke(IEnumerable<JavaScriptValue> args)
        {
            var argsArray = args.PrependWith(this).Select(val => val.handle_.DangerousGetHandle()).ToArray();
            if (argsArray.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(args));

            var eng = GetEngineAndClaimContext();
            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsCallFunction(handle_, argsArray, (ushort)argsArray.Length, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public JavaScriptObject Construct(IEnumerable<JavaScriptValue> args)
        {
            var argsArray = args.PrependWith(this).Select(val => val.handle_.DangerousGetHandle()).ToArray();
            if (argsArray.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(args));

            var eng = GetEngineAndClaimContext();
            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsConstructObject(handle_, argsArray, (ushort)argsArray.Length, out resultHandle));

            return eng.CreateObjectFromHandle(resultHandle);
        }

        public JavaScriptFunction Bind(JavaScriptObject thisObject, IEnumerable<JavaScriptValue> args)
        {
            var eng = GetEngineAndClaimContext();

            if (thisObject == null)
                thisObject = eng.NullValue;
            if (args == null)
                args = Enumerable.Empty<JavaScriptValue>();

            var bindFn = GetBuiltinFunctionProperty("bind", "Function.prototype.bind");
            return bindFn.Invoke(args.PrependWith(thisObject)) as JavaScriptFunction;
        }

        public JavaScriptValue Apply(JavaScriptObject thisObject, JavaScriptArray args = null)
        {
            var eng = GetEngineAndClaimContext();
            if (thisObject == null)
                thisObject = eng.NullValue;

            var applyFn = GetBuiltinFunctionProperty("apply", "Function.prototype.apply");

            List<JavaScriptValue> resultList = new List<JavaScriptValue>();
            resultList.Add(thisObject);
            if (args != null)
                resultList.Add(args);

            return applyFn.Invoke(resultList);
        }

        public JavaScriptValue Call(JavaScriptObject thisObject, IEnumerable<JavaScriptValue> args)
        {
            var eng = GetEngineAndClaimContext();
            if (thisObject == null)
                thisObject = eng.NullValue;
            if (args == null)
                args = Enumerable.Empty<JavaScriptValue>();

            var applyFn = GetBuiltinFunctionProperty("call", "Function.prototype.call");
            return applyFn.Invoke(args.PrependWith(thisObject));
        }

        #region DynamicObject overrides
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            var e = GetEngine();
            var c = e.Converter;
            result = Invoke(args.Select(a => c.FromObject(a)));

            return true;
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            var e = GetEngine();
            var c = e.Converter;
            result = Construct(args.Select(a => c.FromObject(a)));

            return true;
        }
        #endregion
    }
}
