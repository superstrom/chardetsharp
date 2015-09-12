using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharDetSharp.UniversalCharDet.Tests
{
    [TestClass]
    public class GreekDetectionTestFixture : BaseDetectionTestFixure
    {
        public TestContext TestContext { get; set; }

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

        [Ignore]
        [TestMethod]
        public void TestUtf8Detection()
        {
            RunGreekTest(Encoding.UTF8);
        }

        internal void RunGreekTest(Encoding enc)
        {
            RunSBCSGroupTest(enc, @"CharDetSharp.UniversalCharDet.Tests.Samples.el.utf-8.txt",
                new Latin7CharSetProber(),
                new Win1253CharSetProber());
        }
    }
}
