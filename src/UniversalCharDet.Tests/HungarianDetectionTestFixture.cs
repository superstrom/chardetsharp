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
    public class HungarianDetectionTestFixture : BaseDetectionTestFixure
    {
#if !NUNIT
        public TestContext TestContext { get; set; }
#endif

        [TestMethod]
        public void TestLatin2Detection()
        {
            RunHungarianTest(Encoding.GetEncoding("ISO-8859-2"));
        }

        [TestMethod]
        public void Win1250Detection()
        {
            RunHungarianTest(Encoding.GetEncoding("windows-1250"));
        }

        [TestMethod]
        public void TestUtf8Detection()
        {
            RunHungarianTest(Encoding.UTF8);
        }

        internal void RunHungarianTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.hu.utf-8.txt",
                new Latin2CharSetProber(),
                new Win1250CharSetProber());
        }
    }
}
