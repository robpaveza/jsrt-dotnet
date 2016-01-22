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
    public class SymbolsUnitTest
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
        public void SymbolToStringFromScriptShouldEqualSymbolPlusNameInParentheses()
        {
            using (var context = engine_.AcquireContext())
            {
                var fn = engine_.EvaluateScriptText(@"(function() {
    var x = Symbol('foo');
    return x.toString();
})();");
                Assert.AreEqual("Symbol(foo)", fn.Invoke(Enumerable.Empty<JavaScriptValue>()).ToString());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SymbolDescriptionThrows()
        {
            using (var context = engine_.AcquireContext())
            {
                var symbol = engine_.CreateSymbol("foo");
                var x = symbol.Description;

                Assert.Fail("Should have thrown.");
            }
        }

        [TestMethod]
        public void InformationalDisplayOfAllSymbolProperties()
        {
            using (var context = engine_.AcquireContext())
            {
                var src = new ScriptSource("[eval code]", @"(function() {
    var x = Object.getOwnPropertyNames(Symbol).join(', ');

    return x;
})();");
                var func = engine_.Evaluate(src);
                var result = func.Invoke(Enumerable.Empty<JavaScriptValue>()).ToString();
                Assert.AreEqual("length, prototype, name, iterator, species, unscopables, for, keyFor, caller, arguments", result);
            }
        }

        // not a test method because the engine doesn't support it right now
        //[TestMethod]
        public void SymbolShouldCreateAndRenderStringCorrectly()
        {
            using (var context = engine_.AcquireContext())
            {
                var symbol = engine_.CreateSymbol("foo");
                Assert.AreEqual(symbol.Description, "Symbol (foo)");
            }
        }
    }
}
