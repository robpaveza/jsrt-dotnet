using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.JavaScript;
using System.Linq;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class RuntimeBaseTests
    {
        private JavaScriptRuntime runtime_;
        private JavaScriptEngine engine_;

        [TestInitialize]
        public void Setup()
        {
            runtime_ = new JavaScriptRuntime();
            engine_ = runtime_.CreateEngine();
        }

        [TestCleanup]
        public void Cleanup()
        {
            engine_.Dispose();
            engine_ = null;
            runtime_.Dispose();
            runtime_ = null;
        }

        [TestMethod]
        public void MemoryUsageShouldGrowWhenAllocationIsPreservedOnGlobal()
        {
            engine_.RuntimeExceptionRaised += (s, e) =>
            {
                Assert.Fail("No runtime exception should have been raised in this test.");
            };
            runtime_.MemoryChanging += (s, e) =>
            {
                if (e.IsCancelable)
                {
                    e.Cancel = false;
                }
            };

            using (var context = engine_.AcquireContext())
            {
                var baseline = runtime_.RuntimeMemoryUsage;
                var func = engine_.EvaluateScriptText(@"(function(global) {
    var x = [];
    for (var i = 0; i < 1024 * 256; i++) {
        x[i] = i;
    }

    global['x'] = x;
})(this);");
                func.Invoke(Enumerable.Empty<JavaScriptValue>());

                Assert.IsTrue(runtime_.RuntimeMemoryUsage > baseline);
            }
        }

        [TestMethod]
        public void JavaScriptShouldEncounterOOMWhenMemoryAllocationIsDenied()
        {
            runtime_.MemoryChanging += (s, e) =>
            {
                if (e.IsCancelable)
                    e.Cancel = true;
            };
            string scriptError = null;
            engine_.RuntimeExceptionRaised += (s, e) =>
            {
                scriptError = engine_.GetAndClearException().ToString();
            };

            using (var context = engine_.AcquireContext())
            {
                var func = engine_.EvaluateScriptText(@"(function(global) {
    var x = [];
    for (var i = 0; i < 1024 * 256; i++) {
        x[i] = i;
    }

    global['x'] = x;
})(this);");
                Assert.IsNull(func);
            }

            Assert.AreEqual("Error: Out of memory", scriptError);
        }

        [TestMethod]
        public void OnceMemoryAllocationIsDeniedFurtherAttemptsShouldNotPermitItToBeAllowed()
        {
            runtime_.MemoryChanging += (s, e) =>
            {
                if (e.IsCancelable)
                    e.Cancel = true;
            };
            runtime_.MemoryChanging += (s, e) =>
            {
                if (e.IsCancelable && e.Cancel)
                    e.Cancel = false;
            };

            string scriptError = null;
            engine_.RuntimeExceptionRaised += (s, e) =>
            {
                scriptError = engine_.GetAndClearException().ToString();
            };

            using (var context = engine_.AcquireContext())
            {
                var func = engine_.EvaluateScriptText(@"(function(global) {
    var x = [];
    for (var i = 0; i < 1024 * 256; i++) {
        x[i] = i;
    }

    global['x'] = x;
})(this);");
                Assert.IsNull(func);
            }

            Assert.AreEqual("Error: Out of memory", scriptError);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DisableExecutionResultsInException()
        {
            runtime_.DisableExecution();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunIdleWorkResultsInException()
        {
            using (var context = engine_.AcquireContext())
            {
                engine_.RunIdleWork();
            }
        }
    }
}
