using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {

 /**
   *  Given a one frame sprite, this class makes it blink by switching between the original
   *  sprite and an inverted version of it (white on black background -> black on white background).
   *  The following states are provided:
   *        - Off      = original sprite
   *        - On       = inverted sprite
   *        - Blinknig = switching between Off and On
   *  The switching interval is configurable using the WithBlinkingInterval() method. Note tht this
   *  interval is set in frames. The higher the frame rate, the higher the blinking rate.
   *  The blinks count is also configurable when calling the Blink() method. Giving it a negative
   *  value will make it blink indefinitely.
   */
    public class MyBlinkingIcon : MyOnScreenObject {
        private MyStatefulAnimatedSprite Sprite;
        private int blinkingInterval = 3;
        private int blinkTimeout = 0;
        private bool isOn = false;
        private bool isBlinking = false;
        private int nBlinkTimes = 0;

	    public MyBlinkingIcon(int x, int y, MySprite Graphics) : base(null, x, y, true) {
            Sprite = new MyStatefulAnimatedSprite(0, 0)
                .WithState("Off", new MyStatefulAnimatedSpriteState(new MySprite[]{ Graphics }))
                .WithState("On" , new MyStatefulAnimatedSpriteState(new MySprite[]{
                    new MySprite(Graphics.width, Graphics.height, DrawingFrameworkUtils.NegateBoolArray(Graphics.data))
                 }));
            AddChild(Sprite);
        }

        public MyBlinkingIcon WithBlinkingInterval(int blinkingInterval) {
            this.blinkingInterval = blinkingInterval;
            return this;
        }

	    public override int GetWidth() {
		    return Sprite.GetWidth();
	    }

	    public override int GetHeight() {
		    return Sprite.GetHeight();
	    }

	    protected override void Init() {
		    // Nothing to do here
	    }

        private void LocalSwitchOn() {
            Sprite.SetState("On");
            isOn = true;
        }

        private void LocalSwitchOff() {
            Sprite.SetState("Off");
            isOn = false;
        }

        private void LocalSwitch() {
            if (isOn) {
                LocalSwitchOff();
            } else {
                LocalSwitchOn();
            }
        }

        public void SwitchOn() {
            LocalSwitchOn();
            stopBlinking();
        }

        public void SwitchOff() {
            LocalSwitchOff();
            stopBlinking();
        }

        public void Switch() {
            LocalSwitch();
            stopBlinking();
        }

        public void Blink(int nTimes) {
            SwitchOn();
            nBlinkTimes = nTimes;
            isBlinking = true;
            blinkTimeout = 0;
        }

        public void stopBlinking() {
            isBlinking = false;
            nBlinkTimes = 0;
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            if (isBlinking) {
                blinkTimeout++;
                if (blinkTimeout >= blinkingInterval) {
                    blinkTimeout = 0;
                    LocalSwitch();
                    nBlinkTimes--;
                    if (nBlinkTimes == 0) { // initial negative values are expected to make it blink until the caller tells it to stop
                        SwitchOff();
                    }
                }
            }
        }
    }
}
