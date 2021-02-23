
// CUSTOM CODE /////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

/**
 * This is called from the constructor of the program.
 * It should be used for initializing the application.
 *      > adding pages
 *      > adding components to the pages
 *      > linking logic and animations
 */
private void Init() {
 // Initialize the application, giving it a canvass and one or more screens to display it on
    OnScreenApplication = InitSingleScreenApplication("VIDEOWALL_PANEL_1x1", 139, 93);

 // Create the POST page. MyPOSTPage is an extension of the MyPage class, which
 // overrides the Cycle() method to add the logic of switching to the main page
 // after a few frames
    MyPage POSTPage = new MyPOSTPage(OnScreenApplication);
    OnScreenApplication.AddPage(POSTPage);

 // Add a filled panel to the POST page
    MyPanel Panel = new MyPanel(null, 0, 0, 139, 93, true, true, false);
    POSTPage.AddChild(Panel);

 // Add another filled panel to the POST page, to serve as background for the INITIALIZING text
    MyPanel TextBackgroundPanel = new MyPanel(null, 28, 37, 82, 12, true, true, true);
    POSTPage.AddChild(TextBackgroundPanel);

 // Add the INIIALIZING text label to the POST page
    POSTPage.AddChild(new MyTextLabel("INITIALIZING", null, 33, 40, true, false, false));

 // Add the moving square (a panel with some simple animation logic)
    POSTPage.AddChild(
        new MyPanel(null, 28, 51, 7, 4, true, true, true)
            .WithClientCycleMethod((MyOnScreenObject obj) => {
                obj.x++;
                if (obj.x > 103) {
                    obj.x = 28;
                }
            return 0; })
    );

 // Create the main page
    MyPage MainPage = (MyPage) new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Add a smiley face sprite to the main page
 // The sprite has two states: smiling and not smiling
 // Each of the two states has several frames
 // The frames are defined in the CUSTOM SPRITES section
 // The custom class MySmileySprite adds basic logic for
 // movement and switching beween states
    MainPage.AddChild(
        new MySmileySprite(null, 75, 25, true)
            .WithState("smiling", new MyStatefulAnimatedSpriteState(
                new MySprite[]{
                    SPRITE_SMILE_FRAME_0,
                    SPRITE_SMILE_FRAME_1,
                    SPRITE_SMILE_FRAME_2,
                    SPRITE_SMILE_FRAME_3,
                    SPRITE_SMILE_FRAME_4,
                    SPRITE_SMILE_FRAME_3,
                    SPRITE_SMILE_FRAME_2,
                    SPRITE_SMILE_FRAME_1
                }
             ))
            .WithState("not smiling", new MyStatefulAnimatedSpriteState(
                new MySprite[]{
                    SPRITE_NOT_SMILE_FRAME_0,
                    SPRITE_NOT_SMILE_FRAME_1,
                    SPRITE_NOT_SMILE_FRAME_2,
                    SPRITE_NOT_SMILE_FRAME_1
                }
             ))
    );
}

private class MyPOSTPage : MyPage {
    private MyOnScreenApplication OnScreenApplication;
    private int currPage1Frame = 0;

    public MyPOSTPage(MyOnScreenApplication OnScreenApplication) {
        this.OnScreenApplication = OnScreenApplication;
    }

    public override void Cycle(MyCanvas TargetCanvas) {
        base.Cycle(TargetCanvas);

        currPage1Frame++;

        if (currPage1Frame > 200) {
            OnScreenApplication.SwitchToPage(1); // The POST page is page 0. The next page is page 1.
        }
    }
}

/**
 * Logic of the smiley sprite
 */
private class MySmileySprite : MyStatefulAnimatedSprite {
    private int frameNbr = 0;
    private bool isSmiling = true;

    int vectorX = 2;
    int vectorY = 1;

    public MySmileySprite(MyOnScreenObject parent, int x, int y, bool isVisible)
    : base(parent, x, y, isVisible) {
        
    }

 /**
  * The smiley sprite has two states. Each state will be
  * active for 100 frames.
  */
    private void StateSwitcher() {
        frameNbr++;

        if (frameNbr > 100) {
            frameNbr = 0;
            isSmiling = !isSmiling;

            if (isSmiling) {
                SetState("smiling");
            } else {
                SetState("not smiling");
            }
        }
    }

 /**
  * The smiley sprite will bump into the screen
  * margins and invert each of the two vector coordinates
  * (x and y) once the sprite reaches the margin
  */
    private void Movement(MyCanvas TargetCanvas) {
        x += vectorX;
        y += vectorY;

        if (x > TargetCanvas.GetResX() - 16) {
            x = TargetCanvas.GetResX() - 16;
            vectorX = -vectorX;
        }

        if (x < 0) {
            x = 0;
            vectorX = -vectorX;
        }

        if (y > TargetCanvas.GetResY() - 16) {
            y = TargetCanvas.GetResY() - 16;
            vectorY = -vectorY;
        }

        if (y < 0) {
            y = 0;
            vectorY = -vectorY;
        }
    }

    public override void Cycle(MyCanvas TargetCanvas) {
        base.Cycle(TargetCanvas);
        StateSwitcher();
        Movement(TargetCanvas);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// HELP WITH THE SPRITES ///////////////////////////////////////////////////////////////////////////////////////////

private const bool O = true;
private const bool _ = false;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CUSTOM SPRITES //////////////////////////////////////////////////////////////////////////////////////////////////

private static MySprite SPRITE_SMILE_FRAME_0 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,O,_,_,_,O,_,_,O,_,_,
    _,O,_,_,O,O,O,_,O,O,O,_,_,O,_,
    _,O,_,_,_,O,_,_,_,O,_,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,O,_,_,_,_,_,_,_,O,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,O,O,O,O,O,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_SMILE_FRAME_1 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,O,_,O,_,_,O,_,_,O,_,_,
    _,O,_,_,_,O,_,_,O,O,O,_,_,O,_,
    _,O,_,_,O,_,O,_,_,O,_,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,O,_,_,_,_,_,_,_,O,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,O,O,O,O,O,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_SMILE_FRAME_2 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,O,_,O,_,O,_,O,_,O,_,_,
    _,O,_,_,_,O,_,_,_,O,_,_,_,O,_,
    _,O,_,_,O,_,O,_,O,_,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,O,_,_,_,_,_,_,_,O,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,O,O,O,O,O,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_SMILE_FRAME_3 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,O,_,_,O,_,O,_,O,_,_,
    _,O,_,_,O,O,O,_,_,O,_,_,_,O,_,
    _,O,_,_,_,O,_,_,O,_,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,O,_,_,_,_,_,_,_,O,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,O,O,O,O,O,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_SMILE_FRAME_4 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,O,_,_,O,_,O,_,O,_,_,
    _,O,_,_,O,O,O,_,_,O,_,_,_,O,_,
    _,O,_,_,_,O,_,_,O,_,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,O,_,_,_,_,_,_,_,O,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,O,O,O,O,O,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_NOT_SMILE_FRAME_0 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,_,_,_,_,_,_,_,O,_,_,
    _,O,_,_,O,O,O,_,O,O,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,O,O,O,O,O,_,_,_,O,_,
    _,O,_,_,O,_,_,_,_,_,O,_,_,O,_,
    _,_,O,_,_,_,_,_,_,_,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_NOT_SMILE_FRAME_1 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,_,_,_,_,_,_,_,O,_,_,
    _,O,_,_,O,O,O,_,O,O,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,O,_,_,_,_,_,_,_,O,_,
    _,O,_,_,O,_,O,_,_,O,O,_,_,O,_,
    _,_,O,_,_,_,_,O,O,_,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

private static MySprite SPRITE_NOT_SMILE_FRAME_2 = new MySprite(15, 15, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,O,_,_,_,_,_,_,_,_,_,O,_,_,
    _,O,_,_,O,O,O,_,O,O,O,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,_,_,_,_,_,_,_,_,O,_,
    _,O,_,_,_,_,O,_,_,_,_,_,_,O,_,
    _,O,_,_,O,O,_,O,_,O,O,_,_,O,_,
    _,_,O,_,_,_,_,_,O,_,_,_,O,_,_,
    _,_,_,O,_,_,_,_,_,_,_,O,_,_,_,
    _,_,_,_,O,_,_,_,_,_,O,_,_,_,_,
    _,_,_,_,_,O,O,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,_,_
});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// TEXT SPRITES ////////////////////////////////////////////////////////////////////////////////////////////////////

private static MySprite SPRITE_A = new MySprite(6, 7, new bool[] {
    _,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_B = new MySprite(6, 7, new bool[] {
    O,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,_,_
});

private static MySprite SPRITE_C = new MySprite(6, 7, new bool[] {
    _,O,O,O,O,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    _,O,O,O,O,_
});

private static MySprite SPRITE_D = new MySprite(6, 7, new bool[] {
    O,O,O,_,_,_,
    O,_,_,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,O,_,_,
    O,O,O,_,_,_
});

private static MySprite SPRITE_E = new MySprite(6, 7, new bool[] {
    O,O,O,O,O,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,O,O,O,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,O,O,O,O,_
});

private static MySprite SPRITE_F = new MySprite(6, 7, new bool[] {
    O,O,O,O,O,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,O,O,O,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_
});

private static MySprite SPRITE_G = new MySprite(6, 7, new bool[] {
    _,O,O,O,O,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,O,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,O,O,O,_
});

private static MySprite SPRITE_H = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_I = new MySprite(6, 7, new bool[] {
    _,O,O,O,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,O,O,O,_,_
});

private static MySprite SPRITE_J = new MySprite(6, 7, new bool[] {
    _,_,_,O,_,_,
    _,_,_,O,_,_,
    _,_,_,O,_,_,
    _,_,_,O,_,_,
    _,_,_,O,_,_,
    _,_,_,O,_,_,
    _,O,O,_,_,_
});

private static MySprite SPRITE_K = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,O,_,_,
    O,_,O,_,_,_,
    O,O,_,_,_,_,
    O,_,O,_,_,_,
    O,_,_,O,_,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_L = new MySprite(6, 7, new bool[] {
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    _,O,O,O,O,_
});

private static MySprite SPRITE_M = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,O,_,O,O,_,
    O,_,O,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_N = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,O,_,_,O,_,
    O,_,O,_,O,_,
    O,_,_,O,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_O = new MySprite(6, 7, new bool[] {
    _,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,O,O,_,_
});

private static MySprite SPRITE_P = new MySprite(6, 7, new bool[] {
    O,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_
});

private static MySprite SPRITE_Q = new MySprite(6, 7, new bool[] {
    _,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,O,_,O,_,
    O,_,_,O,_,_,
    _,O,O,_,O,_
});

private static MySprite SPRITE_R = new MySprite(6, 7, new bool[] {
    O,O,O,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,O,O,O,_,_,
    O,_,O,_,_,_,
    O,_,_,O,_,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_S = new MySprite(6, 7, new bool[] {
    _,O,O,O,O,_,
    O,_,_,_,_,_,
    O,_,_,_,_,_,
    _,O,O,O,_,_,
    _,_,_,_,O,_,
    _,_,_,_,O,_,
    O,O,O,O,_,_
});

private static MySprite SPRITE_T = new MySprite(6, 7, new bool[] {
    O,O,O,O,O,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_
});

private static MySprite SPRITE_U = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,O,O,_,_
});

private static MySprite SPRITE_V = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,_,O,_,_,
    _,_,O,_,_,_
});

private static MySprite SPRITE_W = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    O,_,O,_,O,_,
    _,O,_,O,_,_
});

private static MySprite SPRITE_X = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,_,O,_,_,
    _,_,O,_,_,_,
    _,O,_,O,_,_,
    O,_,_,_,O,_,
    O,_,_,_,O,_
});

private static MySprite SPRITE_Y = new MySprite(6, 7, new bool[] {
    O,_,_,_,O,_,
    O,_,_,_,O,_,
    _,O,_,O,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_,
    _,_,O,_,_,_
});

private static MySprite SPRITE_Z = new MySprite(6, 7, new bool[] {
    O,O,O,O,O,_,
    _,_,_,_,O,_,
    _,_,_,O,_,_,
    _,_,O,_,_,_,
    _,O,_,_,_,_,
    O,_,_,_,_,_,
    O,O,O,O,O,_
});

private static MySprite[] CreateBitmapFont() {
    MySprite[] BitmapFont = new MySprite[256];

    BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A;
    BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B;
    BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C;
    BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D;
    BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E;
    BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F;
    BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G;
    BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H;
    BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I;
    BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J;
    BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K;
    BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L;
    BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M;
    BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N;
    BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O;
    BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P;
    BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q;
    BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R;
    BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S;
    BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T;
    BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U;
    BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V;
    BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W;
    BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X;
    BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y;
    BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z;

    return BitmapFont;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// THE DRAWING FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////

/**
 * Provides drawing functionality on a bool[] buffer. The bool[] buffe
 * can be copied into sprites, which may be drawn on other MyCanvas
 * objects. The canvas also functions as part of the screen (MyScreen
 * class), which applies its buffer to LCD panels.
 */
private class MyCanvas {
    private bool[] Buffer;
    private int resX;
    private int resY;
    private int length;
    private MySprite[] DefaultFont;

    public MyCanvas(int resX, int resY) {
        this.resX = resX;
        this.resY = resY;
        length = resY * resX;
        Buffer = new bool[length];
    }

 /**
  * Return a reference to the buffer, for use by the screen
  */
    public bool[] GetBuffer() {
        return Buffer;
    }

    public int GetResX() {
        return resX;
    }

    public int GetResY() {
        return resY;
    }

 /**
  * Return a copy of the buffer, for use in the creation of new sprites
  */
    public bool[] GetBufferCopy() {
        return CopyBoolArray(Buffer, false);
    }

 /**
  * A default font must be set if text is to be drawn on this canvas
  */
    public void SetDefaultFont(MySprite[] DefaultFont) {
        this.DefaultFont = DefaultFont;
    }

    public MyCanvas WithDefaultFont(MySprite[] DefaultFont) {
        SetDefaultFont(DefaultFont);
        return this;
    }

 /*
  * Fills the buffer with the given value (either on or off, caller's preference)
  */
    public void Clear(bool value = false) {
        for (int x = 0 ; x < length ; x++) {
            Buffer[x] = value;
        }
    }

 /**
  * Handles drawing options (transparency, color inversion)
  */
    private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) {
        if (invertColors) {
            if (transparentBackground) {
                return targetPixelValue && !sourcePixelValue;
            } else {
                return !sourcePixelValue;
            }
        } else {
            if (transparentBackground) {
                return sourcePixelValue || targetPixelValue;
            } else {
                return sourcePixelValue;
            }
        }
    }

 /**
  * Copies the bits of the given sprite to the chosen location on the canvas
  */
    public void BitBlt(MySprite sprite, int x, int y) {
        BitBltExt(sprite, x, y, false, false);
    }

 /**
  * Copies the bits of the given sprite to the chosen location on the canvas
  */
    public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) {
     // Don't start drawing outside the screen - too complicated for late night programming
        if (x < 0 || y < 0)  {
            return;
        }

     // Move the screen cursor to the initial position
        int screenPos = resX * y + x;

     // Compute the length of the sprite
        int spriteLength = sprite.height * sprite.width;

     // Move the sprite's horizontal cursor to the first column
        int spritePosX = 0;

        // Loop through the sprite's pixels and copy them to the screen buffer
        for (int spritePos = 0 ; spritePos < spriteLength ; spritePos++) {
         // Copy the value of the current pixel, after transforming it according to the given rules
            try {
                Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground);
            } catch(Exception exc) {
         // If it's outside the screen, it will overflow and it will throw an exception which needs to be caught
            }

         // Move the screen cursor to the next pixel on the screens
            screenPos++;

         // Don't draw content outside the screen
            if (x + spritePosX >= resX - 1 || screenPos >= length - 1) {
                return;
            }

         // Move the sprite's horizontal cursor to the next column
            spritePosX++;

         // If the sprite's horizontal cursor has reached the last column
            if (spritePosX == sprite.width) {
                spritePosX = 0;                   // Reset the sprite's horizontal cursor
                screenPos += resX - sprite.width; // Move the screen cursor to the next row
            }
        }
    }

 /**
  * Draws text on the canvas using the default font.
  * Multi-font support may be added at a later time.
  */
    public void DrawText(int x, int y, String text) {
        DrawColorText(x, y, text, false, false);
    }

 /**
  * Draws text on the canvas using the default font.
  * Multi-font support may be added at a later time.
  */
    public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) {
     // Don't crash if there's no font selected or if there's no text provided
        if (DefaultFont == null || text == null) {
            return;
        }

     // Get the chars array from the given text
        char[] textChars = text.ToCharArray();

     // Initialize cursors
        int screenPosX = x;
        int prevSpacing = 7; // This will ensure that, if the font is missing a sprite, there can still be an empty space in its place

     // For each character in the given text
        foreach(char chr in textChars) {
         // Identify the sprite related to to the char
            MySprite CharSprite = DefaultFont[chr];

         // If the bitmap font has a sprite defined for the char,
         // then put it on the screen and set the value of prevSpacing to the width of that last sprite
         // NOTE: some bitmap fonts might have sprites with different widths (they may not all be mono-spaced)
            if (CharSprite != null) {
                BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground);
                prevSpacing = CharSprite.width;
            }

         // Regardless of whether or not there's a sprite defined for the character,
         // the screen cursor needs to advance to the next character position
            screenPosX += prevSpacing;

         // However, if the screen cursor has advanced so far that it's outside the drawing area,
         // then there's no sense in processing any more text
            if (screenPosX >= resX) {
                return;
            }
        }
    }

 /**
  * Draws a rectangle on the canvas. The rectangle may be filed or empty.
  */
    public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) {
     // Make sure the algorithm processes the coordinates from top left to bottom right
        int actualX1 = x1 > x2 ? x2 : x1;
        int actualY1 = y1 > y2 ? y2 : y1;
        int actualX2 = x1 > x2 ? x1 : x2;
        int actualY2 = y1 > y2 ? y1 : y2;

     // Basic overflow handling
        if (actualX1 <     0) actualX1 = 0;
        if (actualY1 <     0) actualY1 = 0;
        if (actualX2 >= resX) actualX2 = resX - 1;
        if (actualY2 >= resY) actualY2 = resY - 1;
 
     // The rectWidth is useful for understanding where the right margin is in relation to the left margin
     // This, in turn, makes it easier to navigate the Buffer
        int rectWidth = actualX2 - actualX1;

     // Initialize the vertical cursor
        int screenPosY = actualY1;

     // Run the vertical cursor through each line of the rectangle
        while (screenPosY <= actualY2) {
         // Set the Buffer cursor to the left margin of the current line
            int screenPos = screenPosY * resX + actualX1;

         // The value to set is either ON (normal value) or OFF (if the invertColors flag is set)
            bool targetColor = !invertColors;

         // The target color must be set to the left and right margin of the current line of the rectangle
            Buffer[screenPos] = targetColor;
            Buffer[screenPos + rectWidth-1] = targetColor;

         // In case the fillRect flag was set or if this is the first or the last line of the rectangle,
         // the target color must be set to all the pixels in between the left and the right margin
            if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) {
                for (int innerPos = screenPos ; innerPos < screenPos + rectWidth ; innerPos++) {
                    Buffer[innerPos] = targetColor;
                }
            }

         // Move the cursors to the next line
            screenPos += resX;
            screenPosY++;
        }
        
        
    }
}



/**
 * The purpose of this class is to link an LCD panel to a MyCanvas.
 * The drawing on the canvas is represented on the target LCD panel
 * as a string of characters representing the pixels of the canvas.
 */
private class MyScreen {
 // Properties needed for the functionality of the screen
    private IMyTextPanel TargetLCD;
    private MyCanvas Canvas;
    private bool mirrorX;

 // Properties which are also needed by FlushBufferToScreen()
    private char pixelValueOn, pixelValueOff;

 // Clipping options
    private int clipRectX1, clipRectY1, clipRectX2, clipRectY2;
    private bool isClipping = false;

    public MyScreen(IMyTextPanel TargetLCD, char pixelValueOn, char pixelValueOff, bool mirrorX) {
        this.TargetLCD = TargetLCD;
        this.mirrorX = mirrorX;
        this.pixelValueOn = pixelValueOn;
        this.pixelValueOff = pixelValueOff;
    }

    public MyScreen WithCanvas(MyCanvas Canvas) {
        this.Canvas = Canvas;
        return this;
    }

 /**
  * Turns on the buffer clipping option, which draws just part of the buffer
  * on the screen. This helps in splitting the buffer onto multiple screens.
  */
    public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) {
        clipRectX1 = x1 > x2 ? x2 : x1;
        clipRectY1 = y1 > y2 ? y2 : y1;
        clipRectX2 = x1 > x2 ? x1 : x2;
        clipRectY2 = y1 > y2 ? y1 : y2;
        isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY();

        return this;
    }

 /**
   * Unlike glass windows, for which there are versions with the tinted glass
   * both on the outside and the inside, LCD panels display the text only on
   * one side. This becomes an issue when dealing with transparent LCDs, which
   * display the text on both sides. On one side, the text will be mirrored.
   * If that's the side that needs to be seen, then the content must be adjusted
   * so that it displays properly. Hence, the method MirrorBufferOnXAxis().
   */
    private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) {
        int length = Buffer.Count();

        bool[] MirroredBuffer = new bool[length];
        
        int mirrorPosX = resX-1;
        int mirrorPos = mirrorPosX;
        
        for (int sourcePos = 0 ; sourcePos < length ; sourcePos++) {
            MirroredBuffer[mirrorPos] = Buffer[sourcePos];
            
            mirrorPos--;
            mirrorPosX--;
            if (mirrorPosX == -1) {
                mirrorPosX = resX-1;
                mirrorPos += resX*2;
            }
        }

        return MirroredBuffer;
    }

 /**
  * Copies a subset of the buffer and returns a reference to the copy.
  * Useful for splitting the buffer on many screens
  */
    private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) {
     // Make sure the coordinates are ordered properly
        int rectX1 = x1 > x2 ? x2 : x1;
        int rectY1 = y1 > y2 ? y2 : y1;
        int rectX2 = x1 > x2 ? x1 : x2;
        int rectY2 = y1 > y2 ? y1 : y2;

     // Make sure the coordinates are not off screen
        if (rectX1 < 0) rectX1 = 0;
        if (rectY1 < 0) rectY1 = 0;
        if (rectX2 > resX) rectX2 = resX;
        if (rectY2 > resY) rectY2 = resY;

     // Initialize the return buffer
        bool [] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1];

     // DEBUG:
     // PROGRAM.Echo("(" + rectX1.ToString() + "," + rectY1.ToString() + ") - (" + rectX2.ToString() + "," + rectY2.ToString() + ")");
     // PROGRAM.Echo(ret.Count().ToString());
     // PROGRAM.Echo("(" + resX.ToString() + "," + resY.ToString() + ")");

     // Initialize the cursors
        int srcCursor = rectY1 * resX + rectX1;
        int trgCursor = 0;

     // Copy the source section into the target buffer
        for (int srcY = rectY1 ; srcY < rectY2 ; srcY++) {
            for (int srcX = rectX1; srcX < rectX2 ; srcX++) {
                ret[trgCursor] = Buffer[srcCursor];
                ret[trgCursor] = true;
                trgCursor++;
                srcCursor++;
            }
            srcCursor += rectX1;
        }

        return ret;
    }

  /**
   * Converts the bool[] buffer of the canvas into a string of pixels which
   * is then attributed to the Text property of the target LDC panel. It is
   * a method used before in Space Engineers to simulate graphics modes on
   * text panels. The font size must be made small enough that characters
   * look like pixels. The best looking characters must be chosen.
   */
    public void FlushBufferToScreen() {
     // Get the buffer from the Canvas
        bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer();
        int length = Buffer.Count();
        int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX();
        int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY();

     // In case the screen needs to be mirrored on the X axis
        bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer;
        
     // Create a new "rendered buffer" having the length of the screen buffer + once more the height to accommodate new line characters
        StringBuilder renderedBuffer = new StringBuilder(length + resY + 1);

     // Fill the rendered buffer
        int currXPos = 0;
        for (int idx = 0 ; idx < length ; idx++) {
         // Append the pixel value to the rendered buffer
            renderedBuffer.Append(SourceBuffer[idx] ? pixelValueOn : pixelValueOff);

         // If the end of the line has been reached, append a new line and reset the counter
            currXPos++;
            if (currXPos == resX) {
                renderedBuffer.Append('\n');
                currXPos = 0;
            }
        }

     // Apply the newly rendered buffer to the target LCD
        TargetLCD.WriteText(renderedBuffer.ToString());
    }

    public MyCanvas GetCanvas() {
        return Canvas;
    }
}

private class MySprite {
    public int width;
    public int height;
    public bool[] data;

    public MySprite(int width, int height, bool[] data) {
        this.width = width;
        this.height = height;
        this.data = data;
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






// THE VISUAL FRAMEWORK ////////////////////////////////////////////////////////////////////////////////////////////

/*
 * The is a set of visual components much like the forms, panels, text boxes and other UI elements
 * which can be found in any UI application, except it's very basic and focuses on drawing sprites
 * such as animated icons on LCD panels in Space Engineers.
 *
 * =================================================================================================================
 *
 * The following classes are provided:
 *
 * MyOnScreenApplication    = Main class.Contains pages. Allows choosing the current page.
 * MyPage                   = An application may have one or more pages. Only the current page is rendered.
 * MyPanel                  = Draws a rectangle around the contained components. Does not scroll.
 * MyStatefulAnimatedSprite = Object with multiple states. Each state is a series of MySprite (an animation).
 * 
 * MyOnScreenObject      = base abstract class which can be extended to add more components.
 * 
 * =================================================================================================================
 *
 * The following tree shows the objects provided by this framework and the way they are meant to
 * act as containers to oneanother. In truth, any kind of object may contain any other kind of
 * object. The base class is MyOnScreenObject, which may be extended by other classes to provide
 * additional UI elements. 
 *
 * MyOnScreenApplication
 *     MyPage
 *         MyOnScreenObject
 *         MyStatefulAnimatedSprite
 *         MyTextLabel
 *         MyPanel
 *             MyOnScreenObject
 *             MyStatefulAnimatedSprite
 *             MyTextLabel
 *             MyPanel
 * 
 * =================================================================================================================
 */



/**
 * This class defines the split between the two stages of the drawing
 * process for on-screen objects, as well as basic properties of such
 * objects. It also provides some basic handling of child objects.
 */
private abstract class MyOnScreenObject {
    public int x;
    public int y;
    public bool isVisible = true;
    public MyOnScreenObject ParentObject;
    private List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>();
    private Func<MyOnScreenObject, int> ClientCycleMethod;
    private Func<MyCanvas, int> ClientDrawMethod;

    public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) {
        this.ParentObject = ParentObject;
        this.x = x;
        this.y = y;
        this.isVisible = isVisible;

        if (ParentObject != null) {
            ParentObject.AddChild(this);
        }
    }

 /**
  * This method should be used in case the caller needs to implement custom functionality
  * which is lite enough to fit inside a lambda expression instead of a new class.
  */
    public MyOnScreenObject WithClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) {
        SetClientCycleMethod(ClientCycleMethod);
        return this;
    }

    public void SetClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) {
        this.ClientCycleMethod = ClientCycleMethod;
    }

    public MyOnScreenObject WithClientDrawMethod(Func<MyCanvas, int> ClientDrawMethod) {
        this.ClientDrawMethod = ClientDrawMethod;
        return this;
    }

 /**
  * Adds the referenced object to the list of child objects while
  * also setting its parent object reference to this object
  */
    public void AddChild(MyOnScreenObject ChildObject) {
        if (ChildObject != null) {
            ChildObjects.Add(ChildObject);
            ChildObject.SetParent(this);
        }
    }

    public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) {
        if (ChildObject != null) {
            ChildObject.SetPosition(x, y);
            AddChild(ChildObject);
        }
    }

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

 /**
  * Removes the referenced child from the list of child objects and
  * sets its parent reference to null
  */
    public void RemoveChild(MyOnScreenObject ChildObject) {
        if (ChildObject != null) {
            ChildObjects.Remove(ChildObject);
            ChildObject.RemoveParent();
        }
    }

 /**
  * Sets the object's parent. Makes sure there are no null.
  * To unset the parent, use the RemoveParent() method.
  */
    public void SetParent(MyOnScreenObject ParentObject) {
        if (ParentObject != null) {
            this.ParentObject = ParentObject;
        }
    }

 /**
  * Removes the object's parent
  */
    public void RemoveParent() {
        this.ParentObject = null;
    }

 /**
  * Recursive call through all the parents to check if the object should be drawn
  */
    public bool IsObjectVisible() {
        return isVisible && (ParentObject == null || ParentObject.IsObjectVisible());
    }

  /**
   * This should be called from the drawing loop. It establishes the order
   * of the operations.
   */
    public virtual void Cycle(MyCanvas TargetCanvas) {
     // Compute position, frame index and other state-related properties
     // which might have to be computed even if the object is not visible
     // on screen
        Compute(TargetCanvas);

     // Run the custom functionality (if any)
        if (ClientCycleMethod != null) {
            ClientCycleMethod(this);
        }

     // Cycle child objects (if any)
        foreach(MyOnScreenObject ChildObject in ChildObjects) {
            ChildObject.Cycle(TargetCanvas);
        }

     // If the object is visible on screen, then draw it
        if (IsObjectVisible()) {
         // Run the client draw method, if it's set
            if (ClientDrawMethod != null) {
                ClientDrawMethod(TargetCanvas);
            }

         // Call the object's draw method
            Draw(TargetCanvas);
        }
    }

 /**
  * Run through all parents recursively to get the absolute X position of
  * the object on screen
  */
    public int GetAbsoluteX() {
        return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX());
    }

 /**
  * Run through all parents recursively to get the absolute Y position of
  * the object on screen
  */
    public int GetAbsoluteY() {
        return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY());
    }

  /**
   * This method handles some drawing-related logic, such as computing
   * the position of certain sprites on the TargetScreen
   */
    protected abstract void Compute(MyCanvas TargetCanvas);

  /**
   * This method handles the drawing of the object onto the TargetScreen
   */
    protected abstract void Draw(MyCanvas TargetCanvas);
}



/**
 * This is a sprite with animated states. Just set its position and current state
 * and it will start running the animation at the given location.
 */
private class MyStatefulAnimatedSprite : MyOnScreenObject {
    /**
     * The states dictionary
     */
    private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>();

    private MyStatefulAnimatedSpriteState CurrentState;
    private bool isBackgroundTransparet = true;

    private MySprite CurrentFrame;

    public MyStatefulAnimatedSprite(MyOnScreenObject ParentObject, int x, int y, bool isVisible)
    : base(ParentObject, x, y, isVisible) {

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
            false, CurrentState.IsBackgroundTransparent()
        );
    }
}



/**
 * Stores and manages the sequence of MySprite objects representing the frames
 * of the animation which will run when the sprite state becomes active
 */
private class MyStatefulAnimatedSpriteState {
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



/**
 * Simple text label to handle the drawing of text at a certain position,
 * with a certain color (ON / OFF) and with an optional transparent background.
 */
private class MyTextLabel : MyOnScreenObject {
    private String text;
    private bool invertColors;
    private bool transparentBackground;
    
    public MyTextLabel(string text, MyOnScreenObject ParentObject, int x, int y, bool isVisible, bool invertColors, bool transparentBackground) 
    : base(ParentObject, x, y, isVisible) {
        this.text = text;
        this.invertColors = invertColors;
        this.transparentBackground = transparentBackground;
    }

    protected override void Compute(MyCanvas TargetCanvas) {
        // Nothing to do here
    }
    
    protected override void Draw(MyCanvas TargetCanvas) {
        TargetCanvas.DrawColorText(GetAbsoluteX(), GetAbsoluteY(), text, invertColors, transparentBackground);
    }
}



/**
 * The purpose of the panel is to provide a border around the contained objects.
 * To do this, the panel needs a width and a height to define the rectangle.
 */
private class MyPanel : MyOnScreenObject {
    private int width;
    private int height;
    private bool isFilled = false;
    private bool invertColors = false;

    public MyPanel(MyOnScreenObject ParentObject, int x, int y, int width, int height, bool isVisible, bool isFilled, bool invertColors)
    : base(ParentObject, x, y, isVisible) {
        this.width = width;
        this.height = height;
        this.isFilled = isFilled;
        this.invertColors = invertColors;
    }
    
    protected override void Compute(MyCanvas TargetCanvas) {
        // Nothing to do here
    }

    protected override void Draw(MyCanvas TargetCanvas) {
        int absoluteX = GetAbsoluteX();
        int absoluteY = GetAbsoluteY();

        TargetCanvas.DrawRect(absoluteX, absoluteY, absoluteX + width, absoluteY + height, invertColors, isFilled);
    }
}



/**
 * The page is just a MyOnScreenObject with empty implementations of all
 * abstract methods. Its only purpose is to help split more complex apps
 * into screens, or pages, each page having its own components. The page
 * is added to the MyOnScreenApplication to work as part of it.
 */
private class MyPage : MyOnScreenObject {

    public MyPage() : base(null, 0, 0, true) { }
    
    protected override void Compute(MyCanvas TargetCanvas) {
        // Nothing to do here
    }
    
    protected override void Draw(MyCanvas TargetCanvas) {
        // Nothing to do here
    }
}



/**
 * This is the main class of the application. Its purpose is to store
 * references to a multitude of pages, allow callers to activate the
 * required page and cycle the current page.
 */
private class MyOnScreenApplication {
    private List<MyScreen> TargetScreens = new List<MyScreen>();
    private List<MyPage> Pages = new List<MyPage>();
    private MyPage CurrentPage;
    private MyCanvas Canvas;

 // When rendering on higher resolutions, the compilation of the string
 // to be displayed on-screen involves more operations than the maximum
 // allowed within one single loop. To work, it must be split into
 // several iterations. Sadly, it also has to be split on multiple screens.
    private int currIteration, nIterations;
    
    public MyOnScreenApplication() {
        currIteration = 0;
        nIterations = 1;
    }

    public MyOnScreenApplication WithCanvas(MyCanvas Canvas) {
        this.Canvas = Canvas;
        return this;
    }

    public MyOnScreenApplication OnScreen(MyScreen TargetScreen) {
        return OnScreen(TargetScreen, 0, 0, Canvas.GetResX(), Canvas.GetResY());
    }

    public MyOnScreenApplication OnScreen(MyScreen TargetScreen, int clipRectX1, int clipRectY1, int clipRectX2, int clipRectY2) {
        if (Canvas == null) {
            throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen().");
        }

        TargetScreen.WithCanvas(Canvas);
        TargetScreen.WithClippedBuffer(clipRectX1, clipRectY1, clipRectX2, clipRectY2);
        TargetScreens.Add(TargetScreen);
        nIterations++;
        return this;
    }

    public void AddPage(MyPage Page) {
        Pages.Add(Page);
        if (CurrentPage == null) {
            CurrentPage = Page;
        }
    }

    public void SwitchToPage(int pageNumber) {
        if (pageNumber < 0 || pageNumber >= Pages.Count){
            return;
        }
        CurrentPage = Pages[pageNumber];
    }

    public void Cycle() {
     // Process the current iteration
        if (currIteration == 0) {
            Canvas.Clear();
            CurrentPage.Cycle(Canvas);
        } else {
            TargetScreens[currIteration - 1].FlushBufferToScreen();
        }
    
     // Go to the next iteration
        currIteration++;
        if (currIteration >= nIterations) {
            currIteration = 0;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// UTILS ///////////////////////////////////////////////////////////////////////////////////////////////////////////

public T FindFirstBlockWithName<T>(String blockName) {
 // Verify the input parameter
    if (blockName == null || blockName.Length == 0) {
        throw new ArgumentException("Invalid block name");
    }

 // Get all the blocks having the given name
    List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks);

 // If there's no block with the given name, then throw an exception
    if (allFoundBlocks == null || allFoundBlocks.Count == 0) {
        throw new ArgumentException("Cannot find a block named [" + blockName + "]");
    }

 // Return the cast result or crash and burn
    foreach(IMyTerminalBlock block in allFoundBlocks) {
        return (T)block;
    }
    return default(T);
}

public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) {
    if (BoolArray == null || BoolArray.Count() == 0) {
        return null;
    }

    bool[] ret = new bool[BoolArray.Count()];

    for (int i = 0 ; i < BoolArray.Count() ; i++) {
        ret[i] = negate ? !BoolArray[i] : BoolArray[i];
    }

    return ret;
}

public static bool[] NegateBoolArray(bool[] BoolArray) {
    return CopyBoolArray(BoolArray, true);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// HELPERS /////////////////////////////////////////////////////////////////////////////////////////////////////////

private const char SCREEN_PIXEL_VALUE_ON  = '@';
private const char SCREEN_PIXEL_VALUE_OFF = ' ';

private MyOnScreenApplication InitSingleScreenApplication(String textPanelName, int resX, int resY) {
    return new MyOnScreenApplication()
                .WithCanvas(
                    new MyCanvas(resX, resY)
                           .WithDefaultFont(CreateBitmapFont())
                 ) 
                .OnScreen(
                    new MyScreen( 
                        FindFirstBlockWithName<IMyTextPanel>(textPanelName),
                        SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF,
                        false
                    )
                 );
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// FOR DEBUGGING ///////////////////////////////////////////////////////////////////////////////////////////////////

public static Program PROGRAM;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// BLOCK INIT AND SAVE /////////////////////////////////////////////////////////////////////////////////////////////

public Program() {
 // Set the update speed in milliseconds
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

 // Get a reference to SELF, for debugging from other contexts
    PROGRAM = this;

 // Initialize the script
    Init();
}

public void Save(){
 // There is no state to be saved
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

public void Main(string argument, UpdateType updateSource) {
    OnScreenApplication.Cycle();
}