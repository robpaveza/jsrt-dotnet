using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class ScriptSourceTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceLocationThrows()
        {
            var src = new ScriptSource(null, "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSourceTextThrows()
        {
            var src = new ScriptSource("[eval code]", null);
        }
    }
}
