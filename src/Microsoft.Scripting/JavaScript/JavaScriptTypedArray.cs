using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
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
                var eng = GetEngineAndClaimContext();
                var val = GetPropertyByName("byteLength");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public uint ByteOffset
        {
            get
            {
                var eng = GetEngineAndClaimContext();
                var val = GetPropertyByName("byteOffset");
                return (uint)eng.Converter.ToDouble(val);
            }
        }

        public uint Length
        {
            get
            {
                var eng = GetEngineAndClaimContext();
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
            GetEngineAndClaimContext();
            IntPtr buf;
            uint len;
            JavaScriptTypedArrayType type;
            int elemSize;

            Errors.ThrowIfIs(NativeMethods.JsGetTypedArrayStorage(handle_, out buf, out len, out type, out elemSize));

            return type;
        }
    }
}
