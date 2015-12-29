namespace Microsoft.Scripting
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;


    internal sealed class ChakraApi
    {
        #region API method type definitions
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetFalseValue(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle globalHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsHasException([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool has);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateObject(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateExternalObject(System.IntPtr data, System.IntPtr finalizeCallback, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle handle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateSymbol(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle descriptionHandle, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateArray(uint length, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateFunction(System.IntPtr callback, System.IntPtr extraData, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateNamedFunction(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle nameHandle, System.IntPtr callback, System.IntPtr extraData, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsIdle(out uint nextTick);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetAndClearException(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetException(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle exception);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateRangeError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateReferenceError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateSyntaxError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateTypeError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateURIError(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle message, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsStartDebugging();

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsParseScript([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string script, long sourceContextId, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string sourceUrl, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle handle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsParseSerializedScript([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string script, System.IntPtr buffer, long sourceContext, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string sourceUrl, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle handle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSerializeScript([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string script, System.IntPtr buffer, ref uint bufferSize);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsRunScript([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string script, long sourceContextId, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string sourceUrl, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle handle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsRunSerializedScript([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string script, System.IntPtr buffer, long sourceContext, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string sourceUrl, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle handle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCallFunction(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle fnHandle, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 2)] IntPtr[] arguments, ushort argCount, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsConstructObject(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle fnHandle, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPArray, SizeParamIndex = 2)] IntPtr[] arguments, ushort argCount, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetPropertyIdFromName([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string propertyName, out System.IntPtr propertyId);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetPropertyIdFromSymbol(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueHandle, out System.IntPtr propertyId);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propertyId, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propertyId, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle propertyValue, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] bool useStrictSemantics);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDeleteProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propertyId, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] bool useStrictSemantics, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetIndexedProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle index, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetIndexedProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle index, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle value);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDeleteIndexedProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle index);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsHasProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propertyId, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool has);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetOwnPropertyDescriptor(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propertyId, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle descriptor);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDefineProperty(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, System.IntPtr propId, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle descriptorRef, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool wasSet);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetOwnPropertyNames(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetOwnPropertySymbols(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsPreventExtension(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetExtensionAllowed(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool allowed);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetPrototype(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle prototype);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetPrototype(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle prototype);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetArrayBufferStorage(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, out System.IntPtr buffer, out uint len);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetTypedArrayStorage(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj, out System.IntPtr buf, out uint len, out Microsoft.Scripting.JavaScript.JavaScriptTypedArrayType type, out int elemSize);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetGlobalObject(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle globalHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetUndefinedValue(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle globalHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetNullValue(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle globalHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetTrueValue(out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle globalHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsAddRef(System.IntPtr @ref, out uint count);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsRelease(System.IntPtr @ref, out uint count);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDisposeRuntime(System.IntPtr runtime);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateRuntime(Microsoft.Scripting.JsRuntimeAttributes attributes, System.IntPtr threadService, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetRuntimeMemoryAllocationCallback(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime, System.IntPtr extraInformation, System.IntPtr pfnCallback);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCollectGarbage(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDisableRuntimeExecution(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsEnableRuntimeExecution(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetRuntimeMemoryUsage(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime, out ulong usage);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsIsRuntimeExecutionDisabled(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsCreateContext(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptRuntimeSafeHandle runtime, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptEngineSafeHandle engine);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetCurrentContext(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptEngineSafeHandle contextHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsBooleanToBool(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueRef, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsConvertValueToBoolean(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueToConvert, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle resultHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsNumberToDouble(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueRef, out double result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsDoubleToNumber(double value, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsConvertValueToNumber(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueToConvert, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle resultHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsGetValueType(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle value, out Microsoft.Scripting.JsValueType kind);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsNumberToInt(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueRef, out int result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsIntToNumber(int value, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        // ***unsafe***
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public unsafe delegate Microsoft.Scripting.JsErrorCode FnJsPointerToString(void* psz, int length, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle result);

        // ***unsafe***
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public unsafe delegate Microsoft.Scripting.JsErrorCode FnJsStringToPointer(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle str, out void* result, out uint strLen);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsConvertValueToString(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle valueToConvert, out Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle resultHandle);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsEquals(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj1, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj2, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool result);

        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsStrictEquals(Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj1, Microsoft.Scripting.JavaScript.SafeHandles.JavaScriptValueSafeHandle obj2, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.U1)] out bool result);
        #endregion

        #region Field definitions
        public readonly FnJsGetFalseValue JsGetFalseValue;

        public readonly FnJsHasException JsHasException;

        public readonly FnJsCreateObject JsCreateObject;

        public readonly FnJsCreateExternalObject JsCreateExternalObject;

        public readonly FnJsCreateSymbol JsCreateSymbol;

        public readonly FnJsCreateArray JsCreateArray;

        public readonly FnJsCreateFunction JsCreateFunction;

        public readonly FnJsCreateNamedFunction JsCreateNamedFunction;

        public readonly FnJsIdle JsIdle;

        public readonly FnJsGetAndClearException JsGetAndClearException;

        public readonly FnJsSetException JsSetException;

        public readonly FnJsCreateError JsCreateError;

        public readonly FnJsCreateRangeError JsCreateRangeError;

        public readonly FnJsCreateReferenceError JsCreateReferenceError;

        public readonly FnJsCreateSyntaxError JsCreateSyntaxError;

        public readonly FnJsCreateTypeError JsCreateTypeError;

        public readonly FnJsCreateURIError JsCreateURIError;

        public readonly FnJsStartDebugging JsStartDebugging;

        public readonly FnJsParseScript JsParseScript;

        public readonly FnJsParseSerializedScript JsParseSerializedScript;

        public readonly FnJsSerializeScript JsSerializeScript;

        public readonly FnJsRunScript JsRunScript;

        public readonly FnJsRunSerializedScript JsRunSerializedScript;

        public readonly FnJsCallFunction JsCallFunction;

        public readonly FnJsConstructObject JsConstructObject;

        public readonly FnJsGetPropertyIdFromName JsGetPropertyIdFromName;

        public readonly FnJsGetPropertyIdFromSymbol JsGetPropertyIdFromSymbol;

        public readonly FnJsGetProperty JsGetProperty;

        public readonly FnJsSetProperty JsSetProperty;

        public readonly FnJsDeleteProperty JsDeleteProperty;

        public readonly FnJsGetIndexedProperty JsGetIndexedProperty;

        public readonly FnJsSetIndexedProperty JsSetIndexedProperty;

        public readonly FnJsDeleteIndexedProperty JsDeleteIndexedProperty;

        public readonly FnJsHasProperty JsHasProperty;

        public readonly FnJsGetOwnPropertyDescriptor JsGetOwnPropertyDescriptor;

        public readonly FnJsDefineProperty JsDefineProperty;

        public readonly FnJsGetOwnPropertyNames JsGetOwnPropertyNames;

        public readonly FnJsGetOwnPropertySymbols JsGetOwnPropertySymbols;

        public readonly FnJsPreventExtension JsPreventExtension;

        public readonly FnJsGetExtensionAllowed JsGetExtensionAllowed;

        public readonly FnJsGetPrototype JsGetPrototype;

        public readonly FnJsSetPrototype JsSetPrototype;

        public readonly FnJsGetArrayBufferStorage JsGetArrayBufferStorage;

        public readonly FnJsGetTypedArrayStorage JsGetTypedArrayStorage;

        public readonly FnJsGetGlobalObject JsGetGlobalObject;

        public readonly FnJsGetUndefinedValue JsGetUndefinedValue;

        public readonly FnJsGetNullValue JsGetNullValue;

        public readonly FnJsGetTrueValue JsGetTrueValue;

        public readonly FnJsAddRef JsAddRef;

        public readonly FnJsRelease JsRelease;

        public readonly FnJsDisposeRuntime JsDisposeRuntime;

        public readonly FnJsCreateRuntime JsCreateRuntime;

        public readonly FnJsSetRuntimeMemoryAllocationCallback JsSetRuntimeMemoryAllocationCallback;

        public readonly FnJsCollectGarbage JsCollectGarbage;

        public readonly FnJsDisableRuntimeExecution JsDisableRuntimeExecution;

        public readonly FnJsEnableRuntimeExecution JsEnableRuntimeExecution;

        public readonly FnJsGetRuntimeMemoryUsage JsGetRuntimeMemoryUsage;

        public readonly FnJsIsRuntimeExecutionDisabled JsIsRuntimeExecutionDisabled;

        public readonly FnJsCreateContext JsCreateContext;

        public readonly FnJsSetCurrentContext JsSetCurrentContext;

        public readonly FnJsBooleanToBool JsBooleanToBool;

        public readonly FnJsConvertValueToBoolean JsConvertValueToBoolean;

        public readonly FnJsNumberToDouble JsNumberToDouble;

        public readonly FnJsDoubleToNumber JsDoubleToNumber;

        public readonly FnJsConvertValueToNumber JsConvertValueToNumber;

        public readonly FnJsGetValueType JsGetValueType;

        public readonly FnJsNumberToInt JsNumberToInt;

        public readonly FnJsIntToNumber JsIntToNumber;

        public readonly FnJsPointerToString JsPointerToString;

        public readonly FnJsStringToPointer JsStringToPointer;

        public readonly FnJsConvertValueToString JsConvertValueToString;

        public readonly FnJsEquals JsEquals;

        public readonly FnJsStrictEquals JsStrictEquals;
        #endregion

        private static System.Lazy<ChakraApi> sharedInstance_ = new System.Lazy<ChakraApi>(Load);

        private static class NativeMethods
        {
            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        }


        internal ChakraApi(IntPtr hModule)
        {
            SetFn(ref JsGetFalseValue, hModule, "JsGetFalseValue");
            SetFn(ref JsHasException, hModule, "JsHasException");
            SetFn(ref JsCreateObject, hModule, "JsCreateObject");
            SetFn(ref JsCreateExternalObject, hModule, "JsCreateExternalObject");
            SetFn(ref JsCreateSymbol, hModule, "JsCreateSymbol");
            SetFn(ref JsCreateArray, hModule, "JsCreateArray");
            SetFn(ref JsCreateFunction, hModule, "JsCreateFunction");
            SetFn(ref JsCreateNamedFunction, hModule, "JsCreateNamedFunction");
            SetFn(ref JsIdle, hModule, "JsIdle");
            SetFn(ref JsGetAndClearException, hModule, "JsGetAndClearException");
            SetFn(ref JsSetException, hModule, "JsSetException");
            SetFn(ref JsCreateError, hModule, "JsCreateError");
            SetFn(ref JsCreateRangeError, hModule, "JsCreateRangeError");
            SetFn(ref JsCreateReferenceError, hModule, "JsCreateReferenceError");
            SetFn(ref JsCreateSyntaxError, hModule, "JsCreateSyntaxError");
            SetFn(ref JsCreateTypeError, hModule, "JsCreateTypeError");
            SetFn(ref JsCreateURIError, hModule, "JsCreateURIError");
            SetFn(ref JsStartDebugging, hModule, "JsStartDebugging");
            SetFn(ref JsParseScript, hModule, "JsParseScript");
            SetFn(ref JsParseSerializedScript, hModule, "JsParseSerializedScript");
            SetFn(ref JsSerializeScript, hModule, "JsSerializeScript");
            SetFn(ref JsRunScript, hModule, "JsRunScript");
            SetFn(ref JsRunSerializedScript, hModule, "JsRunSerializedScript");
            SetFn(ref JsCallFunction, hModule, "JsCallFunction");
            SetFn(ref JsConstructObject, hModule, "JsConstructObject");
            SetFn(ref JsGetPropertyIdFromName, hModule, "JsGetPropertyIdFromName");
            SetFn(ref JsGetPropertyIdFromSymbol, hModule, "JsGetPropertyIdFromSymbol");
            SetFn(ref JsGetProperty, hModule, "JsGetProperty");
            SetFn(ref JsSetProperty, hModule, "JsSetProperty");
            SetFn(ref JsDeleteProperty, hModule, "JsDeleteProperty");
            SetFn(ref JsGetIndexedProperty, hModule, "JsGetIndexedProperty");
            SetFn(ref JsSetIndexedProperty, hModule, "JsSetIndexedProperty");
            SetFn(ref JsDeleteIndexedProperty, hModule, "JsDeleteIndexedProperty");
            SetFn(ref JsHasProperty, hModule, "JsHasProperty");
            SetFn(ref JsGetOwnPropertyDescriptor, hModule, "JsGetOwnPropertyDescriptor");
            SetFn(ref JsDefineProperty, hModule, "JsDefineProperty");
            SetFn(ref JsGetOwnPropertyNames, hModule, "JsGetOwnPropertyNames");
            SetFn(ref JsGetOwnPropertySymbols, hModule, "JsGetOwnPropertySymbols");
            SetFn(ref JsPreventExtension, hModule, "JsPreventExtension");
            SetFn(ref JsGetExtensionAllowed, hModule, "JsGetExtensionAllowed");
            SetFn(ref JsGetPrototype, hModule, "JsGetPrototype");
            SetFn(ref JsSetPrototype, hModule, "JsSetPrototype");
            SetFn(ref JsGetArrayBufferStorage, hModule, "JsGetArrayBufferStorage");
            SetFn(ref JsGetTypedArrayStorage, hModule, "JsGetTypedArrayStorage");
            SetFn(ref JsGetGlobalObject, hModule, "JsGetGlobalObject");
            SetFn(ref JsGetUndefinedValue, hModule, "JsGetUndefinedValue");
            SetFn(ref JsGetNullValue, hModule, "JsGetNullValue");
            SetFn(ref JsGetTrueValue, hModule, "JsGetTrueValue");
            SetFn(ref JsAddRef, hModule, "JsAddRef");
            SetFn(ref JsRelease, hModule, "JsRelease");
            SetFn(ref JsDisposeRuntime, hModule, "JsDisposeRuntime");
            SetFn(ref JsCreateRuntime, hModule, "JsCreateRuntime");
            SetFn(ref JsSetRuntimeMemoryAllocationCallback, hModule, "JsSetRuntimeMemoryAllocationCallback");
            SetFn(ref JsCollectGarbage, hModule, "JsCollectGarbage");
            SetFn(ref JsDisableRuntimeExecution, hModule, "JsDisableRuntimeExecution");
            SetFn(ref JsEnableRuntimeExecution, hModule, "JsEnableRuntimeExecution");
            SetFn(ref JsGetRuntimeMemoryUsage, hModule, "JsGetRuntimeMemoryUsage");
            SetFn(ref JsIsRuntimeExecutionDisabled, hModule, "JsIsRuntimeExecutionDisabled");
            SetFn(ref JsCreateContext, hModule, "JsCreateContext");
            SetFn(ref JsSetCurrentContext, hModule, "JsSetCurrentContext");
            SetFn(ref JsBooleanToBool, hModule, "JsBooleanToBool");
            SetFn(ref JsConvertValueToBoolean, hModule, "JsConvertValueToBoolean");
            SetFn(ref JsNumberToDouble, hModule, "JsNumberToDouble");
            SetFn(ref JsDoubleToNumber, hModule, "JsDoubleToNumber");
            SetFn(ref JsConvertValueToNumber, hModule, "JsConvertValueToNumber");
            SetFn(ref JsGetValueType, hModule, "JsGetValueType");
            SetFn(ref JsNumberToInt, hModule, "JsNumberToInt");
            SetFn(ref JsIntToNumber, hModule, "JsIntToNumber");
            SetFn(ref JsPointerToString, hModule, "JsPointerToString");
            SetFn(ref JsStringToPointer, hModule, "JsStringToPointer");
            SetFn(ref JsConvertValueToString, hModule, "JsConvertValueToString");
            SetFn(ref JsEquals, hModule, "JsEquals");
            SetFn(ref JsStrictEquals, hModule, "JsStrictEquals");
        }

        public static ChakraApi Instance
        {
            get
            {
                return sharedInstance_.Value;
            }
        }

        private static void ThrowForNative()
        {
            throw new Exception(string.Format("Win32 error received for LoadLibrary or GetProcAddress: 0x{0:x8}", Marshal.GetLastWin32Error()));
        }

        private static void SetFn<TDelegate>(ref TDelegate target, System.IntPtr hModule, string procName)
            where TDelegate : class
        {
            System.IntPtr procAddr = NativeMethods.GetProcAddress(hModule, procName);
            if ((hModule == System.IntPtr.Zero))
            {
                ThrowForNative();
            }
            target = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer<TDelegate>(procAddr);
        }

        private static ChakraApi Load()
        {
            string myPath = ".";
            string directory = System.IO.Path.GetDirectoryName(myPath);
            string arch = "x86";
            if ((System.IntPtr.Size == 8))
            {
                arch = "x64";
            }

#if DEBUG
            string build = "dbg";
#else
            string build = "fre";
#endif

            string mainPath = System.IO.Path.Combine(directory, "ChakraCore", build, arch, "ChakraCore.dll");
            if (File.Exists(mainPath))
            {
                return FromFile(mainPath);
            }
            string alternatePath = Path.Combine(directory, "ChakraCore", arch, "ChakraCore.dll");
            if (File.Exists(alternatePath))
            {
                return FromFile(alternatePath);
            }
            string localPath = Path.Combine(directory, "ChakraCore.dll");
            if (File.Exists(localPath))
            {
                return FromFile(localPath);
            }

            IntPtr hMod = NativeMethods.LoadLibrary("chakra.dll");
            if (hMod != IntPtr.Zero)
            {
                return new ChakraApi(hMod);
            }

            throw new System.IO.FileNotFoundException(string.Format("Could not locate a copy of ChakraCore.dll to load.  The following paths were trie" +
                        "d:\n\t{0}\n\t{1}\n\t{2}\n\nEnsure that an architecture-appropriate copy of ChakraCore.dl" +
                        "l is included in your project.  Also tried to load the system-provided Chakra.dll.", mainPath, alternatePath, localPath));
        }

        public static ChakraApi FromFile(string filePath)
        {
            if ((false == System.IO.File.Exists(filePath)))
            {
                throw new System.IO.FileNotFoundException("The library could not be located at the specified path.", filePath);
            }
            System.IntPtr hMod = NativeMethods.LoadLibraryEx(filePath, System.IntPtr.Zero, 0);
            if ((hMod == System.IntPtr.Zero))
            {
                ThrowForNative();
            }
            return new ChakraApi(hMod);
        }

        
    }
}

