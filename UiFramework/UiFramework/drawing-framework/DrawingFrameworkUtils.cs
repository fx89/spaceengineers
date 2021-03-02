using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework {
    public class DrawingFrameworkUtils {
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
    }
}
