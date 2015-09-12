/* 
 * C# port of Mozilla Character Set Detector
 * 
 * Original Mozilla License Block follows
 * 
 */

#region License Block
// Version: MPL 1.1/GPL 2.0/LGPL 2.1
//
// The contents of this file are subject to the Mozilla Public License Version
// 1.1 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
// http://www.mozilla.org/MPL/
//
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
// for the specific language governing rights and limitations under the
// License.
//
// The Original Code is Mozilla Universal charset detector code.
//
// The Initial Developer of the Original Code is
// Netscape Communications Corporation.
// Portions created by the Initial Developer are Copyright (C) 2001
// the Initial Developer. All Rights Reserved.
//
// Contributor(s):
//          Shy Shalom <shooshX@gmail.com>
//
// Alternatively, the contents of this file may be used under the terms of
// either the GNU General Public License Version 2 or later (the "GPL"), or
// the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
// in which case the provisions of the GPL or the LGPL are applicable instead
// of those above. If you wish to allow use of your version of this file only
// under the terms of either the GPL or the LGPL, and not to allow others to
// use your version of this file under the terms of the MPL, indicate your
// decision by deleting the provisions above and replace them with the notice
// and other provisions required by the GPL or the LGPL. If you do not delete
// the provisions above, a recipient may use your version of this file under
// the terms of any one of the MPL, the GPL or the LGPL.
#endregion

using System;
using System.Text;

using CharDetSharp.UniversalCharDet.Model;

namespace CharDetSharp.UniversalCharDet
{
    public abstract class SingleByteCharSetProber : ICharSetProber
    {
        const int SAMPLE_SIZE = 64;
        const int SB_ENOUGH_REL_THRESHOLD = 1024;
        const float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;
        const float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;
        const float SURE_YES = 0.99f;
        const float SURE_NO = 0.01f;
        const int SYMBOL_CAT_ORDER = 250;
        const int NUMBER_OF_SEQ_CAT = 4;
        const int POSITIVE_CAT = (NUMBER_OF_SEQ_CAT - 1);
        const int NEGATIVE_CAT = 0;

        protected ProbingState currentState;
        protected SequenceModel model;
        protected bool reversed; // true if we need to reverse every pair in the model lookup
        protected bool active = true;
        protected byte lastOrder; //char order of last character
        protected int totalSeqs;
        protected int[] seqCounters = new int[NUMBER_OF_SEQ_CAT];
        protected int totalChar;
        protected int freqChar; //characters that fall in our sampling range

        protected SingleByteCharSetProber(SequenceModel mModel)
        {
            this.model = mModel;
        }

        public string CharSetName { get { return model.CharSet.WebName; } }
        public Encoding CharSet { get { return model.CharSet; } }
        public ProbingState State { get { return currentState; } }

        public float Confidence
        {
            get
            {
#if NEGATIVE_APPROACH
                if (mTotalSeqs > 0)
                    if (mTotalSeqs > mSeqCounters[NEGATIVE_CAT] * 10)
                        return ((float)(mTotalSeqs - mSeqCounters[NEGATIVE_CAT] * 10)) / mTotalSeqs * mFreqChar / mTotalChar;
                return SURE_NO;
#else  //POSITIVE_APPROACH
                float r;

                if (totalSeqs > 0)
                {
                    r = ((float)seqCounters[POSITIVE_CAT]) / totalSeqs / model.TypicalPositiveRatio;
                    r = r * freqChar / totalChar;
                    if (r >= 1.00f)
                        r = SURE_YES;
                    return r;
                }
                return SURE_NO;
#endif
            }
        }

        public bool IsActive
        {
            get
            {
                return active;
            }
            set
            {
                // not Active -> active
                if (!active && value)
                    Reset();
                else
                    active = value;
            }
        }

        public ProbingState HandleData(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", Properties.Resources.NullBufferExceptionMessage);

            return HandleData(buffer, 0, buffer.Length);
        }

        public ProbingState HandleData(byte[] buffer, int start, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", Properties.Resources.NullBufferExceptionMessage);
            if (start < 0)
                throw new ArgumentException(Properties.Resources.NegativeStartIndexExceptionMessage, "start");

            // if we are not active, we needn't do any work.
            if (!active) return currentState;

            // otherwise, we continue, even if we've made up our mind.

            byte order;

            int end = start + length;
            for (int i = start; i < buffer.Length && i < end; ++i)
            {
                order = model.CharToOrderMap[buffer[i]];

                if (order < SYMBOL_CAT_ORDER)
                    totalChar++;

                if (order < SAMPLE_SIZE)
                {
                    freqChar++;

                    if (lastOrder < SAMPLE_SIZE)
                    {
                        totalSeqs++;
                        if (!reversed)
                            ++(seqCounters[model.PrecedenceMatrix[lastOrder * SAMPLE_SIZE + order]]);
                        else // reverse the order of the letters in the lookup
                            ++(seqCounters[model.PrecedenceMatrix[order * SAMPLE_SIZE + lastOrder]]);
                    }
                }
                lastOrder = order;
            }

            if (currentState == ProbingState.Detecting)
                if (totalSeqs > SB_ENOUGH_REL_THRESHOLD)
                {
                    float cf = Confidence;
                    if (cf > POSITIVE_SHORTCUT_THRESHOLD)
                        currentState = ProbingState.FoundIt;
                    else if (cf < NEGATIVE_SHORTCUT_THRESHOLD)
                        currentState = ProbingState.NotMe;
                    //else
                    //  stay Detecting
                }

            return currentState;
        }

        public void Reset()
        {
            this.currentState = ProbingState.Detecting;
            this.lastOrder = 255;
            for (int i = 0; i < NUMBER_OF_SEQ_CAT; i++)
                seqCounters[i] = 0;
            this.totalSeqs = 0;
            this.totalChar = 0;
            this.freqChar = 0;
            this.active = true;
        }
    }
}
