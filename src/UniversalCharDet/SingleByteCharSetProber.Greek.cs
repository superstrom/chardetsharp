using System;
using System.Text;

using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public class Latin7CharSetProber : SingleByteCharSetProber
    {
        public Latin7CharSetProber()
            : base(SequenceModel.Latin7Model)
        { }
    }

    public class Win1253CharSetProber : SingleByteCharSetProber
    {
        public Win1253CharSetProber()
            : base(SequenceModel.Win1253Model)
        { }
    }
}
