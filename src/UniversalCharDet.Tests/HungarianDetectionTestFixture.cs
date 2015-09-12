using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharDetSharp.UniversalCharDet.Tests
{
    [TestClass]
    public class HungarianDetectionTestFixture : BaseDetectionTestFixure
    {
        public TestContext TestContext { get; set; }

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

        [Ignore]
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
