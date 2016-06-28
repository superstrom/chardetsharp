using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharDetSharp.UniversalCharDet.Tests
{
    [TestClass]
    public class BulgarianDetectionTestFixture : BaseDetectionTestFixure
    {
        public TestContext TestContext { get; set; }

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

        [Ignore]
        [TestMethod]
        public void TestUtf8Detection()
        {
            RunBulgarianTest(Encoding.UTF8);
        }

        [Ignore]
        [TestMethod]
        public void TestUnicodeDetection()
        {
            RunBulgarianTest(Encoding.Unicode);
        }

        internal void RunBulgarianTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.bg.utf-8.txt",
                new Latin5BulgarianCharSetProber(),
                new Win1251BulgarianCharSetProber());
        }       
    }
}
