using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public class Latin2CharSetProber : SingleByteCharSetProber
    {
        public Latin2CharSetProber()
            : base(SequenceModel.Latin2Model)
        { }
    }

    public class Win1250CharSetProber : SingleByteCharSetProber
    {
        public Win1250CharSetProber()
            : base(SequenceModel.Win1250Model)
        { }
    }
}
