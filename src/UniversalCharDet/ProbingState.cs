
namespace CharDetSharp.UniversalCharDet
{
    using System;

    /// <summary>
    /// State of the CharSetProber.
    /// </summary>
    public enum ProbingState
    {
        /// <summary>
        /// We are still detecting, no sure answer yet, but caller can ask for confidence.
        /// </summary>
        Detecting = 0,

        /// <summary>
        /// That's a positive answer
        /// </summary>
        FoundIt = 1,

        /// <summary>
        /// Negative answer
        /// </summary>
        NotMe = 2,
    }
}
