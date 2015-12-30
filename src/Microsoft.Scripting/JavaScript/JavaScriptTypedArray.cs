using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptTypedArray : JavaScriptObject
    {
        private Lazy<JavaScriptTypedArrayType> arrayType_;
        internal JavaScriptTypedArray(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {
            arrayType_ = new Lazy<JavaScriptTypedArrayType>(GetArrayType);
        }

        public JavaScriptArrayBuffer Buffer
        {
            get
            {
                return GetPropertyByName("buffer") as JavaScriptArrayBuffer;
            }
        }

        public uint ByteLength
        {
            get
            {
                var eng = GetEngine();
                var val = GetPropertyByName("byteLength");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public uint ByteOffset
        {
            get
            {
                var eng = GetEngine();
                var val = GetPropertyByName("byteOffset");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public unsafe Stream GetUnderlyingMemory()
        {
            var buf = Buffer;
            Debug.Assert(buf != null);

            var mem = buf.GetUnderlyingMemoryInfo();
            byte* pMem = (byte*)mem.Item1.ToPointer();

            return new UnmanagedMemoryStream(pMem + ByteOffset, ByteLength);
        }

        public uint Length
        {
            get
            {
                var eng = GetEngine();
                var val = GetPropertyByName("length");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public JavaScriptTypedArrayType ArrayType
        {
            get
            {
                return arrayType_.Value;
            }
        }

        private JavaScriptTypedArrayType GetArrayType()
        {
            GetEngine();
            IntPtr buf;
            uint len;
            JavaScriptTypedArrayType type;
            int elemSize;

            Errors.ThrowIfIs(api_.JsGetTypedArrayStorage(handle_, out buf, out len, out type, out elemSize));

            return type;
        }
    }
}
