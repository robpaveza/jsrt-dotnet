using Microsoft.Scripting.JavaScript;
using Microsoft.Scripting.JavaScript.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    /*internal static class NativeMethods
    {
        [DllImport("chakra")]
        internal static extern JsErrorCode JsAddRef(IntPtr @ref, out uint count);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsRelease(IntPtr @ref, out uint count);

        #region Runtime functions
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDisposeRuntime(IntPtr runtime);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateRuntime(JsRuntimeAttributes attributes, IntPtr threadService, out JavaScriptRuntimeSafeHandle runtime);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, IntPtr pfnCallback);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCollectGarbage(JavaScriptRuntimeSafeHandle runtime);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong usage);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptEngineSafeHandle engine);
        #endregion

        #region Context functions
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetCurrentContext(JavaScriptEngineSafeHandle contextHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsBooleanToBool(
            JavaScriptValueSafeHandle valueRef, 
            [MarshalAs(UnmanagedType.U1)] out bool result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsConvertValueToBoolean(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsNumberToDouble(JavaScriptValueSafeHandle valueRef, out double result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDoubleToNumber(double value, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsConvertValueToNumber(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetValueType(JavaScriptValueSafeHandle value, out JsValueType kind);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsNumberToInt(JavaScriptValueSafeHandle valueRef, out int result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsIntToNumber(int value, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static unsafe extern JsErrorCode JsPointerToString(void* psz, int length, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static unsafe extern JsErrorCode JsStringToPointer(JavaScriptValueSafeHandle str, out void* result, out uint strLen);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsConvertValueToString(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsEquals(JavaScriptValueSafeHandle obj1, JavaScriptValueSafeHandle obj2, [MarshalAs(UnmanagedType.U1)] out bool result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsStrictEquals(JavaScriptValueSafeHandle obj1, JavaScriptValueSafeHandle obj2, [MarshalAs(UnmanagedType.U1)] out bool result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCallFunction(
            JavaScriptValueSafeHandle fnHandle,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] JavaScriptValueSafeHandle[] arguments,
            ushort argCount,
            out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsConstructObject(
            JavaScriptValueSafeHandle fnHandle,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] JavaScriptValueSafeHandle[] arguments,
            ushort argCount,
            out JavaScriptValueSafeHandle result);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetPropertyIdFromName(
            [MarshalAs(UnmanagedType.LPWStr)] string propertyName, 
            out IntPtr propertyId);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle valueHandle, out IntPtr propertyId);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, JavaScriptValueSafeHandle propertyValue, [MarshalAs(UnmanagedType.U1)] bool useStrictSemantics);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDeleteProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, [MarshalAs(UnmanagedType.U1)] bool useStrictSemantics, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDeleteIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsHasProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, [MarshalAs(UnmanagedType.U1)] out bool has);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle obj, IntPtr propertyId, out JavaScriptValueSafeHandle descriptor);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsDefineProperty(JavaScriptValueSafeHandle obj, IntPtr propId, JavaScriptValueSafeHandle descriptorRef, [MarshalAs(UnmanagedType.U1)] out bool wasSet);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetOwnPropertyNames(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetOwnPropertySymbols(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle result);


        [DllImport("chakra")]
        internal static extern JsErrorCode JsPreventExtension(JavaScriptValueSafeHandle obj);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetExtensionAllowed(JavaScriptValueSafeHandle obj, [MarshalAs(UnmanagedType.U1)] out bool allowed);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetPrototype(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle prototype);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetPrototype(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle prototype);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetArrayBufferStorage(JavaScriptValueSafeHandle obj, out IntPtr buffer, out uint len);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetTypedArrayStorage(JavaScriptValueSafeHandle obj, out IntPtr buf, out uint len, out JavaScriptTypedArrayType type, out int elemSize);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetGlobalObject(out JavaScriptValueSafeHandle globalHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetUndefinedValue(out JavaScriptValueSafeHandle globalHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetNullValue(out JavaScriptValueSafeHandle globalHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetTrueValue(out JavaScriptValueSafeHandle globalHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetFalseValue(out JavaScriptValueSafeHandle globalHandle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsHasException([MarshalAs(UnmanagedType.U1)] out bool has);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateObject(out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateExternalObject(IntPtr data, IntPtr finalizeCallback, out JavaScriptValueSafeHandle handle);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateSymbol(JavaScriptValueSafeHandle descriptionHandle, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateArray(uint length, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateFunction(IntPtr callback, IntPtr extraData, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateNamedFunction(JavaScriptValueSafeHandle nameHandle, IntPtr callback, IntPtr extraData, out JavaScriptValueSafeHandle result);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsIdle(out uint nextTick);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsGetAndClearException(out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsSetException(JavaScriptValueSafeHandle exception);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);
        [DllImport("chakra")]
        internal static extern JsErrorCode JsStartDebugging();
        #endregion

        #region Execution functions
        [DllImport("chakra")]
        internal static extern JsErrorCode JsParseScript(
            [MarshalAs(UnmanagedType.LPWStr)] string script,
            long sourceContextId,
            [MarshalAs(UnmanagedType.LPWStr)] string sourceUrl,
            out JavaScriptValueSafeHandle handle);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsParseSerializedScript(
            [MarshalAs(UnmanagedType.LPWStr)] string script,
            IntPtr buffer,
            long sourceContext,
            [MarshalAs(UnmanagedType.LPWStr)] string sourceUrl,
            out JavaScriptValueSafeHandle handle);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsSerializeScript(
            [MarshalAs(UnmanagedType.LPWStr)] string script,
            IntPtr buffer,
            ref uint bufferSize);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsRunScript(
            [MarshalAs(UnmanagedType.LPWStr)] string script,
            long sourceContextId,
            [MarshalAs(UnmanagedType.LPWStr)] string sourceUrl,
            out JavaScriptValueSafeHandle handle);

        [DllImport("chakra")]
        internal static extern JsErrorCode JsRunSerializedScript(
            [MarshalAs(UnmanagedType.LPWStr)] string script,
            IntPtr buffer,
            long sourceContext,
            [MarshalAs(UnmanagedType.LPWStr)] string sourceUrl,
            out JavaScriptValueSafeHandle handle);
        #endregion
    } */

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate bool MemoryCallbackThunkCallback(IntPtr callbackState, JavaScriptMemoryAllocationEventType allocationEvent, ulong allocationSize);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate IntPtr NativeFunctionThunkCallback(
        IntPtr callee, 
        [MarshalAs(UnmanagedType.U1)] bool asConstructor, 
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] IntPtr[] args, 
        ushort argCount, 
        IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate void JsFinalizeCallback(IntPtr data);

    internal enum JsErrorCode
    {
        /// <summary>
        ///     Success error code.
        /// </summary>
        JsNoError = 0,

        /// <summary>
        ///     Category of errors that relates to incorrect usage of the API itself.
        /// </summary>
        JsErrorCategoryUsage = 0x10000,
        /// <summary>
        ///     An argument to a hosting API was invalid.
        /// </summary>
        JsErrorInvalidArgument,
        /// <summary>
        ///     An argument to a hosting API was null in a context where null is not allowed.
        /// </summary>
        JsErrorNullArgument,
        /// <summary>
        ///     The hosting API requires that a context be current, but there is no current context.
        /// </summary>
        JsErrorNoCurrentContext,
        /// <summary>
        ///     The engine is in an exception state and no APIs can be called until the exception is
        ///     cleared.
        /// </summary>
        JsErrorInExceptionState,
        /// <summary>
        ///     A hosting API is not yet implemented.
        /// </summary>
        JsErrorNotImplemented,
        /// <summary>
        ///     A hosting API was called on the wrong thread.
        /// </summary>
        JsErrorWrongThread,
        /// <summary>
        ///     A runtime that is still in use cannot be disposed.
        /// </summary>
        JsErrorRuntimeInUse,
        /// <summary>
        ///     A bad serialized script was used, or the serialized script was serialized by a
        ///     different version of the Chakra engine.
        /// </summary>
        JsErrorBadSerializedScript,
        /// <summary>
        ///     The runtime is in a disabled state.
        /// </summary>
        JsErrorInDisabledState,
        /// <summary>
        ///     Runtime does not support reliable script interruption.
        /// </summary>
        JsErrorCannotDisableExecution,
        /// <summary>
        ///     A heap enumeration is currently underway in the script context.
        /// </summary>
        JsErrorHeapEnumInProgress,
        /// <summary>
        ///     A hosting API that operates on object values was called with a non-object value.
        /// </summary>
        JsErrorArgumentNotObject,
        /// <summary>
        ///     A script context is in the middle of a profile callback.
        /// </summary>
        JsErrorInProfileCallback,
        /// <summary>
        ///     A thread service callback is currently underway.
        /// </summary>
        JsErrorInThreadServiceCallback,
        /// <summary>
        ///     Scripts cannot be serialized in debug contexts.
        /// </summary>
        JsErrorCannotSerializeDebugScript,
        /// <summary>
        ///     The context cannot be put into a debug state because it is already in a debug state.
        /// </summary>
        JsErrorAlreadyDebuggingContext,
        /// <summary>
        ///     The context cannot start profiling because it is already profiling.
        /// </summary>
        JsErrorAlreadyProfilingContext,
        /// <summary>
        ///     Idle notification given when the host did not enable idle processing.
        /// </summary>
        JsErrorIdleNotEnabled,
        /// <summary>
        ///     The context did not accept the enqueue callback.
        /// </summary>
        JsCannotSetProjectionEnqueueCallback,
        /// <summary>
        ///     Failed to start projection.
        /// </summary>
        JsErrorCannotStartProjection,
        /// <summary>
        ///     The operation is not supported in an object before collect callback.
        /// </summary>
        JsErrorInObjectBeforeCollectCallback,
        /// <summary>
        ///     Object cannot be unwrapped to IInspectable pointer.
        /// </summary>
        JsErrorObjectNotInspectable,
        /// <summary>
        ///     A hosting API that operates on symbol property ids but was called with a non-symbol property id.
        ///     The error code is returned by JsGetSymbolFromPropertyId if the function is called with non-symbol property id.
        /// </summary>
        JsErrorPropertyNotSymbol,
        /// <summary>
        ///     A hosting API that operates on string property ids but was called with a non-string property id.
        ///     The error code is returned by existing JsGetPropertyNamefromId if the function is called with non-string property id.
        /// </summary>
        JsErrorPropertyNotString,

        /// <summary>
        ///     Category of errors that relates to errors occurring within the engine itself.
        /// </summary>
        JsErrorCategoryEngine = 0x20000,
        /// <summary>
        ///     The Chakra engine has run out of memory.
        /// </summary>
        JsErrorOutOfMemory,

        /// <summary>
        ///     Category of errors that relates to errors in a script.
        /// </summary>
        JsErrorCategoryScript = 0x30000,
        /// <summary>
        ///     A JavaScript exception occurred while running a script.
        /// </summary>
        JsErrorScriptException,
        /// <summary>
        ///     JavaScript failed to compile.
        /// </summary>
        JsErrorScriptCompile,
        /// <summary>
        ///     A script was terminated due to a request to suspend a runtime.
        /// </summary>
        JsErrorScriptTerminated,
        /// <summary>
        ///     A script was terminated because it tried to use <c>eval</c> or <c>function</c> and eval
        ///     was disabled.
        /// </summary>
        JsErrorScriptEvalDisabled,

        /// <summary>
        ///     Category of errors that are fatal and signify failure of the engine.
        /// </summary>
        JsErrorCategoryFatal = 0x40000,
        /// <summary>
        ///     A fatal error in the engine has occurred.
        /// </summary>
        JsErrorFatal,
    }

    [Flags]
    internal enum JsRuntimeAttributes
    {
        /// <summary>
        ///     No special attributes.
        /// </summary>
        None = 0x00000000,
        /// <summary>
        ///     The runtime will not do any work (such as garbage collection) on background threads.
        /// </summary>
        DisableBackgroundWork = 0x00000001,
        /// <summary>
        ///     The runtime should support reliable script interruption. This increases the number of
        ///     places where the runtime will check for a script interrupt request at the cost of a
        ///     small amount of runtime performance.
        /// </summary>
        AllowScriptInterrupt = 0x00000002,
        /// <summary>
        ///     Host will call <c>JsIdle</c>, so enable idle processing. Otherwise, the runtime will
        ///     manage memory slightly more aggressively.
        /// </summary>
        EnableIdleProcessing = 0x00000004,
        /// <summary>
        ///     Runtime will not generate native code.
        /// </summary>
        DisableNativeCodeGeneration = 0x00000008,
        /// <summary>
        ///     Using <c>eval</c> or <c>function</c> constructor will throw an exception.
        /// </summary>
        DisableEval = 0x00000010,
    }

    internal enum JsValueType
    {
        /// <summary>
        ///     The value is the <c>undefined</c> value.
        /// </summary>
        JsUndefined = 0,
        /// <summary>
        ///     The value is the <c>null</c> value.
        /// </summary>
        JsNull = 1,
        /// <summary>
        ///     The value is a JavaScript number value.
        /// </summary>
        JsNumber = 2,
        /// <summary>
        ///     The value is a JavaScript string value.
        /// </summary>
        JsString = 3,
        /// <summary>
        ///     The value is a JavaScript Boolean value.
        /// </summary>
        JsBoolean = 4,
        /// <summary>
        ///     The value is a JavaScript object value.
        /// </summary>
        JsObject = 5,
        /// <summary>
        ///     The value is a JavaScript function object value.
        /// </summary>
        JsFunction = 6,
        /// <summary>
        ///     The value is a JavaScript error object value.
        /// </summary>
        JsError = 7,
        /// <summary>
        ///     The value is a JavaScript array object value.
        /// </summary>
        JsArray = 8,
        /// <summary>
        ///     The value is a JavaScript symbol value.
        /// </summary>
        JsSymbol = 9,
        /// <summary>
        ///     The value is a JavaScript ArrayBuffer object value.
        /// </summary>
        JsArrayBuffer = 10,
        /// <summary>
        ///     The value is a JavaScript typed array object value.
        /// </summary>
        JsTypedArray = 11,
        /// <summary>
        ///     The value is a JavaScript DataView object value.
        /// </summary>
        JsDataView = 12,
    }

    internal static class ConvertExtensions
    {
        public static JavaScriptValueType ToApiValueType(this JsValueType type)
        {
            switch (type)
            {
                case JsValueType.JsArray:
                    return JavaScriptValueType.Array;

                case JsValueType.JsArrayBuffer:
                    return JavaScriptValueType.ArrayBuffer;

                case JsValueType.JsBoolean:
                    return JavaScriptValueType.Boolean;

                case JsValueType.JsDataView:
                    return JavaScriptValueType.DataView;
                
                case JsValueType.JsFunction:
                    return JavaScriptValueType.Function;

                case JsValueType.JsNumber:
                    return JavaScriptValueType.Number;

                case JsValueType.JsError:
                case JsValueType.JsNull:
                case JsValueType.JsObject:
                    return JavaScriptValueType.Object;

                case JsValueType.JsString:
                    return JavaScriptValueType.String;

                case JsValueType.JsSymbol:
                    return JavaScriptValueType.Symbol;

                case JsValueType.JsTypedArray:
                    return JavaScriptValueType.TypedArray;

                case JsValueType.JsUndefined:
                default:
                    return JavaScriptValueType.Undefined;
            }
        }
    }
}
