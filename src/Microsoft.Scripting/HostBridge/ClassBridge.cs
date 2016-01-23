using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal class ClassBridge
    {
        private volatile int RefCount;
        private JavaScriptObject Prototype;
        private JavaScriptFunction Constructor;
        private Type type_;
        private TypeInfo typeInfo_;
        private List<PropertyModel> instanceProperties_, staticProperties_;
        private List<MethodModel> instanceMethods_, staticMethods_;
        private BridgeManager manager_;

        public ClassBridge(Type type, BridgeManager manager)
        {
            Debug.Assert(type != null && manager != null);
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            type_ = type;
            typeInfo_ = type.GetTypeInfo();

            if (typeInfo_.IsValueType)
                throw new InvalidOperationException($"Type \"{type.FullName}\" cannot be projected to JavaScript as it is a value-type.");

            RefCount = 1;
            manager_ = manager;
            instanceProperties_ = new List<PropertyModel>();
            staticProperties_ = new List<PropertyModel>();

            instanceMethods_ = new List<MethodModel>();
            staticMethods_ = new List<MethodModel>();

            InitializeBridge();
        }

        internal void AddRef()
        {
            Interlocked.Increment(ref RefCount);
        }
        
        internal void Release()
        {
            if (Interlocked.Decrement(ref RefCount) <= 0)
            {
                manager_.ReleaseBridge(type_);
            }
        }

        public JavaScriptObject ProjectObject(object o)
        {
            var result = manager_.Engine.CreateExternalObject(o, null);
            result.Prototype = this.Prototype;

            return result;
        }

        private void InitializeBridge()
        {
            ClassBridge baseTypeBridge = null;
            if (typeInfo_.BaseType != null)
            {
                baseTypeBridge = manager_.GetBridge(typeInfo_.BaseType);
            }

            var instanceProperties = typeInfo_.DeclaredProperties.Where(p => !(p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic));
            var staticProperties = typeInfo_.DeclaredProperties.Where(p => (p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic));

            var engine = manager_.Engine;
            // todo: project constructor using constructor bridge

            // var MyObject = function() { [native code] };
            Constructor = engine.CreateFunction((eng, construct, thisObj, args) =>
            {
                return eng.UndefinedValue;
            }, type_.FullName);
            
            // MyObject.prototype = Object.create(baseTypeProjection.Prototype);
            if (baseTypeBridge != null)
            {
                dynamic global = engine.GlobalObject;
                Prototype = global.Object.create(baseTypeBridge.Prototype);
            }
            else
            {
                Prototype = engine.CreateObject();
            }

            // MyObject.prototype.constructor = MyObject;
            Prototype.SetPropertyByName("constructor", Constructor);
            Prototype.SetPropertyByName("__CLRType__", engine.Converter.FromString(typeInfo_.FullName));

            foreach (var property in instanceProperties)
            {
                BridgeProperty(engine, property, true, Prototype);
            }

            foreach (var property in staticProperties)
            {
                BridgeProperty(engine, property, false, Constructor);
            }

            var instanceMethods = typeInfo_.DeclaredMethods.Where(m => !m.IsSpecialName && !m.IsStatic).GroupBy(m => m.Name);
            var staticMethods = typeInfo_.DeclaredMethods.Where(m => !m.IsSpecialName && m.IsStatic).GroupBy(m => m.Name);
            
            foreach (var methodGroup in instanceMethods)
            {
                BridgeMethod(engine, methodGroup.ToArray(), true, Prototype);
            }

            foreach (var methodGroup in staticMethods)
            {
                BridgeMethod(engine, methodGroup.ToArray(), false, Constructor);
            }

            // todo: events

            Prototype.Freeze();
        }

        private void BridgeProperty(JavaScriptEngine engine, PropertyInfo property, bool isInstance, JavaScriptObject targetObject)
        {
            PropertyModel propertyModel;
            if (PropertyModel.TryCreate(property, !isInstance, out propertyModel))
            {
                (isInstance ? instanceProperties_ : staticProperties_).Add(propertyModel);

                var propertyDefinition = engine.CreateObject();
                propertyDefinition.SetPropertyByName("enumerable", engine.TrueValue);
                if (propertyModel.Getter != null)
                {
                    propertyDefinition.SetPropertyByName("get", engine.CreateFunction(propertyModel.Getter, propertyModel.FullGetterName));
                }
                if (propertyModel.Setter != null)
                {
                    propertyDefinition.SetPropertyByName("set", engine.CreateFunction(propertyModel.Setter, propertyModel.FullSetterName));
                }
                targetObject.DefineProperty(property.Name, propertyDefinition);
            }
        }

        private void BridgeMethod(JavaScriptEngine engine, MethodInfo[] methodGroup, bool isInstance, JavaScriptObject targetObject)
        {
            MethodModel methodModel;
            
            if (MethodModel.TryCreate(methodGroup, !isInstance, out methodModel))
            {
                (isInstance ? instanceMethods_ : staticMethods_).Add(methodModel);
                JavaScriptFunction fn;
                if (methodModel.IsAsync)
                {
                    // todo: enable async to have names
                    // fn = engine.CreateFunction(methodModel.AsyncEntryPoint, methodModel.FullName, AsyncHostFunctionKind.Promise);
                    fn = engine.CreateFunction(methodModel.AsyncEntryPoint, AsyncHostFunctionKind.Promise);
                }
                else
                {
                    fn = engine.CreateFunction(methodModel.EntryPoint, methodModel.FullName);
                }
                targetObject.SetPropertyByName(methodModel.MethodName, fn);
            }
        }


    }
}
