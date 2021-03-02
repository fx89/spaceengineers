using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameScript.drawing_framework.fonts;

namespace IngameScript.drawing_framework {
  /**
    * Provides drawing functionality on a bool[] buffer. The bool[] buffe
    * can be copied into sprites, which may be drawn on other MyCanvas
    * objects. The canvas also functions as part of the screen (MyScreen
    * class), which applies its buffer to LCD panels.
    */
    public class MyCanvas {
        private bool[] Buffer;
        private int resX;
        private int resY;
        private int length;
        private MySprite[] DefaultFont;

        public MyCanvas(int resX, int resY) {
            this.resX = resX;
            this.resY = resY;
            length = resY * resX;
            Buffer = new bool[length];
            this.DefaultFont = DefaultMonospaceFont.BitmapFont;
        }

      /**
        * Return a reference to the buffer, for use by the screen
        */
        public bool[] GetBuffer() {
            return Buffer;
        }

        public int GetResX() {
            return resX;
        }

        public int GetResY() {
            return resY;
        }

      /**
        * Return a copy of the buffer, for use in the creation of new sprites
        */
        public bool[] GetBufferCopy() {
            return DrawingFrameworkUtils.CopyBoolArray(Buffer, false);
        }

      /**
        * A default font must be set if text is to be drawn on this canvas
        */
        public void SetDefaultFont(MySprite[] DefaultFont) {
            this.DefaultFont = DefaultFont;
        }

        public MySprite[] GetDefaultFont() {
            return DefaultFont;
        }

        public MyCanvas WithDefaultFont(MySprite[] DefaultFont) {
            SetDefaultFont(DefaultFont);
            return this;
        }

      /**
        * Fills the buffer with the given value (either on or off, caller's preference)
        */
        public void Clear(bool value = false) {
            for (int x = 0; x < length; x++) {
                Buffer[x] = value;
            }
        }

      /**
        * Handles drawing options (transparency, color inversion)
        */
        private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) {
            if (invertColors) {
                if (transparentBackground) {
                    return targetPixelValue && !sourcePixelValue;
                } else {
                    return !sourcePixelValue;
                }
            } else {
                if (transparentBackground) {
                    return sourcePixelValue || targetPixelValue;
                } else {
                    return sourcePixelValue;
                }
            }
        }

      /**
        * Copies the bits of the given sprite to the chosen location on the canvas
        */
        public void BitBlt(MySprite sprite, int x, int y) {
            BitBltExt(sprite, x, y, false, false);
        }

      /**
        * Copies the bits of the given sprite to the chosen location on the canvas
        */
        public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) {
         // Don't start drawing outside the screen - too complicated for late night programming
            if (x < 0 || y < 0) {
                return;
            }

         // Move the screen cursor to the initial position
            int screenPos = resX * y + x;

         // Compute the length of the sprite
            int spriteLength = sprite.height * sprite.width;

         // Move the sprite's horizontal cursor to the first column
            int spritePosX = 0;

         // Loop through the sprite's pixels and copy them to the screen buffer
            for (int spritePos = 0; spritePos < spriteLength; spritePos++) {
                try {
                 // Copy the value of the current pixel, after transforming it according to the given rules
                    Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground);

                 // Move the screen cursor to the next pixel on the screens
                    screenPos++;
                } catch (Exception exc) {
                    // If it's outside the screen, it will overflow and it will throw an exception which needs to be caught
                }

             // Don't draw content outside the screen
                if (screenPos >= length - 1) {
                    return;
                }

             // Move the sprite's horizontal cursor to the next column
                spritePosX++;

             // If the sprite's horizontal cursor has reached the last column
                if (spritePosX == sprite.width) {
                    spritePosX = 0;                   // Reset the sprite's horizontal cursor
                    screenPos += resX - sprite.width; // Move the screen cursor to the next row
                }
            }
        }

      /**
        * Draws text on the canvas using the default font.
        * Multi-font support may be added at a later time.
        */
        public void DrawText(int x, int y, String text) {
            DrawColorText(x, y, text, false, false);
        }

      /**
        * Draws text on the canvas using the default font.
        * Multi-font support may be added at a later time.
        */
        public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) {
         // Don't crash if there's no font selected or if there's no text provided
            if (DefaultFont == null || text == null) {
                return;
            }

         // Get the chars array from the given text
            char[] textChars = text.ToCharArray();

         // Initialize cursors
            int screenPosX = x;
            int prevSpacing = 7; // This will ensure that, if the font is missing a sprite, there can still be an empty space in its place

         // For each character in the given text
            foreach (char chr in textChars) {
             // Identify the sprite related to to the char
                MySprite CharSprite = DefaultFont[chr];

             // If the bitmap font has a sprite defined for the char,
             // then put it on the screen and set the value of prevSpacing to the width of that last sprite
             // NOTE: some bitmap fonts might have sprites with different widths (they may not all be mono-spaced)
                if (CharSprite != null) {
                    BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground);
                    prevSpacing = CharSprite.width;
                }

             // Regardless of whether or not there's a sprite defined for the character,
             // the screen cursor needs to advance to the next character position
                screenPosX += prevSpacing;

             // However, if the screen cursor has advanced so far that it's outside the drawing area,
             // then there's no sense in processing any more text
                if (screenPosX >= resX) {
                    return;
                }
            }
        }

      /**
        * Draws a rectangle on the canvas. The rectangle may be filed or empty.
        */
        public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) {
         // Make sure the algorithm processes the coordinates from top left to bottom right
            int actualX1 = x1 > x2 ? x2 : x1;
            int actualY1 = y1 > y2 ? y2 : y1;
            int actualX2 = x1 > x2 ? x1 : x2;
            int actualY2 = y1 > y2 ? y1 : y2;

         // Basic overflow handling
            if (actualX1 < 0) actualX1 = 0;
            if (actualY1 < 0) actualY1 = 0;
            if (actualX2 >= resX - 1) actualX2 = resX - 1;
            if (actualY2 >= resY - 1) actualY2 = resY - 1;

         // The rectWidth is useful for understanding where the right margin is in relation to the left margin
         // This, in turn, makes it easier to navigate the Buffer
            int rectWidth = actualX2 - actualX1;

         // Initialize the vertical cursor
            int screenPosY = actualY1;

         // Run the vertical cursor through each line of the rectangle
            while (screenPosY <= actualY2) {
             // Set the Buffer cursor to the left margin of the current line
                int screenPos = screenPosY * resX + actualX1;

                if (screenPos >= length) { 
                    return;
                }

             // The value to set is either ON (normal value) or OFF (if the invertColors flag is set)
                bool targetColor = !invertColors;

             // The target color must be set to the left and right margin of the current line of the rectangle
                Buffer[screenPos] = targetColor;
                Buffer[screenPos + rectWidth - 1] = targetColor;

             // In case the fillRect flag was set or if this is the first or the last line of the rectangle,
             // the target color must be set to all the pixels in between the left and the right margin
                if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) {
                    for (int innerPos = screenPos; innerPos < screenPos + rectWidth; innerPos++) {
                        Buffer[innerPos] = targetColor;
                    }
                }

             // Move the cursors to the next line
                screenPos += resX;
                screenPosY++;
            }


        }
    }

}
