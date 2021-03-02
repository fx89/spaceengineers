using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
  /**
    * Simple text label to handle the drawing of text at a certain position,
    * with a certain color (ON / OFF) and with an optional transparent background.
    */
    public class MyTextLabel : MyOnScreenObject {
        private  String text;
        private bool transparentBackground;
        private MySprite[] Font;
        private int padding = 1;

        public MyTextLabel(string text, int x, int y)
        : base(null, x, y, true) {
            this.text = text;
            this.transparentBackground = true;
        }

        public MyTextLabel WithOptionalParameters(bool isVisible, bool invertColors, bool transparentBackground) { 
            this.isVisible = isVisible;
            this.invertColors = invertColors;
            this.transparentBackground = transparentBackground;
            return this;
        }

        public MyTextLabel WithCustomFont(MySprite[] CustomFont) { 
            this.Font = CustomFont;
            return this;
        }

        public MyTextLabel WithPadding(int padding) { 
            this.padding = padding;
            return this;
        }

        public void SetText(string text) {
            this.text = text;
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            if (Font == null) {
                Font = TargetCanvas.GetDefaultFont();
            }

            TargetCanvas.DrawColorText(
                GetAbsoluteX() + padding, GetAbsoluteY() + padding,
                text,
                invertColors,
                transparentBackground
            );
        }

        private MySprite[] ResolveFont() { 
            if (Font == null) { 
                MyOnScreenObject TopLevelParent = GetTopLevelParent();
                if (TopLevelParent is MyPage) {
                    return ((MyPage) TopLevelParent).GetApplication().GetCanvas().GetDefaultFont();
                } else { 
                    return null;
                }
            } else { 
                return Font;
            }
        }

        public override int GetWidth() {
            MySprite[] Font = ResolveFont();

            if (Font == null || Font['a'] == null) {
                return 0;
            }

         // TODO: adapt for non-monospace fonts (though these might never be used)
            return Font['a'].width * text.Length + (2 * padding);
        }

        public override int GetHeight() {
            MySprite[] Font = ResolveFont();

            if (Font == null || Font['a'] == null) {
                return 0;
            }

            return Font['a'].height + (2 * padding);
        }

        protected override void Init() {
            // Nothing to do here
        }
    }
}
