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
    class Big5Prober : AbstractCSProber
    {
        public Big5Prober()
        {
            mCodingSM = new CodingStateMachine(SMModel.Big5SMModel);
            Reset();
        }

        private CodingStateMachine mCodingSM;
        private ProbingState mState;
        //private Big5ContextAnalysis mContextAnalyser;
        private Big5DistributionAnalysis mDistributionAnalyser = new Big5DistributionAnalysis();
        private byte[] mLastChar = new byte[2];

        private bool active;

        public override void Reset()
        {
            mCodingSM.Reset();
            mState = ProbingState.Detecting;
            mDistributionAnalyser.Reset();
        }

        public override ProbingState HandleData(byte[] buffer)
        {
            return HandleData(buffer, buffer.Length);
        }

        public override ProbingState HandleData(byte[] aBuf, int aLen)
        {
            SMState codingState;

            for (int i = 0; i < aBuf.Length && i < aLen; i++)
            {
                codingState = mCodingSM.NextState(aBuf[i]);
                if (codingState == SMState.Error)
                {
                    mState = ProbingState.NotMe;
                    break;
                }
                if (codingState == SMState.ItsMe)
                {
                    mState = ProbingState.FoundIt;
                    break;
                }
                if (codingState == SMState.Start)
                {
                    int charLen = mCodingSM.CurrentCharLen;

                    if (i == 0)
                    {
                        mLastChar[1] = aBuf[0];
                        mDistributionAnalyser.HandleOneChar(mLastChar, charLen);
                    }
                    else
                        mDistributionAnalyser.HandleOneChar(new byte[] { aBuf[i - 1], aBuf[i] }, charLen);
                }
            }

            mLastChar[0] = aBuf[aLen - 1];

            if (mState == ProbingState.Detecting)
                if (mDistributionAnalyser.GotEnoughData() && GetConfidence() > SHORTCUT_THRESHOLD)
                    mState = ProbingState.FoundIt;

            return mState;
        }

        public override string CharSetName { get { return "Big5"; } }
        public override ProbingState State { get { return mState; } }

        public override float GetConfidence()
        {
            float distribCf = mDistributionAnalyser.Confidence;

            return distribCf;
        }

        public override void SetOpion() { }
        public override bool IsActive
        {
            get { return active; }
            set { active = value; }
        }
    }
}
