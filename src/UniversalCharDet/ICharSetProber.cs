using System;
using System.Text;

namespace CharDetSharp.UniversalCharDet
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICharSetProber
    {
        /// <summary>
        /// Gets the name of the character set that the prober is checking for or has identified.
        /// </summary>
        string CharSetName { get; }

        /// <summary>
        /// Gets the Encoding for the character set that the prober is checking for or has identified.
        /// </summary>
        Encoding CharSet { get; }

        /// <summary>
        /// Gets the current state of the prober.
        /// </summary>
        /// <returns></returns>
        ProbingState State { get; }

        /// <summary>
        /// Gets the confidence in the current character set choice.
        /// </summary>
        float Confidence { get; }

        /// <summary>Gets or sets whether the Prober is active</summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Processes a buffer, through the recognition state machine
        /// </summary>
        ProbingState HandleData(byte[] buffer);

        /// <summary>
        /// Processes a partial buffer, through the recognition state machine
        /// </summary>
        ProbingState HandleData(byte[] buffer, int start, int length);

        /// <summary>
        /// Resets the prober to its initial state
        /// </summary>
        void Reset();
    }
}
