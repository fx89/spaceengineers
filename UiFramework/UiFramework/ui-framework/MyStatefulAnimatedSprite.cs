using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    /**
	* This is a sprite with animated states. Just set its position and current state
	* and it will start running the animation at the given location.
	*/
    public class MyStatefulAnimatedSprite : MyOnScreenObject {
	    /**
		    * The states dictionary
		    */
	    private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>();

	    private MyStatefulAnimatedSpriteState CurrentState;
	    private bool isBackgroundTransparet = true;

	    private MySprite CurrentFrame;

	    public MyStatefulAnimatedSprite(int x, int y)
	    : base(null, x, y, true) {

	    }

	    /**
		    * This works somewhat like a method of a builder class, to help chain up
		    * multiple state-adding operations.
		    */
	    public MyStatefulAnimatedSprite WithState(string stateName, MyStatefulAnimatedSpriteState State) {
		    // Make sure the parameters are ok, to avoid runtime errors
		    if (stateName == null) {
			    throw new ArgumentException("Each state must have a name");
		    }
		    if (State == null) {
			    throw new ArgumentException("The state may not be null");
		    }

		    // Add the state. Override any pre-existing state with the same name.
		    States.Add(stateName, State);

		    // If this is the first state added, then make it the current state.
		    if (CurrentState == null) {
			    SetStateObject(State);
		    }

		    // Return a self reference to allow queuing the next WithState operation
		    return this;
	    }

	    /**
		    * Finds the state with the given name and makes it current.
		    * Doesn't do anything if the name is not correct
		    */
	    public void SetState(String stateName) {
		    if (stateName == null) {
			    return;
		    }

		    MyStatefulAnimatedSpriteState State;
		    States.TryGetValue(stateName, out State);

		    if (State != null) {
			    SetStateObject(State);
		    }
	    }

	    private void SetStateObject(MyStatefulAnimatedSpriteState State) {
		    CurrentState = State;
		    isBackgroundTransparet = State.IsBackgroundTransparent();
	    }


	    protected override void Compute(MyCanvas TargetCanvas) {
		    // Frames must cycle on even if the object is not visible on screen
		    CurrentFrame = CurrentState.GetFrame();
	    }

	    protected override void Draw(MyCanvas TargetCanvas) {
		    TargetCanvas.BitBltExt(
			    CurrentFrame,
			    GetAbsoluteX(), GetAbsoluteY(),
			    invertColors,
			    CurrentState.IsBackgroundTransparent()
		    );
	    }

	    public override int GetWidth() {
		    if (CurrentFrame == null) {
			    return 0;
		    } else {
			    return CurrentFrame.width;
		    }
	    }

	    public override int GetHeight() {
		    if (CurrentFrame == null) {
			    return 0;
		    } else {
			    return CurrentFrame.height;
		    }
	    }

	    protected override void Init() {
		    // Nothing to do here
	    }
    }
}
