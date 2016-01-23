using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal class MethodModel
    {
        internal static HashSet<Type> ImplicitlyConvertibleTypes = PropertyModel.ImplicitlyConvertibleTypes;

        private MethodInfo[] methodGroup_;
        private bool static_;

        private MethodModel(IEnumerable<MethodInfo> methodGroup, bool isStatic)
        {
            if (methodGroup == null)
                throw new ArgumentNullException(nameof(methodGroup));

            static_ = isStatic;
            methodGroup_ = methodGroup.OrderBy(mi => mi.GetParameters().Length).ToArray();
            if (methodGroup_.Length < 1)
                throw new ArgumentException(nameof(methodGroup));

            if (!methodGroup_.NoneOrAll(mi => typeof(Task).IsAssignableFrom(mi.ReturnType)))
                throw new ArgumentException(nameof(methodGroup));
            if (methodGroup.Any(mi => mi.GetParameters().Any(pi => pi.ParameterType.IsByRef)))
                throw new ArgumentException(nameof(methodGroup));

            Initialize();
        }

        public string MethodName
        {
            get
            {
                var result = methodGroup_[0].Name;
                var memberAttr = methodGroup_[0].GetCustomAttribute<JavaScriptHostMemberAttribute>();
                result = memberAttr?.JavaScriptName ?? result;

                return result;
            }
        }

        public string FullName
        {
            get
            {
                if (IsStatic)
                    return $"js#{methodGroup_[0].DeclaringType.FullName}.{MethodName}";

                return $"js#{methodGroup_[0].DeclaringType.FullName}.prototype.{MethodName}";
            }
        }

        public bool IsStatic
        {
            get { return static_; }
        }

        public bool IsAsync
        {
            get { return typeof(Task).IsAssignableFrom(methodGroup_[0].ReturnType); }
        }

        public bool HasOverloads
        {
            get { return methodGroup_.Length > 1; }
        }

        public JavaScriptCallableFunction EntryPoint
        {
            get;
            private set;
        }

        public JavaScriptCallableAsyncFunction AsyncEntryPoint
        {
            get;
            private set;
        }

        public static bool TryCreate(IEnumerable<MethodInfo> methodGroup, bool isStatic, out MethodModel model)
        {
            try
            {
                model = new MethodModel(methodGroup, isStatic);
                return true;
            }
            catch
            {
                model = null;
                return false;
            }
        }

        private void Initialize()
        {
            if (IsAsync)
            {
                if (HasOverloads) InitializeOverloadedAsync();
                else InitializeNonOverloadAsync();
            }
            else
            {
                if (HasOverloads) InitializeOverloadedSync();
                else InitializeNonOverloadSync();
            }
        }

        private void InitializeNonOverloadSync()
        {
            // Simplest case
            /*
fn = (engine, construct, thisValue, args) => {
    var argsArray = arguments.ToArray();
    if (argsArray.Length < {methodArity}) {
        engine.SetException(engine.CreateRangeError("Insufficient number of arguments."));
        return engine.UndefinedValue;
    }
    {if method is nonstatic} 
        var thisObj = thisValue as JavaScriptObject;
        if (thisObj == null)
            throw new ArgumentException(...);
        var hostObj = thisObj.ExternalObject as {DeclaringType};
        if (hostObj == null)
            throw new ArgumentException(...);
    {/if...}
    {foreach arg in methodParamList}
        {ArgType} {argName} = ({ArgType})engine.Converter.ToObject(curArg++);
    {/foreach...}
    {if method is nonstatic}
        {if method returns void}
            hostObj.{Method}(...args);
        {else}
            {ReturnType} result = hostObj.{Method}(...args);
    {else}
        {if method returns void}
            {DeclaringType}.{Method}(...args);        
        {else}
            {ReturnType} result = {DeclaringType}.{Method}(...args);

};
            */
            var method = methodGroup_[0];

            #region Parameter/local expressions
            var engineParamExpr = Expression.Parameter(typeof(JavaScriptEngine), "engine");
            var constructParamExpr = Expression.Parameter(typeof(bool), "construct");
            var thisValueParamExpr = Expression.Parameter(typeof(JavaScriptValue), "thisValue");
            var argsParamExpr = Expression.Parameter(typeof(IEnumerable<JavaScriptValue>), "arguments");
            var argsArrayExpr = Expression.Variable(typeof(JavaScriptValue[]), "argsArray");
            var hostObj = Expression.Variable(method.DeclaringType, "hostObj");
            var thisObj = Expression.Variable(typeof(JavaScriptObject), "thisObj");
            var value = Expression.Variable(method.ReturnType, "value");
            ParameterExpression[] paramsExpr = new[] { engineParamExpr, constructParamExpr, thisValueParamExpr, argsParamExpr };
            LabelTarget returnLabel = Expression.Label(typeof(JavaScriptValue), "exit");

            var invocationParameters = method.GetParameters().Select(pi => Expression.Parameter(pi.ParameterType, pi.Name)).ToArray();
            var localVariables = invocationParameters.PrependWith(hostObj, thisObj, value, argsArrayExpr).ToArray();
            #endregion

            var marshaler = Expression.Lambda(
                typeof(JavaScriptCallableFunction),
                Expression.Block(
                    localVariables,
                    ConstructMethodBody(engineParamExpr, constructParamExpr, thisValueParamExpr, argsParamExpr, argsArrayExpr, 
                                        hostObj, thisObj, value, returnLabel, invocationParameters, localVariables)
                ),
                FullName,
                paramsExpr);
            EntryPoint = marshaler.Compile() as JavaScriptCallableFunction;
        }

        private IEnumerable<Expression> ConstructMethodBody(ParameterExpression engineParamExpr, 
                                                            ParameterExpression constructParamExpr,
                                                            ParameterExpression thisValueParamExpr,
                                                            ParameterExpression argsParamExpr,
                                                            ParameterExpression argsArrayExpr,
                                                            ParameterExpression hostObj,
                                                            ParameterExpression thisObj,
                                                            ParameterExpression value,
                                                            LabelTarget returnLabel,
                                                            ParameterExpression[] invocationParameters,
                                                            ParameterExpression[] localVariables)
        {
            /*
fn = (engine, construct, thisValue, args) => {
    var argsArray = arguments.ToArray();
    if (argsArray.Length < {methodArity}) {
        engine.SetException(engine.CreateRangeError("Insufficient number of arguments."));
        return engine.UndefinedValue;
    }
    {if method is nonstatic} 
        var thisObj = thisValue as JavaScriptObject;
        if (thisObj == null)
            throw new ArgumentException(...);
        var hostObj = thisObj.ExternalObject as {DeclaringType};
        if (hostObj == null)
            throw new ArgumentException(...);
    {/if...}
    {foreach arg in methodParamList}
        {ArgType} {argName} = ({ArgType})engine.Converter.ToObject(curArg++);
    {/foreach...}
    {if method is nonstatic}
        {if method returns void}
            hostObj.{Method}(...args);
            return engine.UndefinedValue;
        {else}
            {ReturnType} result = hostObj.{Method}(...args);
            return engine.Converter.FromObject(result);
    {else}
        {if method returns void}
            {DeclaringType}.{Method}(...args);
            return engine.UndefinedValue;
        {else}
            {ReturnType} result = {DeclaringType}.{Method}(...args);
            return engine.Converter.FromObject(result);

};
            */
            var method = methodGroup_[0];

            #region Parameter/local expressions
            ParameterExpression[] paramsExpr = new[] { engineParamExpr, constructParamExpr, thisValueParamExpr, argsParamExpr };

            #endregion

            // var argsArray = arguments.ToArray();
            yield return Expression.Assign(
                argsArrayExpr,
                Expression.Call(
                    typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(typeof(JavaScriptValue)),
                    argsParamExpr
                )
            );

            // if (argsArray.Length < {methodParametersCount}) 
            yield return Expression.IfThen(
                Expression.LessThan(
                    Expression.Property(
                        argsArrayExpr,
                        nameof(Array.Length)
                    ),
                    Expression.Constant(method.GetParameters().Length)
                ),
                // {
                Expression.Block(
                    Expression.Call(
                        engineParamExpr,
                        typeof(JavaScriptEngine).GetMethod(nameof(JavaScriptEngine.SetException)),
                        Expression.Call(
                            engineParamExpr,
                            typeof(JavaScriptEngine).GetMethod(nameof(JavaScriptEngine.CreateRangeError)),
                            Expression.Constant("Insufficient number of arguments.")
                        )
                    ),
                    // return engine.UndefinedValue;
                    Expression.Return(
                        returnLabel,
                        Expression.Property(
                            engineParamExpr,
                            nameof(JavaScriptEngine.UndefinedValue)
                        )
                    )
                // }
                )
            );

            // if method is nonstatic
            if (!IsStatic)
            {
                // var thisObj = thisValue as JavaScriptObject;
                yield return Expression.Assign(
                    thisObj,
                    Expression.TypeAs(thisValueParamExpr, typeof(JavaScriptObject))
                );
                // if (thisObj == null)
                yield return Expression.IfThen(
                    Expression.ReferenceEqual(
                        Expression.Constant(null),
                        thisObj
                    ),
                    // throw new ArgumentException(...);
                    Expression.Throw(
                        Expression.New(
                            typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                            Expression.Constant("Invalid host object supplied while accessing instance property.")
                        )
                    )
                );

                // var hostObj = thisObj.ExternalObject as { DeclaringType};
                yield return Expression.Assign(
                    hostObj,
                    Expression.TypeAs(
                        Expression.Property(
                            thisObj,
                            nameof(JavaScriptObject.ExternalObject)
                        ),
                        method.DeclaringType
                    )
                );
                // if (hostObj == null)
                yield return Expression.IfThen(
                    Expression.ReferenceEqual(
                        Expression.Constant(null),
                        hostObj
                    ),
                    // throw new ArgumentException(...)
                    Expression.Throw(
                        Expression.New(
                            typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                            Expression.Constant("Invalid host object supplied while accessing instance method.")
                        )
                    )
                );
            } // if (!IsStatic)

            for (int curParamIndex = 0; curParamIndex < invocationParameters.Length; curParamIndex++)
            {
                // note: localVariables is prepended with thisObj, hostObj, and value, so need to +3
                // to get ParameterExpression which corresponds to argsArray[curParamIndex].

                //{ArgType} {argName} = ({ArgType})engine.Converter.ToObject(curArg++);
                yield return Expression.Assign(
                    invocationParameters[curParamIndex],
                    Expression.Convert(
                        Expression.Call(
                            Expression.Property(
                                engineParamExpr,
                                nameof(JavaScriptEngine.Converter)
                            ),
                            typeof(JavaScriptConverter).GetMethod(nameof(JavaScriptConverter.ToObject)),
                            Expression.ArrayIndex(
                                argsArrayExpr,
                                Expression.Constant(curParamIndex)
                            )
                        ),
                        invocationParameters[curParamIndex].Type
                    )
                );
            } // for

            if (IsStatic)
            {
                if (method.ReturnType == typeof(void))
                {
                    //{DeclaringType}.{ Method} (...args);
                    yield return Expression.Call(method, invocationParameters);
                    //return engine.UndefinedValue;
                    yield return Expression.Return(
                        returnLabel,
                        Expression.Property(
                            engineParamExpr,
                            nameof(JavaScriptEngine.UndefinedValue)
                        )
                    );
                }
                else
                {
                    //{ReturnType} result = hostObj.{Method}(...args);
                    yield return Expression.Assign(
                        value,
                        Expression.Call(method, invocationParameters)
                    );
                    //return engine.Converter.FromObject(result);
                    yield return Expression.Return(
                        returnLabel,
                        Expression.Call(
                            Expression.Property(
                                engineParamExpr,
                                nameof(JavaScriptEngine.Converter)
                            ),
                            typeof(JavaScriptConverter).GetMethod(nameof(JavaScriptConverter.FromObjectViaNewBridge)),
                            Expression.Convert(
                                value,
                                typeof(object)
                            )
                        )
                    );
                }
            }
            else // if (!IsStatic)
            {
                if (method.ReturnType == typeof(void))
                {
                    //{DeclaringType}.{ Method} (...args);
                    yield return Expression.Call(hostObj, method, invocationParameters);
                    //return engine.UndefinedValue;
                    yield return Expression.Return(
                        returnLabel,
                        Expression.Property(
                            engineParamExpr,
                            nameof(JavaScriptEngine.UndefinedValue)
                        )
                    );
                }
                else
                {
                    //{ReturnType} result = hostObj.{Method}(...args);
                    yield return Expression.Assign(
                        value,
                        Expression.Call(hostObj, method, invocationParameters)
                    );
                    //return engine.Converter.FromObject(result);
                    yield return Expression.Return(
                        returnLabel,
                        Expression.Call(
                            Expression.Property(
                                engineParamExpr,
                                nameof(JavaScriptEngine.Converter)
                            ),
                            typeof(JavaScriptConverter).GetMethod(nameof(JavaScriptConverter.FromObjectViaNewBridge)),
                            Expression.Convert(
                                value,
                                typeof(object)
                            )
                        )
                    );
                }
            }

            yield return Expression.Label(returnLabel, Expression.Convert(Expression.Constant(null), typeof(JavaScriptValue)));
        }

        private void InitializeOverloadedSync()
        {

        }

        private void InitializeNonOverloadAsync()
        {

        }

        private void InitializeOverloadedAsync()
        {

        }
    }
}
