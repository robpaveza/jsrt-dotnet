using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal abstract class PropertyModel
    {
        internal static HashSet<Type> ImplicitlyConvertibleTypes = new HashSet<Type>()
            {
                typeof(JavaScriptValue),
                typeof(JavaScriptObject),
                typeof(JavaScriptArray),
                typeof(JavaScriptFunction),
                typeof(JavaScriptArrayBuffer),
                typeof(JavaScriptTypedArray),
                typeof(JavaScriptDataView),
                typeof(JavaScriptSymbol),
                typeof(string),
                typeof(double),
                typeof(float),
                typeof(byte),
                typeof(ushort),
                typeof(short),
                typeof(uint),
                typeof(int),
                typeof(long),
                typeof(bool),
                typeof(Task),
                typeof(Task<>),
            };
        public abstract string PropertyName
        {
            get;
        }

        public static bool IsImplicitlyConvertibleType(Type propertyType)
        {
            return ImplicitlyConvertibleTypes.Contains(propertyType) ||
                   typeof(Task).IsAssignableFrom(propertyType);
        }

        public abstract JavaScriptCallableFunction Getter
        {
            get;
        }

        public abstract JavaScriptCallableFunction Setter
        {
            get;
        }

        public abstract bool IsStatic
        {
            get;
        }

        public abstract string FullGetterName
        {
            get;
        }

        public abstract string FullSetterName
        {
            get;
        }

        public static bool TryCreate(PropertyInfo property, bool isStatic, out PropertyModel model)
        {
            model = null;
            if (property.DeclaringType.GetTypeInfo().IsValueType)
            {
                return false;
            }

            if (IsImplicitlyConvertibleType(property.PropertyType))
            {
                var implementationType = typeof(PropertyModel<>).MakeGenericType(property.PropertyType);
                model = Activator.CreateInstance(implementationType, property, isStatic) as PropertyModel;
                return true;
            }

            return false;
        }
    }
    internal class PropertyModel<TPropertyType> : PropertyModel
    {
        private JavaScriptCallableFunction getter_, setter_;
        private bool isStatic_;
        private string name_;

        public PropertyModel(PropertyInfo info, bool isStatic)
        {
            Debug.Assert(info != null);
            if (info == null) throw new ArgumentNullException(nameof(info));

            isStatic_ = isStatic;
            Property = info;

            name_ = Property.Name;
            var hostMemberAttr = info.GetCustomAttribute<JavaScriptHostMemberAttribute>();
            if (hostMemberAttr != null)
            {
                name_ = hostMemberAttr.JavaScriptName ?? name_;
            }

            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            // Need to initialize getter and/or setter
            /* Pseudo high level code:
            if (Property.GetMethod != null) {
                // JavaScript host function
                get = (engine, construct, thisValue, arguments) => {
                    var thisObj = thisValue as JavaScriptObject;
                    if (thisObj == null)
                        throw new ArgumentException(...);
                    var hostObj = thisObj.ExternalObject as {DeclaringType};
                    if (hostObj == null)
                        throw new ArgumentException(...);
                    {if property is static}
                    var value = {DeclaringType}.{Property}
                    {otherwise}
                    var value = hostObj.{Property};
                    return engine.Converter.FromObject(value);
                };
            }
            if (Property.SetMethod != null) {
                set = (engine, construct, thisValue, arguments) => {
                    var argsArray = arguments.ToArray();
                    if (argsArray.Length < 1) {
                        engine.SetException(
                            engine.CreateRangeError("Insufficient number of arguments.")
                        );
                        return engine.UndefinedValue;
                    }
                    var thisObj = thisValue as JavaScriptObject;
                    if (thisObj == null)
                        throw new ArgumentException(...);
                    var hostObj = this.ExternalObject as {DeclaringType}
                    if (hostObj == null)
                        throw new ArgumentException(...);

                    {PropertyType} value = ({PropertyType})engine.Converter.ToObject(argsArray[0]);
                    {if property is static}
                    {DeclaringType}.{Property} = value;
                    {otherwise}
                    hostObj.{Property} = value;

                    return argsArray[0];
                };
            }
                */
            #region Parameter/local expressions
            var engineParamExpr = Expression.Parameter(typeof(JavaScriptEngine), "engine");
            var constructParamExpr = Expression.Parameter(typeof(bool), "construct");
            var thisValueParamExpr = Expression.Parameter(typeof(JavaScriptValue), "thisValue");
            var argsParamExpr = Expression.Parameter(typeof(IEnumerable<JavaScriptValue>), "arguments");
            var argsArrayExpr = Expression.Variable(typeof(JavaScriptValue[]), "argsArray");
            var hostObj = Expression.Variable(Property.DeclaringType, "hostObj");
            var thisObj = Expression.Variable(typeof(JavaScriptObject), "thisObj");
            var value = Expression.Variable(Property.PropertyType, "value");
            ParameterExpression[] paramsExpr = new[] { engineParamExpr, constructParamExpr, thisValueParamExpr, argsParamExpr };
            LabelTarget getReturnLabel = Expression.Label(typeof(JavaScriptValue), "exit");
            #endregion
            #region Get method marshaling
            if (Property.GetMethod != null)
            {
                var marshaler = Expression.Lambda(
                                    typeof(JavaScriptCallableFunction),
                                    Expression.Block(
                                        new ParameterExpression[] { hostObj, thisObj, value },
                                        // thisObj = thisValue as JavaScriptObject;
                                        Expression.Assign(
                                            thisObj,
                                            Expression.TypeAs(thisValueParamExpr, typeof(JavaScriptObject))
                                        ),
                                        // if (thisObj == null) {
                                        Expression.IfThen(
                                            Expression.ReferenceEqual(
                                                Expression.Constant(null),
                                                thisObj
                                            ),
                                            // throw new ArgumentException(...)
                                            Expression.Throw(
                                                Expression.New(
                                                    typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                                                    Expression.Constant("Invalid host object supplied while accessing property.")
                                                )
                                            )
                                        ),
                                        // }
                                        // hostObj = thisObj.ExternalObject as {DeclaringType}
                                        Expression.Assign(
                                            hostObj,
                                            Expression.TypeAs(
                                                Expression.Property(
                                                    thisObj,
                                                    nameof(JavaScriptObject.ExternalObject)
                                                ),
                                                Property.DeclaringType
                                            )
                                        ),
                                        // if (hostObj == null) {
                                        Expression.IfThen(
                                            Expression.ReferenceEqual(
                                                Expression.Constant(null),
                                                hostObj
                                            ),
                                            // throw new ArgumentException(...)
                                            Expression.Throw(
                                                Expression.New(
                                                    typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                                                    Expression.Constant("Invalid host object supplied while accessing property.")
                                                )
                                            )
                                        // }
                                        ),
                                        // {if property is static}
                                        // var value = {DeclaringType}.{Property}
                                        // {otherwise}
                                        // value = hostObj.{PropertyName}
                                        Expression.Assign(
                                            value,
                                                Expression.Property(
                                                    IsStatic ? null : hostObj,
                                                    Property.GetMethod
                                                )
                                        ),
                                        // return engine.Converter.FromObject(value);
                                        Expression.Return(
                                            getReturnLabel,
                                            Expression.Call(
                                                Expression.Property(
                                                    engineParamExpr,
                                                    nameof(JavaScriptEngine.Converter)
                                                ),
                                                typeof(JavaScriptConverter).GetMethod(nameof(JavaScriptConverter.FromObject)),
                                                Expression.Convert(value, typeof(object))
                                            )
                                        ),
                                        Expression.Label(getReturnLabel, Expression.Convert(Expression.Constant(null), typeof(JavaScriptValue)))
                                    ),
                                    FullGetterName,
                                    paramsExpr
                                );
                getter_ = marshaler.Compile() as JavaScriptCallableFunction;
            }
            #endregion
            #region Set method marshaling
            if (Property.SetMethod != null)
            {
                var setMarshaler = Expression.Lambda(
                                    typeof(JavaScriptCallableFunction),
                                    Expression.Block(
                                        new ParameterExpression[] { argsArrayExpr, hostObj, thisObj, value, },
                                        // argsArray = arguments.ToArray() (extension method Enumerable.ToArray<JavaScriptValue)([this] arguments))
                                        Expression.Assign(
                                            argsArrayExpr,
                                            Expression.Call(
                                                typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(typeof(JavaScriptValue)),
                                                argsParamExpr
                                            )
                                        ),
                                        // if (argsArray.Length < 1) 
                                        Expression.IfThen(
                                            Expression.LessThan(
                                                Expression.Property(
                                                    argsArrayExpr,
                                                    nameof(Array.Length)
                                                ),
                                                Expression.Constant(1)
                                            ),
                                            // {
                                            Expression.Block(
                                                // engine.SetException(engine.CreateRangeError(...))
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
                                                    getReturnLabel,
                                                    Expression.Property(
                                                        engineParamExpr,
                                                        nameof(JavaScriptEngine.UndefinedValue)
                                                    )
                                                )
                                            // }
                                            )
                                        ),
                                        // thisObj = thisValue as JavaScriptObject;
                                        Expression.Assign(
                                            thisObj,
                                            Expression.TypeAs(thisValueParamExpr, typeof(JavaScriptObject))
                                        ),
                                        // if (thisObj == null) {
                                        Expression.IfThen(
                                            Expression.ReferenceEqual(
                                                Expression.Constant(null),
                                                thisObj
                                            ),
                                            // throw new ArgumentException(...)
                                            Expression.Throw(
                                                Expression.New(
                                                    typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                                                    Expression.Constant("Invalid host object supplied while accessing property.")
                                                )
                                            )
                                        ),
                                        // }
                                        // hostObj = thisObj.ExternalObject as {PropertyType}
                                        Expression.Assign(
                                            hostObj,
                                            Expression.TypeAs(
                                                Expression.Property(
                                                    thisObj,
                                                    nameof(JavaScriptObject.ExternalObject)
                                                ),
                                                Property.DeclaringType
                                            )
                                        ),
                                        // if (hostObj == null) {
                                        Expression.IfThen(
                                            Expression.ReferenceEqual(
                                                Expression.Constant(null),
                                                hostObj
                                            ),
                                            // throw new ArgumentException(...)
                                            Expression.Throw(
                                                Expression.New(
                                                    typeof(ArgumentException).GetConstructor(new[] { typeof(string) }),
                                                    Expression.Constant("Invalid host object supplied while accessing property.")
                                                )
                                            )
                                        // }
                                        ),
                                        // {PropertyType} value = ({PropertyType})engine.Converter.ToObject(argsArray[0]);
                                        Expression.Assign(
                                            value,
                                            Expression.Convert(
                                                Expression.Call(
                                                    Expression.Property(
                                                        engineParamExpr,
                                                        nameof(JavaScriptEngine.Converter)
                                                    ),
                                                    typeof(JavaScriptConverter).GetMethod(nameof(JavaScriptConverter.ToObject)),
                                                    Expression.ArrayIndex(argsArrayExpr, Expression.Constant(0))
                                                ),
                                                Property.PropertyType
                                            )
                                        ),
                                        // {if property is static}
                                        // {DeclaringType}.{Property} = value;
                                        // {otherwise}
                                        // hostObj.{Property} = value;
                                        Expression.Assign(
                                            Expression.Property(
                                                IsStatic ? null : hostObj,
                                                Property),
                                            value
                                        ),

                                        Expression.Return(  
                                            getReturnLabel, 
                                            Expression.ArrayIndex(argsArrayExpr, Expression.Constant(0))
                                        ),
                                        Expression.Label(getReturnLabel, Expression.Convert(Expression.Constant(null), typeof(JavaScriptValue)))
                                    ),
                                    FullSetterName,
                                    paramsExpr
                                   );
                setter_ = setMarshaler.Compile() as JavaScriptCallableFunction;
            }
            #endregion
        }


        public override string FullGetterName
        {
            get
            {
                if (IsStatic)
                    return $"js#{Property.DeclaringType.FullName}.{PropertyName}.get";

                return $"js#{Property.DeclaringType.FullName}.prototype.{PropertyName}.get";
            }
        }

        public override string FullSetterName
        {
            get
            {
                if (IsStatic)
                    return $"js#{Property.DeclaringType.FullName}.{PropertyName}.set";

                return $"js#{Property.DeclaringType.FullName}.prototype.{PropertyName}.set";
            }
        }

        public override string PropertyName
        {
            get
            {
                var result = Property.Name;

                var attr = Property.GetCustomAttribute<JavaScriptHostMemberAttribute>();
                result = attr?.JavaScriptName ?? result;

                return result;
            }
        }

        public PropertyInfo Property;

        public override JavaScriptCallableFunction Getter
        {
            get
            {
                return getter_;
            }
        }

        public override JavaScriptCallableFunction Setter
        {
            get
            {
                return setter_;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return isStatic_;
            }
        }
    }
}
