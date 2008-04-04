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
    public enum InputState : int
    {
        PureAscii = 0,
        EscAscii = 1,
        Highbyte = 2
    };

    public class UniversalDetector
    {
        const int NS_OK = 0;

        const float SHORTCUT_THRESHOLD = 0.95F;
        const float MINIMUM_THRESHOLD = 0.20F;


        public UniversalDetector()
        {
            mDone = false;
            mBestGuess = -1;   //illegal value as signal
            mInTag = false;
            mEscCharSetProber = null;

            mStart = true;
            mDetectedCharset = null;
            mGotData = false;
            mInputState = InputState.PureAscii;
            mLastChar = 0;

            mCharSetProbers.Clear();
        }

        public virtual int HandleData(byte[] aBuf)
        {
            if (mDone)
                return NS_OK;

            if (aBuf.Length > 0)
                mGotData = true;

            //If the data starts with BOM, we know it is UTF
            if (mStart)
            {
                mStart = false;
                if (aBuf.Length > 3)
                    switch (aBuf[0])
                    {
                        case 0xEF:
                            if ((0xBB == aBuf[1]) && (0xBF == aBuf[2]))
                                // EF BB BF  UTF-8 encoded BOM
                                mDetectedCharset = "UTF-8";
                            break;
                        case 0xFE:
                            if ((0xFF == aBuf[1]) && (0x00 == aBuf[2]) && (0x00 == aBuf[3]))
                                // FE FF 00 00  UCS-4, unusual octet order BOM (3412)
                                mDetectedCharset = "X-ISO-10646-UCS-4-3412";
                            else if (0xFF == aBuf[1])
                                // FE FF  UTF-16, big endian BOM
                                mDetectedCharset = "UTF-16BE";
                            break;
                        case 0x00:
                            if ((0x00 == aBuf[1]) && (0xFE == aBuf[2]) && (0xFF == aBuf[3]))
                                // 00 00 FE FF  UTF-32, big-endian BOM
                                mDetectedCharset = "UTF-32BE";
                            else if ((0x00 == aBuf[1]) && (0xFF == aBuf[2]) && (0xFE == aBuf[3]))
                                // 00 00 FF FE  UCS-4, unusual octet order BOM (2143)
                                mDetectedCharset = "X-ISO-10646-UCS-4-2143";
                            break;
                        case 0xFF:
                            if ((0xFE == aBuf[1]) && (0x00 == aBuf[2]) && (0x00 == aBuf[3]))
                                // FF FE 00 00  UTF-32, little-endian BOM
                                mDetectedCharset = "UTF-32LE";
                            else if (0xFE == aBuf[1])
                                // FF FE  UTF-16, little endian BOM
                                mDetectedCharset = "UTF-16LE";
                            break;
                    }  // switch

                if (mDetectedCharset != null)
                {
                    mDone = true;
                    return NS_OK;
                }
            }

            for (int i = 0; i < aBuf.Length; i++)
            {
                //other than 0xa0, if every othe character is ascii, the page is ascii
                if ((aBuf[i] & 0x80) > 0 && aBuf[i] != 0xA0)  //Since many Ascii only page contains NBSP 
                {
                    //we got a non-ascii byte (high-byte)
                    if (mInputState != InputState.Highbyte)
                    {
                        //adjust state
                        mInputState = InputState.Highbyte;

                        //kill mEscCharSetProber if it is active
                        if (mEscCharSetProber != null)
                        {
                            mEscCharSetProber = null;
                        }

                        //TODO: take out when implement nsMBCSGroupProber
                        mCharSetProbers.Add(new UTF8Prober());

                        //start multibyte and singlebyte charset prober
                        //mCharSetProbers.Add(new nsMBCSGroupProber());
                        mCharSetProbers.Add(new SBCSGroupProber());
                        mCharSetProbers.Add(new Latin1Prober());
                    }
                }
                else
                {
                    //ok, just pure ascii so far
                    if (InputState.PureAscii == mInputState &&
                    (aBuf[i] == (byte)33 || (aBuf[i] == '{' && mLastChar == '~')))
                    {
                        //found escape character or HZ "~{"
                        mInputState = InputState.EscAscii;
                    }
                    mLastChar = aBuf[i];
                }
            }

            ProbingState st;
            switch (mInputState)
            {
                case InputState.EscAscii:
                    /*
                      if (mEscCharSetProber == null) {
                          mEscCharSetProber = new nsEscCharSetProber();
                      }
           
                      st = mEscCharSetProber.HandleData(aBuf, aLen);
                      if (st == eFoundIt)
                      {
                          mDone = PR_TRUE;
                          mDetectedCharset = mEscCharSetProber.GetCharSetName();
                      }
                     */
                    break;
                case InputState.Highbyte:
                    foreach (AbstractCSProber prober in mCharSetProbers)
                    {
                        if (!prober.IsActive) continue;

                        st = prober.HandleData(aBuf);
                        if (st == ProbingState.FoundIt)
                        {
                            mDone = true;
                            mDetectedCharset = prober.CharSetName;
                            return NS_OK;
                        }
                    }
                    break;

                default:  //pure ascii
                    break;//do nothing here
            }
            return NS_OK;
        }

        public virtual void DataEnd()
        {
            if (!mGotData)
            {
                // we haven't got any data yet, return immediately 
                // caller program sometimes call DataEnd before anything has been sent to detector
                return;
            }

            if (mDetectedCharset != null)
            {
                mDone = true;
                Report(mDetectedCharset);
                return;
            }

            switch (mInputState)
            {
                case InputState.Highbyte:
                    {
                        AbstractCSProber maxProber=null;
                        foreach (AbstractCSProber prober in mCharSetProbers)
                        {
                            if (prober == null) continue;
                            if (maxProber == null || ( prober.GetConfidence() > maxProber.GetConfidence()) )
                            {
                                maxProber = prober;
                            }
                        }
                        //do not report anything because we are not confident of it, that's in fact a negative answer
                        if ( maxProber != null && (maxProber.GetConfidence() > MINIMUM_THRESHOLD))
                            Report(maxProber.CharSetName);
                    }
                    break;
                case InputState.EscAscii:
                    break;
                default:
                    break;
            }
            return;
        }

        public virtual void Report(string aCharset)
        {
            mDetectedCharset = aCharset;
            Console.Out.WriteLine(aCharset);
        }

        public virtual void Reset()
        {
            mDone = false;
            mBestGuess = -1;   //illegal value as signal
            mInTag = false;

            mStart = true;
            mDetectedCharset = null;
            mGotData = false;
            mInputState = InputState.PureAscii;
            mLastChar = 0;

            if (mEscCharSetProber != null)
                mEscCharSetProber.Reset();

            foreach (AbstractCSProber prober in mCharSetProbers)
                    prober.Reset();
        }

        public virtual string DetectedCharsetName
        {
            get { return mDetectedCharset; }
        }

        protected InputState mInputState;
        protected bool mDone;
        protected bool mInTag;
        protected bool mStart;
        protected bool mGotData;
        protected byte mLastChar;
        protected string mDetectedCharset;
        protected int mBestGuess;

        protected ArrayList mCharSetProbers = new ArrayList();
        protected AbstractCSProber mEscCharSetProber;
    }
}
