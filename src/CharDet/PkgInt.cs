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
    public enum nsIdxSft
    {
        eIdxSft4bits = 3,
        eIdxSft8bits = 2,
        eIdxSft16bits = 1
    };

    public enum nsSftMsk
    {
        eSftMsk4bits = 7,
        eSftMsk8bits = 3,
        eSftMsk16bits = 1
    };

    public enum nsBitSft
    {
        eBitSft4bits = 2,
        eBitSft8bits = 3,
        eBitSft16bits = 4
    };

    public enum nsUnitMsk
    {
        eUnitMsk4bits = 0x000F,
        eUnitMsk8bits = 0x00FF,
        eUnitMsk16bits = 0xFFFF
    };

    public class PkgInt
    {
        public nsIdxSft idxsft;
        public nsSftMsk sftmsk;
        public nsBitSft bitsft;
        public nsUnitMsk unitmsk;
        public int[] data;

        public PkgInt(nsIdxSft idxsft,
         nsSftMsk sftmsk,
         nsBitSft bitsft,
         nsUnitMsk unitmsk,
         int[] data)
        {
            this.idxsft = idxsft;
            this.sftmsk = sftmsk;
            this.bitsft = bitsft;
            this.unitmsk = unitmsk;
            this.data = data;
        }
        
        public static int PCK16BITS(int a, int b)
        {
            return ((int)(((b) << 16) | (a)));
        }

        public static int PCK8BITS(int a, int b, int c, int d)
        {
            return PCK16BITS((int)((b << 8) | a),
                            (int)((d << 8) | c));
        }

        public static int PCK4BITS(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            return PCK8BITS(((int)(((b) << 4) | (a))),
                            ((int)(((d) << 4) | (c))),
                            ((int)(((f) << 4) | (e))),
                            ((int)(((h) << 4) | (g))));
        }

        public static int GETFROMPCK(int i, PkgInt c)
        {
            return ((c.data[(i) >> (int)c.idxsft]) >> ((i & (int)c.sftmsk) << (int)c.bitsft)) & (int)c.unitmsk;
        }
    }
}
