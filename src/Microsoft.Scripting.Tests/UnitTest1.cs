using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Scripting.JavaScript;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            bool ok = false;
            JavaScriptCallableFunction callback = (eng, construct, thisObj, args) =>
            {
                ok = true;
                return eng.UndefinedValue;
            };

            using (var rt = new JavaScriptRuntime())
            {
                rt.MemoryChanging += Rt_MemoryChanging;
                using (var eng = rt.CreateEngine())
                {
                    eng.SetGlobalFunction("hitIt", callback);

                    eng.Execute(new ScriptSource("[eval code]", "hitIt();"));
                }
            }
            Assert.IsTrue(ok);
        }

        private void Rt_MemoryChanging(object sender, JavaScriptMemoryAllocationEventArgs e)
        {
            System.Diagnostics.Debugger.Log(0, "Log", $"Allocation/Change: {e.Type} :: {e.Amount}");
        }
    }
}
