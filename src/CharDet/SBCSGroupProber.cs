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
using System.Collections;

namespace Mozilla.CharDet
{
    class SBCSGroupProber: AbstractCSProber
    {
        #region Constants
        //const int NUM_OF_SBCS_PROBERS = 13;
        #endregion

        public SBCSGroupProber()
        {
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Win1251Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Koi8rModel));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Latin5Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.MacCyrillicModel));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Ibm866Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Ibm855Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Latin7Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Win1253Model));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Latin5BulgarianModel));
            mProbers.Add(new SingleByteCharSetProber(SequenceModel.Win1251BulgarianModel));

            HebrewProber hebprober = new HebrewProber();
            SingleByteCharSetProber logical, visual;
            mProbers.Add(hebprober);
            mProbers.Add(logical = new SingleByteCharSetProber(SequenceModel.Win1255Model, false, hebprober)); // Logical Hebrew
            mProbers.Add(visual = new SingleByteCharSetProber(SequenceModel.Win1255Model, true, hebprober)); // Visual Hebrew
            // Tell the Hebrew prober about the logical and visual probers
            hebprober.SetModelProbers(logical, visual);

            // disable latin2 before latin1 is available, otherwise all latin1 
            // will be detected as latin2 because of their similarity.
            //mProbers.Add(new nsSingleByteCharSetProber(SequenceModel.Latin2HungarianModel));
            //mProbers.Add(new nsSingleByteCharSetProber(SequenceModel.Win1250HungarianModel));

            Reset();
        }


        #region Functions
        public override ProbingState HandleData(byte[] buffer)
        {
            return HandleData(buffer, buffer.Length);
        }

        public override ProbingState HandleData(byte[] buffer, int length)
        {
            ProbingState st;
            byte[] filtered = new byte[buffer.Length];
            int filteredLength = 0;

            //apply filter to original buffer, and we got new buffer back
            //depend on what script it is, we will feed them the new buffer 
            //we got after applying proper filter
            //this is done without any consideration to KeepEnglishLetters
            //of each prober since as of now, there are no probers here which
            //recognize languages with English characters.
            filteredLength = FilterWithoutEnglishLetters(buffer, filtered);

            if (filteredLength == 0)
                return mState; // Nothing to see here, move on.

            foreach (AbstractCSProber prober in mProbers)
            {
                if (!prober.IsActive) continue;

                st = prober.HandleData(filtered,filteredLength);
                if (st == ProbingState.FoundIt)
                {
                    mBestGuess = prober;
                    mState = ProbingState.FoundIt;
                    break;
                }
                else if (st == ProbingState.NotMe)
                {
                    prober.IsActive = false;
                    mActiveNum--;
                    if (mActiveNum <= 0)
                    {
                        mState = ProbingState.NotMe;
                        break;
                    }
                }
            }

            return mState;
        }


        public override void Reset()
        {
            mActiveNum = 0;
            foreach (AbstractCSProber prober in mProbers)
            {
                if (prober!=null) // not null
                {
                    prober.Reset();
                    prober.IsActive = true;
                    ++mActiveNum;
                }
            }
            mBestGuess = null;
            mState = ProbingState.Detecting;
        }

        #endregion

        #region Accessors
        public override string CharSetName
        {
            get
            {
                //if we have no answer yet
                if (mBestGuess == null)
                {
                    GetConfidence();
                    //no charset seems positive
                    if (mBestGuess == null)
                        //we will use default.
                        mBestGuess = (AbstractCSProber)mProbers[0];
                }
                return mBestGuess.CharSetName;
            }
        }

        public override ProbingState State { get { return mState; } }

        public override float GetConfidence()
        {
            float bestConf = 0.0f, cf;

            switch (mState)
            {
                case ProbingState.FoundIt:
                    return (float)0.99; //sure yes
                case ProbingState.NotMe:
                    return (float)0.01;  //sure no
                default:
                    foreach (AbstractCSProber prober in mProbers)
                    {
                        if (!prober.IsActive)
                            continue;
                        cf = prober.GetConfidence();
                        if (bestConf < cf)
                        {
                            bestConf = cf;
                            mBestGuess = prober;
                        }
                    }
                    break;
            }
            return bestConf;
        }
        public override void SetOpion() { }

        public override bool IsActive
        {
            get { return mActiveNum > 0; }
            set { if (!value) mActiveNum = 0; }
        }
        #endregion

        #region privates
        private ProbingState mState;
        private ArrayList mProbers = new ArrayList();
        private AbstractCSProber mBestGuess;
        private int mActiveNum;
        #endregion

#if DEBUG
        void DumpStatus()
        {

        }
#endif
    }
}
