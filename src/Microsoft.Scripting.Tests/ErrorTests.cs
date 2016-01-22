using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SetProjectionEnqueueCallback()
        {
            Errors.ThrowFor(JsErrorCode.JsCannotSetProjectionEnqueueCallback);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AlreadyDebuggingContext()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorAlreadyDebuggingContext);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AlreadyProfilingContext()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorAlreadyProfilingContext);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentNotObject()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorArgumentNotObject);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BadSerializedScript()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorBadSerializedScript);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotDisableExecution()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorCannotDisableExecution);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotSerializeDebugScript()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorCannotSerializeDebugScript);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CannotStartProjection()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorCannotStartProjection);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Fatal()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorFatal);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void HeapEnumInProgress()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorHeapEnumInProgress);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IdleNotEnabled()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorIdleNotEnabled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InDisabledState()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInDisabledState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InExceptionState()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInExceptionState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InObjectBeforeCollectCallback()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInObjectBeforeCollectCallback);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InProfileCallback()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInProfileCallback);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InThreadServiceCallback()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInThreadServiceCallback);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidArgument()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorInvalidArgument);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoCurrentContext()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorNoCurrentContext);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void NotImplemented()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorNotImplemented);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorNullArgument);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ObjectNotInspectable()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorObjectNotInspectable);
        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void OutOfMemory()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorOutOfMemory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyNotString()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorPropertyNotString);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PropertyNotSymbol()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorPropertyNotSymbol);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RuntimeInUse()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorRuntimeInUse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ScriptCompile()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorScriptCompile);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ScriptEvalDisabled()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorScriptEvalDisabled);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WrongThread()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorWrongThread);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ErrorCodeWithoutExplicitException()
        {
            Errors.ThrowFor(JsErrorCode.JsErrorCategoryUsage);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThrowInvalidOpFormat()
        {
            Errors.ThrowIOEFmt("{0}", "1");
        }
    }
}
