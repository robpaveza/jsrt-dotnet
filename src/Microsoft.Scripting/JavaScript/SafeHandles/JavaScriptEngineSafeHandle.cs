using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript.SafeHandles
{
    internal class JavaScriptEngineSafeHandle : SafeHandle
    {
        public JavaScriptEngineSafeHandle() :
            base(IntPtr.Zero, ownsHandle: true)
        {

        }

        public JavaScriptEngineSafeHandle(IntPtr handle):
            base(handle, true)
        {

        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;

            uint count;
            var error = ChakraApi.Instance.JsRelease(handle, out count);

            Debug.Assert(error == JsErrorCode.JsNoError);
            return true;
        }
    }
}
