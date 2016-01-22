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
        private List<PropertyBridge.PropertyModel> instanceProperties_, staticProperties_;
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
            instanceProperties_ = new List<PropertyBridge.PropertyModel>();
            staticProperties_ = new List<PropertyBridge.PropertyModel>();

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

            foreach (var property in instanceProperties)
            {
                BridgeProperty(engine, property, true, Prototype);
            }

            foreach (var property in staticProperties)
            {
                BridgeProperty(engine, property, false, Constructor);
            }

            // todo: functions
            // todo: events

            Prototype.Freeze();
        }

        private void BridgeProperty(JavaScriptEngine engine, PropertyInfo property, bool isInstance, JavaScriptObject targetObject)
        {
            PropertyBridge.PropertyModel propertyModel;
            if (PropertyBridge.PropertyModel.TryCreate(property, false, out propertyModel))
            {
                instanceProperties_.Add(propertyModel);

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


    }
}
