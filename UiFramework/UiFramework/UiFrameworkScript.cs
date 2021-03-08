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

The UI Framework enables low resolution graphics on text panels by dumping the frame buffer into a string,
which is then set as the text property of a given text panel, which is set up to use the Monospace font and
very small font size. The UI framework is minified in this script. The development version can be found here:
    https://github.com/fx89/spaceengineers/tree/main/UiFramework/UiFramework

For a working example, please visit the following workshop item:
    https://steamcommunity.com/sharedfiles/filedetails/?id=2415572447

Development and partial building is done using MDK-SE: https://github.com/malware-dev/MDK-SE

*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// COMMON PARAMETERS USED BY THE FRAMEWORK /////////////////////////////////////////////////////////////////////////

/* IMPORTANT NOTE:
 * ==========================================================================================
 * If the resolution is set too high, the script will crash with a "Script too Complex" error.
 * A possible workaround would be to split the rendering across multiple iterations, but this
 * would just affect the already low frame rate.
 */
private const string TARGET_BLOCK_NAME     = "Text Panel"; // The name of the LCD panel or control seat where the image will be rendered
private const int    SURFACE_INDEX         = 0;            // Control seats and other similar blocks may have multiple display panels. The first one is 0, the second one is 1, and so on.
private const float  PIXEL_SIZE            = 0.190f;       // The font size to set for the target text surface
private const int    RES_X                 = 139;          // Depending on the font size, more or less pixels will fit horizontally
private const int    RES_Y                 = 93;           // Depending on the font size, ore or less pixels will fit vertically
private const bool   MIRROR_X_AXIS         = false;        // If a transparent screen is placed the other way around, the image will have to be mirrored
private const int    POST_SCREEN_DURATION  = 100;          // Set this to 0 to disable the POST screen. Its purpose is mainly to test that the set font size and resolution produce an image of the desired size

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// UI FRAMEWORK INIT STEPS /////////////////////////////////////////////////////////////////////////////////////////

/**
 * This method is called after the InitSprites*() steps.
 * This is where pages and other on-screen objects can be defined.
 */
private void InitPages(MyOnScreenApplication OnScreenApplication) {
    
}

/**
 * The InitSprites[1-7]() methods are called from the first 7 loops,
 * each loop calling one method. This helps spread the workload of
 * loading sprites across multiple initialization steps, to avoid
 * "script too complex" errors.
 */
private void InitSprites1() {
    
}
private void InitSprites2() {

}
private void InitSprites3() {

}
private void InitSprites4() {

}
private void InitSprites5() {

}
private void InitSprites6() {

}
private void InitSprites7() {

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// BOILERPLATING ///////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

int currFrame = 0;


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
 
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(
        GridTerminalSystem, TARGET_BLOCK_NAME, SURFACE_INDEX, // The target panel
        RES_X, RES_Y,                                         // The target resolution
        MIRROR_X_AXIS                                         // Rendering option
    )
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after 100 frames
            currFrame++;
            return currFrame >= POST_SCREEN_DURATION;
        });

    // Add more pages
       InitPages(OnScreenApplication);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * This is here mainly so that it can be used to write stuff to the console
 * from other classes which would otherwise have no access to this functionality.
 */
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

/**
 * To avoid the "Script too Complex" error, the initialization
 * must be split across multiple iterations
 */
private int initStepNbr = 0;

public void Main(string argument, UpdateType updateSource) {
 // Initialize the script. Do it in steps, to avoid "script too complex" errors
 // caused by loading too many pictures in one single frame.
    if (initStepNbr < 8) {
        initStepNbr++;
        if (initStepNbr == 1) InitSprites1();
        if (initStepNbr == 2) InitSprites2();
        if (initStepNbr == 3) InitSprites3();
        if (initStepNbr == 4) InitSprites4();
        if (initStepNbr == 5) InitSprites5();
        if (initStepNbr == 6) InitSprites6();
        if (initStepNbr == 7) InitSprites7();
        if (initStepNbr == 8) InitApplication();
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