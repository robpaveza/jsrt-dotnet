using Microsoft.Scripting.JavaScript;
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
    public class ArrayBufferTests
    {
        private JavaScriptRuntime runtime_;
        private JavaScriptEngine engine_;
        private JavaScriptArrayBuffer buffer_;
        private JavaScriptTypedArray typedArray_;
        private JavaScriptDataView dataView_;

        [TestInitialize]
        public void Setup()
        {
            runtime_ = new JavaScriptRuntime();
            engine_ = runtime_.CreateEngine();

            var baseline = new ScriptSource("test://init.js", @"(function(global) {
    global.buffer = new ArrayBuffer(1024);
    global.typedArray = new Uint8ClampedArray(buffer);
    global.dataView = new DataView(buffer, 1);
})(this);");
            using (engine_.AcquireContext())
            {
                engine_.Execute(baseline);

                buffer_ = (JavaScriptArrayBuffer)engine_.GetGlobalVariable("buffer");
                typedArray_ = (JavaScriptTypedArray)engine_.GetGlobalVariable("typedArray");
                dataView_ = (JavaScriptDataView)engine_.GetGlobalVariable("dataView");
            }
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
        public void DefaultPropertiesOfArrayBufferTypesAreCorrect()
        {
            using (engine_.AcquireContext())
            {
                Assert.AreEqual(1024u, buffer_.ByteLength);
                Assert.AreEqual(1024u, typedArray_.ByteLength);
                Assert.AreEqual(0u, typedArray_.ByteOffset);
                Assert.AreEqual(JavaScriptTypedArrayType.Uint8Clamped, typedArray_.ArrayType);
                Assert.AreEqual(1023u, dataView_.ByteLength);
                Assert.AreEqual(1u, dataView_.ByteOffset);
            }
        }

        [TestMethod]
        public void MemoryTypedArrayAndDataViewInteractCorrectly()
        {
            using (engine_.AcquireContext())
            {
                engine_.RuntimeExceptionRaised += (s, e) =>
                {
                    dynamic error = engine_.GetAndClearException();
                };
                using (var ibuf = buffer_.GetUnderlyingMemory())
                using (var reader = new BinaryReader(ibuf, Encoding.UTF8, true))
                using (var writer = new BinaryWriter(ibuf, Encoding.UTF8, true))
                {
                    writer.Write((int)0x01020304);

                    Assert.AreEqual(2, engine_.Converter.ToInt32(typedArray_.GetValueAtIndex(engine_.Converter.FromInt32(2))));
                    Assert.AreEqual((short)3, dataView_.GetInt8(0));
                    Assert.AreEqual((short)0x0102, dataView_.GetInt16(1, true));
                    Assert.AreEqual((short)0x0201, dataView_.GetInt16(1, false));
                }
            }
        }
    }
}
