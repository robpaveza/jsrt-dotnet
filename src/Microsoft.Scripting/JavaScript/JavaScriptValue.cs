using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public class JavaScriptValue : IDisposable
    {
        internal JavaScriptValueSafeHandle handle_;
        internal JavaScriptValueType type_;
        internal WeakReference<JavaScriptEngine> engine_;
        internal JavaScriptEngine GetEngine()
        {
            JavaScriptEngine result;
            if (!engine_.TryGetTarget(out result))
                throw new ObjectDisposedException(nameof(JavaScriptEngine));

            return result;
        }

        internal JavaScriptEngine GetEngineAndClaimContext()
        {
            var result = GetEngine();
            result.ClaimContext();

            return result;
        }

        internal JavaScriptValue(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine)
        {
            Debug.Assert(handle != null);
            Debug.Assert(engine != null);
            Debug.Assert(Enum.IsDefined(typeof(JavaScriptValueType), type));

            uint count;
            Errors.ThrowIfIs(NativeMethods.JsAddRef(handle.DangerousGetHandle(), out count));

            handle_ = handle;
            type_ = type;
            engine_ = new WeakReference<JavaScriptEngine>(engine);
        }

        public override string ToString()
        {
            var engine = GetEngine();
            return engine.Converter.ToString(this);
        }

        public JavaScriptValueType Type
        {
            get { return type_; }
        }

        public bool IsTruthy
        {
            get
            {
                var engine = GetEngine();
                return engine.Converter.ToBoolean(this);
            }
        }

        public bool SimpleEquals(JavaScriptValue other)
        {
            var eng = GetEngineAndClaimContext();
            bool result;
            Errors.ThrowIfIs(NativeMethods.JsEquals(this.handle_, other.handle_, out result));

            return result;
        }

        public bool StrictEquals(JavaScriptValue other)
        {
            var eng = GetEngineAndClaimContext();
            bool result;
            Errors.ThrowIfIs(NativeMethods.JsStrictEquals(this.handle_, other.handle_, out result));

            return result;
        }

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (handle_ != null)
                {
                    handle_.Dispose();
                    handle_ = null;
                }
            }
        }

        ~JavaScriptValue()
        {
            Dispose(false);
        }
        #endregion
    }
}
