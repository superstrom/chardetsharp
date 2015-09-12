using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public class Latin5BulgarianCharSetProber : SingleByteCharSetProber
    {
        public Latin5BulgarianCharSetProber()
            : base(SequenceModel.Latin5BulgarianModel)
        { }
    }

    public class Win1251BulgarianCharSetProber : SingleByteCharSetProber
    {
        public Win1251BulgarianCharSetProber()
            : base(SequenceModel.Win1251BulgarianModel)
        { }
    }
}
