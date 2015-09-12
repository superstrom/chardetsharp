using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public class TIS620CharSetProber : SingleByteCharSetProber
    {
        public TIS620CharSetProber()
            : base(SequenceModel.TIS620ThaiModel)
        { }
    }
}
