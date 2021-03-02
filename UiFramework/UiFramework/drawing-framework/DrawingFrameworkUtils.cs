using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework {
    public class DrawingFrameworkUtils {
        private static readonly byte[] BYTE_POW = new byte[]{ 128,64,32,16,8,4,2,1 };

        public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) {
            if (BoolArray == null || BoolArray.Count() == 0) {
                return null;
            }

            bool[] ret = new bool[BoolArray.Count()];

            for (int i = 0; i < BoolArray.Count(); i++) {
                ret[i] = negate ? !BoolArray[i] : BoolArray[i];
            }

            return ret;
        }

        public static bool[] NegateBoolArray(bool[] BoolArray) {
            return CopyBoolArray(BoolArray, true);
        }

        public static bool[] ByteArrayToBoolArray(byte [] byteArray) {
            if (byteArray == null || byteArray.Length == 0) {
                return new bool[]{};
            }

            bool[] ret = new bool[byteArray.Length * 8];
            int retIdx = 0;

            for (int bIdx = 0 ; bIdx < byteArray.Length ; bIdx++) {
                byte b = byteArray[bIdx];
                for (int divIdx = 0 ; divIdx < 8 ; divIdx++) {
                    ret[retIdx] = (b / BYTE_POW[divIdx] > 0);
                    b = (byte)(b % BYTE_POW[divIdx]);
                    retIdx++;
                }
            }

            return ret;
        }
    }
}
