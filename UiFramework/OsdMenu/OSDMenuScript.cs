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
        .WithTextOption("Grid Control")
          // The sub-menu has its own title. It can also have its own alignment. If the alignment
          // is not specified, then it defaults to LEFT, regardless of the alignment of the parent menu
            .WithSubMenu("> Grid control <").WithCustomHorizontalAlignment(Constants.HORIZONTAL_ALIGN_RIGHT)
                .WithTextOption("Lights")
                    .WithSubMenu("> Lights <")
                        .WithTextOption("Turn on")
                            .WithAction(() => {
                             // Find all the lights and turn them on
                                List<IMyLightingBlock> Lights = new List<IMyLightingBlock>();
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(Lights);
                                foreach(IMyLightingBlock Light in Lights) { Light.Enabled = true; }
                            })
                        .WithTextOption("Turn off")
                            .WithAction(() => {
                             // Find all the lights and turn them off
                                List<IMyLightingBlock> Lights = new List<IMyLightingBlock>();
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(Lights);
                                foreach(IMyLightingBlock Light in Lights) { Light.Enabled = false; }
                            })
                    .EndSubMenu()
                .WithTextOption("Sub option 2")
                .WithTextOption("Sub option 3")
            .EndSubMenu()

      // Add an icon of an animated smiley face with text underneath as a menu option
        .WithIconOption("Smile", new MySprite[]{StockSprites.SPRITE_SMILE_HAPPY, StockSprites.SPRITE_SMILE_NEUTRAL, StockSprites.SPRITE_SMILE_SAD, StockSprites.SPRITE_SMILE_NEUTRAL}, Constants.FLOATING_POSITION_TOP)

      // Add a picture option
        .WithPictureOption(new MySprite[]{StockSprites.SPRITE_LEFTRIGHT})

      // Add a text option which switches the application back to the POST page,
      // while also resetting the frame counter, so that the POST page stays on
      // for another X frames
        .WithTextOption("Back to POST page")
            .WithAction(() => {
                currFrame = 0;
                OnScreenApplication.SwitchToPage(0);
             })
        .End();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CUSTOM SPRITES //////////////////////////////////////////////////////////////////////////////////////////////////

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