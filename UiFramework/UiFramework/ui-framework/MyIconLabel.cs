using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    /**
      * An animated sprite joined by a text label which can be placed
      * on any of the 4 sides of the sprite, as the caller desires
      */
    public class MyIconLabel : MyOnScreenObject {
        private MyStatefulAnimatedSprite AnimatedSprite;
        private MyTextLabel TextLabel;
        private int width;
        private int height;
        private int floatingIconPosition = Constants.FLOATING_POSITION_TOP;
        private int spacing = 3;

        public MyIconLabel(int x, int y, string text, MySprite[] Frames) :base(null, x, y, true){
         // Check that the text is not null
            if (text == null) {
                throw new ArgumentException("The text of the MyIconLabel must not be null");
            }

         // Check that there is at least one frame
            if (Frames == null || Frames.Length == 0) {
                throw new ArgumentException("There has to be at least one frame if the picture is to be displayed by the MyIconLabel");
            }

         // Check that all frames have the exact same dimensions
            int frameWidth = Frames[0].width;
            int frameHeight = Frames[0].height;
            foreach (MySprite Frame in Frames) {
                if (Frame.width != frameWidth || Frame.height != frameHeight) {
                    throw new ArgumentException("All the frames of the MyIconLabel must have the same width and height");
                }
            }

         // Create the icon
            AnimatedSprite = new MyStatefulAnimatedSprite(0,0).WithState("Default", new MyStatefulAnimatedSpriteState(Frames));
            AddChild(AnimatedSprite);

         // Create the text label
            TextLabel = new MyTextLabel(text, 0,0);
            AddChild(TextLabel);
        }

        /**
         * Sets where the icon is positioned in relation to the text label:
         *      - FLOATING_POSITION_TOP (default)
         *      - FLOATING_POSITION_LEFT
         *      - FLOATING_POSITION_RIGHT
         *      - FLOATING_POSITION_BOTTOM
         */
        public MyIconLabel WithFloatingIconPosition(int floatingIconPosition) {
            this.floatingIconPosition = floatingIconPosition;
            return this;
        }

        /**
         * The space between the text and the icon (either horizontally or vertically)
         */
        public MyIconLabel WithSpaceing(int spacing) {
            this.spacing = spacing;
            return this;
        }

        public override int GetHeight() {
            return height;
        }

        public override int GetWidth() {
            return width;
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        // Here is where the canvas and the font are available,
        // so this is where the location of both the text label
        // and the animated sprite can be properly positioned
        protected override void Init() {
         // Get sprite dimensions
            int spriteWidth = AnimatedSprite.GetWidth();
            int spriteHeight = AnimatedSprite.GetHeight();

         // Get text dimensions
            int textWidth = TextLabel.GetWidth();
            int textHeight = TextLabel.GetHeight();

         // Compute the dimensions of the MyIconLabel
            if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) {
                width = spriteWidth + spacing + textWidth;
                height = spriteHeight > textHeight ? spriteHeight : textHeight;
            } else {
                width = spriteWidth > textWidth ? spriteWidth : textWidth;
                height = spriteHeight + spacing + textHeight;
            }

         // Compute the horizontal positions of the icon and label
            if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT) {
                AnimatedSprite.x = 0;
                TextLabel.x = spriteWidth + spacing;
            } else if (floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) {
                AnimatedSprite.x = width - spriteWidth;
                TextLabel.x = AnimatedSprite.x - spacing - textWidth;
            } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP || floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) {
                AnimatedSprite.x = (width - spriteWidth) / 2;
                TextLabel.x = (width - textWidth) / 2;
            }

         // Compute the vertical positions of the icon and label
            if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) {
                TextLabel.y = (height - textHeight) / 2;
                AnimatedSprite.y = (height - spriteHeight) / 2;
            } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP) {
                AnimatedSprite.y = 0;
                TextLabel.y = spriteHeight + spacing;
            } else if (floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) {
                TextLabel.y = 0;
                AnimatedSprite.y = textHeight + spacing;
            }
        }
    }
}
