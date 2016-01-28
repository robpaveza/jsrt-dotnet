using Microsoft.Scripting.JavaScript;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.HostBridge
{
    internal class ObjectModel
    {
        private ModelPrototype bridge_;
        private ModelPrototype baseType_;
        private object target_;
        private JavaScriptObject jsObj_;
        private HostModelManager manager_;
        private JavaScriptFunction Constructor;

        public ObjectModel(object target, HostModelManager manager)
        {
            Debug.Assert(target != null && manager != null);
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            manager_ = manager;
            bridge_ = manager.GetBridge(target); // Can't fail
            target_ = target;
            baseType_ = bridge_.GetBaseTypeModel();

            InitializeModel();
        }

        private void InitializeModel()
        {
            var engine = manager_.Engine;

            // var MyObject = function() { [native code] };
            // todo: project constructor using constructor model
            Constructor = engine.CreateFunction((eng, construct, thisVal, args) =>
            {
                return eng.UndefinedValue;
            }, bridge_.FullTypeName);

            // MyObject.prototype = Object.create(baseTypeProjection.Prototype);
            if (baseType_ != null)
            {

            }
        }
    }
}
