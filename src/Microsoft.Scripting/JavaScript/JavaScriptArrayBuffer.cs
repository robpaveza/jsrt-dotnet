using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptArrayBuffer : JavaScriptObject
    {
        private Lazy<uint> len_;

        internal JavaScriptArrayBuffer(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {
            len_ = new Lazy<uint>(GetLength);
        }

        private uint GetLength()
        {
            var eng = GetEngineAndClaimContext();
            IntPtr buffer;
            uint len;
            Errors.ThrowIfIs(api_.JsGetArrayBufferStorage(handle_, out buffer, out len));

            return len;
        }

        public uint ByteLength
        {
            get
            {
                return len_.Value;
            }
        }

        public unsafe Stream GetUnderlyingMemory()
        {
            var eng = GetEngineAndClaimContext();
            IntPtr buffer;
            uint len;
            Errors.ThrowIfIs(api_.JsGetArrayBufferStorage(handle_, out buffer, out len));

            return new UnmanagedMemoryStream((byte*)buffer.ToPointer(), len);
        }
    }
}
