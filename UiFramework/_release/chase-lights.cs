/* README //////////////////////////////////////////////////////////////////////////////////////////////////////////

This script draws moving lights on corner LCDs, for an 80s style Sci Fi look. The lights
can take user-defined shapes and can move in a number of patterns, at user-defined speeds.
The script can operate one or two corner LDCs at a time.

NOTE: The script output will not look good on anything other than corner LCDs.




To configure the script, please edit the following sections:
    - CONFIGURATION  = This is where the behavior of the lights is configured
    - CUSTOM SPRITES = This is where the custom shapes are defined



This is a showcase of the UI Framework, which enables low resolution graphics on text panels by dumping the frame
buffer into a string, which is then set as the text property of a given text panel, which is set up to use the
Monospace font and very small font size. The UI framework is minified in this script. The development version can be
found here: https://github.com/fx89/spaceengineers/tree/main/UiFramework/UiFramework

For a working example, please visit the following workshop item:
    https://steamcommunity.com/sharedfiles/filedetails/?id=2415572447

Development and partial building is done using MDK-SE: https://github.com/malware-dev/MDK-SE

*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIGURATION ///////////////////////////////////////////////////////////////////////////////////////////////////
/*
   The script supports two LCD panels: the primary panel and the secondary panel.
   The primary panel must be configured for the script to work. The secondary panel
   is optional and can be discard by setting the USE_SECONDARY_PANEL property to false.
   Please look for it below.

   Each panel must be configured with the name of the LCD panel on which the lights
   are to be displayed, as well as the desired operating mode, the movement speed and
   a flag that tells if the image drawn on the panel should be mirrored.

   The following operating modes are supported:
    - OP_MODE_LEFT_TO_RIGHT
        The chase light will run from left to right.
        When it reaches the right margin, the chase light will jump to the left margin.

    - OP_MODE_RIGHT_TO_LEFT
        The chase light will run from right to left.
        When it reaches the left margin, the chase light will jump to the right margin.

    - OP_MODE_BOUNCE_START_FROM_LEFT
        The chase light will run from the left margin to the right margin and back again.

    - OP_MODE_BOUNCE_START_FROM_RIGHT
        The chase light will run from the right margin to the left margin and back again.
 */

// Configure the primary panel
private static string     TEXT_PANEL_1_NAME         = "LEFT_CHASE_LIGHT_PANEL";   // Name of the primary panel
private static bool       TEXT_PANEL_1_MIRRORED     = true;                       // If the panel is placed in certain ways, it might have to be mirrored so that left is left and right is right
private static int        TEXT_PANEL_1_OP_MODE      = OP_MODE_LEFT_TO_RIGHT;      // Operating mode of the secondary panel (explained above)
private static int        TEXT_PANEL_1_SHAPE_SPEED  = 3;                          // How fast the chase light moves across the screen (1 - 10)

// The shape of the chase light running across the primary panel
// This is set to CHASE_LIGHT_LEFT, which is defined in the CUSTOM SPRITES section,
// but it can be changed to any other custome sprite that may be added for the user's convenience.
private static Func<MySprite[]> TEXT_PANEL_1_SHAPE = () => { return CHASE_LIGHT_LEFT; };

// Configure the secondary panel
private static bool       USE_SECONDARY_PANEL       = true;                       // TRUE/FALSE = use this panel or not
private static string     TEXT_PANEL_2_NAME         = "RIGHT_CHASE_LIGHT_PANEL";  // Name of the second panel (ignored if not used)
private static bool       TEXT_PANEL_2_MIRRORED     = true;                       // If the panel is placed in certain ways, it might have to be mirrored so that left is left and right is right
private static int        TEXT_PANEL_2_OP_MODE      = OP_MODE_RIGHT_TO_LEFT;      // Operating mode of the secondary panel (explained above)
private static int        TEXT_PANEL_2_SHAPE_SPEED  = 3;                          // How fast the chase light moves across the screen (1 - 10)

// The shape of the chase light running across the secondary panel
// This is set to CHASE_LIGHT_RIGHT, which is defined in the CUSTOM SPRITES section,
// but it can be changed to any other custome sprite that may be added for the user's convenience.
private static Func<MySprite[]> TEXT_PANEL_2_SHAPE = () => { return CHASE_LIGHT_RIGHT; };

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CUSTOM SPRITES //////////////////////////////////////////////////////////////////////////////////////////////////

// These are the chase light shapes. They are initialized below.
// Different shapes may be defined in a similar manner.
private static MySprite[] CHASE_LIGHT_LEFT;
private static MySprite[] CHASE_LIGHT_RIGHT;

// These are required to beautify the definition of the sprites below.
private const bool O = true, _ = false;

/**
 * The InitSprites[1-4]() methods are called from the first 4 loops,
 * each loop calling one method at a time. This helps spread the
 * workload of loading sprites across multiple initialization steps,
 * to avoid "Script too Complex" errors.
 */
private void InitSprites1() {
 // The default chase light shape is an animation with only one frame of 15x8 pixels.
 // The pixels array is defined below.
    CHASE_LIGHT_LEFT = new MySprite[]{new MySprite(15, 8, new bool[] {
        _,_,_,_,_,_,_,O,O,O,O,O,O,_,_,
        _,_,_,_,O,_,O,O,O,O,O,O,O,O,_,
        _,_,_,O,_,O,O,O,O,O,O,O,O,O,O,
        _,O,O,_,O,O,O,O,O,O,O,O,O,O,O,
        _,_,O,_,O,O,O,O,O,O,O,O,O,O,O,
        _,_,_,O,_,O,O,O,O,O,O,O,O,O,O,
        _,_,_,_,O,_,O,O,O,O,O,O,O,O,_,
        _,_,_,_,_,_,_,O,O,O,O,O,O,_,_
    })};

    CHASE_LIGHT_RIGHT = new MySprite[]{new MySprite(15, 8, new bool[] {
        _,_,O,O,O,O,O,O,_,_,_,_,_,_,_,
        _,O,O,O,O,O,O,O,O,_,O,_,_,_,_,
        O,O,O,O,O,O,O,O,O,O,_,O,_,_,_,
        O,O,O,O,O,O,O,O,O,O,O,_,O,O,_,
        O,O,O,O,O,O,O,O,O,O,O,_,O,_,_,
        O,O,O,O,O,O,O,O,O,O,_,O,_,_,_,
        _,O,O,O,O,O,O,O,O,_,O,_,_,_,_,
        _,_,O,O,O,O,O,O,_,_,_,_,_,_,_
    })};
}
private void InitSprites2() {

}
private void InitSprites3() {

}
private void InitSprites4() {

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// THE CHASE LIGHT LOGIC ///////////////////////////////////////////////////////////////////////////////////////////

// These are the operating modes mentioned at the top of the script.
// They are defined here so that they are kept out of the way.
private const int OP_MODE_LEFT_TO_RIGHT           = 0;
private const int OP_MODE_RIGHT_TO_LEFT           = 1;
private const int OP_MODE_BOUNCE_START_FROM_LEFT  = 2;
private const int OP_MODE_BOUNCE_START_FROM_RIGHT = 3;

/**
 * This is the class managing the chase light movement across the screen.
 * As seen above, there can be two instances of this class: one for the
 * primary screen and one for the secondary screen. Upon construction,
 * this class initializes a MyOnScreenApplication object linked to the
 * target screen and having one page, on which the chase light shape is
 * drawn. The ChaseLightShapeFrames array allows users to specify multiple
 * frames, in case animated shapes are required for the project.
 */
private class MyChaseLightController {
    private MyOnScreenApplication OnScreenApplication;
    private MyStatefulAnimatedSprite ChaseLightShape;
    private int movementVector = 0;

    private const int RES_X = 282;
    private const int RES_Y = 27;

    private const float FONT_SIZE = 0.373f;

    public MyChaseLightController(
        IMyGridTerminalSystem GridTerminalSystem,
        string textPanelName,
        bool mirrored,
        int operatingMode,
        int movementSpeed,
        MySprite[] ChaseLightShapeFrames
    ) {
     // Sanity check
        if (textPanelName == null || textPanelName.Length == 0) {
            throw new ArgumentException("The name of the text panel must not be at least one character long");
        }

        if (ChaseLightShapeFrames == null || ChaseLightShapeFrames.Length == 0) {
            throw new ArgumentException("The ChaseLightShapeFrames array must have at least one element");
        }

        if (operatingMode < OP_MODE_LEFT_TO_RIGHT || operatingMode > OP_MODE_BOUNCE_START_FROM_RIGHT) {
            throw new ArgumentException("The operating mode must have one of the following values: OP_MODE_LEFT_TO_RIGHT, OP_MODE_RIGHT_TO_LEFT, OP_MODE_BOUNCE_START_FROM_LEFT, OP_MODE_BOUNCE_START_FROM_RIGHT");
        }

        if (movementSpeed < 1 || movementSpeed > 10) {
            throw new ArgumentException("The movement speed must be between 1 and 10");
        }

     // Set up the text panel
        TerminalUtils.SetupTextPanelForMatrixDisplay(GridTerminalSystem, textPanelName, FONT_SIZE);

     // Initialize the application
        OnScreenApplication
        = UiFrameworkUtils.InitSingleScreenApplication(
            GridTerminalSystem, textPanelName, // Reference to the target text panel
            RES_X, RES_Y,                      // The target display resolution
            mirrored                           // The screen image might have to be mirrored
          );

     // Create the main page and add it to the application
        MyPage MainPage = new MyPage();
        OnScreenApplication.AddPage(MainPage);

     // Create the ChaseLightShape with only one state, named "Default",
     // having the referenced frames array as its animation
        ChaseLightShape = new MyStatefulAnimatedSprite(0, 0)
            .WithState("Default", new MyStatefulAnimatedSpriteState(ChaseLightShapeFrames));

     // Add the ChaseLightShape to the main page
        MainPage.AddChild(ChaseLightShape);

     // Set the movement vector
        if (operatingMode == OP_MODE_RIGHT_TO_LEFT || operatingMode == OP_MODE_BOUNCE_START_FROM_RIGHT) {
            movementVector = -movementSpeed;
        } else {
            movementVector = movementSpeed;
        }

     // Set the client cycle method to the chase light shape according to the referenced operating mode
        ChaseLightShape.WithClientCycleMethod((MyOnScreenObject Obj) => {
            // Center vertically (each frame might have a different height,
            // so this is required to run on every frame)
            Obj.y = (RES_Y - Obj.GetHeight()) / 2;

            // Move 
            Obj.x += movementVector;

         // Apply the proper action for when the object goes off-screen,
         // according to the set operating mode
            if (operatingMode == OP_MODE_RIGHT_TO_LEFT) {
             // If it's right to left, then the objects exits through the
             // left side and enters through the right side of the screen.
                if (Obj.x < 0) {
                    Obj.x = RES_X - 1 - Obj.GetWidth();
                }
            } else if (             
                operatingMode == OP_MODE_BOUNCE_START_FROM_LEFT ||
                operatingMode == OP_MODE_BOUNCE_START_FROM_RIGHT
            ) {
             // If it's bouncing, then the object's vector has to be switched
             // whenever it reches one side or the other.
                if (Obj.x < 0 || Obj.x + Obj.GetWidth() > RES_X - 1) {
                    movementVector = -movementVector;
                }
            } else {
             // The default is OP_MODE_LEFT_TO_RIGHT.
             // In this case, the objects exits the screen through the right side
             // and enters through the left side.
                if (Obj.x + Obj.GetWidth() > RES_X - 1) {
                    Obj.x = 0;
                }
            }

        return 1;});

    }

    public void Cycle() {
        OnScreenApplication.Cycle();
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SCRIPT INITIALIZATION ///////////////////////////////////////////////////////////////////////////////////////////

private MyChaseLightController PrimaryPanelController;
private MyChaseLightController SecondaryPanelController;

/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *      > adding pages
  *      > adding components to the pages
  *      > linking logic and animations
  */
private void InitApplication() {
     PrimaryPanelController = new MyChaseLightController(
        GridTerminalSystem, TEXT_PANEL_1_NAME, TEXT_PANEL_1_MIRRORED,
        TEXT_PANEL_1_OP_MODE,
        TEXT_PANEL_1_SHAPE_SPEED,
        TEXT_PANEL_1_SHAPE()
    );

    if (USE_SECONDARY_PANEL) {
        if (TEXT_PANEL_2_NAME == TEXT_PANEL_1_NAME) {
            throw new ArgumentException("The name of the secondary panel is not allowed to be the same as that of the primary panel");
        }

        SecondaryPanelController = new MyChaseLightController(
            GridTerminalSystem, TEXT_PANEL_2_NAME, TEXT_PANEL_2_MIRRORED,
            TEXT_PANEL_2_OP_MODE,
            TEXT_PANEL_2_SHAPE_SPEED,
            TEXT_PANEL_2_SHAPE()
        );
    }
}

/**
 * This is called regularly starting with the 6th loop.
 * Its purpose is to cycle the chase light controllers at runtime.
 */
 private bool isScreenTwo = false;
private void CycleApplication() {
 // If the secondary panel controller is active,
 // then avoid the "Script too Complex" error
 // by cycling the two controllers on different frames
    if (SecondaryPanelController != null) {
        if (isScreenTwo) {
            SecondaryPanelController.Cycle();
        } else {
            PrimaryPanelController.Cycle();
        }
        isScreenTwo = !isScreenTwo;
    } else {
     // If the secondary panel controller is not active,
     // then just go ahead and cycle the primary controller
     // on every frame
        PrimaryPanelController.Cycle();
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static MyGridProgram PROGRAM;

public Program() {
    // Set the update speed in milliseconds
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    // Get a reference to SELF, for debugging from other contexts
    PROGRAM = this;
}

public void Save() {
    // There is no state to be saved
}

private int initStepNbr = 0;

public void Main(string argument, UpdateType updateSource) {
 // Initialize the script. Do it in many steps, to avoid "script too complex" errors
 // caused by loading too many pictures in one single frame.
    if (initStepNbr < 5) {
        initStepNbr++;
        if (initStepNbr == 1) InitSprites1();
        if (initStepNbr == 2) InitSprites2();
        if (initStepNbr == 3) InitSprites3();
        if (initStepNbr == 4) InitSprites4();
        if (initStepNbr == 5) InitApplication();
    } else {
        CycleApplication();
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MINIFIED UI FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////
}public class Constants { public const int FLOATING_POSITION_TOP = 0, FLOATING_POSITION_LEFT = 1, FLOATING_POSITION_RIGHT = 2, FLOATING_POSITION_BOTTOM = 3; public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2;}public class DefaultMonospaceFont { private static MySprite CreateFontSprite(byte[] bytes) { MyCanvas Cvs = new MyCanvas(6, 7); Cvs.BitBlt(new MySprite(8, 7, DrawingFrameworkUtils.ByteArrayToBoolArray(bytes)), 0, 0); return new MySprite(6, 7, Cvs.GetBuffer()); } private static MySprite SPRITE_A = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_B = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0 }); private static MySprite SPRITE_C = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_D = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0x88,0x88,0x88,0xf0 }); private static MySprite SPRITE_E = CreateFontSprite(new byte[] { 0xf0,0x80,0x80,0xf0,0x80,0x80,0xf0 }); private static MySprite SPRITE_F = CreateFontSprite(new byte[] { 0xf8,0x80,0x80,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_G = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x98,0x88,0x88,0x78 }); private static MySprite SPRITE_H = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_I = CreateFontSprite(new byte[] { 0x70,0x20,0x20,0x20,0x20,0x20,0x70 }); private static MySprite SPRITE_J = CreateFontSprite(new byte[] { 0x10,0x10,0x10,0x10,0x10,0x10,0x60 }); private static MySprite SPRITE_K = CreateFontSprite(new byte[] { 0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88 }); private static MySprite SPRITE_L = CreateFontSprite(new byte[] { 0x80,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_M = CreateFontSprite(new byte[] { 0x88,0xd8,0xa8,0x88,0x88,0x88,0x88 }); private static MySprite SPRITE_N = CreateFontSprite(new byte[] { 0x88,0xc8,0xa8,0x98,0x88,0x88,0x88 }); private static MySprite SPRITE_O = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_P = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_Q = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0xa8,0x90,0x68 }); private static MySprite SPRITE_R = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0xa0,0x90,0x88 }); private static MySprite SPRITE_S = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_T = CreateFontSprite(new byte[] { 0xf8,0x20,0x20,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_U = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_V = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x50,0x20 }); private static MySprite SPRITE_W = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0xa8,0x50 }); private static MySprite SPRITE_X = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x50,0x88,0x88 }); private static MySprite SPRITE_Y = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_Z = CreateFontSprite(new byte[] { 0xf8,0x08,0x10,0x20,0x40,0x80,0xf8 }); private static MySprite SPRITE_1 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x10,0x10,0x10,0x38 }); private static MySprite SPRITE_2 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x08,0x70,0x40,0x78 }); private static MySprite SPRITE_3 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x30,0x08,0x48,0x30 }); private static MySprite SPRITE_4 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x90,0xf8,0x10,0x10 }); private static MySprite SPRITE_5 = CreateFontSprite(new byte[] { 0x78,0x40,0x40,0x70,0x08,0x08,0x70 }); private static MySprite SPRITE_6 = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0xf0,0x88,0x88,0x70 }); private static MySprite SPRITE_7 = CreateFontSprite(new byte[] { 0xf8,0x08,0x08,0x10,0x20,0x40,0x40 }); private static MySprite SPRITE_8 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x88,0x88,0x70 }); private static MySprite SPRITE_9 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_0 = CreateFontSprite(new byte[] { 0x70,0x88,0x98,0xa8,0xc8,0x88,0x70 }); private static MySprite SPRITE_DASH = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0x00,0x00,0x00,0xf8 }); private static MySprite SPRITE_HYPHEN = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0xf8,0x00,0x00,0x00 }); private static MySprite SPRITE_GT = CreateFontSprite(new byte[] { 0x40,0x20,0x10,0x08,0x10,0x20,0x40 }); private static MySprite SPRITE_LT = CreateFontSprite(new byte[] { 0x08,0x10,0x20,0x40,0x20,0x10,0x08 }); private static MySprite SPRITE_EQ = CreateFontSprite(new byte[] { 0x00,0x00,0xf8,0x00,0xf8,0x00,0x00 }); private static MySprite SPRITE_PCT = CreateFontSprite(new byte[] { 0xc0,0xc8,0x10,0x20,0x40,0x98,0x18 }); private static MySprite[] Create() { MySprite[] BitmapFont = new MySprite[256]; BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A; BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B; BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C; BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D; BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E; BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F; BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G; BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H; BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I; BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J; BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K; BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L; BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M; BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N; BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O; BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P; BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q; BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R; BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S; BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T; BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U; BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V; BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W; BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X; BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y; BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z; BitmapFont['1'] = SPRITE_1; BitmapFont['2'] = SPRITE_2; BitmapFont['3'] = SPRITE_3; BitmapFont['4'] = SPRITE_4; BitmapFont['5'] = SPRITE_5; BitmapFont['6'] = SPRITE_6; BitmapFont['7'] = SPRITE_7; BitmapFont['8'] = SPRITE_8; BitmapFont['9'] = SPRITE_9; BitmapFont['0'] = SPRITE_0; BitmapFont['_'] = SPRITE_DASH; BitmapFont['-'] = SPRITE_HYPHEN; BitmapFont['<'] = SPRITE_LT; BitmapFont['>'] = SPRITE_GT; BitmapFont['='] = SPRITE_EQ; BitmapFont['%'] = SPRITE_PCT; return BitmapFont; } public static MySprite[] BitmapFont = Create();}public class DrawingFrameworkConstants { public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2; public const int VERTICAL_ALIGN_TOP = 0, VERTICAL_ALIGN_MIDDLE = 1, VERTICAL_ALIGN_BOTTOM = 2;}public class DrawingFrameworkUtils { private static readonly byte[] BYTE_POW = new byte[]{ 128,64,32,16,8,4,2,1 }; public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) { if (BoolArray == null || BoolArray.Count() == 0) { return null; } bool[] ret = new bool[BoolArray.Count()]; for (int i = 0; i < BoolArray.Count(); i++) { ret[i] = negate ? !BoolArray[i] : BoolArray[i]; } return ret; } public static bool[] NegateBoolArray(bool[] BoolArray) { return CopyBoolArray(BoolArray, true); } public static bool[] ByteArrayToBoolArray(byte [] byteArray) { if (byteArray == null || byteArray.Length == 0) { return new bool[]{}; } bool[] ret = new bool[byteArray.Length * 8]; int retIdx = 0; for (int bIdx = 0 ; bIdx < byteArray.Length ; bIdx++) { byte b = byteArray[bIdx]; for (int divIdx = 0 ; divIdx < 8 ; divIdx++) { ret[retIdx] = (b / BYTE_POW[divIdx] > 0); b = (byte)(b % BYTE_POW[divIdx]); retIdx++; } } return ret; } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight) { return ResizeSpriteCanvas(Sprite, newWidth, newHeight, DrawingFrameworkConstants.HORIZONTAL_ALIGN_CENTER, DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE); } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight, int horizontalAlignment, int verticalAlignment) { if (Sprite == null || newWidth < 1 || newHeight < 1) { return null; } MyCanvas NewCanvas = new MyCanvas(newWidth, newHeight); int posX = ComputePos(Sprite.width, newWidth, horizontalAlignment); int posY = ComputePos(Sprite.height, newHeight, verticalAlignment); NewCanvas.BitBlt(Sprite, posX, posY); return new MySprite(newWidth, newHeight, NewCanvas.GetBuffer()); } private static int ComputePos(int origSize, int newSize, int alignemnt) { if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE) { return (newSize - origSize) / 2; } if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_BOTTOM) { return newSize - 1 - origSize; } return 0; }}public class MyBlinkingIcon : MyOnScreenObject { private MyStatefulAnimatedSprite Sprite; private int blinkingInterval = 3; private int blinkTimeout = 0; private bool isOn = false; private bool isBlinking = false; private int nBlinkTimes = 0; public MyBlinkingIcon(int x, int y, MySprite Graphics) : base(null, x, y, true) { Sprite = new MyStatefulAnimatedSprite(0, 0) .WithState("Off", new MyStatefulAnimatedSpriteState(new MySprite[]{ Graphics })) .WithState("On" , new MyStatefulAnimatedSpriteState(new MySprite[]{ new MySprite(Graphics.width, Graphics.height, DrawingFrameworkUtils.NegateBoolArray(Graphics.data)) })); AddChild(Sprite); } public MyBlinkingIcon WithBlinkingInterval(int blinkingInterval) { this.blinkingInterval = blinkingInterval; return this; } public override int GetWidth() { return Sprite.GetWidth(); } public override int GetHeight() { return Sprite.GetHeight(); } protected override void Init() { } private void LocalSwitchOn() { Sprite.SetState("On"); isOn = true; } private void LocalSwitchOff() { Sprite.SetState("Off"); isOn = false; } private void LocalSwitch() { if (isOn) { LocalSwitchOff(); } else { LocalSwitchOn(); } } public void SwitchOn() { LocalSwitchOn(); stopBlinking(); } public void SwitchOff() { LocalSwitchOff(); stopBlinking(); } public void Switch() { LocalSwitch(); stopBlinking(); } public void Blink(int nTimes) { SwitchOn(); nBlinkTimes = nTimes; isBlinking = true; blinkTimeout = 0; } public void stopBlinking() { isBlinking = false; nBlinkTimes = 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (isBlinking) { blinkTimeout++; if (blinkTimeout >= blinkingInterval) { blinkTimeout = 0; LocalSwitch(); nBlinkTimes--; if (nBlinkTimes == 0) { SwitchOff(); } } } }}public class MyCanvas { private bool[] Buffer; private int resX; private int resY; private int length; private MySprite[] DefaultFont; public MyCanvas(int resX, int resY) { this.resX = resX; this.resY = resY; length = resY * resX; Buffer = new bool[length]; this.DefaultFont = DefaultMonospaceFont.BitmapFont; } public bool[] GetBuffer() { return Buffer; } public int GetResX() { return resX; } public int GetResY() { return resY; } public bool[] GetBufferCopy() { return DrawingFrameworkUtils.CopyBoolArray(Buffer, false); } public void SetDefaultFont(MySprite[] DefaultFont) { this.DefaultFont = DefaultFont; } public MySprite[] GetDefaultFont() { return DefaultFont; } public MyCanvas WithDefaultFont(MySprite[] DefaultFont) { SetDefaultFont(DefaultFont); return this; } public void Clear(bool value = false) { for (int x = 0; x < length; x++) { Buffer[x] = value; } } private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) { if (invertColors) { if (transparentBackground) { return targetPixelValue && !sourcePixelValue; } else { return !sourcePixelValue; } } else { if (transparentBackground) { return sourcePixelValue || targetPixelValue; } else { return sourcePixelValue; } } } public void BitBlt(MySprite sprite, int x, int y) { BitBltExt(sprite, x, y, false, false); } public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) { if (x < 0 || y < 0) { return; } int screenPos = resX * y + x; int spriteLength = sprite.height * sprite.width; int spritePosX = 0; for (int spritePos = 0; spritePos < spriteLength; spritePos++) { try { Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground); screenPos++; } catch (Exception exc) { } if (screenPos >= length - 1) { return; } spritePosX++; if (spritePosX == sprite.width) { spritePosX = 0; screenPos += resX - sprite.width; } } } public void DrawText(int x, int y, String text) { DrawColorText(x, y, text, false, false); } public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) { if (DefaultFont == null || text == null) { return; } char[] textChars = text.ToCharArray(); int screenPosX = x; int prevSpacing = 7; foreach (char chr in textChars) { MySprite CharSprite = DefaultFont[chr]; if (CharSprite != null) { BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground); prevSpacing = CharSprite.width; } screenPosX += prevSpacing; if (screenPosX >= resX) { return; } } } public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) { int actualX1 = x1 > x2 ? x2 : x1; int actualY1 = y1 > y2 ? y2 : y1; int actualX2 = x1 > x2 ? x1 : x2; int actualY2 = y1 > y2 ? y1 : y2; if (actualX1 < 0) actualX1 = 0; if (actualY1 < 0) actualY1 = 0; if (actualX2 >= resX - 1) actualX2 = resX - 1; if (actualY2 >= resY - 1) actualY2 = resY - 1; int rectWidth = actualX2 - actualX1; int screenPosY = actualY1; while (screenPosY <= actualY2) { int screenPos = screenPosY * resX + actualX1; if (screenPos >= length) { return; } bool targetColor = !invertColors; Buffer[screenPos] = targetColor; Buffer[screenPos + rectWidth - 1] = targetColor; if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) { for (int innerPos = screenPos; innerPos < screenPos + rectWidth; innerPos++) { Buffer[innerPos] = targetColor; } } screenPos += resX; screenPosY++; } }}public class MyIconLabel : MyOnScreenObject { private MyStatefulAnimatedSprite AnimatedSprite; private MyTextLabel TextLabel; private int width; private int height; private int floatingIconPosition = Constants.FLOATING_POSITION_TOP; private int spacing = 3; public MyIconLabel(int x, int y, string text, MySprite[] Frames) :base(null, x, y, true){ if (text == null) { throw new ArgumentException("The text of the MyIconLabel must not be null"); } if (Frames == null || Frames.Length == 0) { throw new ArgumentException("There has to be at least one frame if the picture is to be displayed by the MyIconLabel"); } int frameWidth = Frames[0].width; int frameHeight = Frames[0].height; foreach (MySprite Frame in Frames) { if (Frame.width != frameWidth || Frame.height != frameHeight) { throw new ArgumentException("All the frames of the MyIconLabel must have the same width and height"); } } AnimatedSprite = new MyStatefulAnimatedSprite(0,0).WithState("Default", new MyStatefulAnimatedSpriteState(Frames)); AddChild(AnimatedSprite); TextLabel = new MyTextLabel(text, 0,0); AddChild(TextLabel); } public MyIconLabel WithFloatingIconPosition(int floatingIconPosition) { this.floatingIconPosition = floatingIconPosition; return this; } public MyIconLabel WithSpaceing(int spacing) { this.spacing = spacing; return this; } public override int GetHeight() { return height; } public override int GetWidth() { return width; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { int spriteWidth = AnimatedSprite.GetWidth(); int spriteHeight = AnimatedSprite.GetHeight(); int textWidth = TextLabel.GetWidth(); int textHeight = TextLabel.GetHeight(); if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { width = spriteWidth + spacing + textWidth; height = spriteHeight > textHeight ? spriteHeight : textHeight; } else { width = spriteWidth > textWidth ? spriteWidth : textWidth; height = spriteHeight + spacing + textHeight; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT) { AnimatedSprite.x = 0; TextLabel.x = spriteWidth + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { AnimatedSprite.x = width - spriteWidth; TextLabel.x = AnimatedSprite.x - spacing - textWidth; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP || floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { AnimatedSprite.x = (width - spriteWidth) / 2; TextLabel.x = (width - textWidth) / 2; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { TextLabel.y = (height - textHeight) / 2; AnimatedSprite.y = (height - spriteHeight) / 2; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP) { AnimatedSprite.y = 0; TextLabel.y = spriteHeight + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { TextLabel.y = 0; AnimatedSprite.y = textHeight + spacing; } }}public class MyList : MyOnScreenObject { private MyPanel Panel; private MyScrollbar Scrollbar; private MyPanel SelectionBackground; private MyOnScreenObject SelectedItem; private bool oneItemPerPage = false; private int selectedItemIndex; private int startPosY; private int padding = 2; private int horizontalAlignment = Constants.HORIZONTAL_ALIGN_LEFT; private List<MyOnScreenObject> Items = new List<MyOnScreenObject>(); public MyList(int x, int y, int width, int height) : base(null, x, y, true) { Panel = new MyPanel(0, 0, width, height); base.AddChild(Panel); Scrollbar = new MyScrollbar(Panel); SelectionBackground = new MyPanel(0, 0, width, 1).WithOptionalParameters(true, true, false); base.AddChild(SelectionBackground); } public MyList WithCustomScrollbarWidth(int scrollbarWidth) { Scrollbar.WithCustomWidth(scrollbarWidth); return this; } public MyList WithOneItemPerPage() { this.oneItemPerPage = true; return this; } public MyList WithPadding(int padding) { this.padding = padding; return this; } public MyList WithHorizontalAlignment(int horizontalAlignment) { this.horizontalAlignment = horizontalAlignment; return this; } public override void AddChild(MyOnScreenObject Item) { base.AddChild(Item); if (SelectedItem == null) { SetInitialSelectedItem(Item); UpdateScrollbarPosition(); } Items.Add(Item); UpdateItemPositions(); } public override void RemoveChild(MyOnScreenObject Item) { base.RemoveChild(Item); if (Item == SelectedItem) { SetInitialSelectedItem(ChildObjects[0]); UpdateScrollbarPosition(); } Items.Remove(Item); UpdateItemPositions(); } private void SetInitialSelectedItem(MyOnScreenObject Item) { SelectedItem = Item; selectedItemIndex = 0; startPosY = padding; } private int ComputeItemHorizontalPosition(MyOnScreenObject Item) { if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_RIGHT) { return GetWidth() - Scrollbar.GetWidth() - padding - Item.GetWidth(); } else if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_CENTER) { return (GetWidth() - Scrollbar.GetWidth() - Item.GetWidth()) / 2; } else { return padding; } } private void UpdateItemPositions() { if (Items.Count() == 0) { return; } if (oneItemPerPage) { foreach (MyOnScreenObject Item in Items) { if (Item == SelectedItem) { Item.isVisible = true; Item.x = ComputeItemHorizontalPosition(Item); Item.y = (Panel.GetHeight() - Item.GetHeight()) / 2; } else { Item.isVisible = false; } Item.invertColors = false; } SelectionBackground.isVisible = false; } else { int listMaxHeight = GetHeight() - (padding * 2); int selItemY = startPosY; for (int idx = 0 ; idx < selectedItemIndex ; idx++) { selItemY += Items[idx].GetHeight(); } if (selItemY < padding) { startPosY += padding - selItemY; } else if (selItemY + SelectedItem.GetHeight() > listMaxHeight) { startPosY -= selItemY + SelectedItem.GetHeight() - listMaxHeight; } int currPosY = startPosY; foreach (MyOnScreenObject Item in Items) { Item.y = currPosY; Item.x = ComputeItemHorizontalPosition(Item); currPosY += Item.GetHeight(); Item.isVisible = Item.y >= padding && Item.y + Item.GetHeight() <= listMaxHeight; Item.invertColors = Item == SelectedItem; } SelectionBackground.x = padding; SelectionBackground.y = SelectedItem.y; SelectionBackground.SetWidth(GetWidth() - Scrollbar.GetWidth() - (padding * 2)); SelectionBackground.SetHeight(SelectedItem.GetHeight()); SelectionBackground.isVisible = true; } } private void UpdateScrollbarPosition() { Scrollbar.SetPosPct(Items.Count() == 0 ? 0f : ((float)selectedItemIndex / ((float)Items.Count() - 1))); } public MyOnScreenObject SelectNextItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex >= 0 && selectedItemIndex < Items.Count() - 1) { selectedItemIndex++; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject SelectPreviousItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex > 0) { selectedItemIndex--; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject GetSelectedItem() { return SelectedItem; } public MyOnScreenObject SelectFirstItem() { SetInitialSelectedItem(Items[0]); return SelectedItem; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } public override int GetWidth() { return Panel.GetWidth(); } public override int GetHeight() { return Panel.GetHeight(); } public List<MyOnScreenObject> GetItems() { return Items; } protected override void Init() { UpdateItemPositions(); }}public class MyOnScreenApplication { private List<MyScreen> TargetScreens = new List<MyScreen>(); private List<MyPage> Pages = new List<MyPage>(); private MyPage CurrentPage; private MyCanvas Canvas; private int currIteration, nIterations; public MyOnScreenApplication() { currIteration = 0; nIterations = 1; } public MyOnScreenApplication WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyOnScreenApplication OnScreen(MyScreen TargetScreen) { return OnScreen(TargetScreen, 0, 0, Canvas.GetResX(), Canvas.GetResY()); } public MyOnScreenApplication OnScreen(MyScreen TargetScreen, int clipRectX1, int clipRectY1, int clipRectX2, int clipRectY2) { if (Canvas == null) { throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen()."); } TargetScreen.WithCanvas(Canvas); TargetScreen.WithClippedBuffer(clipRectX1, clipRectY1, clipRectX2, clipRectY2); TargetScreens.Add(TargetScreen); nIterations++; return this; } public MyOnScreenApplication WithDefaultPostPage(Func<MyOnScreenApplication,bool> initializationMonitoringFunction) { if (Pages.Count() > 0) { throw new InvalidOperationException("The POST page must be the first page ever added to the application"); } if (Canvas == null) { throw new InvalidOperationException("Please call WithCanvas() before calling WithDefaultPostPage()"); } if (initializationMonitoringFunction == null) { throw new ArgumentException("The initialization monitoring function must be a lambda taking in a MyOnScreenObject and returning a bool"); } MyPage POSTPage = (MyPage) new MyPage().WithClientCycleMethod((MyOnScreenObject obj) => { if (initializationMonitoringFunction(this)) { SwitchToPage(1); } return 0; }); this.AddPage(POSTPage); MyPanel Panel = new MyPanel(0, 0, Canvas.GetResX(), Canvas.GetResY()).WithOptionalParameters(true, true, false); POSTPage.AddChild(Panel); MyPanel TextBackgroundPanel = new MyPanel(0, 0, 2, 2).WithOptionalParameters(true, true, true); POSTPage.AddChild(TextBackgroundPanel); MyTextLabel TextLabel = new MyTextLabel("INITIALIZING", 1, 1).WithOptionalParameters(true, false, false); POSTPage.AddChild(TextLabel); int textLabelWidth = TextLabel.GetWidth(); int textLabelHeight = TextLabel.GetHeight(); TextLabel.x = (Canvas.GetResX() - textLabelWidth) / 2; TextLabel.y = (Canvas.GetResY() - textLabelHeight) / 2 - 3; TextBackgroundPanel.x = TextLabel.x - 3; TextBackgroundPanel.y = TextLabel.y - 2; TextBackgroundPanel.SetWidth(textLabelWidth + 6); TextBackgroundPanel.SetHeight(textLabelHeight + 3); POSTPage.AddChild( new MyPanel(TextBackgroundPanel.x, TextBackgroundPanel.y + TextBackgroundPanel.GetHeight() + 2, 7, 4) .WithOptionalParameters(true, true, true) .WithClientCycleMethod((MyOnScreenObject obj) => { obj.x++; if (obj.x > TextBackgroundPanel.x + TextBackgroundPanel.GetWidth() - 7) { obj.x = TextBackgroundPanel.x; } return 0; }) ); return this; } public void AddPage(MyPage Page) { Pages.Add(Page); Page.SetApplication(this); if (CurrentPage == null) { CurrentPage = Page; } } public void SwitchToPage(MyPage Page) { foreach (MyPage Pg in Pages) { if (Pg == Page) { CurrentPage = Pg; } } } public void SwitchToPage(int pageNumber) { if (pageNumber < 0 || pageNumber >= Pages.Count) { return; } CurrentPage = Pages[pageNumber]; } public MyPage GetCurrentPage() { return CurrentPage; } public void Cycle() { if (currIteration == 0) { Canvas.Clear(); CurrentPage.Cycle(Canvas); } else { TargetScreens[currIteration - 1].FlushBufferToScreen(CurrentPage.invertColors); } currIteration++; if (currIteration >= nIterations) { currIteration = 0; } } public MyCanvas GetCanvas() { return Canvas; }}public abstract class MyOnScreenObject { public int x; public int y; public bool isVisible = true; private bool _invertColors = false; public bool invertColors { get { return _invertColors; } set { _invertColors = value; foreach(MyOnScreenObject Child in ChildObjects) { Child.invertColors = value; } } } public MyOnScreenObject ParentObject; public List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>(); private Func<MyOnScreenObject, int> ClientCycleMethod; private Func<MyCanvas, int> ClientDrawMethod; private bool isObjectNotInitialized = true; public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) { this.SetParent(ParentObject); this.x = x; this.y = y; this.isVisible = isVisible; if (ParentObject != null) { ParentObject.AddChild(this); } } public MyOnScreenObject WithClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { SetClientCycleMethod(ClientCycleMethod); return this; } public void SetClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { this.ClientCycleMethod = ClientCycleMethod; } public MyOnScreenObject WithClientDrawMethod(Func<MyCanvas, int> ClientDrawMethod) { this.ClientDrawMethod = ClientDrawMethod; return this; } public virtual void AddChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Add(ChildObject); ChildObject.SetParent(this); } } public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) { if (ChildObject != null) { ChildObject.SetPosition(x, y); AddChild(ChildObject); } } public void SetPosition(int x, int y) { this.x = x; this.y = y; } public virtual void RemoveChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Remove(ChildObject); ChildObject.RemoveParent(); } } public virtual void SetParent(MyOnScreenObject ParentObject) { if (ParentObject != null) { this.ParentObject = ParentObject; } } public void RemoveParent() { this.ParentObject = null; } public MyOnScreenObject GetTopLevelParent() { if (ParentObject == null) { return this; } return ParentObject.GetTopLevelParent(); } public bool IsObjectVisible() { return isVisible && (ParentObject == null || ParentObject.IsObjectVisible()); } public virtual void Cycle(MyCanvas TargetCanvas) { Compute(TargetCanvas); if (ClientCycleMethod != null) { ClientCycleMethod(this); } foreach (MyOnScreenObject ChildObject in ChildObjects) { ChildObject.Cycle(TargetCanvas); } if (isObjectNotInitialized) { Init(); isObjectNotInitialized = true; } if (IsObjectVisible()) { if (ClientDrawMethod != null) { ClientDrawMethod(TargetCanvas); } Draw(TargetCanvas); } } public int GetAbsoluteX() { return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX()); } public int GetAbsoluteY() { return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY()); } protected abstract void Compute(MyCanvas TargetCanvas); protected abstract void Draw(MyCanvas TargetCanvas); public abstract int GetWidth(); public abstract int GetHeight(); protected abstract void Init();}public class MyPage : MyOnScreenObject { private MyOnScreenApplication OnScreenApplication; public MyPage() : base(null, 0, 0, true) { } public void SetApplication(MyOnScreenApplication OnScreenApplication) { this.OnScreenApplication = OnScreenApplication; } public MyPage WithInvertedColors() { this.invertColors = true; return this; } public MyOnScreenApplication GetApplication() { return OnScreenApplication; } public override int GetHeight() { return 0; } public override int GetWidth() { return 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { }}public class MyPanel : MyOnScreenObject { private int width; private int height; private bool isFilled = false; public MyPanel(int x, int y, int width, int height) : base(null, x, y, true) { this.width = width; this.height = height; this.isFilled = false; } public MyPanel WithOptionalParameters(bool isVisible, bool isFilled, bool invertColors) { this.isVisible = isVisible; this.isFilled = isFilled; this.invertColors = invertColors; return this; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int absoluteX = GetAbsoluteX(); int absoluteY = GetAbsoluteY(); TargetCanvas.DrawRect( absoluteX, absoluteY, absoluteX + width, absoluteY + height, invertColors, isFilled ); } public override int GetWidth() { return width; } public override int GetHeight() { return height; } public void SetWidth(int width) { this.width = width; } public void SetHeight(int height) { this.height = height; } protected override void Init() { }}public class MyScreen { private IMyTextPanel TargetLCD; private MyCanvas Canvas; private bool mirrorX; private char pixelValueOn, pixelValueOff; private int clipRectX1, clipRectY1, clipRectX2, clipRectY2; private bool isClipping = false; public MyScreen(IMyTextPanel TargetLCD, char pixelValueOn, char pixelValueOff, bool mirrorX) { this.TargetLCD = TargetLCD; this.mirrorX = mirrorX; this.pixelValueOn = pixelValueOn; this.pixelValueOff = pixelValueOff; } public MyScreen WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) { clipRectX1 = x1 > x2 ? x2 : x1; clipRectY1 = y1 > y2 ? y2 : y1; clipRectX2 = x1 > x2 ? x1 : x2; clipRectY2 = y1 > y2 ? y1 : y2; isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY(); return this; } private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) { int length = Buffer.Count(); bool[] MirroredBuffer = new bool[length]; int mirrorPosX = resX - 1; int mirrorPos = mirrorPosX; for (int sourcePos = 0; sourcePos < length; sourcePos++) { MirroredBuffer[mirrorPos] = Buffer[sourcePos]; mirrorPos--; mirrorPosX--; if (mirrorPosX == -1) { mirrorPosX = resX - 1; mirrorPos += resX * 2; } } return MirroredBuffer; } private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) { int rectX1 = x1 > x2 ? x2 : x1; int rectY1 = y1 > y2 ? y2 : y1; int rectX2 = x1 > x2 ? x1 : x2; int rectY2 = y1 > y2 ? y1 : y2; if (rectX1 < 0) rectX1 = 0; if (rectY1 < 0) rectY1 = 0; if (rectX2 > resX) rectX2 = resX; if (rectY2 > resY) rectY2 = resY; bool[] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1]; int srcCursor = rectY1 * resX + rectX1; int trgCursor = 0; for (int srcY = rectY1; srcY < rectY2; srcY++) { for (int srcX = rectX1; srcX < rectX2; srcX++) { ret[trgCursor] = Buffer[srcCursor]; ret[trgCursor] = true; trgCursor++; srcCursor++; } srcCursor += rectX1; } return ret; } public void FlushBufferToScreen(bool invertColors) { bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer(); int length = Buffer.Count(); int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX(); int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY(); bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer; StringBuilder renderedBuffer = new StringBuilder(length + resY + 1); char pxValOn = invertColors ? pixelValueOff : pixelValueOn; char pxValOff = invertColors ? pixelValueOn : pixelValueOff; int currXPos = 0; for (int idx = 0; idx < length; idx++) { renderedBuffer.Append(SourceBuffer[idx] ? pxValOn : pxValOff); currXPos++; if (currXPos == resX) { renderedBuffer.Append('\n'); currXPos = 0; } } TargetLCD.WriteText(renderedBuffer.ToString()); } public MyCanvas GetCanvas() { return Canvas; }}public class MyScrollbar : MyOnScreenObject { private int width = 7; private int height = 10; private float posPct = 0.5f; private bool snapToParent = true; public MyScrollbar(MyOnScreenObject ParentObject) : base(ParentObject, 0, 0, true) { } public MyScrollbar DetachedFromParent(int height) { this.snapToParent = false; this.height = height; return this; } public MyScrollbar WithCustomWidth(int width) { this.width = width; return this; } public MyScrollbar AtCoordinates(int x, int y) { this.x = x; this.y = y; return this; } public void SetPosPct(float posPct) { this.posPct = posPct; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int x1 = snapToParent ? ParentObject.GetAbsoluteX() + ResolveClientX() : GetAbsoluteX(); int y1 = snapToParent ? ParentObject.GetAbsoluteY() : GetAbsoluteY(); int x2 = x1 + width; int actualHeight = GetHeight(); int y2 = y1 + actualHeight; TargetCanvas.DrawRect(x1, y1, x2, y2, invertColors, false); int sliderX = x1 + 1; int sliderY = (int)(y1 + 1 + (posPct * ((actualHeight - 5 - 2)))); TargetCanvas.BitBltExt( StockSprites.SCROLLBAR_SLIDER, sliderX, sliderY, invertColors ? (!this.invertColors) : this.invertColors, false ); } private int ResolveHight() { if (ParentObject is MyPanel) { return ((MyPanel)ParentObject).GetHeight(); } return height; } private int ResolveClientX() { if (ParentObject is MyPanel) { return ParentObject.GetWidth() - this.width; } return 0; } public override int GetWidth() { return width; } public override int GetHeight() { return snapToParent ? ResolveHight() : height; } protected override void Init() { }}public class MySprite { public int width; public int height; public bool[] data; public MySprite(int width, int height, bool[] data) { this.width = width; this.height = height; this.data = data; }}public class MyStatefulAnimatedSprite : MyOnScreenObject { private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>(); private MyStatefulAnimatedSpriteState CurrentState; private bool isBackgroundTransparet = true; private MySprite CurrentFrame; public MyStatefulAnimatedSprite(int x, int y) : base(null, x, y, true) { } public MyStatefulAnimatedSprite WithState(string stateName, MyStatefulAnimatedSpriteState State) { if (stateName == null) { throw new ArgumentException("Each state must have a name"); } if (State == null) { throw new ArgumentException("The state may not be null"); } States.Add(stateName, State); if (CurrentState == null) { SetStateObject(State); } return this; } public void SetState(String stateName) { if (stateName == null) { return; } MyStatefulAnimatedSpriteState State; States.TryGetValue(stateName, out State); if (State != null) { SetStateObject(State); } } private void SetStateObject(MyStatefulAnimatedSpriteState State) { CurrentState = State; isBackgroundTransparet = State.IsBackgroundTransparent(); } protected override void Compute(MyCanvas TargetCanvas) { CurrentFrame = CurrentState.GetFrame(); } protected override void Draw(MyCanvas TargetCanvas) { TargetCanvas.BitBltExt( CurrentFrame, GetAbsoluteX(), GetAbsoluteY(), invertColors, CurrentState.IsBackgroundTransparent() ); } public override int GetWidth() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.width; } } public override int GetHeight() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.height; } } protected override void Init() { }}public class MyStatefulAnimatedSpriteState { private MySprite[] Frames; private int nFrames; private int currFrame = 0; private bool transparentBackground = true; public MyStatefulAnimatedSpriteState(MySprite[] Frames) { if (Frames == null || Frames.Count() == 0) { throw new ArgumentException("The frames array must have at least one frame"); } this.Frames = Frames; this.nFrames = Frames.Count(); } public bool IsBackgroundTransparent() { return transparentBackground; } public MyStatefulAnimatedSpriteState WithOpaqueBackground() { transparentBackground = false; return this; } public MySprite GetFrame() { MySprite ret = Frames[currFrame]; currFrame++; if (currFrame >= nFrames) { currFrame = 0; } return ret; }}public class MyTextLabel : MyOnScreenObject { private String text; private bool transparentBackground; private MySprite[] Font; private int padding = 1; public MyTextLabel(string text, int x, int y) : base(null, x, y, true) { this.text = text; this.transparentBackground = true; } public MyTextLabel WithOptionalParameters(bool isVisible, bool invertColors, bool transparentBackground) { this.isVisible = isVisible; this.invertColors = invertColors; this.transparentBackground = transparentBackground; return this; } public MyTextLabel WithCustomFont(MySprite[] CustomFont) { this.Font = CustomFont; return this; } public MyTextLabel WithPadding(int padding) { this.padding = padding; return this; } public void SetText(string text) { this.text = text; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (Font == null) { Font = TargetCanvas.GetDefaultFont(); } TargetCanvas.DrawColorText( GetAbsoluteX() + padding, GetAbsoluteY() + padding, text, invertColors, transparentBackground ); } private MySprite[] ResolveFont() { if (Font == null) { MyOnScreenObject TopLevelParent = GetTopLevelParent(); if (TopLevelParent is MyPage) { return ((MyPage) TopLevelParent).GetApplication().GetCanvas().GetDefaultFont(); } else { return null; } } else { return Font; } } public override int GetWidth() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].width * text.Length + (2 * padding); } public override int GetHeight() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].height + (2 * padding); } protected override void Init() { }}public class StockSprites { private const bool O = true; private const bool _ = false; public static MySprite SPRITE_PWR = new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0x02,0,0x1a,0xc0,0x22,0x20,0x22,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x1f,0xc0,0,0 })); public static MySprite SPRITE_UP = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_DOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_LEFTRIGHT = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0d,0x80,0x1d,0xc0,0x3d,0xe0,0x7d,0xf0,0x3d,0xe0,0x1d,0xc0,0x0d,0x80,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_UPDOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0,0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0 })), 14, 10, 0, 0); public static MySprite SPRITE_REVERSE = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0f,0xe0,0x10,0x10,0x7c,0x10,0x38,0x10,0x10,0x10,0,0x10,0x10,0x10,0x0f,0xe0 })), 14, 10, 0, 0); public static MySprite SCROLLBAR_SLIDER = new MySprite(5, 5, new bool[] { _,O,O,O,_, O,O,O,O,O, O,O,_,_,O, O,O,_,_,O, _,O,O,O,_ }); public static MySprite SPRITE_SMILE_SAD = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0x7e,0,0xc0,0,0xc1,0x81, 0x81,0x80,0,0x62,0,0x43,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_NEUTRAL = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0,0,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_HAPPY = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x82,0,0x40,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0x7e,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 }));}public class TerminalUtils { public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { if (blockName == null || blockName.Length == 0) { throw new ArgumentException("Invalid block name"); } List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>(); MyGridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks); if (allFoundBlocks == null || allFoundBlocks.Count == 0) { throw new ArgumentException("Cannot find a block named [" + blockName + "]"); } foreach (IMyTerminalBlock block in allFoundBlocks) { return (T)block; } return default(T); } public static void SetupTextPanelForMatrixDisplay( IMyGridTerminalSystem GridTerminalSystem, string textPanelName, float fontSize ) { IMyTextPanel TextPanel = TerminalUtils.FindFirstBlockWithName<IMyTextPanel>(GridTerminalSystem, textPanelName); if (TextPanel == null) { throw new ArgumentException("Cannot find a text panel named [" + textPanelName + "]"); } TextPanel.ContentType = ContentType.TEXT_AND_IMAGE; TextPanel.FontSize = fontSize; TextPanel.TextPadding = 0; TextPanel.SetValue<long>("Font", 1147350002); TextPanel.Alignment = TextAlignment.LEFT; }}public class UiFrameworkUtils { private const char SCREEN_PIXEL_VALUE_ON = '@'; private const char SCREEN_PIXEL_VALUE_OFF = ' '; public static MyOnScreenApplication InitSingleScreenApplication(IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int resX, int resY, bool mirrorX) { return new MyOnScreenApplication() .WithCanvas( new MyCanvas(resX, resY) ) .OnScreen( new MyScreen( TerminalUtils.FindFirstBlockWithName<IMyTextPanel>(MyGridTerminalSystem, textPanelName), SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF, mirrorX ) ); }
