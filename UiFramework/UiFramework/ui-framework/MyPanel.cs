using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
  /**
     * The purpose of the panel is to provide a border around the contained objects.
     * To do this, the panel needs a width and a height to define the rectangle.
     */
    public class MyPanel : MyOnScreenObject {
        private int width;
        private int height;
        private bool isFilled = false;

        public MyPanel(int x, int y, int width, int height)
        : base(null, x, y, true) {
            this.width = width;
            this.height = height;
            this.isFilled = false;
        }

        public MyPanel WithOptionalParameters(bool isVisible, bool isFilled, bool invertColors) { 
            this.isVisible = isVisible;
            this.isFilled = isFilled;
            this.invertColors = invertColors;
            return this;
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            int absoluteX = GetAbsoluteX();
            int absoluteY = GetAbsoluteY();

            TargetCanvas.DrawRect(
                absoluteX, absoluteY, absoluteX + width, absoluteY + height,
                invertColors,
                isFilled
            );
        }

        public override int GetWidth() {
            return width;
        }

        public override int GetHeight() {
            return height;
        }

        public void SetWidth(int width) {
            this.width = width;
        }

        public void SetHeight(int height) {
            this.height = height;
        }

        protected override void Init() {
            // Nothing to do here
        }
    }
}
