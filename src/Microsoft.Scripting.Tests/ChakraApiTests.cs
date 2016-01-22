using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Scripting.Tests
{
    [TestClass]
    public class ChakraApiTests
    {
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ThrowsIfFileIsMissing()
        {
            ChakraApi.FromFile("does.not.exist.dll");
        }

        [TestMethod]
        [ExpectedException(typeof(BadImageFormatException))]
        public void ThrowIfFileIsInvalid()
        {
            ChakraApi.FromFile("project.json");
        }
    }
}
