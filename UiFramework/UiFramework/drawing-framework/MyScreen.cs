﻿using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework {
  /**
    * The purpose of this class is to link an LCD panel to a MyCanvas.
    * The drawing on the canvas is represented on the target LCD panel
    * as a string of characters representing the pixels of the canvas.
    */
    public class MyScreen {
     // Properties needed for the functionality of the screen
        public IMyTextSurface TargetSurface;
        private MyCanvas Canvas;
        private bool mirrorX;

     // The buffer where the actual content of the text surface is computed
        private StringBuilder RenderedBuffer;

     // Properties which are also needed by FlushBufferToScreen()
        private char pixelValueOn, pixelValueOff;

     // Clipping options
        private int clipRectX1, clipRectY1, clipRectX2, clipRectY2;
        private bool isClipping = false;

        public MyScreen(IMyTextSurface TargetSurface, char pixelValueOn, char pixelValueOff, bool mirrorX) {
            this.TargetSurface = TargetSurface;
            this.mirrorX = mirrorX;
            this.pixelValueOn = pixelValueOn;
            this.pixelValueOff = pixelValueOff;
        }

        public MyScreen WithCanvas(MyCanvas Canvas) {
            this.Canvas = Canvas;
            return this;
        }

     /**
       * Turns on the buffer clipping option, which draws just part of the buffer
       * on the screen. This helps in splitting the buffer onto multiple screens.
       */
        public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) {
            clipRectX1 = x1 > x2 ? x2 : x1;
            clipRectY1 = y1 > y2 ? y2 : y1;
            clipRectX2 = x1 > x2 ? x1 : x2;
            clipRectY2 = y1 > y2 ? y1 : y2;
            isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY();

            return this;
        }

      /**
        * Unlike glass windows, for which there are versions with the tinted glass
        * both on the outside and the inside, LCD panels display the text only on
        * one side. This becomes an issue when dealing with transparent LCDs, which
        * display the text on both sides. On one side, the text will be mirrored.
        * If that's the side that needs to be seen, then the content must be adjusted
        * so that it displays properly. Hence, the method MirrorBufferOnXAxis().
        */
        private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) {
            int length = Buffer.Count();

            bool[] MirroredBuffer = new bool[length];

            int mirrorPosX = resX - 1;
            int mirrorPos = mirrorPosX;

            for (int sourcePos = 0; sourcePos < length; sourcePos++) {
                MirroredBuffer[mirrorPos] = Buffer[sourcePos];

                mirrorPos--;
                mirrorPosX--;
                if (mirrorPosX == -1) {
                    mirrorPosX = resX - 1;
                    mirrorPos += resX * 2;
                }
            }

            return MirroredBuffer;
        }

      /**
        * Copies a subset of the buffer and returns a reference to the copy.
        * Useful for splitting the buffer on many screens
        */
        private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) {
         // Make sure the coordinates are ordered properly
            int rectX1 = x1 > x2 ? x2 : x1;
            int rectY1 = y1 > y2 ? y2 : y1;
            int rectX2 = x1 > x2 ? x1 : x2;
            int rectY2 = y1 > y2 ? y1 : y2;

         // Make sure the coordinates are not off screen
            if (rectX1 < 0) rectX1 = 0;
            if (rectY1 < 0) rectY1 = 0;
            if (rectX2 > resX) rectX2 = resX;
            if (rectY2 > resY) rectY2 = resY;

         // Initialize the return buffer
            bool[] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1];

         // Initialize the cursors
            int srcCursor = rectY1 * resX + rectX1;
            int trgCursor = 0;

         // Copy the source section into the target buffer
            for (int srcY = rectY1; srcY < rectY2; srcY++) {
                for (int srcX = rectX1; srcX < rectX2; srcX++) {
                    ret[trgCursor] = Buffer[srcCursor];
                    ret[trgCursor] = true;
                    trgCursor++;
                    srcCursor++;
                }
                srcCursor += rectX1;
            }

            return ret;
        }

      /**
        * Converts the specified block of the bool[] buffer of the canvas into a
        * string of pixels which is then copied over to the Text property of the
        * target text surface. If this is the first block, the text is replaced.
        * If this is not the first block, then the text is appended. This allows
        * splitting the rendering into multiple steps and running each step into
        * its own cycle, to allow drawing higher resolution images than the game
        * allows in one single cycle.
        */
        public void FlushBufferToScreen(bool invertColors, int currentBlock, int nBlocks) {
         // Get the buffer from the Canvas
            bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer();
            int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX();
            int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY();

         // In case the screen needs to be mirrored on the X axis
            bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer;

         // Compute the range to output
            int blockSize = (resY / nBlocks);
            int lowY  = currentBlock * blockSize;
            int highY = (currentBlock == nBlocks - 1) ? resY - 1 : lowY + blockSize;

            int capacity = resY * resX + resY + 1;

         // Clear the text from the target surface
            if (currentBlock == 0) {
                if (RenderedBuffer != null) {
                    TargetSurface.WriteText(RenderedBuffer);
                }

                RenderedBuffer = new StringBuilder(capacity);
                RenderedBuffer.EnsureCapacity(capacity);
            }

         // Swap ON / OFF pixel values in case invertColors = true
            char pxValOn  = invertColors ? pixelValueOff : pixelValueOn;
            char pxValOff = invertColors ? pixelValueOn : pixelValueOff;

         // Fill the rendered buffer
            for (int y = lowY ; y < highY ; y++) {
                for (int x = 0 ; x < resX ; x++) {
                    RenderedBuffer.Append(SourceBuffer[y * resX + x] ? pxValOn : pxValOff);
                }
                RenderedBuffer.Append('\n');
            }
        }

        public MyCanvas GetCanvas() {
            return Canvas;
        }
    }
}
