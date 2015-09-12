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
    public class BulgarianDetectionTestFixture : BaseDetectionTestFixure
    {
#if !NUNIT
        public TestContext TestContext { get; set; }
#endif

        [TestMethod]
        public void TestLatin5Detection()
        {
            RunBulgarianTest(Encoding.GetEncoding("ISO-8859-5"));
        }

        [TestMethod]
        public void Win1251Detection()
        {
            RunBulgarianTest(Encoding.GetEncoding("windows-1251"));
        }

        [TestMethod]
        public void TestUtf8Detection()
        {
            RunBulgarianTest(Encoding.UTF8);
        }

        internal void RunBulgarianTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.bg.utf-8.txt",
                new Latin5BulgarianCharSetProber(),
                new Win1251BulgarianCharSetProber());
        }       
    }
}
