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

This script makes it rain pixels on a 1x1 LCD panel. The name of the panel, as well as other configuration,
can be set in the CONFIGURATION section below. To make it work on control stations and other kinds of screens,
one must adjust the resolution, pixel size and surface index.

The development version of the script can be found here:
    https://github.com/fx89/spaceengineers/tree/main/UiFramework/MatrixScreenSaver

For a working example, please visit the following workshop item:
    https://steamcommunity.com/sharedfiles/filedetails/?id=2415572447

Development and partial building is done using MDK-SE: https://github.com/malware-dev/MDK-SE

*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIGURATION ///////////////////////////////////////////////////////////////////////////////////////////////////

// Name of the block to which to draw the screen saver. This can be an LCD panel,
// control seat or anything with a text surface.
private const string TARGET_BLOCK_NAME = "MATRIX_SCREENSAVER_SCREEN";

// Control seats and other similar blocks may have multiple display panels.
// The first one is 0, the second one is 1, and so on.
private const int SURFACE_INDEX = 0;

// The font size to set for the target text surface
private const float PIXEL_SIZE = 0.190f;

// Adjust the resoluton to fit the image to the target text surface
private const int RES_X = 139;
private const int RES_Y =  93;

// false = white foreground on black background
// true  = black foreground on white background
private static bool INVERT_COLORS = false;

// For how many frames should the script display the "INITIALIZING" message on screen
private const int POST_SCREEN_DURATION = 145;

// Increase this to have the screen light up more
// Decrease it to have the screen light up less
//
// IMPORTANT NOTE: 
//     --- if this number is too large, or if it's less than 0, the script will crash
//
private const int NUMBER_OF_DROPLETS = 500;

private const int MAX_VERTICAL_SKEW = 120;

// These are the minimum and maximum speeds at which the droplets will drop
private const int MIN_SPEED = 1;
private const int MAX_SPEED = 4;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SCRIPT INTERNALS ////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * This is a list of templates for the graphics of the matrix column sprites
 * which will be initialized in the InitMainPage() method.
 */
private List<MyStatefulAnimatedSprite> MatrixColumnSpriteTemplates
    = new List<MyStatefulAnimatedSprite>();



/**
 * This is where the matrix column sprite enters the screen
 */
private const int INITIAL_TOP_POSITION = -10;

/**
 * This is where the matrix column sprite exists the screen
 * and goes back to the top to enter again
 */
private const int BOTTOM_MARGIN = RES_Y;


/**
 * The matrix column sprites will move once every X frames
 */
private const int STEP_DURATION_FRAMES = 2;



/**
 * Random number generator to randomize the droplet template
 */
private static Random RND = new Random();

/**
 * Initializes the main page and constructs instances of the matrix column sprites
 * according to the configured parameters
 */
private MyPage InitMainPage() {
 // Initialize the main page
    MyPage MainPage = new MyPage();

 // Create and add the droplets to the main page
    for (int s = 0 ; s < NUMBER_OF_DROPLETS ; s++) {
        MyMatrixColumnSprite Sprite = CreateMatrixColumnSprite(MainPage, MAX_VERTICAL_SKEW);
        MainPage.AddChild(Sprite);
    }

 // Return a reference to the main page
    return MainPage;
}

/**
 * Create te sprite at a random horizontal position, from a random template and with a random speed
 */
private MyMatrixColumnSprite CreateMatrixColumnSprite(MyPage MainPage, int maxVerticalSkew) {
    return new MyMatrixColumnSprite(RND.Next(0, RES_X), INITIAL_TOP_POSITION - RND.Next(0, maxVerticalSkew))
        .FromTemplate(MatrixColumnSpriteTemplates[RND.Next(0, MatrixColumnSpriteTemplates.Count - 1)])
        .WithSpeed(RND.Next(MIN_SPEED, MAX_SPEED))
    ;
}



private class MyMatrixColumnSprite : MyStatefulAnimatedSprite {
    private int speed;

    private int stepIndex = 0;

    public MyMatrixColumnSprite(int x, int y) : base(x, y) {}

/**
  * Re-use the template state, to avoid re-initialization of the buffer,
  * which requires additional operations which, summed up, may lead to
  * a "Script too Complex" error
  */
    public  MyMatrixColumnSprite FromTemplate(MyStatefulAnimatedSprite Template) {
        WithState("Default", Template.GetState("Default"));
    return this; }

/**
  * Set the speed at which the sprite will drop
  */
    public MyMatrixColumnSprite WithSpeed(int speed) {
        this.speed = speed;
        return this;
    }

/**
 *  This drops the sprite at the configured speed
 */
    protected override void Compute(MyCanvas TargetCanvas, int currFrameIndex) {
        base.Compute(TargetCanvas, currFrameIndex);

        stepIndex++;
        if (stepIndex == STEP_DURATION_FRAMES) {
            y += speed;
            if (y + GetHeight() > BOTTOM_MARGIN) {
                y = INITIAL_TOP_POSITION;
            }

            stepIndex = 0;
        }
        
    }
}



/**
 * These are helpers, to make the sprite defiinitions look more inteligible
 */
private const bool O = true, _ = false;



/**
 * Initializes the graphics used by the matrix column sprites
 */
private void InitMatrixColumnSpriteTemplates() {
    MatrixColumnSpriteTemplates.Add(
        new MyStatefulAnimatedSprite(0, 0)
            .WithState("Default", new MyStatefulAnimatedSpriteState(new MySprite[]{
                new MySprite(1, 10, new bool[]{
                    _,O,_,_,O,_,O,_,O,O
                })
            }))
    );

    MatrixColumnSpriteTemplates.Add(
        new MyStatefulAnimatedSprite(0, 0)
            .WithState("Default", new MyStatefulAnimatedSpriteState(new MySprite[]{
                new MySprite(1, 10, new bool[]{
                    _,_,O,_,O,_,O,O,O,O
                })
            }))
    );

    MatrixColumnSpriteTemplates.Add(
        new MyStatefulAnimatedSprite(0, 0)
            .WithState("Default", new MyStatefulAnimatedSpriteState(new MySprite[]{
                new MySprite(1, 10, new bool[]{
                    O,_,O,_,O,O,O,_,O,O
                })
            }))
    );
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// BOILERPLATING ///////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

int currFrame = 0;

/**
 * The InitSprites[1-4]() methods are called from the first 4 loops,
 * each loop calling one method. This helps spread the workload of
 * loading sprites across multiple initialization steps, to avoid
 * "script too complex" errors.
 */
private void InitSprites1() {
    InitMatrixColumnSpriteTemplates();
}
private void InitSprites2() {

}
private void InitSprites3() {

}
private void InitSprites4() {

}

/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *      > adding pages
  *      > adding components to the pages
  *      > linking logic and animations
  */
private void InitApplication() {
 // Set up the target screen
    TerminalUtils.SetupTextSurfaceForMatrixDisplay(GridTerminalSystem, TARGET_BLOCK_NAME, SURFACE_INDEX, PIXEL_SIZE);

 // Initialize the application
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, TARGET_BLOCK_NAME, SURFACE_INDEX, RES_X, RES_Y, false)
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after the configured number of frames
            currFrame++;
            return currFrame >= POST_SCREEN_DURATION;
        });

 // Create the main page and add it to the application
    MyPage MainPage = InitMainPage();
    if (INVERT_COLORS) {
        MainPage.WithInvertedColors();
    }
    OnScreenApplication.AddPage(MainPage);
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
        OnScreenApplication.Cycle();
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}}