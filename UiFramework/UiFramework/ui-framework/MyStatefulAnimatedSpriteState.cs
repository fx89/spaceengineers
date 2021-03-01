using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
   /**
     * Stores and manages the sequence of MySprite objects representing the frames
     * of the animation which will run when the sprite state becomes active
     */
    public class MyStatefulAnimatedSpriteState {
	    // Using arrays instead of lists to spare the processor
	    // This means the exact number of frames will have to be given at initialization
	    // This might be a limitation for building sprites dynamically
	    private MySprite[] Frames;
	    private int nFrames;
	    private int currFrame = 0;
	    private bool transparentBackground = true;

	    public MyStatefulAnimatedSpriteState(MySprite[] Frames) {
		    if (Frames == null || Frames.Count() == 0) {
			    throw new ArgumentException("The frames array must have at least one frame");
		    }
		    this.Frames = Frames;
		    this.nFrames = Frames.Count();
	    }

	    public bool IsBackgroundTransparent() {
		    return transparentBackground;
	    }

	    /**
		    * Sometimes, though extremely rarely, an opaque background might be needed.
		    */
	    public MyStatefulAnimatedSpriteState WithOpaqueBackground() {
		    transparentBackground = false;
		    return this;
	    }

	    /**
		    * Returns the next frame in the animation. Loops forward without
		    * skipping or duplicating frames. Additional functionality might
		    * be added at a later time.
		    */
	    public MySprite GetFrame() {
		    MySprite ret = Frames[currFrame];

		    currFrame++;
		    if (currFrame >= nFrames) {
			    currFrame = 0;
		    }

		    return ret;
	    }
    }
}
