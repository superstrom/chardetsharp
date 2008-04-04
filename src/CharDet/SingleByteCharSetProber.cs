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

namespace Mozilla.CharDet
{
    class SingleByteCharSetProber : AbstractCSProber
    {
        const int SAMPLE_SIZE = 64;
        const int SB_ENOUGH_REL_THRESHOLD = 1024;
        const float POSITIVE_SHORTCUT_THRESHOLD = 0.95f;
        const float NEGATIVE_SHORTCUT_THRESHOLD = 0.05f;
        const int SYMBOL_CAT_ORDER = 250;
        const int NUMBER_OF_SEQ_CAT = 4;
        const int POSITIVE_CAT = (NUMBER_OF_SEQ_CAT - 1);
        const int NEGATIVE_CAT = 0;

        public SingleByteCharSetProber(SequenceModel model) : this(model, false, null) { }
        public SingleByteCharSetProber(SequenceModel model, bool reversed, AbstractCSProber nameProber)
        {
            mModel = model;
            mReversed = reversed;
            mNameProber = nameProber;

            Reset();
        }

        ProbingState mState;
        SequenceModel mModel;
        bool mReversed; // PR_TRUE if we need to reverse every pair in the model lookup

        //char order of last character
        byte mLastOrder;

        int mTotalSeqs;
        int[] mSeqCounters = new int[NUMBER_OF_SEQ_CAT];

        int mTotalChar;
        //characters that fall in our sampling range
        int mFreqChar;

        // Optional auxiliary prober for name decision. created and destroyed by the GroupProber
        AbstractCSProber mNameProber;

        bool active;


        public override ProbingState HandleData(byte[] aBuf)
        {
            return HandleData(aBuf, aBuf.Length);
        }
        public override ProbingState HandleData(byte[] aBuf, int length)
        {
            byte order;

            for (int i = 0; i < aBuf.Length && i < length; i++)
            {
                order = mModel.charToOrderMap[aBuf[i]];

                if (order < SYMBOL_CAT_ORDER)
                    mTotalChar++;
                if (order < SAMPLE_SIZE)
                {
                    mFreqChar++;

                    if (mLastOrder < SAMPLE_SIZE)
                    {
                        mTotalSeqs++;
                        if (!mReversed)
                            ++(mSeqCounters[mModel.precedenceMatrix[mLastOrder * SAMPLE_SIZE + order]]);
                        else // reverse the order of the letters in the lookup
                            ++(mSeqCounters[mModel.precedenceMatrix[order * SAMPLE_SIZE + mLastOrder]]);
                    }
                }
                mLastOrder = order;
            }


            if (mState == ProbingState.Detecting)
                if (mTotalSeqs > SB_ENOUGH_REL_THRESHOLD)
                {
                    float cf = GetConfidence();
                    if (cf > POSITIVE_SHORTCUT_THRESHOLD)
                        mState = ProbingState.FoundIt;
                    else if (cf < NEGATIVE_SHORTCUT_THRESHOLD)
                        mState = ProbingState.NotMe;
                }

            return mState;
        }

        public override void Reset()
        {
            mState = ProbingState.Detecting;
            mLastOrder = 255;
            for (int i = 0; i < NUMBER_OF_SEQ_CAT; i++)
                mSeqCounters[i] = 0;
            mTotalSeqs = 0;
            mTotalChar = 0;
            mFreqChar = 0;
            active = true;
        }
        public override void SetOpion() { }

        public override ProbingState State { get { return mState; } }

        public override float GetConfidence()
        {
#if NEGATIVE_APPROACH
            if (mTotalSeqs > 0)
                if (mTotalSeqs > mSeqCounters[NEGATIVE_CAT] * 10)
                    return ((float)(mTotalSeqs - mSeqCounters[NEGATIVE_CAT] * 10)) / mTotalSeqs * mFreqChar / mTotalChar;
            return 0.01f;
#else  //POSITIVE_APPROACH
            float r;

            if (mTotalSeqs > 0)
            {
                r = ((float)1.0) * mSeqCounters[POSITIVE_CAT] / mTotalSeqs / mModel.mTypicalPositiveRatio;
                r = r * mFreqChar / mTotalChar;
                if (r >= 1.00f)
                    r = 0.99f;
                return r;
            }
            return 0.01f;
#endif
        }

        public override string CharSetName
        {
            get
            {
                if (mNameProber == null)
                    return mModel.charsetName;
                return mNameProber.CharSetName;
            }
        }

        public override bool IsActive
        {
            get { return active; }
            set { active = value; }
        }

  // This feature is not implemented yet. any current language model
  // contain this parameter as PR_FALSE. No one is looking at this
  // parameter or calling this method.
  // Moreover, the nsSBCSGroupProber which calls the HandleData of this
  // prober has a hard-coded call to FilterWithoutEnglishLetters which gets rid
  // of the English letters.
        public bool KeepEnglishLetters() {return mModel.keepEnglishLetter;} // (not implemented)

#if DEBUG
        internal void DumpStatus()
        {
            Console.Out.WriteLine("  SBCS: {0:0.000} [{1}]", GetConfidence(), CharSetName);
        }
#endif
    }
}
