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

namespace CharDetSharp.UnitTests
{
    [TestClass]
    public class GreekDetectionTestFixture : BaseDetectionTestFixure
    {
#if !NUNIT
        public TestContext TestContext { get; set; }
#endif

        [TestMethod]
        public void TestLatin7Detection()
        {
            RunGreekTest(Encoding.GetEncoding("ISO-8859-7"));
        }

        [TestMethod]
        public void Win1253Detection()
        {
            RunGreekTest(Encoding.GetEncoding("windows-1253"));
        }

        [TestMethod]
        public void TestUtf8Detection()
        {
            RunGreekTest(Encoding.UTF8);
        }

        internal void RunGreekTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UnitTests.Samples.el.utf-8.txt",
                new Latin7CharSetProber(),
                new Win1253CharSetProber());
        }
    }
}
