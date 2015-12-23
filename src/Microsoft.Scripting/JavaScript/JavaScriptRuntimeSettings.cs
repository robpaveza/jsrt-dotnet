using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptRuntimeSettings
    {
        private bool backgroundWork_;
        private bool allowScriptInterrupt_;
        private bool enableIdle_;
        private bool disableNativeCode_;
        private bool disableEval_;
        private bool used_;


        public JavaScriptRuntimeSettings()
        {

        }

        public bool DisableBackgroundWork
        {
            get { return backgroundWork_; }
            set
            {
                if (used_)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                backgroundWork_ = value;
            }
        }

        public bool AllowScriptInterrupt
        {
            get { return allowScriptInterrupt_; }
            set
            {
                if (used_)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                allowScriptInterrupt_ = value;
            }
        }

        public bool EnableIdle
        {
            get { return enableIdle_; }
            set
            {
                if (used_)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                enableIdle_ = value;
            }
        }

        public bool DisableNativeCode
        {
            get { return disableNativeCode_; }
            set
            {
                if (used_)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                disableNativeCode_ = value;
            }
        }

        public bool DisableEval
        {
            get { return disableEval_; }
            set
            {
                if (used_)
                    throw new InvalidOperationException(Errors.NoMutateJsRuntimeSettings);

                disableEval_ = value;
            }
        }

        internal bool Used
        {
            get { return used_; }
            set
            {
                used_ = value;
            }
        }

        internal JsRuntimeAttributes GetRuntimeAttributes()
        {
            var result = JsRuntimeAttributes.None;
            if (backgroundWork_)
                result |= JsRuntimeAttributes.DisableBackgroundWork;
            if (allowScriptInterrupt_)
                result |= JsRuntimeAttributes.AllowScriptInterrupt;
            if (enableIdle_)
                result |= JsRuntimeAttributes.EnableIdleProcessing;
            if (disableNativeCode_)
                result |= JsRuntimeAttributes.DisableNativeCodeGeneration;
            if (disableEval_)
                result |= JsRuntimeAttributes.DisableEval;

            return result;
        }
    }
}
