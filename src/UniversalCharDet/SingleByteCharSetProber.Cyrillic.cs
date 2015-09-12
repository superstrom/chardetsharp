using System;
using System.Text;

using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public class Koi8RCharSetProber : SingleByteCharSetProber
    {
        public Koi8RCharSetProber()
            : base(SequenceModel.Koi8rModel)
        { }
    }

    public class Win1251CharSetProber : SingleByteCharSetProber
    {
        public Win1251CharSetProber()
            : base(SequenceModel.Win1251Model)
        { }
    }

    public class Latin5CharSetProber : SingleByteCharSetProber
    {
        public Latin5CharSetProber()
            : base(SequenceModel.Latin5Model)
        { }
    }

    public class MacCyrillicCharSetProber : SingleByteCharSetProber
    {
        public MacCyrillicCharSetProber()
            : base(SequenceModel.MacCyrillicModel)
        { }
    }

    public class Ibm855CharSetProber : SingleByteCharSetProber
    {
        public Ibm855CharSetProber()
            : base(SequenceModel.Ibm855Model)
        { }
    }

    public class Ibm866CharSetProber : SingleByteCharSetProber
    {
        public Ibm866CharSetProber()
            : base(SequenceModel.Ibm866Model)
        { }
    }
}
