using IngameScript.drawing_framework;
using IngameScript.drawing_framework.sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
  /**
    * Plain scrollbar with a slider that goes up and down according to the set position
    */
    public class MyScrollbar : MyOnScreenObject {
        private int width = 7;
        private int height = 10;

        private float posPct = 0.5f;

        private bool snapToParent = true;

        public MyScrollbar(MyOnScreenObject ParentObject)
        : base(ParentObject, 0, 0, true) {
        }

        public MyScrollbar DetachedFromParent(int height) {
            this.snapToParent = false;
            this.height = height;
            return this;
        }

        public MyScrollbar WithCustomWidth(int width) {
            this.width = width;
            return this;
        }

        public MyScrollbar AtCoordinates(int x, int y) {
            this.x = x;
            this.y = y;
            return this;
        }

        public void SetPosPct(float posPct) {
            this.posPct = posPct;
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            // Compute the corners of the scrollbar
            int x1 = snapToParent ? ParentObject.GetAbsoluteX() + ResolveClientX() : GetAbsoluteX();
            int y1 = snapToParent ? ParentObject.GetAbsoluteY() : GetAbsoluteY();
            int x2 = x1 + width;
            int actualHeight = GetHeight();
            int y2 = y1 + actualHeight;

            // Draw the scrollbar
            TargetCanvas.DrawRect(x1, y1, x2, y2, invertColors, false);

            // Compute the coordinates of the slider
            int sliderX = x1 + 1;
            int sliderY = (int)(y1 + 1 + (posPct * ((actualHeight - 5 - 2))));

            // Draw the slider
            TargetCanvas.BitBltExt(
                StockSprites.SCROLLBAR_SLIDER,
                sliderX, sliderY,
                invertColors ? (!this.invertColors) : this.invertColors,
                false
            );
        }

        private int ResolveHight() {
            if (ParentObject is MyPanel) {
                return ((MyPanel)ParentObject).GetHeight();
            }

            return height;
        }

        private int ResolveClientX() {
            if (ParentObject is MyPanel) {
                return ParentObject.GetWidth() - this.width;
            }

            return 0;
        }

        public override int GetWidth() {
            return width;
        }

        public override int GetHeight() {
            return snapToParent ? ResolveHight() : height;
        }

        protected override void Init() {
            // Nothing to do here
        }
    }
}
