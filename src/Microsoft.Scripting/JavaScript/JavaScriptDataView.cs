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
    public sealed class JavaScriptDataView : JavaScriptObject
    {
        internal JavaScriptDataView(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine) :
            base(handle, type, engine)
        {

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

        /// <summary>
        /// Gets a signed 8-bit integer (byte) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        public short GetInt8(uint byteOffset)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getInt8", "DataView.prototype.getInt8");
            return (short)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset) }));
        }
        /// <summary>
        /// Gets an unsigned 8-bit integer (unsigned byte) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        public byte GetUint8(uint byteOffset)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getUint8", "DataView.prototype.getUint8");
            return (byte)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset) }));
        }
        /// <summary>
        /// Gets a signed 16-bit integer (short) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public short GetInt16(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getInt16", "DataView.prototype.getInt16");
            return (short)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }
        /// <summary>
        /// Gets an unsigned 16-bit integer (unsigned short) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public ushort GetUint16(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getUint16", "DataView.prototype.getUint16");
            return (ushort)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }
        /// <summary>
        /// Gets a signed 32-bit integer (long) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public int GetInt32(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getInt32", "DataView.prototype.getInt32");
            return eng.Converter.ToInt32(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }
        /// <summary>
        /// Gets an unsigned 32-bit integer (unsigned long) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public uint GetUint32(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getUint32", "DataView.prototype.getUint32");
            return (uint)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }
        /// <summary>
        /// Gets a signed 32-bit float (float) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public float GetFloat32(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getFloat32", "DataView.prototype.getFloat32");
            return (float)eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }
        /// <summary>
        /// Gets a signed 64-bit float (double) at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="littleEndian">True to read as little-endian; otherwise read as big-endian.</param>
        public double GetFloat64(uint byteOffset, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("getFloat64", "DataView.prototype.getFloat64");
            return eng.Converter.ToDouble(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian) }));
        }


        /// <summary>
        /// Stores a signed 8-bit integer (byte) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        public void SetInt8(uint byteOffset, short value)
        {
            if (value < -128 || value > 127)
                throw new ArgumentOutOfRangeException(nameof(value));

            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setInt8", "DataView.prototype.setInt8");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores an unsigned 8-bit integer (unsigned byte) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        public void SetUint8(uint byteOffset, byte value)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setUint8", "DataView.prototype.setUint8");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores a signed 16-bit integer (short) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetInt16(uint byteOffset, short value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setInt16", "DataView.prototype.setInt16");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores an unsigned 16-bit integer (ushort) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetUint16(uint byteOffset, ushort value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setUint16", "DataView.prototype.setUint16");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores a signed 32-bit integer (int) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetInt32(uint byteOffset, int value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setInt32", "DataView.prototype.setInt32");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromInt32(value) });
        }
        /// <summary>
        /// Stores an unsigned 32-bit integer (uint) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetUint32(uint byteOffset, uint value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setUint32", "DataView.prototype.setUint32");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores a 32-bit float (float) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetFloat32(uint byteOffset, float value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setFloat32", "DataView.prototype.setFloat32");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromDouble(value) });
        }
        /// <summary>
        /// Stores a 64-bit integer (double) value at the specified byte offset from the start of the view.
        /// </summary>
        /// <param name="byteOffset">The offset from the beginning of the DataView's view of the underlying <c>ArrayBuffer</c>.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="littleEndian">True to store as little-endian; otherwise store as big-endian.</param>
        public void SetFloat64(uint byteOffset, double value, bool littleEndian = false)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("setFloat64", "DataView.prototype.setFloat64");
            fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromDouble(byteOffset), eng.Converter.FromBoolean(littleEndian), eng.Converter.FromDouble(value) });
        }
    }
}
