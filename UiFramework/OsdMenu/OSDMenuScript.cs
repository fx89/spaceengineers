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

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIG //////////////////////////////////////////////////////////////////////////////////////////////////////////

// The name of the text panel where the OSD menu will be displayed
private const string TEXT_PANEL_NAME = "VIDEOWALL_PANEL_2x2";

// The display resolution (if it's too large, the script will crash with a "Script too Complex" error)
// These values work with a text panel having the following properties:
//      CONTENT      = TEXT AND IMAGES
//      FONT         = MONOSPACE
//      TEXT PADDING = 0.0%
//      ALIGNMENT    = LEFT
//      FONT SIZE    = 0.190
private const int RESOLUTION_WIDTH  = 139;
private const int RESOLUTION_HEIGHT =  93;

// Set this to TRUE in case you placed a transparent text panel the other way around
private const bool IS_SCREEN_MIRRORED_HORIZONTALLY = false;

// The number of frames after which the POST page disappears
private const int POST_PAGE_DURATION = 100;

// Set to TRUE for black foreground on white background
// Set to FALSE for white foreground on black background
// The white is actually the color that's set in the properties the text panel
private const bool IS_OSD_MENU_INVERTED = true;

// The names of the control switch status blocks:
//     > The script looks for the status of these switches (On / Off).
//       If the switch is on, then the proper action is executed and
//       the switch is automatically turned off immediately after.
//     > The actions are:
//             - CURSOR_DOWN - selects the next item in the list
//             - CURSOR_UP   - selects the previous item in the list
//             - CURSOR_IN   - activates the selected item, triggering
//                             an action or navigating to the sub-menu,
//                             depending on how the OSD menu was initialized
//             - CURSOR_OUT  - goes back to the parent menu, but only if a
//                             sub-menu is currently displayed
//     > These switch status blocks can be of any type as long as they ca be
//       turned on or off. It is preferable that they are lighting blocks,
//       so that they may help identify script errors. If an error occurs,
//       the switch status block is not turned off automatically (because of
//       the crash) and, in the case of a lighting block, the light will remain
//       on, so that it's easy to spot.
private const string CURSOR_DOWN_SWITCH_STATUS_BLOCK_NAME = "Console Switch 1";
private const string CURSOR_UP_SWITCH_STATUS_BLOCK_NAME   = "Console Switch 2";
private const string CURSOR_IN_SWITCH_STATUS_BLOCK_NAME   = "Console Switch 3";
private const string CURSOR_OUT_SWITCH_STATUS_BLOCK_NAME  = "Console Switch 4";

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * Counter for the number of frames after which the POST page should disappear
 */
private int currFrame = 0;

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// OSD MENU INITIALIZATION /////////////////////////////////////////////////////////////////////////////////////////

private MyOsdMenu InitOsdMenu() {
    return new MyOsdMenu("Main Menu", RESOLUTION_WIDTH, RESOLUTION_HEIGHT)
        .WithCustomHorizontalAlignment(Constants.HORIZONTAL_ALIGN_CENTER) // alginment of the main menu
        .WithOneItemPerPage() // the main menu will display just one item per page - this does not affect any of the sub-menus

      // Add an option to the menu - this option has a sub-menu
        .WithIconOption("Grid control", new MySprite[]{SPRITE_GRID}, Constants.FLOATING_POSITION_TOP)
          // The sub-menu has its own title. It can also have its own alignment. If the alignment
          // is not specified, then it defaults to LEFT, regardless of the alignment of the parent menu
            .WithSubMenu("> Grid control <")
                .WithTextOption("Lights")
                    .WithSubMenu("> Lights <")
                        .WithTextOption("Turn on")
                            .WithAction(() => {
                             // Find all the lights and turn them on
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(null, (Light) => {
                                    Light.Enabled = true;
                                return false;});
                            })
                        .WithTextOption("Turn off")
                            .WithAction(() => {
                             // Find all the lights and turn them off
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(null, (Light) => {
                                    Light.Enabled = false;
                                return false;});
                            })
                    .EndSubMenu()
                .WithTextOption("Sub option 2")
                .WithTextOption("Sub option 3")
            .EndSubMenu()

      // Menu options may be set up to navigate to a different page
        .WithIconOption(
            "Smile Screen saver",
            new MySprite[]{StockSprites.SPRITE_SMILE_HAPPY,StockSprites.SPRITE_SMILE_NEUTRAL,StockSprites.SPRITE_SMILE_SAD,StockSprites.SPRITE_SMILE_NEUTRAL }
            , Constants.FLOATING_POSITION_TOP
         ).WithAction(() => {
            OnScreenApplication.SwitchToPage(2);
         })

      // Add a text option which switches the application back to the POST page,
      // while also resetting the frame counter, so that the POST page stays on
      // for another X frames
        .WithTextOption("Back to POST page")
            .WithAction(() => {
                currFrame = 0;
                OnScreenApplication.SwitchToPage(0);
             })

        .WithIconOption("Shut down", new MySprite[]{SPRITE_SHUTDOWN}, Constants.FLOATING_POSITION_TOP)
            .WithAction(() => {
             // Find all the reactors and shut them down
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(null, (Reactor) => {
                    Reactor.Enabled = true;
                return false;});
            })

      // End of the menu definition
        .End();

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CUSTOM SPRITES //////////////////////////////////////////////////////////////////////////////////////////////////

private const bool _ = false, O = true;

// Sprites, especially larger ones, should be drawn using graphics software. Then they can be converted using any tool
// that can turn images into raw 1 bit per pixel byte arrays.
//
// For example: https://javl.github.io/image2cpp/
//     > Here it's best to use monochrome bmp files, which can be created using Windows Paint, amongts others
//     > The following options should be applied on the site, under the Output section:
//               - Code output format = plain bytes
//               - Draw mode: Horizontal - 1 bit per pixel
//
// The conversion result can be put in the script as follows. One may replace 0x00 with 0 and 0xff with 255 to save
// some space.
// 
// If the image appears garbled, please increase the width property (first parameter of the MySprite constructor) until
// the pixels line up.
//
private static MySprite SPRITE_SHUTDOWN = new MySprite(64, 36, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0x1f,0xf8,0,0,0,0,0,0x03,255,255,0xc0,0,0,
0,0,0x1f,255,255,0xf8,0,0,0,0,255,0xf0,0x47,255,0,0,
0,0x01,0xfc,0,0,0x3f,0x80,0,0,0x07,0xf8,0x82,0x08,0x27,0xe0,0,
0,0x0f,0xc0,0,0,0x03,0xf0,0,0,0x3f,0x04,0x10,0x41,0x04,0xfc,0,
0,0x3e,0,0,0,0,0x7c,0,0,0x7e,0x20,0x01,0xc0,0x20,0x9e,0,
0,0x78,0,0x71,0xc6,0,0x1f,0x80,0x01,0xf1,0,0xf1,0xc7,0x04,0x1f,0x80,
0x01,0xf0,0x03,0xf1,0xc7,0xc0,0x0f,0x80,0x01,0xf8,0x27,0xe1,0xc7,0xe0,0x8f,0xc0,
0xab,0xc0,0x07,0x81,0xc1,0xe0,0x03,0xea,0x57,0xc1,0x1f,0x01,0xc0,0xf8,0x13,0xd5,
0xab,0xc0,0x1f,0,0,0xf8,0x03,0xea,0x57,0xc8,0x1f,0,0,0xf8,0x83,0xd5,
0xab,0xc0,0x1f,0,0,0xf8,0x03,0xea,0x57,0xc1,0x1f,0,0,0xf8,0x13,0xd5,
0xab,0xc0,0x07,0x80,0x01,0xe0,0x03,0xea,0x01,0xf8,0x27,0xe0,0x07,0xe0,0x8f,0xc0,
0x01,0xf0,0x03,0xf0,0x0f,0xc0,0x0f,0x80,0x01,0xf1,0x04,255,255,0x04,0x1f,0x80,
0x01,0xf8,0,0x7f,0xfe,0,0x1f,0x80,0,0x78,0x20,0x0f,0xf8,0x20,0x9e,0,
0,0x3e,0,0,0,0,0x7c,0,0,0x3f,0x04,0x10,0x41,0x04,0xfc,0,
0,0x0f,0xc0,0,0,0x03,0xf0,0,0,0x07,0xe0,0x82,0x08,0x27,0xe0,0,
0,0x01,0xfc,0,0,0x3f,0x80,0,0,0,255,0xf0,0x47,255,0,0,
0,0,0x1f,255,255,0xf8,0,0,0,0,0x07,255,255,0xe0,0,0,
0,0,0,0x7f,0xfe,0,0,0
}));

private static MySprite SPRITE_GRID = new MySprite(104, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0xfc,255,255,255,255,255,255,255,255,0xc1,255,255,0xc0,0xcc,255,255,
255,255,255,255,255,255,0xc7,255,255,0xf0,0xcc,255,255,255,255,255,
255,255,255,0xc7,255,255,0xf0,0xcc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,
0xc7,255,255,0xf0,0xcc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc1,255,255,
0xc0,0xcc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc0,0,0,0,0xcc,0xfe,
0,0x30,0,0xe0,0x01,0x80,0x0f,0xc1,255,255,0xc0,0xfc,0xfe,0,0x30,0,
0xe0,0x01,0x80,0x0f,0xc7,255,255,0xf0,0,255,255,255,255,255,255,255,
255,0xc7,255,255,0xf0,0xfc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc7,255,
255,0xf0,0xfc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc1,255,255,0xc0,0,
0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc0,0,0,0,0xfc,0xfe,0,0x30,
0,0xe0,0x01,0x80,0x0f,0xc1,255,255,0xc0,0,0xfe,0,0x30,0,0xe0,0x01,
0x80,0x0f,0xc7,255,255,0xf0,0x30,255,255,255,255,255,255,255,255,0xc7,
255,255,0xf0,0,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc7,255,255,0xf0,
0xfc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc1,255,255,0xc0,0,0xfe,0,
0x30,0,0xe0,0x01,0x80,0x0f,0xc0,0,0,0,0xfc,0xfe,0,0x30,0,0xe0,
0x01,0x80,0x0f,0xc1,255,255,0xc0,0xfc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,
0xc7,255,255,0xf0,0,255,255,255,255,255,255,255,255,0xc7,255,255,
0xf0,0xfc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc7,255,255,0xf0,0xcc,0xfe,
0,0x30,0,0xe0,0x01,0x80,0x0f,0xc1,255,255,0xc0,0xcc,0xfe,0,0x30,0,
0xe0,0x01,0x80,0x0f,0xc0,0,0,0,0xcc,0xfe,0,0x30,0,0xe0,0x01,0x80,
0x0f,0xc1,255,255,0xc0,0xcc,0xfe,0,0x30,0,0xe0,0x01,0x80,0x0f,0xc7,255,
255,0xf0,0xcc,255,255,255,255,255,255,255,255,0xc7,255,255,0xf0,0xcc,
255,255,255,255,255,255,255,255,0xc7,255,255,0xf0,0xfc,255,255,255,
255,255,255,255,255,0xc1,255,255,0xc0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0xfe,0x1f,0xc3,0xf0,0xfe,0x1f,0x83,0xf0,0xfe,
0x1f,0x83,0xf0,0x03,0xf8,0x7e,0x0f,0xc3,0xf8,0x7e,0x0f,0xc3,0xf8,0x7e,0x1f,0xc0,
0x0f,0xe1,0xf8,0x3f,0x0f,0xe1,0xf8,0x7f,0x0f,0xc1,0xf8,0x7f,0,0x3f,0x87,0xe1,
0xfc,0x3f,0x07,0xe1,0xfc,0x3f,0x07,0xe1,0xfc,0,0xfc,0x1f,0x87,0xf0,0xfc,0x1f,
0x87,0xf0,0xfc,0x3f,0x87,0xf0,0,255,255,255,255,255,255,255,255,255,
255,255,255,0xf0,255,255,255,255,255,255,255,255,255,255,255,255,
0xf0
}));

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SCRIPT INITIALIZATION ///////////////////////////////////////////////////////////////////////////////////////////

/**
    * This is called from the constructor of the program.
    * It should be used for initializing the application.
    *      > adding pages
    *      > adding components to the pages
    *      > linking logic and animations
    */
private void Init() {
 // Initialize the application, giving it a canvass and one or more screens to display it on.
 // Also add the default POST page to the application. The default POST page is an optional
 // built-in page which can be included by calling the WithDefaultPostPage() method.
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, TEXT_PANEL_NAME, RESOLUTION_WIDTH, RESOLUTION_HEIGHT, IS_SCREEN_MIRRORED_HORIZONTALLY)
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after POST_PAGE_DURATION frames
            currFrame++;
            return currFrame >= POST_PAGE_DURATION;
        });

 // Create the main page
    MyPage MainPage = IS_OSD_MENU_INVERTED ? new MyPage().WithInvertedColors() : new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Create the OSD menu
    MyOsdMenu OsdMenu = InitOsdMenu();

 // Add the OSD menu to the main page
    MainPage.AddChild(OsdMenu);

 // Link the states of the lights to the actions of the OSD menu
    MainPage.WithClientCycleMethod((MyOnScreenObject obj) => {
        MonitorSwitch(CURSOR_DOWN_SWITCH_STATUS_BLOCK_NAME, () => { OsdMenu.CursorDown(); });
        MonitorSwitch(CURSOR_UP_SWITCH_STATUS_BLOCK_NAME  , () => { OsdMenu.CursorUp  (); });
        MonitorSwitch(CURSOR_IN_SWITCH_STATUS_BLOCK_NAME  , () => { OsdMenu.CursorIn  (); });
        MonitorSwitch(CURSOR_OUT_SWITCH_STATUS_BLOCK_NAME , () => { OsdMenu.CursorOut (); });
        return 1;
    });
}

private void MonitorSwitch(String switchName, Action SwitchAction) {
    IMyLightingBlock Switch = TerminalUtils.FindFirstBlockWithName<IMyLightingBlock>(GridTerminalSystem, switchName);
    if(Switch.Enabled) {
        SwitchAction();
        Switch.Enabled = false;
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static Program PROGRAM;

public Program() {
 // Set the update speed in milliseconds
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

 // Get a reference to SELF, for debugging from other contexts
    PROGRAM = this;

 // Initialize the script
    Init();
}

public void Save() {
    // There is no state to be saved
}

public void Main(string argument, UpdateType updateSource) {
    OnScreenApplication.Cycle();
}





}}