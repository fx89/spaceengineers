using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.drawing_framework {
    public class MySprite {
        public int width;
        public int height;
        public bool[] data;

        public MySprite(int width, int height, bool[] data) {
            this.width = width;
            this.height = height;
            this.data = data;
        }
    }
}
