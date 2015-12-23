using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptRuntime : IDisposable
    {
        private JavaScriptRuntimeSettings settings_;
        private JavaScriptRuntimeSafeHandle handle_;
        
        public JavaScriptRuntime(JavaScriptRuntimeSettings settings = null)
        {
            if (settings == null)
                settings = new JavaScriptRuntimeSettings();

            var attrs = settings.GetRuntimeAttributes();

            var errorCode = NativeMethods.JsCreateRuntime(attrs, IntPtr.Zero, out handle_);
            if (errorCode != JsErrorCode.JsNoError)
                Errors.ThrowFor(errorCode);

            settings.Used = true;

            GCHandle handle = GCHandle.Alloc(this, GCHandleType.Weak);
            errorCode = NativeMethods.JsSetRuntimeMemoryAllocationCallback(handle_, GCHandle.ToIntPtr(handle), MemoryCallbackThunkPtr);
            if (errorCode != JsErrorCode.JsNoError)
                Errors.ThrowFor(errorCode);
        }

        public void CollectGarbage()
        {
            if (handle_ == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = NativeMethods.JsCollectGarbage(handle_);
            Errors.ThrowIfIs(error);
        }

        public JavaScriptEngine CreateEngine()
        {
            if (handle_ == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            JavaScriptEngineSafeHandle engine;
            var error = NativeMethods.JsCreateContext(handle_, out engine);
            Errors.ThrowIfIs(error);

            return new JavaScriptEngine(engine, this);
        }

        public void EnableExecution()
        {
            if (handle_ == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = NativeMethods.JsEnableRuntimeExecution(handle_);
            Errors.ThrowIfIs(error);
        }

        public void DisableExecution()
        {
            if (handle_ == null)
                throw new ObjectDisposedException(nameof(JavaScriptRuntime));

            var error = NativeMethods.JsDisableRuntimeExecution(handle_);
            Errors.ThrowIfIs(error);
        }

        public ulong RuntimeMemoryUsage
        {
            get
            {
                if (handle_ == null)
                    throw new ObjectDisposedException(nameof(JavaScriptRuntime));

                ulong result;
                var error = NativeMethods.JsGetRuntimeMemoryUsage(handle_, out result);
                Errors.ThrowIfIs(error);

                return result;
            }
        }

        public bool IsExecutionEnabled
        {
            get
            {
                if (handle_ == null)
                    throw new ObjectDisposedException(nameof(JavaScriptRuntime));

                bool result;
                var error = NativeMethods.JsIsRuntimeExecutionDisabled(handle_, out result);
                Errors.ThrowIfIs(error);

                return !result;
            }
        }

        public event EventHandler<JavaScriptMemoryAllocationEventArgs> MemoryChanging;
        private void OnMemoryChanging(JavaScriptMemoryAllocationEventArgs args)
        {
            var changing = MemoryChanging;
            if (changing != null)
            {
                changing(this, args);
            }
        }

        #region Disposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
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

        ~JavaScriptRuntime()
        {
            Dispose(false);
        }
        #endregion

        #region Memory callback implementation
        static JavaScriptRuntime()
        {
            MemoryCallbackThunkDelegate = MemoryCallbackThunk;
            MemoryCallbackThunkPtr = Marshal.GetFunctionPointerForDelegate(MemoryCallbackThunkDelegate);
        }

        private static bool MemoryCallbackThunk(IntPtr callbackState, JavaScriptMemoryAllocationEventType allocationEvent, ulong allocationSize)
        {
            GCHandle handle = GCHandle.FromIntPtr(callbackState);
            JavaScriptRuntime runtime = handle.Target as JavaScriptRuntime;
            if (runtime == null)
            {
                Debug.Assert(false, "Runtime has been freed.");
                return false;
            }

            var args = new JavaScriptMemoryAllocationEventArgs(allocationSize, allocationEvent);
            runtime.OnMemoryChanging(args);

            if (args.IsCancelable && args.Cancel)
                return false;

            return true;
        }
        private static IntPtr MemoryCallbackThunkPtr;
        private static MemoryCallbackThunkCallback MemoryCallbackThunkDelegate;
        #endregion
    }
}
