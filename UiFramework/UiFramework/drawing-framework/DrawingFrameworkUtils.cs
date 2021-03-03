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

        public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight) {
            return ResizeSpriteCanvas(Sprite, newWidth, newHeight, DrawingFrameworkConstants.HORIZONTAL_ALIGN_CENTER, DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE);
        }

        public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight, int horizontalAlignment, int verticalAlignment) {
         // Garbage in, garbage out
            if (Sprite == null || newWidth < 1 || newHeight < 1) {
                return null;
            }

         // Create a new canvas to paint the original sprite to
            MyCanvas NewCanvas = new MyCanvas(newWidth, newHeight);

         // Compute the coordinates
            int posX = ComputePos(Sprite.width, newWidth, horizontalAlignment);
            int posY = ComputePos(Sprite.height, newHeight, verticalAlignment);

         // Draw the sprite onto the canvas
            NewCanvas.BitBlt(Sprite, posX, posY);

         // Return a new sprite with the data of a new canvas
            return new MySprite(newWidth, newHeight, NewCanvas.GetBuffer());
        }

        private static int ComputePos(int origSize, int newSize, int alignemnt) {
         // Vertical alignment constants have the same values as horizontal alignment constants
            if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE) {
                return (newSize - origSize) / 2;
            }

            if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_BOTTOM) {
                return newSize - 1 - origSize;
            }

            return 0;
        }
    }
}
