using IngameScript.drawing_framework;
using IngameScript.drawing_framework.sprites;
using IngameScript.terminal_utils;
using IngameScript.ui_framework;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using MySprite = IngameScript.drawing_framework.MySprite;

namespace IngameScript { partial class Program : MyGridProgram {

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





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






////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}}