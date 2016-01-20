using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Scripting.JavaScript.SafeHandles;

namespace Microsoft.Scripting
{
    internal sealed class ChakraApi
    {
        #region API method type definitions
        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetFalseValue(out JavaScriptValueSafeHandle globalHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsHasException([MarshalAsAttribute(UnmanagedType.U1)] out bool has);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateObject(out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateExternalObject(IntPtr data, IntPtr finalizeCallback, out JavaScriptValueSafeHandle handle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateSymbol(JavaScriptValueSafeHandle descriptionHandle, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateArray(uint length, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateFunction(IntPtr callback, IntPtr extraData, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateNamedFunction(JavaScriptValueSafeHandle nameHandle, IntPtr callback, IntPtr extraData, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsIdle(out uint nextTick);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetAndClearException(out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetException(JavaScriptValueSafeHandle exception);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateRangeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateReferenceError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateSyntaxError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateTypeError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateURIError(JavaScriptValueSafeHandle message, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsStartDebugging();

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsParseScript([MarshalAsAttribute(UnmanagedType.LPWStr)] string script, IntPtr sourceContextId, [MarshalAsAttribute(UnmanagedType.LPWStr)] string sourceUrl, out JavaScriptValueSafeHandle handle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsParseSerializedScript([MarshalAsAttribute(UnmanagedType.LPWStr)] string script, IntPtr buffer, IntPtr sourceContext, [MarshalAsAttribute(UnmanagedType.LPWStr)] string sourceUrl, out JavaScriptValueSafeHandle handle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSerializeScript([MarshalAsAttribute(UnmanagedType.LPWStr)] string script, IntPtr buffer, ref uint bufferSize);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsRunScript([MarshalAsAttribute(UnmanagedType.LPWStr)] string script, IntPtr sourceContextId, [MarshalAsAttribute(UnmanagedType.LPWStr)] string sourceUrl, out JavaScriptValueSafeHandle handle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsRunSerializedScript([MarshalAsAttribute(UnmanagedType.LPWStr)] string script, IntPtr buffer, IntPtr sourceContext, [MarshalAsAttribute(UnmanagedType.LPWStr)] string sourceUrl, out JavaScriptValueSafeHandle handle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCallFunction(JavaScriptValueSafeHandle fnHandle, [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 2)] IntPtr[] arguments, ushort argCount, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsConstructObject(JavaScriptValueSafeHandle fnHandle, [MarshalAsAttribute(UnmanagedType.LPArray, SizeParamIndex = 2)] IntPtr[] arguments, ushort argCount, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetPropertyIdFromName([MarshalAsAttribute(UnmanagedType.LPWStr)] string propertyName, out IntPtr propertyId);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle valueHandle, out IntPtr propertyId);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, JavaScriptValueSafeHandle propertyValue, [MarshalAsAttribute(UnmanagedType.U1)] bool useStrictSemantics);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDeleteProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, [MarshalAsAttribute(UnmanagedType.U1)] bool useStrictSemantics, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDeleteIndexedProperty(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle index);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsHasProperty(JavaScriptValueSafeHandle obj, IntPtr propertyId, [MarshalAsAttribute(UnmanagedType.U1)] out bool has);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle obj, IntPtr propertyId, out JavaScriptValueSafeHandle descriptor);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDefineProperty(JavaScriptValueSafeHandle obj, IntPtr propId, JavaScriptValueSafeHandle descriptorRef, [MarshalAsAttribute(UnmanagedType.U1)] out bool wasSet);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetOwnPropertyNames(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetOwnPropertySymbols(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsPreventExtension(JavaScriptValueSafeHandle obj);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetExtensionAllowed(JavaScriptValueSafeHandle obj, [MarshalAsAttribute(UnmanagedType.U1)] out bool allowed);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetPrototype(JavaScriptValueSafeHandle obj, out JavaScriptValueSafeHandle prototype);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetPrototype(JavaScriptValueSafeHandle obj, JavaScriptValueSafeHandle prototype);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetArrayBufferStorage(JavaScriptValueSafeHandle obj, out IntPtr buffer, out uint len);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetTypedArrayStorage(JavaScriptValueSafeHandle obj, out IntPtr buf, out uint len, out JavaScript.JavaScriptTypedArrayType type, out int elemSize);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetGlobalObject(out JavaScriptValueSafeHandle globalHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetUndefinedValue(out JavaScriptValueSafeHandle globalHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetNullValue(out JavaScriptValueSafeHandle globalHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetTrueValue(out JavaScriptValueSafeHandle globalHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsAddRef(IntPtr @ref, out uint count);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsRelease(IntPtr @ref, out uint count);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDisposeRuntime(IntPtr runtime);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateRuntime(JsRuntimeAttributes attributes, IntPtr threadService, out JavaScriptRuntimeSafeHandle runtime);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr extraInformation, IntPtr pfnCallback);
        [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Winapi)]
        public delegate Microsoft.Scripting.JsErrorCode FnJsSetRuntimeMemoryAllocationCallbackWithIntPtr(IntPtr runtime, System.IntPtr extraInformation, System.IntPtr pfnCallback);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCollectGarbage(JavaScriptRuntimeSafeHandle runtime);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime, out ulong usage);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime, out bool isDisabled);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsCreateContext(JavaScriptRuntimeSafeHandle runtime, out JavaScriptEngineSafeHandle engine);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetCurrentContext(JavaScriptEngineSafeHandle contextHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsBooleanToBool(JavaScriptValueSafeHandle valueRef, [MarshalAsAttribute(UnmanagedType.U1)] out bool result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsConvertValueToBoolean(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsNumberToDouble(JavaScriptValueSafeHandle valueRef, out double result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsDoubleToNumber(double value, out JavaScriptValueSafeHandle result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsConvertValueToNumber(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetValueType(JavaScriptValueSafeHandle value, out JsValueType kind);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsNumberToInt(JavaScriptValueSafeHandle valueRef, out int result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsIntToNumber(int value, out JavaScriptValueSafeHandle result);

        // ***unsafe***
        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public unsafe delegate JsErrorCode FnJsPointerToString(void* psz, int length, out JavaScriptValueSafeHandle result);

        // ***unsafe***
        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public unsafe delegate JsErrorCode FnJsStringToPointer(JavaScriptValueSafeHandle str, out void* result, out uint strLen);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsConvertValueToString(JavaScriptValueSafeHandle valueToConvert, out JavaScriptValueSafeHandle resultHandle);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsEquals(JavaScriptValueSafeHandle obj1, JavaScriptValueSafeHandle obj2, [MarshalAsAttribute(UnmanagedType.U1)] out bool result);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsStrictEquals(JavaScriptValueSafeHandle obj1, JavaScriptValueSafeHandle obj2, [MarshalAsAttribute(UnmanagedType.U1)] out bool result);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsGetExternalData(JavaScriptValueSafeHandle @ref, out IntPtr externalData);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate JsErrorCode FnJsSetExternalData(JavaScriptValueSafeHandle @ref, IntPtr externalData);
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
        public readonly FnJsSetRuntimeMemoryAllocationCallbackWithIntPtr JsSetRuntimeMemoryAllocationCallbackWithIntPtr;

        public readonly FnJsCollectGarbage JsCollectGarbage;

        public readonly FnJsDisableRuntimeExecution JsDisableRuntimeExecution;

        public readonly FnJsEnableRuntimeExecution JsEnableRuntimeExecution;

        public readonly FnJsGetRuntimeMemoryUsage JsGetRuntimeMemoryUsage;

        public readonly FnJsIsRuntimeExecutionDisabled JsIsRuntimeExecutionDisabled;

        public readonly FnJsCreateContext JsCreateContext;

        public readonly FnJsSetCurrentContext JsSetCurrentContext;

        public JsErrorCode JsReleaseCurrentContext()
        {
            return JsSetCurrentContext(new JavaScriptEngineSafeHandle(IntPtr.Zero));
        }

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

        public readonly FnJsGetExternalData JsGetExternalData;

        public readonly FnJsSetExternalData JsSetExternalData;
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
            SetFn(ref JsAddRef, hModule, "JsAddRef");
            SetFn(ref JsBooleanToBool, hModule, "JsBooleanToBool");
            SetFn(ref JsCallFunction, hModule, "JsCallFunction");
            SetFn(ref JsCollectGarbage, hModule, "JsCollectGarbage");
            SetFn(ref JsConstructObject, hModule, "JsConstructObject");
            SetFn(ref JsConvertValueToBoolean, hModule, "JsConvertValueToBoolean");
            SetFn(ref JsConvertValueToNumber, hModule, "JsConvertValueToNumber");
            SetFn(ref JsConvertValueToString, hModule, "JsConvertValueToString");
            SetFn(ref JsCreateArray, hModule, "JsCreateArray");
            SetFn(ref JsCreateContext, hModule, "JsCreateContext");
            SetFn(ref JsCreateError, hModule, "JsCreateError");
            SetFn(ref JsCreateExternalObject, hModule, "JsCreateExternalObject");
            SetFn(ref JsCreateFunction, hModule, "JsCreateFunction");
            SetFn(ref JsCreateNamedFunction, hModule, "JsCreateNamedFunction");
            SetFn(ref JsCreateObject, hModule, "JsCreateObject");
            SetFn(ref JsCreateRangeError, hModule, "JsCreateRangeError");
            SetFn(ref JsCreateReferenceError, hModule, "JsCreateReferenceError");
            SetFn(ref JsCreateRuntime, hModule, "JsCreateRuntime");
            SetFn(ref JsCreateSymbol, hModule, "JsCreateSymbol");
            SetFn(ref JsCreateSyntaxError, hModule, "JsCreateSyntaxError");
            SetFn(ref JsCreateTypeError, hModule, "JsCreateTypeError");
            SetFn(ref JsCreateURIError, hModule, "JsCreateURIError");
            SetFn(ref JsDefineProperty, hModule, "JsDefineProperty");
            SetFn(ref JsDeleteIndexedProperty, hModule, "JsDeleteIndexedProperty");
            SetFn(ref JsDeleteProperty, hModule, "JsDeleteProperty");
            SetFn(ref JsDisableRuntimeExecution, hModule, "JsDisableRuntimeExecution");
            SetFn(ref JsDisposeRuntime, hModule, "JsDisposeRuntime");
            SetFn(ref JsDoubleToNumber, hModule, "JsDoubleToNumber");
            SetFn(ref JsEnableRuntimeExecution, hModule, "JsEnableRuntimeExecution");
            SetFn(ref JsEquals, hModule, "JsEquals");
            SetFn(ref JsGetAndClearException, hModule, "JsGetAndClearException");
            SetFn(ref JsGetArrayBufferStorage, hModule, "JsGetArrayBufferStorage");
            SetFn(ref JsGetExtensionAllowed, hModule, "JsGetExtensionAllowed");
            SetFn(ref JsGetFalseValue, hModule, "JsGetFalseValue");
            SetFn(ref JsGetGlobalObject, hModule, "JsGetGlobalObject");
            SetFn(ref JsGetIndexedProperty, hModule, "JsGetIndexedProperty");
            SetFn(ref JsGetNullValue, hModule, "JsGetNullValue");
            SetFn(ref JsGetOwnPropertyDescriptor, hModule, "JsGetOwnPropertyDescriptor");
            SetFn(ref JsGetOwnPropertyNames, hModule, "JsGetOwnPropertyNames");
            SetFn(ref JsGetOwnPropertySymbols, hModule, "JsGetOwnPropertySymbols");
            SetFn(ref JsGetProperty, hModule, "JsGetProperty");
            SetFn(ref JsGetPropertyIdFromName, hModule, "JsGetPropertyIdFromName");
            SetFn(ref JsGetPropertyIdFromSymbol, hModule, "JsGetPropertyIdFromSymbol");
            SetFn(ref JsGetPrototype, hModule, "JsGetPrototype");
            SetFn(ref JsGetRuntimeMemoryUsage, hModule, "JsGetRuntimeMemoryUsage");
            SetFn(ref JsGetTrueValue, hModule, "JsGetTrueValue");
            SetFn(ref JsGetTypedArrayStorage, hModule, "JsGetTypedArrayStorage");
            SetFn(ref JsGetUndefinedValue, hModule, "JsGetUndefinedValue");
            SetFn(ref JsGetValueType, hModule, "JsGetValueType");
            SetFn(ref JsHasException, hModule, "JsHasException");
            SetFn(ref JsHasProperty, hModule, "JsHasProperty");
            SetFn(ref JsIdle, hModule, "JsIdle");
            SetFn(ref JsIntToNumber, hModule, "JsIntToNumber");
            SetFn(ref JsIsRuntimeExecutionDisabled, hModule, "JsIsRuntimeExecutionDisabled");
            SetFn(ref JsNumberToDouble, hModule, "JsNumberToDouble");
            SetFn(ref JsNumberToInt, hModule, "JsNumberToInt");
            SetFn(ref JsParseScript, hModule, "JsParseScript");
            SetFn(ref JsParseSerializedScript, hModule, "JsParseSerializedScript");
            SetFn(ref JsPointerToString, hModule, "JsPointerToString");
            SetFn(ref JsPreventExtension, hModule, "JsPreventExtension");
            SetFn(ref JsRelease, hModule, "JsRelease");
            SetFn(ref JsRunScript, hModule, "JsRunScript");
            SetFn(ref JsRunSerializedScript, hModule, "JsRunSerializedScript");
            SetFn(ref JsSerializeScript, hModule, "JsSerializeScript");
            SetFn(ref JsSetCurrentContext, hModule, "JsSetCurrentContext");
            SetFn(ref JsSetException, hModule, "JsSetException");
            SetFn(ref JsSetIndexedProperty, hModule, "JsSetIndexedProperty");
            SetFn(ref JsSetProperty, hModule, "JsSetProperty");
            SetFn(ref JsSetPrototype, hModule, "JsSetPrototype");
            SetFn(ref JsSetRuntimeMemoryAllocationCallback, hModule, "JsSetRuntimeMemoryAllocationCallback");
            SetFn(ref JsStartDebugging, hModule, "JsStartDebugging", optional: true);
            SetFn(ref JsStrictEquals, hModule, "JsStrictEquals");
            SetFn(ref JsStringToPointer, hModule, "JsStringToPointer");
            SetFn(ref JsGetExternalData, hModule, "JsGetExternalData");
            SetFn(ref JsSetExternalData, hModule, "JsSetExternalData");
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
            int hr = Marshal.GetHRForLastWin32Error();
            throw Marshal.GetExceptionForHR(hr);
        }

        private static void SetFn<TDelegate>(ref TDelegate target, IntPtr hModule, string procName, bool optional = false)
            where TDelegate : class
        {
            IntPtr procAddr = NativeMethods.GetProcAddress(hModule, procName);
            if (IntPtr.Zero != procAddr)
            {
                target = Marshal.GetDelegateForFunctionPointer<TDelegate>(procAddr);
            }
            else if (!optional)
            {
                ThrowForNative();
            }            
        }

        private static ChakraApi Load()
        {
            string myPath = ".";
            string directory = Path.GetDirectoryName(myPath);
            string arch = "x86";
            if (IntPtr.Size == 8)
            {
                arch = "x64";
            }

#if DEBUG
            string build = "dbg";
#else
            string build = "fre";
#endif

            string mainPath = Path.Combine(directory, "ChakraCore", build, arch, "ChakraCore.dll");
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

            throw new FileNotFoundException(string.Format("Could not locate a copy of ChakraCore.dll to load.  The following paths were trie" +
                        "d:\n\t{0}\n\t{1}\n\t{2}\n\nEnsure that an architecture-appropriate copy of ChakraCore.dl" +
                        "l is included in your project.  Also tried to load the system-provided Chakra.dll.", mainPath, alternatePath, localPath));
        }

        public static ChakraApi FromFile(string filePath)
        {
            if ((false == File.Exists(filePath)))
            {
                throw new FileNotFoundException("The library could not be located at the specified path.", filePath);
            }
            IntPtr hMod = NativeMethods.LoadLibraryEx(filePath, IntPtr.Zero, 0);
            if ((hMod == IntPtr.Zero))
            {
                ThrowForNative();
            }
            return new ChakraApi(hMod);
        }

        
    }
}

