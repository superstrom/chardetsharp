using System;
using System.IO;
using System.Text;

#if NUNIT
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using CharDetSharp.UniversalCharDet;

namespace CharDetSharp.UniversalCharDet.Tests
{
    [TestClass]
    public class ThaiDetectionTestFixture : BaseDetectionTestFixure
    {
#if !NUNIT
        public TestContext TestContext { get; set; }
#endif

        [TestMethod]
        public void TestLatin7Detection()
        {
            RunThaiTest(Encoding.GetEncoding("TIS-620"));
        }

        [TestMethod]
        public void TestUtf8Detection()
        {
            RunThaiTest(Encoding.UTF8);
        }

        internal void RunThaiTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.th.utf-8.txt",
                new TIS620CharSetProber());
        }
    }
}
