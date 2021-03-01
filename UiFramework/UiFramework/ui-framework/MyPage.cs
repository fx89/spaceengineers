using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
  /**
	* The page is just a MyOnScreenObject with empty implementations of all
	* abstract methods. Its only purpose is to help split more complex apps
	* into screens, or pages, each page having its own components. The page
	* is added to the MyOnScreenApplication to work as part of it.
	*/
    public class MyPage : MyOnScreenObject {
	    private MyOnScreenApplication OnScreenApplication;

	    public MyPage() : base(null, 0, 0, true) { }

	    public void SetApplication(MyOnScreenApplication OnScreenApplication) { 
		    this.OnScreenApplication = OnScreenApplication;
	    }

	    public MyPage WithInvertedColors() {
		    this.invertColors = true;
		    return this;
	    }

	    public MyOnScreenApplication GetApplication() { 
		    return OnScreenApplication;
	    }

	    public override int GetHeight() {
		    return 0;
	    }

	    public override int GetWidth() {
		    return 0;
	    }

	    protected override void Compute(MyCanvas TargetCanvas) {
	    // Nothing to do here
	    }

	    protected override void Draw(MyCanvas TargetCanvas) {
		    // Nothing to do here
	    }

	    protected override void Init() {
		    // Nothing to do here
	    }
    }
}
