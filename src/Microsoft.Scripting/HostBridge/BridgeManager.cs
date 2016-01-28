using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    /// <summary>
    /// Manages the list of active ClassBridge instances for a given script engine.
    /// Class bridges are per-engine.
    /// </summary>
    [Obsolete("HostModelManager is preferred and BridgeManager will be removed.  Currently mid-refactor.")]
    internal class BridgeManager
    {
        private JavaScriptEngine engine_;
        private Dictionary<Type, ClassBridge> classBridges_;

        public BridgeManager(JavaScriptEngine engine)
        {
            Debug.Assert(engine != null);
            if (engine == null)
                throw new ArgumentNullException(nameof(engine));

            engine_ = engine;
            classBridges_ = new Dictionary<Type, ClassBridge>();
        }

        public ClassBridge GetBridge(Type type)
        {
            ClassBridge result;
            if (classBridges_.TryGetValue(type, out result))
            {
                result.AddRef();
            }
            else
            {
                result = new ClassBridge(type, this);
                classBridges_.Add(type, result);
            }

            return result;
        }

        internal void ReleaseBridge(Type bridgeType)
        {
            classBridges_.Remove(bridgeType);
        }

        public JavaScriptEngine Engine
        {
            get { return engine_; }
        }
    }
}
