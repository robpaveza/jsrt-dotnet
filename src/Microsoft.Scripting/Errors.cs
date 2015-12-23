using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting
{
    internal static class Errors
    {
        public const string NoMutateJsRuntimeSettings = "Can't change JavaScriptRuntimeSettings once it has been used to create a runtime.";
        public const string DefaultFnOverwritten = "The built-in function '{0}' has been overwritten and is no longer a function.";

        public static void ThrowFor(JsErrorCode errorCode)
        {
            Debug.Assert(errorCode != JsErrorCode.JsNoError);

            throw new Exception(errorCode.ToString());
        }

        public static void ThrowIfIs(JsErrorCode errorCode)
        {
            if (errorCode != JsErrorCode.JsNoError)
                throw new Exception(errorCode.ToString());
        }

        public static void ThrowIOEFmt(string formatStr, string param)
        {
            string result = string.Format(formatStr, param);
            throw new InvalidOperationException(result);
        }
    }
}
