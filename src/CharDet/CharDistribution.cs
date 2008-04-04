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
    internal class CharDistributionAnalysis
    {
        public CharDistributionAnalysis() { Reset(); }

        //Feed a character with known length
        public void HandleOneChar(byte[] aStr, int aCharLen)
        {
            int order;

            //we only care about 2-bytes character in our distribution analysis
            order = (aCharLen == 2) ? GetOrder(aStr) : -1;

            if (order >= 0)
            {
                mTotalChars++;
                //order is valid
                if (order < mCharToFreqOrder.Length)
                {
                    if (512 > mCharToFreqOrder[order])
                        mFreqChars++;
                }
            }
        }

        const float SURE_YES = 0.99f;
        const float SURE_NO = 0.01f;

        const int MINIMUM_DATA_THRESHOLD = 4;
        const int ENOUGH_DATA_THRESHOLD = 1024;

        //return confidence base on received data
        public float Confidence
        {
            get
            {
                //if we didn't receive any character in our consideration range, or the
                // number of frequent characters is below the minimum threshold, return
                // negative answer
                if (mTotalChars <= 0 || mFreqChars <= MINIMUM_DATA_THRESHOLD)
                    return SURE_NO;

                if (mTotalChars != mFreqChars)
                {
                    float r = mFreqChars / ((mTotalChars - mFreqChars) * mTypicalDistributionRatio);

                    if (r < SURE_YES)
                        return r;
                }
                //normalize confidence, (we don't want to be 100% sure)
                return SURE_YES;
            }
        }

        //Reset analyser, clear any state 
        public void Reset()
        {
            mDone = false;
            mTotalChars = 0;
            mFreqChars = 0;
        }

        //This function is for future extension. Caller can use this function to control
        //analyser's behavior
        public void SetOpion() { }

        //It is not necessary to receive all data to draw conclusion. For charset detection,
        // certain amount of data is enough
        public bool GotEnoughData() { return mTotalChars > ENOUGH_DATA_THRESHOLD; }

        //we do not handle character base on its original encoding string, but 
        //convert this encoding string to a number, here called order.
        //This allow multiple encoding of a language to share one frequency table 
        internal virtual int GetOrder(byte[] str) { return -1; }

        //If this flag is set to true, detection is done and conclusion has been made
        private bool mDone;

        //The number of characters whose frequency order is less than 512
        private int mFreqChars;

        //Total character encounted.
        private int mTotalChars;

        //Mapping table to get frequency order from char order (get from GetOrder())
        internal int[] mCharToFreqOrder;

        //This is a constant value varies from language to language, it is used in 
        //calculating confidence. See my paper for further detail.
        internal float mTypicalDistributionRatio;
    }

    internal partial class EUCTWDistributionAnalysis : CharDistributionAnalysis
    {
        public EUCTWDistributionAnalysis()
        {
            mCharToFreqOrder = EUCTWCharToFreqOrder;
            mTypicalDistributionRatio = EUCTW_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for euc-TW encoding, we are interested 
        //  first  byte range: 0xc4 -- 0xfe
        //  second byte range: 0xa1 -- 0xfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            if (str[0] >= 0xc4)
                return 94 * (str[0] - 0xc4) + str[1] - 0xa1;
            else
                return -1;
        }
    }

    internal partial class EUCKRDistributionAnalysis : CharDistributionAnalysis
    {
        public EUCKRDistributionAnalysis()
        {
            mCharToFreqOrder = EUCKRCharToFreqOrder;
            mTypicalDistributionRatio = EUCKR_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for euc-KR encoding, we are interested 
        //  first  byte range: 0xb0 -- 0xfe
        //  second byte range: 0xa1 -- 0xfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            if (str[0] >= 0xb0)
                return 94 * (str[0] - 0xb0) + str[1] - 0xa1;
            else
                return -1;
        }
    }

    internal partial class GB2312DistributionAnalysis : CharDistributionAnalysis
    {
        public GB2312DistributionAnalysis()
        {
          mCharToFreqOrder = GB2312CharToFreqOrder;
          mTypicalDistributionRatio = GB2312_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for GB2312 encoding, we are interested 
        //  first  byte range: 0xb0 -- 0xfe
        //  second byte range: 0xa1 -- 0xfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            if (str[0] >= 0xb0 && str[1] >= 0xa1)
                return 94 * (str[0] - 0xb0) + str[1] - 0xa1;
            else
                return -1;
        }
    }

    internal partial class Big5DistributionAnalysis : CharDistributionAnalysis
    {
        public Big5DistributionAnalysis()
        {
            mCharToFreqOrder = Big5CharToFreqOrder;
            mTypicalDistributionRatio = BIG5_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for big5 encoding, we are interested 
        //  first  byte range: 0xa4 -- 0xfe
        //  second byte range: 0x40 -- 0x7e , 0xa1 -- 0xfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            if (str[0] >= 0xa4)
                if (str[1] >= 0xa1)
                    return 157 * (str[0] - 0xa4) + str[1] - 0xa1 + 63;
                else
                    return 157 * (str[0] - 0xa4) + str[1] - 0x40;
            else
                return -1;
        }
    }

    internal abstract partial class JISDistributionAnalysis : CharDistributionAnalysis
    {
        internal JISDistributionAnalysis() { }
    }

    internal partial class SJISDistributionAnalysis : JISDistributionAnalysis
    {
        public SJISDistributionAnalysis()
        {
            mCharToFreqOrder = JISCharToFreqOrder;
            mTypicalDistributionRatio = JIS_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for sjis encoding, we are interested 
        //  first  byte range: 0x81 -- 0x9f , 0xe0 -- 0xfe
        //  second byte range: 0x40 -- 0x7e,  0x81 -- oxfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            int order;
            if (str[0] >= 0x81 && str[0] <= 0x9f)
                order = 188 * (str[0] - 0x81);
            else if (str[0] >= 0xe0 && str[0] <= 0xef)
                order = 188 * (str[0] - 0xe0 + 31);
            else
                return -1;
            order += str[1] - 0x40;
            if (str[1] > 0x7f)
                order--;
            return order;
        }
    }

    internal partial class EUCJPDistributionAnalysis : JISDistributionAnalysis
    {
        public EUCJPDistributionAnalysis()
        {
            mCharToFreqOrder = JISCharToFreqOrder;
            mTypicalDistributionRatio = JIS_TYPICAL_DISTRIBUTION_RATIO;
        }

        //for euc-JP encoding, we are interested 
        //  first  byte range: 0xa0 -- 0xfe
        //  second byte range: 0xa1 -- 0xfe
        //no validation needed here. State machine has done that
        internal override int GetOrder(byte[] str)
        {
            if (str[0] >= 0xa0)
                return 94 * (str[0] - 0xa1) + str[1] - 0xa1;
            else
                return -1;
        }
    }
}
