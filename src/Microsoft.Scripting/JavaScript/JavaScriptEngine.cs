using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.JavaScript
{
    public delegate void JavaScriptExternalObjectFinalizeCallback(object additionalData);
    public delegate JavaScriptValue JavaScriptCallableFunction(JavaScriptEngine callingEngine, bool asConstructor, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> arguments);

    public sealed class JavaScriptEngine : IDisposable
    {
        private class NativeFunctionThunkData
        {
            public JavaScriptCallableFunction callback;
            public WeakReference<JavaScriptEngine> engine;
        }
        private JavaScriptEngineSafeHandle handle_;
        private WeakReference<JavaScriptRuntime> runtime_;
        private JavaScriptConverter converter_;
        private List<NativeFunctionThunkData> nativeFunctionThunks_;
        private static NativeFunctionThunkCallback NativeCallback;
        private static IntPtr NativeCallbackPtr;

        static JavaScriptEngine()
        {
            NativeCallback = NativeCallbackThunk;
            NativeCallbackPtr = Marshal.GetFunctionPointerForDelegate(NativeCallback);
        }

        internal JavaScriptEngine(JavaScriptEngineSafeHandle handle, JavaScriptRuntime runtime)
        {
            Debug.Assert(handle != null);
            Debug.Assert(runtime != null);

            handle_ = handle;
            runtime_ = new WeakReference<JavaScriptRuntime>(runtime);
            converter_ = new JavaScriptConverter(this);
            nativeFunctionThunks_ = new List<NativeFunctionThunkData>();
        }

        public JavaScriptRuntime Runtime
        {
            get
            {
                JavaScriptRuntime result;
                if (!runtime_.TryGetTarget(out result))
                    throw new ObjectDisposedException(nameof(JavaScriptEngine));

                return result;
            }
        }

        public JavaScriptConverter Converter
        {
            get
            {
                return converter_;
            }
        }

        #region Internal methods
        internal void ClaimContext()
        {
            if (handle_ == null)
                throw new ObjectDisposedException(nameof(JavaScriptEngine));

            Errors.ThrowIfIs(NativeMethods.JsSetCurrentContext(handle_));
        }

        internal JavaScriptValue CreateValueFromHandle(JavaScriptValueSafeHandle handle)
        {
            ClaimContext();

            JsValueType kind;
            Errors.ThrowIfIs(NativeMethods.JsGetValueType(handle, out kind));

            switch (kind)
            {
                case JsValueType.JsArray:
                    return new JavaScriptArray(handle, JavaScriptValueType.Array, this);

                case JsValueType.JsFunction:
                    return new JavaScriptFunction(handle, JavaScriptValueType.Function, this);

                case JsValueType.JsObject:
                case JsValueType.JsNull:
                case JsValueType.JsError:
                    return new JavaScriptObject(handle, JavaScriptValueType.Object, this);

                case JsValueType.JsSymbol:
                    return new JavaScriptSymbol(handle, JavaScriptValueType.Symbol, this);

                case JsValueType.JsArrayBuffer:
                    return new JavaScriptArrayBuffer(handle, JavaScriptValueType.ArrayBuffer, this);

                case JsValueType.JsTypedArray:
                    return new JavaScriptTypedArray(handle, JavaScriptValueType.TypedArray, this);

                case JsValueType.JsDataView:
                    return new JavaScriptDataView(handle, JavaScriptValueType.DataView, this);

                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                default:
                    return new JavaScriptValue(handle, kind.ToApiValueType(), this);
            }
        }

        internal JavaScriptObject CreateObjectFromHandle(JavaScriptValueSafeHandle handle)
        {
            ClaimContext();

            JsValueType kind;
            Errors.ThrowIfIs(NativeMethods.JsGetValueType(handle, out kind));

            switch (kind)
            {
                case JsValueType.JsArray:
                    return new JavaScriptArray(handle, JavaScriptValueType.Array, this);

                case JsValueType.JsFunction:
                    return new JavaScriptFunction(handle, JavaScriptValueType.Function, this);

                case JsValueType.JsObject:
                case JsValueType.JsError:
                case JsValueType.JsNull:
                    return new JavaScriptObject(handle, JavaScriptValueType.Object, this);

                case JsValueType.JsArrayBuffer:
                    return new JavaScriptArrayBuffer(handle, JavaScriptValueType.ArrayBuffer, this);

                case JsValueType.JsTypedArray:
                    return new JavaScriptTypedArray(handle, JavaScriptValueType.TypedArray, this);

                case JsValueType.JsDataView:
                    return new JavaScriptDataView(handle, JavaScriptValueType.DataView, this);

                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                case JsValueType.JsSymbol:
                default:
                    throw new ArgumentException();
            }
        }

        internal JavaScriptArray CreateArrayFromHandle(JavaScriptValueSafeHandle handle)
        {
            ClaimContext();

            JsValueType kind;
            Errors.ThrowIfIs(NativeMethods.JsGetValueType(handle, out kind));

            switch (kind)
            {
                case JsValueType.JsArray:
                    return new JavaScriptArray(handle, JavaScriptValueType.Array, this);

                case JsValueType.JsFunction:
                case JsValueType.JsObject:
                case JsValueType.JsError:
                case JsValueType.JsNull:
                case JsValueType.JsArrayBuffer:
                case JsValueType.JsTypedArray:
                case JsValueType.JsDataView:
                case JsValueType.JsBoolean:
                case JsValueType.JsNumber:
                case JsValueType.JsString:
                case JsValueType.JsUndefined:
                case JsValueType.JsSymbol:
                default:
                    throw new ArgumentException();
            }
        }
        #endregion

        #region Base properties
        public JavaScriptObject GlobalObject
        {
            get
            {
                ClaimContext();

                JavaScriptValueSafeHandle global;
                Errors.ThrowIfIs(NativeMethods.JsGetGlobalObject(out global));

                return CreateObjectFromHandle(global);
            }
        }

        public JavaScriptValue UndefinedValue
        {
            get
            {
                ClaimContext();

                JavaScriptValueSafeHandle global;
                Errors.ThrowIfIs(NativeMethods.JsGetUndefinedValue(out global));

                return CreateValueFromHandle(global);
            }
        }

        public JavaScriptObject NullValue
        {
            get
            {
                ClaimContext();

                JavaScriptValueSafeHandle global;
                Errors.ThrowIfIs(NativeMethods.JsGetNullValue(out global));

                return CreateObjectFromHandle(global);
            }
        }

        public JavaScriptValue TrueValue
        {
            get
            {
                ClaimContext();

                JavaScriptValueSafeHandle global;
                Errors.ThrowIfIs(NativeMethods.JsGetTrueValue(out global));

                return CreateValueFromHandle(global);
            }
        }

        public JavaScriptValue FalseValue
        {
            get
            {
                ClaimContext();

                JavaScriptValueSafeHandle global;
                Errors.ThrowIfIs(NativeMethods.JsGetFalseValue(out global));

                return CreateValueFromHandle(global);
            }
        }

        public bool HasException
        {
            get
            {
                ClaimContext();

                bool has;
                Errors.ThrowIfIs(NativeMethods.JsHasException(out has));

                return has;
            }
        }
        #endregion

        #region Code execution
        public JavaScriptFunction EvaluateScriptText(string code)
        {
            // todo
            return null;
        }

        public JavaScriptFunction Evaluate(ScriptSource source)
        {
            // todo
            return null;
        }

        public JavaScriptFunction Evaluate(ScriptSource source, Stream compiledCode)
        {
            // todo
            return null;
        }

        public void Compile(ScriptSource source, Stream compiledCodeDestination)
        {
            // todo
        }

        public JavaScriptValue Execute(ScriptSource source)
        {
            // todo
            return null;
        }

        public JavaScriptValue Execute(ScriptSource source, Stream compiledCode)
        {
            // todo
            return null;
        }
        #endregion

        #region Main interaction functions
        public JavaScriptObject CreateObject(JavaScriptObject prototype = null)
        {
            ClaimContext();

            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(NativeMethods.JsCreateObject(out handle));

            if (prototype != null)
            {
                Errors.ThrowIfIs(NativeMethods.JsSetPrototype(handle, prototype.handle_));
            }

            return CreateObjectFromHandle(handle);
        }

        public JavaScriptObject CreateExternalObject(object externalData, JavaScriptExternalObjectFinalizeCallback finalizeCallback)
        {
            // todo
            return null;
        }

        public JavaScriptSymbol CreateSymbol(string description)
        {
            JavaScriptValueSafeHandle handle;
            using (var str = converter_.FromString(description))
            {
                Errors.ThrowIfIs(NativeMethods.JsCreateSymbol(str.handle_, out handle));
            }

            return CreateValueFromHandle(handle) as JavaScriptSymbol;
        }

        public TimeSpan RunIdleWork()
        {
            ClaimContext();
            uint nextTick;
            Errors.ThrowIfIs(NativeMethods.JsIdle(out nextTick));

            return TimeSpan.FromTicks(nextTick);
        }

        public bool HasGlobalVariable(string name)
        {
            return GlobalObject.HasOwnProperty(name);
        }

        public JavaScriptValue GetGlobalVariable(string name)
        {
            return GlobalObject.GetPropertyByName(name);
        }

        public void SetGlobalVariable(string name, JavaScriptValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            GlobalObject.SetPropertyByName(name, value);
        }

        public JavaScriptValue CallGlobalFunction(string functionName)
        {
            return CallGlobalFunction(functionName, Enumerable.Empty<JavaScriptValue>());
        }

        public JavaScriptValue CallGlobalFunction(string functionName, IEnumerable<JavaScriptValue> args)
        {
            var global = GlobalObject;
            var fn = global.GetPropertyByName(functionName) as JavaScriptFunction;
            return fn.Call(global, args);
        }

        public void SetGlobalFunction(string functionName, JavaScriptCallableFunction hostFunction)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));

            GlobalObject.SetPropertyByName(functionName, CreateFunction(hostFunction, functionName));
        }

        public JavaScriptArray CreateArray(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            ClaimContext();

            JavaScriptValueSafeHandle handle;
            Errors.ThrowIfIs(NativeMethods.JsCreateArray(unchecked((uint)length), out handle));

            return CreateArrayFromHandle(handle);
        }

        private static JavaScriptValueSafeHandle NativeCallbackThunk(
            JavaScriptValueSafeHandle callee, 
            [MarshalAs(UnmanagedType.U1)] bool asConstructor, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] JavaScriptValueSafeHandle[] args, 
            ushort argCount, 
            IntPtr data)
        {
            // callee and args[0] are the same
            if (data == IntPtr.Zero)
                return new JavaScriptValueSafeHandle(IntPtr.Zero);

            GCHandle handle = GCHandle.FromIntPtr(data);
            var nativeThunk = handle.Target as NativeFunctionThunkData;
            JavaScriptEngine engine;
            if (!nativeThunk.engine.TryGetTarget(out engine))
                return new JavaScriptValueSafeHandle(IntPtr.Zero);

            JavaScriptValue thisValue = null;
            if (argCount > 0)
            {
                thisValue = engine.CreateValueFromHandle(args[0]);
            }

            try
            {
                var result = nativeThunk.callback(engine, asConstructor, thisValue, args.Skip(1).Select(h => engine.CreateValueFromHandle(h)));
                return result.handle_;
            }
            catch (Exception ex)
            {
                var error = engine.CreateError(ex.Message);
                engine.SetException(error);

                return engine.UndefinedValue.handle_;
            }
        }

        public JavaScriptFunction CreateFunction(JavaScriptCallableFunction hostFunction)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));

            ClaimContext();

            NativeFunctionThunkData td = new NativeFunctionThunkData() { callback = hostFunction, engine = new WeakReference<JavaScriptEngine>(this) };
            GCHandle handle = GCHandle.Alloc(td, GCHandleType.Weak);
            nativeFunctionThunks_.Add(td);

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsCreateFunction(NativeCallbackPtr, GCHandle.ToIntPtr(handle), out resultHandle));

            return CreateObjectFromHandle(resultHandle) as JavaScriptFunction;
        }

        public JavaScriptFunction CreateFunction(JavaScriptCallableFunction hostFunction, string name)
        {
            if (hostFunction == null)
                throw new ArgumentNullException(nameof(hostFunction));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            ClaimContext();

            var nameVal = Converter.FromString(name);

            NativeFunctionThunkData td = new NativeFunctionThunkData() { callback = hostFunction, engine = new WeakReference<JavaScriptEngine>(this) };
            GCHandle handle = GCHandle.Alloc(td, GCHandleType.Weak);
            nativeFunctionThunks_.Add(td);

            JavaScriptValueSafeHandle resultHandle;
            Errors.ThrowIfIs(NativeMethods.JsCreateNamedFunction(nameVal.handle_, NativeCallbackPtr, GCHandle.ToIntPtr(handle), out resultHandle));

            return CreateObjectFromHandle(resultHandle) as JavaScriptFunction;
        }

        public JavaScriptValue GetAndClearException()
        {
            // todo
            return null;
        }

        public void SetException(JavaScriptValue exception)
        {
            // todo
        }
        #endregion

        #region Errors
        public JavaScriptObject CreateError(string message)
        {
            // todo
            return null;
        }

        public JavaScriptObject CreateRangeError(string message)
        {
            // todo
            return null;
        }

        public JavaScriptObject CreateReferenceError(string message)
        {
            // todo
            return null;
        }

        public JavaScriptObject CreateSyntaxError(string message)
        {
            // todo
            return null;
        }

        public JavaScriptObject CreateTypeError(string message)
        {
            // todo
            return null;
        }

        public JavaScriptObject CreateUriError(string message)
        {
            // todo
            return null;
        }
        #endregion

        public void EnableDebugging()
        {
            // todo
        }

        #region IDisposable implementation
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

        ~JavaScriptEngine()
        {
            Dispose(false);
        }
        #endregion
    }
}
