using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal class HostModelManager
    {
        private WeakReference<JavaScriptEngine> engine_;
        private Dictionary<Type, ModelPrototype> instancePrototypes_;
        private Dictionary<Type, ModelPrototype> staticPrototypes_;

        public HostModelManager(JavaScriptEngine engine)
        {
            Debug.Assert(engine != null);
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            engine_ = new WeakReference<JavaScriptEngine>(engine);
            instancePrototypes_ = new Dictionary<Type, ModelPrototype>();
            staticPrototypes_ = new Dictionary<Type, ModelPrototype>();
        }

        public ModelPrototype GetBridge(Type type)
        {
            Debug.Assert(type != null);
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return GetBridge(type, true);
        }

        public ModelPrototype GetBridge(object o)
        {
            Debug.Assert(o != null);
            if (o == null)
                throw new ArgumentNullException(nameof(o));

            return GetBridge(o.GetType(), false);
        }

        private ModelPrototype GetBridge(Type type, bool isStatic)
        {
            ModelPrototype result;
            Dictionary<Type, ModelPrototype> prototypes = (isStatic ? staticPrototypes_ : instancePrototypes_);
            if (prototypes.TryGetValue(type, out result))
            {
                result.AddRef();
            }
            else
            {
                result = new ModelPrototype(type, this, isStatic);
                prototypes.Add(type, result);
            }

            return result;
        }

        internal void ReleasePrototype(Type type, bool isStatic)
        {
            Dictionary<Type, ModelPrototype> container = isStatic ? staticPrototypes_ : instancePrototypes_;
            container.Remove(type);
        }

        public JavaScriptEngine Engine
        {
            get
            {
                JavaScriptEngine result;
                engine_.TryGetTarget(out result);

                return result;
            }
        }
    }
}
