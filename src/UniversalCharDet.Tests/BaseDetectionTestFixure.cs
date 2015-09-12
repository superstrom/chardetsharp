using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharDetSharp.UniversalCharDet.Tests
{
    public class BaseDetectionTestFixure
    {
        public void RunSBCSGroupTest(Encoding enc, string testResourceName, params ICharSetProber[] others)
        {
            Console.Out.WriteLine("Testing [{0}]", enc.WebName);

            ICharSetProber group = new SBCSGroupProber();

            using (StreamReader reader = new StreamReader(typeof(BaseDetectionTestFixure).Assembly.GetManifestResourceStream(testResourceName)))
            {
                StringBuilder sb = new StringBuilder();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    byte[] bytes = enc.GetBytes(line + "\n");

                    group.HandleData(bytes);
                    foreach (ICharSetProber other in others)
                        other.HandleData(bytes);

                    sb.Length = 0;
                    foreach (ICharSetProber other in others)
                        sb.AppendFormat("{0}\t", other.Confidence);
                    sb.AppendFormat("[{0}]", group.Confidence);

                    Console.Out.WriteLine(sb.ToString());

                    continue;
                }
            }

            Console.Out.WriteLine("Expected: [{0}]   Got: [{1}]  Confidence: [{2}]", enc.WebName, group.CharSet.WebName, group.Confidence);

            Assert.AreEqual(enc.WebName, group.CharSet.WebName);

            group.Reset();
        }
    }
}
