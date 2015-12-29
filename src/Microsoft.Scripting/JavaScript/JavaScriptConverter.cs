using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public sealed class JavaScriptConverter
    {
        private WeakReference<JavaScriptEngine> engine_;
        private ChakraApi api_;

        public JavaScriptConverter(JavaScriptEngine engine)
        {
            engine_ = new WeakReference<JavaScriptEngine>(engine);
            api_ = engine.Api;
        }

        private JavaScriptEngine GetEngine()
        {
            JavaScriptEngine result;
            if (!engine_.TryGetTarget(out result))
                throw new ObjectDisposedException(nameof(JavaScriptEngine));

            return result;
        }

        private JavaScriptEngine GetEngineAndClaimContext()
        {
            var result = GetEngine();
            result.ClaimContext();

            return result;
        }

        public bool ToBoolean(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            bool result;
            
            if (value.Type == JavaScriptValueType.Boolean)
            {
                Errors.ThrowIfIs(api_.JsBooleanToBool(value.handle_, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempBool;
                Errors.ThrowIfIs(api_.JsConvertValueToBoolean(value.handle_, out tempBool));
                using (tempBool)
                {
                    Errors.ThrowIfIs(api_.JsBooleanToBool(tempBool, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromBoolean(bool value)
        {
            var eng = GetEngine();
            if (value)
                return eng.TrueValue;

            return eng.FalseValue;
        }

        public double ToDouble(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            double result;

            if (value.Type == JavaScriptValueType.Number)
            {
                Errors.ThrowIfIs(api_.JsNumberToDouble(value.handle_, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempVal;
                Errors.ThrowIfIs(api_.JsConvertValueToNumber(value.handle_, out tempVal));
                using (tempVal)
                {
                    Errors.ThrowIfIs(api_.JsNumberToDouble(tempVal, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromDouble(double value)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(api_.JsDoubleToNumber(value, out result));

            return eng.CreateValueFromHandle(result);
        }

        public int ToInt32(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();

            int result;

            if (value.Type == JavaScriptValueType.Number)
            {
                Errors.ThrowIfIs(api_.JsNumberToInt(value.handle_, out result));
            }
            else
            {
                JavaScriptValueSafeHandle tempVal;
                Errors.ThrowIfIs(api_.JsConvertValueToNumber(value.handle_, out tempVal));
                using (tempVal)
                {
                    Errors.ThrowIfIs(api_.JsNumberToInt(tempVal, out result));
                }
            }

            return result;
        }

        public JavaScriptValue FromInt32(int value)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(api_.JsIntToNumber(value, out result));

            return eng.CreateValueFromHandle(result);
        }

        public unsafe string ToString(JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var eng = GetEngineAndClaimContext();
            if (value.Type == JavaScriptValueType.String)
            {
                void* str;
                uint len;
                Errors.ThrowIfIs(api_.JsStringToPointer(value.handle_, out str, out len));
                if (len > int.MaxValue)
                    throw new OutOfMemoryException("Exceeded maximum string length.");

                return Marshal.PtrToStringUni(new IntPtr(str), unchecked((int)len));
            }
            else if (value.Type == JavaScriptValueType.Symbol)
            {
                // Presently, JsRT doesn't have a way for the host to query the description of a Symbol
                // Using JsConvertValueToString resulted in putting the runtime into an exception state
                // Thus, detect the condition and just return a known string.

                return "(Symbol)";
            }
            else
            {
                JavaScriptValueSafeHandle tempStr;
                Errors.ThrowIfIs(api_.JsConvertValueToString(value.handle_, out tempStr));
                using (tempStr)
                {
                    void* str;
                    uint len;
                    Errors.ThrowIfIs(api_.JsStringToPointer(value.handle_, out str, out len));
                    if (len > int.MaxValue)
                        throw new OutOfMemoryException("Exceeded maximum string length.");

                    return Marshal.PtrToStringUni(new IntPtr(str), unchecked((int)len));
                }
            }
        }

        public unsafe JavaScriptValue FromString(string value)
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle result;
            var encoded = Encoding.Unicode.GetBytes(value);
            fixed (byte* ptr = &encoded[0])
            {
                Errors.ThrowIfIs(api_.JsPointerToString(ptr, value.Length, out result));
            }

            return eng.CreateValueFromHandle(result);
        }

        public JavaScriptValue FromObject(object o)
        {
            var eng = GetEngine();
            if (o == null)
            {
                return eng.NullValue;
            }

            var jsVal = o as JavaScriptValue;
            if (jsVal != null)
                return jsVal;

            Type t = o.GetType();
            if (t == typeof(string))
            {
                return FromString((string)o);
            }
            else if (t == typeof(double) || t == typeof(float))
            {
                return FromDouble((double)o);
            }
            else if (t == typeof(int) || t == typeof(short) || t == typeof(ushort) || t == typeof(byte) || t == typeof(sbyte))
            {
                return FromInt32((int)o);
            }
            else if (t == typeof(uint))
            {
                return FromDouble((uint)o);
            }
            else if (t == typeof(long))
            {
                return FromDouble((long)o);
            }
            else if (t == typeof(bool))
            {
                bool b = (bool)o;
                return b ? eng.TrueValue : eng.FalseValue;
            }
            else
            {
                throw new NotSupportedException("Could not convert a non-primitive type.");
            }
        }
    }
}
