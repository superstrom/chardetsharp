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
using System.Collections.Generic;
using System.Text;

namespace CharDetSharp.UniversalCharDet
{
    public class SBCSGroupProber : ICharSetProber
    {
        private IList<ICharSetProber> probers = new List<ICharSetProber>();
        private bool isActive;
        private ProbingState state;
        private ICharSetProber bestGuess;
        private int activeNum;

        public SBCSGroupProber()
        {
            this.probers.Add(new Win1251CharSetProber());
            this.probers.Add(new Koi8RCharSetProber());
            this.probers.Add(new Latin5CharSetProber());
            this.probers.Add(new MacCyrillicCharSetProber());
            this.probers.Add(new Ibm866CharSetProber());
            this.probers.Add(new Ibm855CharSetProber());

            this.probers.Add(new Latin7CharSetProber());
            this.probers.Add(new Win1253CharSetProber());
            this.probers.Add(new Latin5BulgarianCharSetProber());
            this.probers.Add(new Win1251BulgarianCharSetProber());
            this.probers.Add(new Latin2CharSetProber());
            this.probers.Add(new Win1250CharSetProber());
            this.probers.Add(new TIS620CharSetProber());
            //this.probers.Add(new HebrewGroupProber());

            this.bestGuess = this.probers[0];
            this.activeNum = this.probers.Count;
            this.isActive = true;
        }

        public ICharSetProber this[int i]
        {
            get { return this.probers[i]; }
        }

        public string CharSetName
        {
            get { return this.bestGuess.CharSetName; }
        }

        public Encoding CharSet
        {
            get { return this.bestGuess.CharSet; }
        }

        public ProbingState State
        {
            get { return this.state; }
        }

        public float Confidence
        {
            get
            {
                float bestConf = 0.0f, cf;

                switch (this.state)
                {
                    case ProbingState.FoundIt:
                        return (float)0.99; // sure yes
                    case ProbingState.NotMe:
                        return (float)0.01;  // sure no
                    default:
                        foreach (ICharSetProber prober in this.probers)
                        {
                            if (!prober.IsActive)
                                continue;
                            cf = prober.Confidence;
                            if (bestConf < cf)
                            {
                                bestConf = cf;
                                this.bestGuess = prober;
                            }
                        }
                        break;
                }
                return bestConf;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                if (!this.isActive && value)
                    this.Reset();
                this.isActive = value;
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
            if (!this.isActive) return this.state;
            // otherwise, we continue, even if we've made up our mind.

            foreach (ICharSetProber prober in this.probers)
            {
                if (!prober.IsActive) continue;

                ProbingState st = prober.HandleData(buffer, start, length);
                if (st == ProbingState.FoundIt)
                {
                    this.bestGuess = prober;
                    this.state = ProbingState.FoundIt;
                    break;
                }
                else if (st == ProbingState.NotMe)
                {
                    prober.IsActive = false;
                    this.activeNum--;
                    if (this.activeNum <= 0)
                    {
                        this.state = ProbingState.NotMe;
                        this.isActive = false;
                        break;
                    }
                }
            }

            return this.state;
        }

        public void Reset()
        {
            foreach (ICharSetProber prober in this.probers)
            {
                prober.Reset();
                prober.IsActive = true;
            }
            this.isActive = true;
            this.activeNum = this.probers.Count;
        }
    }
}
