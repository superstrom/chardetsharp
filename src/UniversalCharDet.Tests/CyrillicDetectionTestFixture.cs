using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharDetSharp.UniversalCharDet.Tests
{
    [TestClass]
    public class CyrillicDetectionTestFixture : BaseDetectionTestFixure
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestKoi8RDetection()
        {
            RunCyrillicTest(Encoding.GetEncoding("koi8-r"));
        }

        [TestMethod]
        public void TestWin1251Detection()
        {
            RunCyrillicTest(Encoding.GetEncoding("windows-1251"));
        }

        [TestMethod]
        public void TestLatin5Detection()
        {
            RunCyrillicTest(Encoding.GetEncoding("iso-8859-5"));
        }

        [TestMethod]
        public void TestMacCyrillicDetection()
        {
            RunCyrillicTest(Encoding.GetEncoding("x-mac-cyrillic"));
        }

        [TestMethod]
        public void TestIbm855Detection()
        {
            RunCyrillicTest(Encoding.GetEncoding("IBM855"));
        }

        [TestMethod]
        public void TestIbm866Detection()
        {
            RunCyrillicTest(Encoding.GetEncoding("cp866"));
        }

        [Ignore]
        [TestMethod]
        public void TestUtf8Detection()
        {
            RunCyrillicTest(Encoding.UTF8);
        }

        [Ignore]
        [TestMethod]
        public void TestUnicodeDetection()
        {
            RunCyrillicTest(Encoding.Unicode);
        }

        internal void RunCyrillicTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.ru.utf-8.txt",
                new Koi8RCharSetProber(),
                new Win1251CharSetProber(),
                new Latin5CharSetProber(),
                new MacCyrillicCharSetProber(),
                new Ibm855CharSetProber(),
                new Ibm866CharSetProber());
        }
    }
}

