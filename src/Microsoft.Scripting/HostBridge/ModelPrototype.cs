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
    internal class ModelPrototype
    {
        private volatile int RefCount;
        private Type type_;
        private TypeInfo typeInfo_;
        private HostModelManager manager_;
        private HostClassMode hostMode_;
        private bool static_;

        public IEnumerable<PropertyModel> Properties
        {
            get;
            private set;
        }

        public IEnumerable<MethodModel> Methods
        {
            get;
            private set;
        }

        public IDictionary<string, EventModel2> Events
        {
            get;
            private set;
        }

        public string FullTypeName
        {
            get
            {
                return type_.FullName;
            }
        }

        public ModelPrototype GetBaseTypeModel()
        {
            ModelPrototype baseType = null;
            if (typeInfo_.BaseType != null)
            {
                if (hostMode_ == HostClassMode.FullClass ||
                   (hostMode_ == HostClassMode.OptIn && IsOptedIn(typeInfo_.BaseType)))
                {
                    baseType = manager_.GetBridge(typeInfo_.BaseType);
                }
            }

            return baseType;
        }

        public ModelPrototype(Type type, HostModelManager manager, bool isStatic)
        {
            Debug.Assert(type != null && manager != null);
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            type_ = type;
            typeInfo_ = type.GetTypeInfo();

            var attr = typeInfo_.GetCustomAttribute<JavaScriptHostClassAttribute>();
            hostMode_ = attr?.Mode ?? HostClassMode.FullClass;

            if (typeInfo_.IsValueType)
                throw new InvalidOperationException($"Type \"{type.FullName}\" cannot be bridged to JavaScript as it is a value-type.");

            RefCount = 1;
            manager_ = manager;

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
                manager_.ReleasePrototype(type_, static_);
            }
        }

        private static bool IsOptedIn(Type type)
        {
            TypeInfo info = type.GetTypeInfo();
            var attr = info.GetCustomAttribute<JavaScriptHostClassAttribute>();
            return attr != null;
        }

        private bool ShouldProjectMember(MemberInfo member)
        {
            return hostMode_ == HostClassMode.FullClass ||
                  (hostMode_ == HostClassMode.OptIn && member.GetCustomAttribute<JavaScriptHostMemberAttribute>() != null);
        }

        private void InitializeBridge()
        {
            #region Properties
            List<PropertyModel> properties = new List<PropertyModel>();
            IEnumerable<PropertyInfo> typeProperties = null;
            if (static_)
            {
                typeProperties = typeInfo_.DeclaredProperties.Where(p => (p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic) && ShouldProjectMember(p));
            }
            else
            {
                typeProperties = typeInfo_.DeclaredProperties.Where(p => !(p.GetMethod?.IsStatic ?? p.SetMethod.IsStatic) && ShouldProjectMember(p));
            }

            foreach (var property in typeProperties)
            {
                PropertyModel propertyModel;
                if (PropertyModel.TryCreate(property, static_, out propertyModel))
                {
                    properties.Add(propertyModel);
                }
            }

            Properties = properties.AsEnumerable();
            #endregion

            #region Methods
            List<MethodModel> methods = new List<MethodModel>();
            IEnumerable<IGrouping<string, MethodInfo>> typeMethods = null;
            if (static_)
            {
                typeMethods = typeInfo_.DeclaredMethods.Where(m => !m.IsSpecialName && m.IsStatic && ShouldProjectMember(m)).GroupBy(m => m.Name);
            }
            else
            {
                typeMethods = typeInfo_.DeclaredMethods.Where(m => !m.IsSpecialName && !m.IsStatic && ShouldProjectMember(m)).GroupBy(m => m.Name);
            }

            foreach (var methodGroup in typeMethods)
            {
                MethodModel methodModel;
                if (MethodModel.TryCreate(methodGroup, static_, out methodModel))
                {
                    methods.Add(methodModel);
                }
            }

            Methods = methods.AsEnumerable();
            #endregion

            #region Events
            List<EventModel2> events = new List<EventModel2>();
            IEnumerable<EventInfo> typeEvents = null;
            if (static_)
            {
                typeEvents = typeInfo_.DeclaredEvents.Where(e => e.AddMethod.IsStatic && ShouldProjectMember(e));
            }
            else
            {
                typeEvents = typeInfo_.DeclaredEvents.Where(e => !e.AddMethod.IsStatic && ShouldProjectMember(e));
            }

            foreach (var @event in typeEvents)
            {
                EventModel2 eventModel;
                if (EventModel2.TryCreate(@event, static_, out eventModel))
                {
                    events.Add(eventModel);
                }
            }

            Events = events.ToDictionary(em => em.Name);
            #endregion
        }
    }
}
