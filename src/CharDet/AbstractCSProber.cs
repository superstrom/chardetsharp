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
    public enum ProbingState
    {
        Detecting = 0,   //We are still detecting, no sure answer yet, but caller can ask for confidence.
        FoundIt = 1,     //That's a positive answer
        NotMe = 2        //Negative answer
    }

    public abstract class AbstractCSProber
    {
        protected const float SHORTCUT_THRESHOLD = 0.95F;

        /// <summary>
        /// Processes a buffer, through the recognition state machine
        /// </summary>
        /// <param name="aBuf">the buffer</param>
        /// <returns>the state of the state machine</returns>
        public abstract ProbingState HandleData(byte[] aBuf);

        /// <summary>
        /// Processes a partial buffer, through the recognition state machine
        /// </summary>
        /// <param name="aBuf"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public abstract ProbingState HandleData(byte[] aBuf, int length);

        /// <summary>
        /// Resets the prober to its initial state
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// retreives the name of the characterset that the prober is checking for
        /// </summary>
        /// <returns>a character set name</returns>
        public abstract string CharSetName { get; }

        /// <summary>
        /// the current state of the prober
        /// </summary>
        /// <returns></returns>
        public abstract ProbingState State { get; }

        /// <summary>the confidence in the current charset choice</summary>
        /// <returns></returns>
        public abstract float GetConfidence();

        /// <summary>Whether the Prober is Active</summary>
        public abstract bool IsActive { get; set; }

        /// <summary>
        /// unknown???
        /// </summary>
        public abstract void SetOpion();

        // Helper functions used in the Latin1 and Group probers.
        // both functions Allocate a new buffer for newBuf. This buffer should be 
        // freed by the caller using PR_FREEIF.
        // Both functions return PR_FALSE in case of memory allocation failure.

        //This filter applies to all scripts which do not use English characters
        public static int FilterWithoutEnglishLetters(byte[] inBuf, byte[] outBuf)
        {
            int newpos;
            int prevpos, curpos;

            bool meetMSB = false;
            newpos = 0;
            //byte[] outBuf = new byte[inBuf.Length];

            for (prevpos = curpos = 0; curpos < inBuf.Length; curpos++)
            {
                if ((inBuf[curpos] & 0x80) > 0)
                {
                    meetMSB = true;
                }
                else if (inBuf[curpos] < 'A' || (inBuf[curpos] > 'Z' && inBuf[curpos] < 'a') || inBuf[curpos] > 'z')
                {
                    if (meetMSB && curpos > prevpos)
                    {
                        while (prevpos < curpos) { outBuf[newpos++] = inBuf[prevpos++]; }
                        prevpos++;
                        outBuf[newpos++] = (byte)' ';
                        meetMSB = false;
                    }
                }
                else prevpos = curpos + 1;
            }
            if (meetMSB && curpos > prevpos)
            {
                while (prevpos < curpos)
                {
                    outBuf[newpos++] = inBuf[prevpos++];
                }
            }
            return newpos;
        }

        public static int FilterWithEnglishLetters(byte[] inBuf, byte[] outBuf)
        {
            if (inBuf == null || outBuf == null) return 0;

            int newptr;
            int prevPtr, curPtr;
            bool isInTag = false;

            //byte[] outBuf = new byte[inBuf.Length];
            newptr = 0;
            for (curPtr = prevPtr = 0; curPtr < inBuf.Length; curPtr++)
            {

                if (inBuf[curPtr] == '>')
                    isInTag = false;
                else if (inBuf[curPtr] == '<')
                    isInTag = true;

                if ((inBuf[curPtr] & 0x80) == 0 &&
                    (inBuf[curPtr] < 'A' || (inBuf[curPtr] > 'Z' && inBuf[curPtr] < 'a') || inBuf[curPtr] > 'z'))
                {
                    if (curPtr > prevPtr && !isInTag) // Current segment contains more than just a symbol 
                    // and it is not inside a tag, keep it.
                    {
                        while (prevPtr < curPtr) outBuf[newptr++] = inBuf[prevPtr++];
                        prevPtr++;
                        outBuf[newptr++] = (byte)' ';
                    }
                    else
                        prevPtr = curPtr + 1;
                }
            }

            if (!isInTag)
                while (prevPtr < curPtr)
                    outBuf[newptr++] = inBuf[prevPtr++];

            return newptr;

        }
    }
}
