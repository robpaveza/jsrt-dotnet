using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Microsoft.Scripting.JavaScript
{
    public class JavaScriptObject : JavaScriptValue
    {
        internal JavaScriptObject(JavaScriptValueSafeHandle handle, JavaScriptValueType type, JavaScriptEngine engine):
            base(handle, type, engine)
        {

        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public JavaScriptArray Keys
        {
            get
            {
                var eng = GetEngine();
                var fn = GetObjectBuiltinFunction("keys", "Object.keys");
                return fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }) as JavaScriptArray;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsExtensible
        {
            get
            {
                var eng = GetEngine();

                bool result;
                Errors.ThrowIfIs(api_.JsGetExtensionAllowed(handle_, out result));

                return result;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public JavaScriptObject Prototype
        {
            get
            {
                var eng = GetEngine();

                JavaScriptValueSafeHandle handle;
                Errors.ThrowIfIs(api_.JsGetPrototype(handle_, out handle));

                return eng.CreateObjectFromHandle(handle);
            }
            set
            {
                var eng = GetEngine();
                if (value == null)
                    value = eng.NullValue;

                Errors.ThrowIfIs(api_.JsSetPrototype(handle_, value.handle_));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object ExternalObject
        {
            get
            {
                var eng = GetEngine();
                return eng.GetExternalObjectFrom(this);
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsSealed
        {
            get
            {
                var eng = GetEngine();
                var fn = GetObjectBuiltinFunction("isSealed", "Object.isSealed");

                return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this }));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsFrozen
        {
            get
            {
                var eng = GetEngine();
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
            var eng = GetEngine();
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

            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("isPrototypeOf", "Object.prototype.isPrototypeOf");

            var args = new List<JavaScriptValue>() { this, other };

            return eng.Converter.ToBoolean(fn.Invoke(args));
        }

        public bool PropertyIsEnumerable(string propertyName)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("propertyIsEnumerable", "Object.prototype.propertyIsEnumerable");
            using (var jsPropName = eng.Converter.FromString(propertyName))
            {
                var args = new List<JavaScriptValue>() { this, jsPropName };
                return eng.Converter.ToBoolean(fn.Invoke(args));
            }
        }

        public JavaScriptValue GetPropertyByName(string propertyName)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsGetProperty(handle_, propId, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetPropertyByName(string propertyName, JavaScriptValue value)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));
            Errors.ThrowIfIs(api_.JsSetProperty(handle_, propId, value.handle_, false));
        }
        
        public void DeletePropertyByName(string propertyName)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));

            JavaScriptValueSafeHandle tmpResult;
            Errors.ThrowIfIs(api_.JsDeleteProperty(handle_, propId, false, out tmpResult));
            tmpResult.Dispose();
        }

        public JavaScriptValue this[string name]
        {
            get
            {
                return GetPropertyByName(name);
            }
            set
            {
                SetPropertyByName(name, value);
            }
        }

        public JavaScriptValue GetPropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsGetProperty(handle_, propId, out resultHandle));

            return eng.CreateValueFromHandle(resultHandle);
        }

        public void SetPropertyBySymbol(JavaScriptSymbol symbol, JavaScriptValue value)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));
            Errors.ThrowIfIs(api_.JsSetProperty(handle_, propId, value.handle_, false));
        }

        public void DeletePropertyBySymbol(JavaScriptSymbol symbol)
        {
            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromSymbol(symbol.handle_, out propId));

            JavaScriptValueSafeHandle tmpResult;
            Errors.ThrowIfIs(api_.JsDeleteProperty(handle_, propId, false, out tmpResult));
            tmpResult.Dispose();
        }

        public JavaScriptValue this[JavaScriptSymbol symbol]
        {
            get
            {
                return GetPropertyBySymbol(symbol);
            }
            set
            {
                SetPropertyBySymbol(symbol, value);
            }
        }

        public JavaScriptValue GetValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetEngine();
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfIs(api_.JsGetIndexedProperty(handle_, index.handle_, out result));

            return eng.CreateValueFromHandle(result);
        }

        public void SetValueAtIndex(JavaScriptValue index, JavaScriptValue value)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            var eng = GetEngine();
            if (value == null)
                value = eng.NullValue;

            Errors.ThrowIfIs(api_.JsSetIndexedProperty(handle_, index.handle_, value.handle_));
        }

        public void DeleteValueAtIndex(JavaScriptValue index)
        {
            if (index == null)
                throw new ArgumentNullException(nameof(index));

            Errors.ThrowIfIs(api_.JsDeleteIndexedProperty(handle_, index.handle_));
        }

        public JavaScriptValue this[JavaScriptValue index]
        {
            get
            {
                return GetValueAtIndex(index);
            }
            set
            {
                SetValueAtIndex(index, value);
            }
        }

        public bool HasOwnProperty(string propertyName)
        {
            var eng = GetEngine();
            var fn = GetBuiltinFunctionProperty("hasOwnProperty", "Object.prototype.hasOwnProperty");

            return eng.Converter.ToBoolean(fn.Invoke(new JavaScriptValue[] { this, eng.Converter.FromString(propertyName) }));
        }

        public bool HasProperty(string propertyName)
        {
            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));
            bool has;
            Errors.ThrowIfIs(api_.JsHasProperty(handle_, propId, out has));

            return has;
        }

        public JavaScriptObject GetOwnPropertyDescriptor(string propertyName)
        {
            var eng = GetEngine();
            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));
            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsGetOwnPropertyDescriptor(handle_, propId, out resultHandle));

            return eng.CreateObjectFromHandle(resultHandle);
        }

        public void DefineProperty(string propertyName, JavaScriptObject descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException(nameof(descriptor));

            var eng = GetEngine();

            IntPtr propId;
            Errors.ThrowIfIs(api_.JsGetPropertyIdFromName(propertyName, out propId));

            bool wasSet;
            Errors.CheckForScriptExceptionOrThrow(api_.JsDefineProperty(handle_, propId, descriptor.handle_, out wasSet), eng);
        }

        public void DefineProperties(JavaScriptObject propertiesContainer)
        {
            var eng = GetEngine();
            var fnDP = GetObjectBuiltinFunction("defineProperties", "Object.defineProperties");

            fnDP.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this, propertiesContainer });
        }

        public JavaScriptArray GetOwnPropertyNames()
        {
            var eng = GetEngine();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsGetOwnPropertyNames(handle_, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public JavaScriptArray GetOwnPropertySymbols()
        {
            var eng = GetEngine();

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(api_.JsGetOwnPropertySymbols(handle_, out resultHandle));

            return eng.CreateArrayFromHandle(resultHandle);
        }

        public void PreventExtensions()
        {
            Errors.ThrowIfIs(api_.JsPreventExtension(handle_));
        }

        public void Seal()
        {
            var eng = GetEngine();
            var fn = GetObjectBuiltinFunction("seal", "Object.seal");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

        public void Freeze()
        {
            var eng = GetEngine();
            var fn = GetObjectBuiltinFunction("freeze", "Object.freeze");

            fn.Invoke(new JavaScriptValue[] { eng.UndefinedValue, this });
        }

        #region DynamicObject overrides
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var fn = GetPropertyByName(binder.Name) as JavaScriptFunction;
            var eng = GetEngine();
            var c = eng.Converter;

            if (fn != null)
            {
                result = fn.Invoke(args.Select(a => c.FromObject(a)));
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length > 1)
                return base.TryGetIndex(binder, indexes, out result);

            var eng = GetEngine();
            var jsIndex = eng.Converter.FromObject(indexes[0]);
            result = GetValueAtIndex(jsIndex);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1)
                return base.TrySetIndex(binder, indexes, value);

            var eng = GetEngine();
            var jsIndex = eng.Converter.FromObject(indexes[0]);
            var jsVal = eng.Converter.FromObject(value);
            SetValueAtIndex(jsIndex, jsVal);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyByName(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var jsVal = GetEngine().Converter.FromObject(value);
            SetPropertyByName(binder.Name, jsVal);

            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            DeletePropertyByName(binder.Name);
            return true;
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            if (indexes.Length > 1)
                return base.TryDeleteIndex(binder, indexes);

            var jsIndex = GetEngine().Converter.FromObject(indexes[0]);
            DeleteValueAtIndex(jsIndex);

            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Keys.Select(v => v.ToString());
        }
        #endregion

    }
}
