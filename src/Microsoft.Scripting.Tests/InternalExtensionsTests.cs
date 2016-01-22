using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class InternalExtensionsTests
    {
        [TestMethod]
        public void PrependSingleWorksAsExpected()
        {
            var range = Enumerable.Range(2, 4);
            var toTest = range.PrependWith(1).ToArray();
            Assert.AreEqual(1, toTest[0]);
            Assert.AreEqual(2, toTest[1]);
            Assert.AreEqual(3, toTest[2]);
            Assert.AreEqual(4, toTest[3]);
        }

        [TestMethod]
        public void PrependMultipleWorksAsExpected()
        {
            var range = Enumerable.Range(3, 4);
            var toTest = range.PrependWith(1, 2).ToArray();
            Assert.AreEqual(1, toTest[0]);
            Assert.AreEqual(2, toTest[1]);
            Assert.AreEqual(3, toTest[2]);
            Assert.AreEqual(4, toTest[3]);
        }

        [TestMethod]
        public void PrependMultipleOfNoneWorksAsExpected()
        {
            var range = Enumerable.Range(2, 4);
            var toTest = range.PrependWith().ToArray();
            Assert.AreEqual(2, toTest[0]);
            Assert.AreEqual(3, toTest[1]);
            Assert.AreEqual(4, toTest[2]);
        }

    }
}
