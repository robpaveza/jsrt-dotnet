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
    public class JavaScriptObjectTests
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
        public void ObjectKeysFunctionsAsExpected()
        {
            using (engine_.AcquireContext())
            {
                var obj = engine_.CreateObject();
                obj.SetPropertyByName("test", engine_.TrueValue);
                obj.SetPropertyByName("other", engine_.NullValue);

                var keys = obj.Keys;
                Assert.AreEqual("test, other", keys.Join(", "));
            }
        }
    }
}
