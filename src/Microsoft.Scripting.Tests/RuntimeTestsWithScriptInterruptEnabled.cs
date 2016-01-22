using Microsoft.Scripting.JavaScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class RuntimeTestsWithScriptInterruptEnabled
    {
        private JavaScriptRuntime runtime_;
        private JavaScriptEngine engine_;

        [TestInitialize]
        public void Setup()
        {
            var settings = new JavaScriptRuntimeSettings()
            {
                AllowScriptInterrupt = true,
            };
            runtime_ = new JavaScriptRuntime(settings);
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
        public void DisableExecutionSucceeds()
        {
            runtime_.DisableExecution();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DisabledExecutionThrowsOnExecute()
        {
            runtime_.DisableExecution();

            engine_.Execute(new ScriptSource("[eval code]", "var x = 1;"));
        }
    }
}
