using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    public class MyBlinkingIcon : MyOnScreenObject {

	    public MyBlinkingIcon(MyOnScreenObject ParentObject, int x, int y, bool isVisible)
	    :base(ParentObject, x, y, isVisible) { }

	    public override int GetWidth() {
		    return 0;
	    }

	    public override int GetHeight() {
		    return 0;
	    }

	    protected override void Init() {
		    // Nothing to do here
	    }

        protected override void Compute(MyCanvas TargetCanvas) {
        
        }

        protected override void Draw(MyCanvas TargetCanvas) {
        
        }
    }
}
