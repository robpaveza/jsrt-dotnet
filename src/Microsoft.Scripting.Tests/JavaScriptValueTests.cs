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
    public class JavaScriptValueTests
    {
        private JavaScriptRuntime runtime_;
        private JavaScriptEngine engine_;

        private JavaScriptValue number1_, number0_, stringText_, stringEmpty_, true_, false_,
            null_, undefined_;

        [TestInitialize]
        public void Setup()
        {
            runtime_ = new JavaScriptRuntime();
            engine_ = runtime_.CreateEngine();

            using (engine_.AcquireContext())
            {
                number1_ = engine_.Converter.FromInt32(1);
                number0_ = engine_.Converter.FromInt32(0);
                stringText_ = engine_.Converter.FromString("text");
                stringEmpty_ = engine_.Converter.FromString("");
                true_ = engine_.TrueValue;
                false_ = engine_.FalseValue;
                null_ = engine_.NullValue;
                undefined_ = engine_.UndefinedValue;
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            number1_ = number0_ = stringText_ = stringEmpty_ = true_ = false_ = null_ =
                undefined_ = null;

            engine_.Dispose();
            engine_ = null;
            runtime_.Dispose();
            runtime_ = null;
        }

        [TestMethod]
        public void IsTruthyIsCorrectForPrimitives()
        {
            using (engine_.AcquireContext())
            {
                Assert.IsTrue(number1_.IsTruthy);
                Assert.IsFalse(number0_.IsTruthy);
                Assert.IsTrue(stringText_.IsTruthy);
                Assert.IsFalse(stringEmpty_.IsTruthy);
                Assert.IsTrue(true_.IsTruthy);
                Assert.IsFalse(false_.IsTruthy);
                Assert.IsFalse(null_.IsTruthy);
                Assert.IsFalse(undefined_.IsTruthy);
            }
        }

        [TestMethod]
        public void SimpleEqualsObeysJavaScriptRules()
        {
            using (engine_.AcquireContext())
            {
                Assert.IsTrue(number0_.SimpleEquals(stringEmpty_));
                Assert.IsTrue(number0_.SimpleEquals(false_));
                Assert.IsFalse(number0_.SimpleEquals(null_));
                Assert.IsFalse(number0_.SimpleEquals(undefined_));

                Assert.IsTrue(stringEmpty_.SimpleEquals(false_));
                Assert.IsFalse(stringEmpty_.SimpleEquals(null_));
                Assert.IsFalse(stringEmpty_.SimpleEquals(undefined_));

                Assert.IsFalse(false_.SimpleEquals(null_));
                Assert.IsFalse(false_.SimpleEquals(undefined_));

                Assert.IsTrue(null_.SimpleEquals(undefined_));

                Assert.IsFalse(number0_.SimpleEquals(number1_));
            }
        }

        [TestMethod]
        public void StrictEqualsIsFalseForEverything()
        {
            using (engine_.AcquireContext())
            {
                Assert.IsFalse(number0_.StrictEquals(stringEmpty_));
                Assert.IsFalse(number0_.StrictEquals(false_));
                Assert.IsFalse(number0_.StrictEquals(null_));
                Assert.IsFalse(number0_.StrictEquals(undefined_));

                Assert.IsFalse(stringEmpty_.StrictEquals(false_));
                Assert.IsFalse(stringEmpty_.StrictEquals(null_));
                Assert.IsFalse(stringEmpty_.StrictEquals(undefined_));

                Assert.IsFalse(false_.StrictEquals(null_));
                Assert.IsFalse(false_.StrictEquals(undefined_));

                Assert.IsFalse(null_.StrictEquals(undefined_));

                Assert.IsFalse(number0_.StrictEquals(number1_));
            }
        }

        [TestMethod]
        public void DynamicConversionsFunctionAsExpected()
        {
            using (engine_.AcquireContext())
            {
                dynamic num1 = number1_;
                Assert.AreEqual(1, (int)num1);
                Assert.AreEqual(1.0, (double)num1);
                Assert.AreEqual("1", (string)num1);
                Assert.IsTrue((bool)num1);

                if (num1)
                {
                    // pass
                }
                else
                {
                    Assert.Fail("Should not be encountered.");
                }

                Assert.AreEqual(-1.0, -num1);

                dynamic tru = true_;
                Assert.AreEqual(-1, -tru);

                dynamic fal = false_;
                Assert.AreEqual(-0.0, -fal);

                dynamic txt = stringText_;
                Assert.IsTrue(double.IsNaN(-txt));

                Assert.AreEqual(1.0, +num1);
                Assert.AreEqual(1, +tru);
                Assert.AreEqual(0.0, +fal);
                Assert.IsTrue(double.IsNaN(+txt));
            }
        }
    }
}
