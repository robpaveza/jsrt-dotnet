using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public class JavaScriptObject : JavaScriptValue
    {
        internal JavaScriptObject(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {

        }

        public JavaScriptArray Keys
        {
            get
            {
                var eng = GetEngineAndClaimContext();
                var fn = GetObjectBuiltinFunction("keys", "Object.keys");
                return fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }) as JavaScriptArray;
            }
        }

        public bool IsExtensible
        {
            get
            {
                var eng = GetEngineAndClaimContext();

                bool result;
                Errors.ThrowIfIs(NativeMethods.JsGetExtensionAllowed(handle_, out result));

                return result;
            }
        }

        public JavaScriptObject Prototype
        {
            get
            {
                var eng = GetEngineAndClaimContext();

                JavaScriptValueSafeHandle handle;
                Errors.ThrowIfIs(NativeMethods.JsGetPrototype(handle_, out handle));

                return eng.CreateObjectFromHandle(handle);
            }
            set
            {
                var eng = GetEngineAndClaimContext();
                if (value == null)
                    value = eng.NullValue;

                Errors.ThrowIfIs(NativeMethods.JsSetPrototype(handle_, value.handle_));
            }
        }

        public bool IsSealed
        {
            get
            {
                var eng = GetEngineAndClaimContext();
                var fn = GetObjectBuiltinFunction("isSealed", "Object.isSealed");

                return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }));
            }
        }

        public bool IsFrozen
        {
            get
            {
                var eng = GetEngineAndClaimContext();
                var fn = GetObjectBuiltinFunction("isFrozen", "Object.isFrozen");

                return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }));
            }
        }

        internal JavaScriptFunction GetBuiltinFunctionProperty(string functionName, string nameIfNotFound)
        {
            var fn = GetPropertyByName(functionName) as JavaScriptFunction;
            if (fn == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, nameIfNotFound);

            return fn;
        }

        internal JavaScriptFunction GetObjectBuiltinFunction(string functionName, string nameIfNotFound)
        {
            var eng = GetEngineAndClaimContext();
            var obj = eng.GlobalObject.GetPropertyByName("Object") as JavaScriptFunction;
            if (obj == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, "Object");
            var fn = obj.GetPropertyByName(functionName) as JavaScriptFunction;
            if (fn == null)
                Errors.ThrowIOEFmt(Errors.DefaultFnOverwritten, nameIfNotFound);

            return fn;
        }

        public bool IsPrototypeOf(JavaScriptObject other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            var eng = GetEngineAndClaimContext();
            var fn = GetBuiltinFunctionProperty("isPrototypeOf", "Object.prototype.isPrototypeOf");

            var args = new List<JavaScriptValue>() { this, other };

            return eng.Converter.ToBoolean(fn.Invoke(args));
        }

        public bool PropertyIsEnumerable(string propertyName)
        {
            var eng = GetEngineAndClaimContext();
            var fn = GetBuiltinFunctionProperty("propertyIsEnumerable", "Object.prototype.propertyIsEnumerable");
            using (var jsPropName = eng.Converter.FromString(propertyName))
            {
                var args = new List<JavaScriptValue>() { this, jsPropName };
                return eng.Converter.ToBoolean(fn.Invoke(args));
            }
        }

        public JavaScriptValue GetPropertyByName(string propertyName)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsGetProperty(handle_, propId, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetPropertyByName(string propertyName, JavaScriptValue value)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));
            Errors.ThrowIfIs(NativeMethods.JsSetProperty(handle_, propId, value.handle_, false));
        }
        
        public void DeletePropertyByName(string propertyName)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));

            JavaScriptValueSafeHandle tmpResult;
            Errors.ThrowIfIs(NativeMethods.JsDeleteProperty(handle_, propId, false, out tmpResult));
            tmpResult.Dispose();
        }

        public JavaScriptValue GetPropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsGetProperty(handle_, propId, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetPropertyBySymbol(JavaScriptSymbol symbol, JavaScriptValue value)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));
            Errors.ThrowIfIs(NativeMethods.JsSetProperty(handle_, propId, value.handle_, false));
        }

        public void DeletePropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));

            JavaScriptValueSafeHandle tmpResult;
            Errors.ThrowIfIs(NativeMethods.JsDeleteProperty(handle_, propId, false, out tmpResult));
            tmpResult.Dispose();
        }

        public JavaScriptValue GetValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetEngineAndClaimContext();
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(NativeMethods.JsGetIndexedProperty(handle_, index.handle_, out result));

            return eng.CreateValueFromHandle(result);
        }

        public void SetValueAtIndex(JavaScriptValue index, JavaScriptValue value)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetEngineAndClaimContext();
            if (value == null)
                value = eng.NullValue;

            Errors.ThrowIfIs(NativeMethods.JsSetIndexedProperty(handle_, index.handle_, value.handle_));
        }

        public void DeleteValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            GetEngineAndClaimContext(); // unused but the call is needed to claim the context
            Errors.ThrowIfIs(NativeMethods.JsDeleteIndexedProperty(handle_, index.handle_));
        }

        public bool HasOwnProperty(string propertyName)
        {
            var eng = GetEngineAndClaimContext();
            var fn = GetBuiltinFunctionProperty("hasOwnProperty", "Object.prototype.hasOwnProperty");

            return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromString(propertyName) }));
        }

        public bool HasProperty(string propertyName)
        {
            GetEngineAndClaimContext();
            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));
            bool has;
            Errors.ThrowIfIs(NativeMethods.JsHasProperty(handle_, propId, out has));

            return has;
        }

        public JavaScriptObject GetOwnPropertyDescriptor(string propertyName)
        {
            var eng = GetEngineAndClaimContext();
            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));
            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsGetOwnPropertyDescriptor(handle_, propId, out resultHandle));

            return eng.CreateObjectFromHandle(resultHandle);
        }

        public void DefineProperty(string propertyName, JavaScriptObject descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var eng = GetEngineAndClaimContext();

            IntPtr propId;
            Errors.ThrowIfIs(NativeMethods.JsGetPropertyIdFromName(propertyName, out propId));

            bool wasSet;
            Errors.ThrowIfIs(NativeMethods.JsDefineProperty(handle_, propId, descriptor.handle_, out wasSet));
        }

        public void DefineProperties(JavaScriptObject propertiesContainer)
        {
            var eng = GetEngineAndClaimContext();
            var fnDP = GetObjectBuiltinFunction("defineProperties", "Object.defineProperties");

            fnDP.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this, propertiesContainer });
        }

        public JavaScriptArray GetOwnPropertyNames()
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsGetOwnPropertyNames(handle_, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public JavaScriptArray GetOwnPropertySymbols()
        {
            var eng = GetEngineAndClaimContext();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsGetOwnPropertySymbols(handle_, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public void PreventExtensions()
        {
            GetEngineAndClaimContext();

            Errors.ThrowIfIs(NativeMethods.JsPreventExtension(handle_));
        }

        public void Seal()
        {
            var eng = GetEngineAndClaimContext();
            var fn = GetObjectBuiltinFunction("seal", "Object.seal");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

        public void Freeze()
        {
            var eng = GetEngineAndClaimContext();
            var fn = GetObjectBuiltinFunction("freeze", "Object.freeze");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

    }
}
