
// CONFIG //////////////////////////////////////////////////////////////////////////////////////////////////////////

// Target LCD configuration
private const String TARGET_LCD_NAME                = "DISH_STATUS_LCD";
private const int TARGET_LCD_RESOLUTION_WIDTH       = 130;
private const int TARGET_LCD_RESOLUTION_HEIGHT      =  50;
private const bool TARGET_LCD_IS_MIRRORED_ON_X_AXIS = true;

// Items from which to perform readouts
private const String HORIZONTAL_ROTOR_NAME = "HORIZONTAL AXIS ROTOR";
private const String VERTICAL_ROTOR_NAME   = "VERTICAL AXIS ROTOR";
private const String PISTON_NAME           = "Dish Piston";
private const String DISH_NAME             = "X-COM HQ";

// The POST message will be displayed for a given number of loops.
private const int POST_DURATION = 10;



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






// FOR DEBUGGING ///////////////////////////////////////////////////////////////////////////////////////////////////

public static Program PROGRAM;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// WORKING VARIABLES AND CONSTANTS /////////////////////////////////////////////////////////////////////////////////

private const char SCREEN_PIXEL_VALUE_ON  = '@';
private const char SCREEN_PIXEL_VALUE_OFF = ' ';

private const bool O = true;
private const bool _ = false;

private IMyTextPanel    TargetLcd;
private IMyPistonBase   PistonBase;
private IMyMotorStator  HorizontalRotor; // attribute = Orientation
private IMyMotorStator  VerticalRotor;
private IMyRadioAntenna Dish;

private MyScreen Screen;

private MyOnScreenApplication OnScreenApplication;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// SPRITES /////////////////////////////////////////////////////////////////////////////////////////////////////////

private static MySprite SPRITE_PWR_OFF = new MySprite(13, 10, new bool[] {
    _,_,_,_,_,_,O,_,_,_,_,_,_,
    _,_,_,_,_,_,O,_,_,_,_,_,_,
    _,_,_,O,O,_,O,_,O,O,_,_,_,
    _,_,O,_,_,_,O,_,_,_,O,_,_,
    _,_,O,_,_,_,O,_,_,_,O,_,_,
    _,_,O,_,_,_,_,_,_,_,O,_,_,
    _,_,O,_,_,_,_,_,_,_,O,_,_,
    _,_,O,_,_,_,_,_,_,_,O,_,_,
    _,_,_,O,O,O,O,O,O,O,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,
});

private static MySprite SPRITE_PWR_ON = new MySprite(13, 10, NegateBoolArray(SPRITE_PWR_OFF.data));

private static MySprite SPRITE_UP_OFF = new MySprite(13, 10, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,_,O,_,_,_,_,_,_,
    _,_,_,_,_,O,O,O,_,_,_,_,_,
    _,_,_,_,O,O,O,O,O,_,_,_,_,
    _,_,_,O,O,O,O,O,O,O,_,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,
});

private static MySprite SPRITE_UP_ON = new MySprite(13, 10, NegateBoolArray(SPRITE_UP_OFF.data));

private static MySprite SPRITE_DOWN_OFF = new MySprite(13, 10, new bool[] {
    _,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,O,O,O,O,O,O,O,O,O,_,_,
    _,_,_,O,O,O,O,O,O,O,_,_,_,
    _,_,_,_,O,O,O,O,O,_,_,_,_,
    _,_,_,_,_,O,O,O,_,_,_,_,_,
    _,_,_,_,_,_,O,_,_,_,_,_,_,
    _,_,_,_,_,_,_,_,_,_,_,_,_,
});

private static MySprite SPRITE_DOWN_ON = new MySprite(13, 10, NegateBoolArray(SPRITE_DOWN_OFF.data));

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



// ANIMATED SPRITES ////////////////////////////////////////////////////////////////////////////////////////////////

MyStatefulAnimatedSprite PwrButtonSprite = new MyStatefulAnimatedSprite(null, 0, 0, true)
    .WithState("off", new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_PWR_OFF}))
    .WithState("on" , new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_PWR_ON }))
;

MyStatefulAnimatedSprite DownButtonSprite = new MyStatefulAnimatedSprite(null, 0, 0, true)
    .WithState("off", new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_DOWN_OFF}))
    .WithState("on" , new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_DOWN_OFF, SPRITE_DOWN_ON }))
;

MyStatefulAnimatedSprite UpButtonSprite = new MyStatefulAnimatedSprite(null, 0, 0, true)
    .WithState("off", new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_UP_OFF}))
    .WithState("on" , new MyStatefulAnimatedSpriteState(new MySprite[] { SPRITE_UP_OFF, SPRITE_UP_ON }))
;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// INIT ////////////////////////////////////////////////////////////////////////////////////////////////////////////

private void InitScript() {
 // Initial message
    PROGRAM.Echo("Initializing...");

 // Fetching objects from the GridTerminalSystem
    TargetLcd       = FindFirstBlockWithName<IMyTextPanel>(TARGET_LCD_NAME);
    PistonBase      = FindFirstBlockWithName<IMyPistonBase>(PISTON_NAME);
    HorizontalRotor = FindFirstBlockWithName<IMyMotorStator>(HORIZONTAL_ROTOR_NAME);
    VerticalRotor   = FindFirstBlockWithName<IMyMotorStator>(VERTICAL_ROTOR_NAME);
    Dish            = FindFirstBlockWithName<IMyRadioAntenna>(DISH_NAME);
    PROGRAM.Echo("Required grid items identified");

 // Initializing the screens
    Screen = new MyScreen(
                    TargetLcd, // Target text panel
                    TARGET_LCD_RESOLUTION_WIDTH, TARGET_LCD_RESOLUTION_HEIGHT, // Screen resolution
                    SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF, // Which chars to represent the ON / OFF values of a given pixel
                    TARGET_LCD_IS_MIRRORED_ON_X_AXIS // Whether or not to mirror the image on the X asis
                );

 // Add a bitmap font to the screen, so it can draw text
    Screen.SetDefaultFont(CreateBitmapFont());

 // Announce the ending of the screen initialization
    PROGRAM.Echo("Screen initialized");

 // Create the application for the previously initialized screen
    OnScreenApplication = new MyOnScreenApplication(Screen);

 // Initialize the pages of the application
    InitPages(OnScreenApplication);

 // Final message
    PROGRAM.Echo("Application initialized");
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// PAGES ///////////////////////////////////////////////////////////////////////////////////////////////////////////

private const int PAGE_INDEX_POST = 0;
private const int PAGE_INDEX_ELEVATION_CONTROL = 1;

private int nFramesOnPostMessage = 0;



private void InitPages(MyOnScreenApplication OnScreenApplication) {

 // The POST message
    MyPage POSTMessagePage = (MyPage) new MyPage()
      // Custom drawing
        .WithClientDrawMethod((MyScreen Screen) => {
            Screen.FillEntireScreen(O);
            Screen.DrawColorText(31, 13, "Dish Control", true, true);
            Screen.DrawRect(31, 23, 102, 33, true, true);
            Screen.DrawColorText(34, 25, "INITIALIZED", false, false);
            Screen.FlushBufferToScreen();
        return 0;})
      // Custom cycling
        .WithClientCycleMethod((MyOnScreenObject obj) => {
            nFramesOnPostMessage++;
            if (nFramesOnPostMessage > POST_DURATION) {
                OnScreenApplication.SwitchToPage(PAGE_INDEX_ELEVATION_CONTROL);
            }
        return 0;});
    OnScreenApplication.AddPage(POSTMessagePage);



 // The elevation control screen
    MyPage ElevationControlScreen = new MyPage();
    OnScreenApplication.AddPage(ElevationControlScreen);

    ElevationControlScreen.AddChild(new MyTextLabel("ELEVATION CONTROL", null, 13, 0, true, false, false));

    ElevationControlScreen.AddChildAtLocation(PwrButtonSprite , 22, 40);
    ElevationControlScreen.AddChildAtLocation(DownButtonSprite, 69, 40);
    ElevationControlScreen.AddChildAtLocation(UpButtonSprite  , 96, 40);

    ElevationControlScreen.AddChild(new MyPanel(null, 0, 10, 130, 30, true, false));
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SCRIPT LOGIC ////////////////////////////////////////////////////////////////////////////////////////////////////

private int dishEnabledState = 0;

private void MonitorAntenna() {
    if (Dish.Enabled) {
        if (dishEnabledState == 0) {
            dishEnabledState = 1;
            PwrButtonSprite.SetState("on");
        }
    } else {
        if (dishEnabledState == 1) {
            dishEnabledState = 0;
            PwrButtonSprite.SetState("off");
        }
    }
}

private float prevPistonPosition = 0;
private bool isPistonPositionChangingUp = false;
private bool isPistonPositionChangingDown = false;

private void MonitorThePiston() {
    isPistonPositionChangingUp = false;
    isPistonPositionChangingDown = false;

    if (PistonBase.CurrentPosition > prevPistonPosition) {
        isPistonPositionChangingUp = true;
    } else {
        if (PistonBase.CurrentPosition < prevPistonPosition) {
            isPistonPositionChangingDown = true;
        }
    }

    prevPistonPosition = PistonBase.CurrentPosition;

    if (isPistonPositionChangingUp) {
        UpButtonSprite.SetState("on");
    } else {
        UpButtonSprite.SetState("off");
    }

    if (isPistonPositionChangingDown) {
        DownButtonSprite.SetState("on");
    } else {
        DownButtonSprite.SetState("off");
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// OPERATION ///////////////////////////////////////////////////////////////////////////////////////////////////////

private void LogicLoop() {
    MonitorAntenna();
    MonitorThePiston();
}

private void RenderLoop() {
    OnScreenApplication.Cycle();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// THE DRAWING FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////

private class MyScreen {
    private IMyTextPanel TargetLCD;
    private char pixelValueOn;
    private char pixelValueOff;
    private bool mirrorX;

    private bool[] ScreenBuffer;
    private int resX;
    private int resY;
    private int length;

    private MySprite[] DefaultFont;


    public MyScreen(IMyTextPanel TargetLCD, int resX, int resY, char pixelValueOn, char pixelValueOff, bool mirrorX) {
        this.TargetLCD = TargetLCD;
        this.resX = resX;
        this.resY = resY;
        length = resY * resX;
        ScreenBuffer = new bool[length];
        this.pixelValueOn = pixelValueOn;
        this.pixelValueOff = pixelValueOff;
        this.mirrorX = mirrorX;
    }

    public void SetDefaultFont(MySprite[] DefaultFont) {
        this.DefaultFont = DefaultFont;
    }

    public void Clear() {
        FillEntireScreen(false);
    }

    public void FillEntireScreen(bool value) {
        for (int x = 0 ; x < length ; x++) {
            ScreenBuffer[x] = value;
        }
    }

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

    public void BitBlt(MySprite sprite, int x, int y) {
        BitBltExt(sprite, x, y, false, false);
    }

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
                ScreenBuffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], ScreenBuffer[screenPos], invertColors, transparentBackground);
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

    public void DrawText(int x, int y, String text) {
        DrawColorText(x, y, text, false, false);
    }

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
     // This, in turn, makes it easier to navigate the ScreenBuffer
        int rectWidth = actualX2 - actualX1;

     // Initialize the vertical cursor
        int screenPosY = actualY1;

     // Run the vertical cursor through each line of the rectangle
        while (screenPosY <= actualY2) {
         // Set the ScreenBuffer cursor to the left margin of the current line
            int screenPos = screenPosY * resX + actualX1;

         // The value to set is either ON (normal value) or OFF (if the invertColors flag is set)
            bool targetColor = !invertColors;

         // The target color must be set to the left and right margin of the current line of the rectangle
            ScreenBuffer[screenPos] = targetColor;
            ScreenBuffer[screenPos + rectWidth-1] = targetColor;

         // In case the fillRect flag was set or if this is the first or the last line of the rectangle,
         // the target color must be set to all the pixels in between the left and the right margin
            if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) {
                for (int innerPos = screenPos ; innerPos < screenPos + rectWidth ; innerPos++) {
                    ScreenBuffer[innerPos] = targetColor;
                }
            }

         // Move the cursors to the next line
            screenPos += resX;
            screenPosY++;
        }
        
        
    }

    private bool[] MirrorBufferOnXAxis() {
        bool[] MirroredBuffer = new bool[length];
        
        int mirrorPosX = resX-1;
        int mirrorPos = mirrorPosX;
        
        for (int sourcePos = 0 ; sourcePos < length ; sourcePos++) {
            MirroredBuffer[mirrorPos] = ScreenBuffer[sourcePos];
            
            mirrorPos--;
            mirrorPosX--;
            if (mirrorPosX == -1) {
                mirrorPosX = resX-1;
                mirrorPos += resX*2;
            }
        }

        return MirroredBuffer;
    }

    public void FlushBufferToScreen() {
     // In case the screen needs to be mirrored on the X axis
        bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis() : ScreenBuffer;
        
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
    protected int x;
    protected int y;
    protected bool isVisible = true;
    protected MyOnScreenObject ParentObject;
    private List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>();
    private Func<MyOnScreenObject, int> ClientCycleMethod;
    private Func<MyScreen, int> ClientDrawMethod;

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
        this.ClientCycleMethod = ClientCycleMethod;
        return this;
    }

    public MyOnScreenObject WithClientDrawMethod(Func<MyScreen, int> ClientDrawMethod) {
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
    public void Cycle(MyScreen TargetScreen) {
     // Compute position, frame index and other state-related properties
     // which might have to be computed even if the object is not visible
     // on screen
        Compute(TargetScreen);

     // Run the custom functionality (if any)
        if (ClientCycleMethod != null) {
            ClientCycleMethod(this);
        }

     // Cycle child objects (if any)
        foreach(MyOnScreenObject ChildObject in ChildObjects) {
            ChildObject.Cycle(TargetScreen);
        }

     // If the object is visible on screen, then draw it
        if (IsObjectVisible()) {
         // Run the client draw method, if it's set
            if (ClientDrawMethod != null) {
                ClientDrawMethod(TargetScreen);
            }

         // Call the object's draw method
            Draw(TargetScreen);
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
    protected abstract void Compute(MyScreen TargetScreen);

  /**
   * This method handles the drawing of the object onto the TargetScreen
   */
    protected abstract void Draw(MyScreen TargetScreen);
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

    
    protected override void Compute(MyScreen TargetScreen) {
     // Frames must cycle on even if the object is not visible on screen
        CurrentFrame = CurrentState.GetFrame();
    }

    protected override void Draw(MyScreen TargetScreen) {
        TargetScreen.BitBltExt(
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

    protected override void Compute(MyScreen TargetScreen) {
        // Nothing to do here
    }
    
    protected override void Draw(MyScreen TargetScreen) {
        TargetScreen.DrawColorText(GetAbsoluteX(), GetAbsoluteY(), text, invertColors, transparentBackground);
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

    public MyPanel(MyOnScreenObject ParentObject, int x, int y, int width, int height, bool isVisible, bool isFilled)
    : base(ParentObject, x, y, isVisible) {
        this.width = width;
        this.height = height;
        this.isFilled = isFilled;
    }
    
    protected override void Compute(MyScreen TargetScreen) {
        // Nothing to do here
    }

    protected override void Draw(MyScreen TargetScreen) {
        int absoluteX = GetAbsoluteX();
        int absoluteY = GetAbsoluteY();

        TargetScreen.DrawRect(absoluteX, absoluteY, absoluteX + width, absoluteY + height, false, isFilled);
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
    
    protected override void Compute(MyScreen TargetScreen) {
        // Nothing to do here
    }
    
    protected override void Draw(MyScreen TargetScreen) {
        // Nothing to do here
    }
}



/**
 * This is the main class of the application. Its purpose is to store
 * references to a multitude of pages, allow callers to activate the
 * required page and cycle the current page.
 */
private class MyOnScreenApplication {
    private MyScreen TargetScreen;
    private List<MyPage> Pages = new List<MyPage>();
    private MyPage CurrentPage;
    
    public MyOnScreenApplication(MyScreen TargetScreen) {
        this.TargetScreen = TargetScreen;
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

        MyPage Page = Pages.ElementAtOrDefault(pageNumber);

        if (Page != null) {
            CurrentPage = Page;
        }
    }

    public void Cycle() {
        TargetScreen.Clear();
        CurrentPage.Cycle(TargetScreen);
        TargetScreen.FlushBufferToScreen();
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

public static bool[] NegateBoolArray(bool[] BoolArray) {
    if (BoolArray == null || BoolArray.Count() == 0) {
        return null;
    }

    bool[] ret = new bool[BoolArray.Count()];

    for (int i = 0 ; i < BoolArray.Count() ; i++) {
        ret[i] = !BoolArray[i];
    }

    return ret;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





// BLOCK INIT AND SAVE /////////////////////////////////////////////////////////////////////////////////////////////

public Program() {
 // Set the update speed in milliseconds
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

 // Get a reference to SELF, for debugging from other contexts
    PROGRAM = this;

 // Init
    InitScript();
}

public void Save(){
 // There is no state to be saved
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

public void Main(string argument, UpdateType updateSource) {
    LogicLoop();
    RenderLoop();
}