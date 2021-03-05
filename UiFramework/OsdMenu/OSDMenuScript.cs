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



/* README //////////////////////////////////////////////////////////////////////////////////////////////////////////

This script creates a retro-looking menu that can display monochrome low resolution animated graphics within a
scrollable list. List options can take the form of icons, text labels or text labels with icons and they can be
set to activate sub-menus or to perform custom actions. Interaction with the menu is achieved by altering the
"Enabled" state of 4 blocks which support this property (i.e. lighting blocks, reactors, jump drives, etc).

NOTE: This script is meant as an example to explain how the OSD menu can be configured. Users may remove the current
      configuration and add their own configuration in its place.

To modify the menu to suit your needs, please update the configuration in the following sections:
    - CONFIG section          = sets the target text panel, display resolution and other display properties,
                                as well as the names of the blocks to be used as switches for navigating the menu
    - OSD MENU INITIALIZATION = defines the structure of the menu and custom code to be run when activating options
    - CUSTOM SPRITES section  = defines boolean arrays to be used as icons in the menu

For a working example, please visit the following workshop item:
    https://steamcommunity.com/sharedfiles/filedetails/?id=2415572447

This is a showcase of the UI Framework, which enables low resolution graphics on text panels by dumping the frame
buffer into a string, which is then set as the text property of a given text panel, which is set up to use the
Monospace font and very small font size. The UI framework is minified in this script. The development version can be
found here: https://github.com/fx89/spaceengineers/tree/main/UiFramework/UiFramework

Development and partial building is done using MDK-SE: https://github.com/malware-dev/MDK-SE

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/



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
// The above parameters will be applied automatically to the target text panel if the
// AUTO_APPLY_SCREEN_PARAMETERS is set to true. The default parameters will work
// well on a 1x1 display. For other types of text panels or for custom setup, such
// as displaying the menu on only the top half of the panel because the bottom half
// is covered by the console, the AUTO_APPLY_SCREEN_PARAMETERS parameter will have
// to be set to false and the panel settings will have to be adjusted manually.
private const bool AUTO_APPLY_SCREEN_PARAMETERS = true;
private const int  RESOLUTION_WIDTH             = 139;
private const int  RESOLUTION_HEIGHT            =  93;

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

      // Add the Grid Control option to the menu.
      // This will be represented by an animated icon with 9 frames (SPRITE_GRID_[00-08]).
      // Each frame is inserted 4 times, so that the animation appears to run 4 times slower.
      // The icon will be set on top of a text label pelling "Grid control"
      // This option will also have a sub-menu
        .WithIconOption(
            "Grid control",
            new MySprite[]{
                    SPRITE_GRID_00,SPRITE_GRID_00,SPRITE_GRID_00,SPRITE_GRID_00,
                    SPRITE_GRID_01,SPRITE_GRID_01,SPRITE_GRID_01,SPRITE_GRID_01,
                    SPRITE_GRID_02,SPRITE_GRID_02,SPRITE_GRID_02,SPRITE_GRID_02,
                    SPRITE_GRID_03,SPRITE_GRID_03,SPRITE_GRID_03,SPRITE_GRID_03,
                    SPRITE_GRID_04,SPRITE_GRID_04,SPRITE_GRID_04,SPRITE_GRID_04,
                    SPRITE_GRID_05,SPRITE_GRID_05,SPRITE_GRID_05,SPRITE_GRID_05,
                    SPRITE_GRID_06,SPRITE_GRID_06,SPRITE_GRID_06,SPRITE_GRID_06,
                    SPRITE_GRID_07,SPRITE_GRID_07,SPRITE_GRID_07,SPRITE_GRID_07,
                    SPRITE_GRID_08,SPRITE_GRID_08,SPRITE_GRID_08,SPRITE_GRID_08
                },
            Constants.FLOATING_POSITION_TOP
        )
          // The sub-menu has its own title. It can also have its own alignment. If the alignment
          // is not specified, then it defaults to LEFT, regardless of the alignment of the parent menu
            .WithSubMenu("> Grid control <")
              // This sub-menu item has an icon depicting a light bulb, defined by SPRITE_LIGHTS. The icon
              // is positioned to the left of the text, which reads "Lights".
                .WithIconOption("Lights", new MySprite[]{ SPRITE_LIGHTS }, Constants.FLOATING_POSITION_LEFT)
                    .WithSubMenu("> Lights <")
                        .WithTextOption("Turn On")
                            .WithAction(() => {
                             // Find all the lights and turn them on
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(null, (Light) => {
                                    Light.Enabled = true;
                                return false;});
                            })
                        .WithTextOption("Turn Off")
                            .WithAction(() => {
                             // Find all the lights and turn them off
                                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(null, (Light) => {
                                    Light.Enabled = false;
                                return false;});
                            })
                    .EndSubMenu()
              // The above code is lengthy and makes it hard to follow the menu.
              // To make it easier to follow, one can make use of the WithAggregatedOptions() method.
              // The WithAggregatedOptions method allows grouping multiple repetitive actions, such as
              // creating "Turn On" and "Turn Off" options and their similarly looking implementations.
                .WithTextOption("Reactors").WithAggregatedOptions(CreateOnOffOptions<IMyReactor>(">Reactors<"))
                .WithTextOption("Doors").WithAggregatedOptions(CreateOnOffOptions<IMyDoor>(">Doors<"))
                .WithTextOption("Gravity").WithAggregatedOptions(CreateOnOffOptions<IMyGravityGenerator>(">Gravity<"))
                .WithTextOption("Gyroscopes").WithAggregatedOptions(CreateOnOffOptions<IMyGyro>(">Gyroscopes<"))
                .WithTextOption("Turrets").WithAggregatedOptions(CreateOnOffOptions<IMyLargeTurretBase>(">Turrets<"))
                .WithTextOption("Jump drives").WithAggregatedOptions(CreateOnOffOptions<IMyJumpDrive>(">Jump drives<"))
                .WithTextOption("Thrusters").WithAggregatedOptions(CreateOnOffOptions<IMyThrust>(">Thrusters<"))
                .WithTextOption("Beacons").WithAggregatedOptions(CreateOnOffOptions<IMyBeacon>(">Beacons<"))
                .WithTextOption("Rotors").WithAggregatedOptions(CreateOnOffOptions<IMyMotorRotor>(">Rotors<"))
                .WithTextOption("Pistons").WithAggregatedOptions(CreateOnOffOptions<IMyPistonBase>(">Pistons<"))
            .EndSubMenu()

      // Menu options may be set up to navigate to a different page
        .WithIconOption(
            "Smile Screen saver",
            new MySprite[]{
                StockSprites.SPRITE_SMILE_HAPPY,
                StockSprites.SPRITE_SMILE_NEUTRAL,
                StockSprites.SPRITE_SMILE_SAD,
                StockSprites.SPRITE_SMILE_NEUTRAL
            }
            , Constants.FLOATING_POSITION_TOP
         ).WithAction(() => {
            OnScreenApplication.SwitchToPage(2);
         })

      // Add another option to go back to the POST menu. This will be a rotating
      // arrow pointing left, given by the frames SPRITE_BACK_ARROW_*, which are
      // initialized in the CUSTOM SPRITES section. The POST menu is page 0, as
      // it is the first page added to the application.
        .WithIconOption(
            "Back to POST page",
            new MySprite[]{
                SPRITE_BACK_ARROW_00,
                SPRITE_BACK_ARROW_01,
                SPRITE_BACK_ARROW_02,
                SPRITE_BACK_ARROW_03,
                SPRITE_BACK_ARROW_02,
                SPRITE_BACK_ARROW_01
            },
            Constants.FLOATING_POSITION_TOP
         )
            .WithAction(() => {
                currFrame = 0;
                OnScreenApplication.SwitchToPage(0);
             })

        .WithIconOption("Shut down", new MySprite[]{SPRITE_SHUTDOWN}, Constants.FLOATING_POSITION_TOP)
            .WithAction(() => {
             // Find all the reactors and shut them down
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(null, (Reactor) => {
                    Reactor.Enabled = false;
                return false;});
            })

      // End of the menu definition
        .End();
}

/**
 * This method creates a lambda which adds a submenu with the "Turn On"
 * and "Turn Off" options to the selected menu option. These options apply
 * to all functional blocks of the given type (T).
 */
private Action<MyOsdMenu> CreateOnOffOptions<T>(String submenuTitle) {
    return (MyOsdMenu Menu) => {
        Menu
            .WithSubMenu(submenuTitle)
                .WithTextOption("Turn On")
                    .WithAction(() => {
                        List<IMyFunctionalBlock> Blocks = new List<IMyFunctionalBlock>();
                        GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(Blocks);
                        foreach (IMyFunctionalBlock Block in Blocks) {
                            if (Block is T) {
                                Block.Enabled = true;
                            }
                        }
                    })
                .WithTextOption("Turn Off")
                    .WithAction(() => {
                        List<IMyFunctionalBlock> Blocks = new List<IMyFunctionalBlock>();
                        GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(Blocks);
                        foreach (IMyFunctionalBlock Block in Blocks) {
                            if (Block is T) {
                                Block.Enabled = false;
                            }
                        }
                    })
            .EndSubMenu();
    };
}


// Movement vector for the smiley sprite defined below
private int vecX = 2, vecY = -1;



private void InitOtherStuff() {
 // Create the smiley face screen saver page and add it to the application
    MyPage ScreensaverPage = new MyPage();
    OnScreenApplication.AddPage(ScreensaverPage);

 // Create the smiley sprite and add it to the screensaver page
 // The sprite will have one state (named Default) with 4 frames
 // The sprite will also have a client cycle method, which will
 // move it around the screen
    ScreensaverPage.AddChild(new MyStatefulAnimatedSprite(10, 15)
        .WithState("Default", new MyStatefulAnimatedSpriteState(new MySprite[]{
            StockSprites.SPRITE_SMILE_HAPPY,
            StockSprites.SPRITE_SMILE_NEUTRAL,
            StockSprites.SPRITE_SMILE_SAD,
            StockSprites.SPRITE_SMILE_NEUTRAL
        }))
        .WithClientCycleMethod((MyOnScreenObject Sprt) => {
         // Apply the movement vector to the sprite
            Sprt.x += vecX;
            Sprt.y += vecY;

         // If the sprite has reached the left or right margin, then reverse the horizontal coordinate of the movement vector
            if (Sprt.x <= 0 || Sprt.x + Sprt.GetWidth() >= RESOLUTION_WIDTH - 1) {
                vecX = -vecX;
            }

         // If the sprite has reached the top margin, then reverse the vertical coordinate of the movement vector
            if (Sprt.y <= 0 || Sprt.y + Sprt.GetHeight() >= RESOLUTION_HEIGHT - 1) {
                vecY = -vecY;
            }

         // Go back to the OSD menu if the switch is turned one
            MonitorSwitch(CURSOR_OUT_SWITCH_STATUS_BLOCK_NAME, () => {
                if (OnScreenApplication.GetCurrentPage() == ScreensaverPage) {
                    OnScreenApplication.SwitchToPage(1);
                }
            });
        return 0;})
    );
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CUSTOM SPRITES //////////////////////////////////////////////////////////////////////////////////////////////////

// This is required for defining the SPRITE_LIGHTS below
private const bool _ = false, O = true;


// This is a copy of a stock sprite, except its colors are inverted.
// The constructor takes 3 parameters: the width, the height and the data.
// The width and the height are copied over rom SPRITE_PWR.
// The data (which is an array of booleans) is first inverted by using the utility method NegateBoolArray()
MySprite SPRITE_PWR_ON = new MySprite(
    StockSprites.SPRITE_PWR.width,
    StockSprites.SPRITE_PWR.height,
    DrawingFrameworkUtils.NegateBoolArray(StockSprites.SPRITE_PWR.data)
);



// The following sprites will be initialized inside the InitSprites[1-4]() methods.
// This is necessary to avoid the "script too complex" error raised by the game if
// the script executes too many operations in one single loop. These operations are
// part of the for loops that have to do with parsing the bytes given below and
// turning them into boolean arrays.
private static MySprite SPRITE_SHUTDOWN;
private static MySprite SPRITE_LIGHTS;
private static MySprite SPRITE_GRID_00, SPRITE_GRID_01, SPRITE_GRID_02, SPRITE_GRID_03, SPRITE_GRID_04, SPRITE_GRID_05, SPRITE_GRID_06, SPRITE_GRID_07, SPRITE_GRID_08;
private static MySprite SPRITE_BACK_ARROW_00, SPRITE_BACK_ARROW_01, SPRITE_BACK_ARROW_02, SPRITE_BACK_ARROW_03;



/**
 * The InitSprites[1-4]() methods are called from the first 4 loops,
 * each loop calling one method at a time. This helps spread the
 * workload of loading sprites across multiple initialization steps,
 * to avoid "Script too Complex" errors.
 */
private void InitSprites1() {
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
SPRITE_SHUTDOWN = new MySprite(64, 36, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
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

//////////////////////////////////////////////////

// Smaller sprites may be easier to define directly in code. This can be done like this:
SPRITE_LIGHTS = new MySprite(9, 7, new bool[]{
_,_,_,O,O,O,_,_,_,
_,_,O,O,O,O,O,_,_,
_,_,O,O,O,O,O,_,_,
_,_,_,O,O,O,_,_,_,
_,_,_,_,_,_,_,_,_,
_,_,_,O,O,O,_,_,_,
_,_,_,_,O,_,_,_,_
});
// The above method makes the script longer than it has to. For small icons,
// this is ok, but larger ones might make the script exceed 100 000 characters,
// which is the current limit of the game.
}

//////////////////////////////////////////////////

private void InitSprites2() {
SPRITE_GRID_00 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0x3f,255,255,0xe0,0,
0,0x20,0xc2,0x0c,0x30,0,0,0x21,0x83,0x0c,0x30,0,0,0x7d,0xf7,0xbf,
0xb8,0,0,0x77,0xdf,0xdf,0x70,0,0,0x41,0x82,0x0c,0x18,0,0,0xc1,
0x83,0x06,0x18,0,0,0xf7,0xd7,0xaf,0x58,0,0,255,255,255,0xf8,0,
0,0xc3,0x87,0x06,0x0c,0,0x01,0x83,0x02,0x06,0x0c,0,0x01,0x83,0x83,0x06,
0x0c,0,0x01,255,255,255,0xfc,0,0x01,255,255,255,0xfe,0,0x03,0x87,
0x07,0x07,0x06,0,0x03,0x03,0x03,0x03,0x06,0,0x07,0x07,0x07,0x07,0x06,0,
0x03,0x07,0x07,0x07,0x07,0,0x07,255,255,255,255,0,0x07,255,255,255,
255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_01 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0x3f,0x80,0,0,0x02,255,0xf8,0xc0,0,
0,255,0x0c,0x30,0xc0,0,0,0x83,0x0c,0x3f,0xe0,0,0,0x83,0x2f,0xfe,
0xe0,0,0,0xef,255,0x9c,0x30,0,0,255,0x86,0x0c,0x30,0,0,0xc3,
0x06,0x1f,0xf8,0,0,0x83,0x8f,255,0xf8,0,0,0xd7,255,0xef,0x0c,0,
0,255,0xd3,0x06,0x0c,0,0,0xe3,0x83,0x07,0x1e,0,0,0x81,0x83,0x9f,
255,0,0,0x81,0xcf,255,0xf7,0,0,0xd7,255,0xeb,0x83,0x80,0,255,
0xf9,0xc1,0x81,0x80,0,0xf1,0xc1,0xc1,0xc3,0xc0,0x01,0xc1,0xc0,0xc1,255,0xc0,
0,0xc0,0xc1,255,255,0xc0,0,0xc1,255,255,0xd0,0,0,0xef,255,0xe8,
0,0,0x01,255,0xf8,0,0,0,0,0xf8,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_02 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0x1c,0,0,0,0,0x03,0xe6,0,0,0,0,0x3d,0xc3,0,0,
0,0x0f,0xf0,0xe7,0x80,0,0,0xfc,0x30,0xfd,0xc0,0,0x03,0x0c,0x3f,0xf8,
0x60,0,0x02,0x0e,0xfe,0x38,0xe0,0,0x03,0x1f,0x8c,0x1f,0xf0,0,0x03,255,
0x0e,0x3f,0x98,0,0x01,0x83,0x0f,0xfe,0x0c,0,0x01,0x83,0xbf,0x87,0x0e,0,
0x01,0x87,0xf7,0x83,0xbf,0,0,255,0xc1,0x87,255,0,0,0xf1,0xc1,255,
0xe1,0x80,0,0xe0,0xc3,0xfd,0xc1,0xc0,0,0xc0,255,0xf0,0xe1,0xe0,0,0x43,
0xfe,0xe0,0x77,0xe0,0,0x7f,0xf0,0x70,0x7f,0x80,0,0x7f,0x70,0x3b,0xfc,0,
0,0x38,0x30,0x7f,0xc0,0,0,0x30,0x3d,0xfe,0,0,0,0x30,0x3f,0xe0,
0,0,0,0x38,255,0,0,0,0,0x1f,0xf8,0,0,0,0,0x1f,
0xc0,0,0,0,0,0x1c,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_03 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0x01,0xf0,0,0,0,0,0x0f,0x18,0,0,0,0,0x7f,0x1c,0,0,
0,0x03,0xc3,255,0,0,0,0x1d,0xc3,0xe1,0x80,0,0,0xf8,0x7f,0xe1,
0xc0,0,0x03,0xf0,0xfc,0x7f,0xf0,0,0x1e,0x1f,0xf8,0x3e,0x38,0,0x0c,0x1f,
0x1c,0xfe,0x1c,0,0x06,0x7e,0x0f,0xc7,0x3e,0,0x07,0xe7,0x1f,0x87,0xfb,0x80,
0x03,0x83,255,0x83,0xe1,0xc0,0x03,0x83,0xe1,0xcf,0xe0,0xe0,0x01,0x8f,0xe0,0xfc,
0x71,0xe0,0,0xfe,0xe1,0xf8,0x3f,0xc0,0,0x70,0x7f,0xf8,0x3f,0,0,0x70,
0x7f,0x1c,0x3c,0,0,0x30,0x7e,0x0f,0xf0,0,0,0x39,0xfe,0x0f,0xc0,0,
0,0x1f,0xcf,0x1f,0,0,0,0x0f,0x07,0x7c,0,0,0,0x06,0x03,0xf8,
0,0,0,0x07,0x07,0xe0,0,0,0,0x03,0x9f,0x80,0,0,0,0x03,
0xfe,0,0,0,0,0x01,0xf8,0,0,0,0,0,0xe0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_04 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0x02,0,0,0,0,0,
0x0f,0x80,0,0,0,0,0x38,0xe0,0,0,0,0,0xf8,0xf8,0,0,
0,0x03,0x1f,0x8e,0,0,0,0x0f,0x0f,0x87,0x80,0,0,0x3f,0x9d,0xce,
0xe0,0,0,0xe1,0xf8,0xfc,0x38,0,0x01,0xe1,0xf0,0xfc,0x3e,0,0x06,0xf3,
0xbd,0xee,0x77,0x80,0x1c,0x3e,0x1f,0x87,0xe1,0xc0,0x38,0x3e,0x0f,0x83,0xe0,0xe0,
0x1c,255,0x3f,0xc7,0xf9,0x80,0x0f,0xc3,0xf8,0xfe,0x3f,0x80,0x07,0x83,0xf0,0x7c,
0x1e,0,0x03,0x83,0xf0,0x7e,0x1c,0,0x01,0xef,0x3d,0xef,0xb8,0,0,0x7e,
0x1f,0xc3,0xf0,0,0,0x3c,0x0f,0x81,0xe0,0,0,0x1c,0x1f,0xc3,0x80,0,
0,0x0e,0x7d,0xe7,0x80,0,0,0x07,0xf0,0xfe,0,0,0,0x03,0xe0,0x7c,
0,0,0,0x01,0xe0,0x78,0,0,0,0,0x70,0xf0,0,0,0,0,
0x3d,0xe0,0,0,0,0,0x1f,0xc0,0,0,0,0,0x0f,0,0,0,
0,0,0x06,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_05 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0x10,0,0,0,0,0,
0x3e,0,0,0,0,0,0xc7,0x80,0,0,0,0x01,0xc7,0xf0,0,0,
0,0x07,0xfe,0x1e,0,0,0,0x0c,0x3e,0x1b,0xc0,0,0,0x1c,0x3f,0xf8,
0xf0,0,0,0x3f,0xf1,0xf0,0x7f,0,0,0xc3,0xe1,0xfd,0xc3,0xc0,0x01,0xc3,
0xf9,0x87,0xc1,0x80,0x03,0xc7,0x3f,0x87,0xf3,0,0x0e,0xfe,0x1f,0x87,0x7f,0,
0x18,0x3e,0x0f,0xfe,0x1e,0,0x78,0x7f,0xb8,0x7e,0x0c,0,0x78,0x63,0xf8,0x3f,
0x1c,0,0x1f,0xe1,0xf8,0x7b,0xf8,0,0x07,0xc1,0xfe,0xe0,0xf0,0,0x01,0xe3,
0x8f,0xe0,0x60,0,0,255,0x87,0xf0,0xe0,0,0,0x3f,0x03,0xfd,0xc0,0,
0,0x0f,0x87,0x3f,0x80,0,0,0x03,0xee,0x0f,0x80,0,0,0,0xfe,0x07,
0,0,0,0,0x3e,0x0e,0,0,0,0,0x1f,0x0e,0,0,0,0,
0x07,0xfc,0,0,0,0,0x01,0xf8,0,0,0,0,0,0x70,0,0,
0,0,0,0x10,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_06 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x01,
0xe0,0,0,0,0,0x03,0x3e,0,0,0,0,0x06,0x1d,0xf0,0,0,
0,0x07,0x38,0x7f,0,0,0,0x1d,0xfc,0x61,0xf8,0,0,0x10,0x7f,0xe1,
0x9f,0,0,0x38,0xe3,0xfb,0x82,0,0,0x7f,0xc1,0xbf,0x86,0,0,0xcf,
0xf3,0x07,0xfe,0,0x01,0x83,255,0x86,0x3c,0,0x03,0x87,0x1f,0xee,0x0c,0,
0x07,0xfe,0x0e,0xfe,0x0c,0,0x0e,255,0x1c,0x3f,0xf8,0,0x18,0x3f,0xfc,0x1d,
0xf8,0,0x38,0x39,0xfc,0x38,0x38,0,0x78,0x70,0x7f,0xf0,0x30,0,0x3f,0xe0,
0x77,0xf8,0x30,0,0x07,0xf0,0xe0,255,0xf0,0,0,0xfd,0xc0,0xef,0xe0,0,
0,0x1f,0xe0,0xc1,0xe0,0,0,0x03,0xf9,0xc0,0xc0,0,0,0,0x7f,0xc0,
0xc0,0,0,0,0x0f,0xe1,0xc0,0,0,0,0x01,255,0x80,0,0,0,
0,0x3f,0x80,0,0,0,0,0x07,0x80,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_07 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0x0f,0xe0,0,0,0,0,0x08,0x7f,0xfa,0,0,
0,0x18,0x61,0x97,0xf8,0,0,0x1e,0xe1,0x82,0x18,0,0,0x37,255,0xc6,
0x08,0,0,0x61,0xcf,255,0x58,0,0,0x61,0x83,0x0f,0xfc,0,0,255,
0xc3,0x04,0x18,0,0,255,255,0x0c,0x08,0,0x01,0x87,0xbf,255,0x18,0,
0x03,0x07,0x0e,255,0xf8,0,0x03,0xce,0x0c,0x1c,0x78,0,0x07,255,0x0c,0x0c,
0x18,0,0x06,0x7f,255,0x1c,0x18,0,0x0c,0x1d,255,0xfe,0x18,0,0x1c,0x18,
0x3d,255,0xf8,0,0x1c,0x38,0x18,0x3f,0xf8,0,0x3f,0xf8,0x38,0x38,0x38,0,
0x0f,255,0x78,0x30,0x18,0,0,0x3f,255,0x78,0x30,0,0,0,255,0xfc,
0x38,0,0,0,0x01,255,0xf0,0,0,0,0,0x07,0xf8,0,0,0,
0,0,0x20,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));

SPRITE_GRID_08 = new MySprite(48, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0x1f,255,255,0xf0,0,
0,0x30,0xc3,0x0e,0x30,0,0,0x30,0xc3,0x0c,0x10,0,0,0x3f,255,0xdf,
0x78,0,0,0x6b,0xdf,255,0xf8,0,0,0x41,0x83,0x0e,0x18,0,0,0xe1,
0x83,0x06,0x18,0,0,255,255,0xef,0x78,0,0,255,255,255,0xf8,0,
0,0x83,0x87,0x0e,0x1c,0,0x01,0x83,0x02,0x06,0x0c,0,0x01,0xc3,0x87,0x06,
0x0c,0,0x01,255,255,255,0xfc,0,0x03,255,255,255,0xfe,0,0x03,0x07,
0x07,0x0f,0xae,0,0x03,0x07,0x06,0x06,0x06,0,0x07,0x06,0x06,0x06,0x06,0,
0x07,0xaf,0x0f,0x07,0x06,0,0x07,255,255,255,0xfe,0,0x07,255,255,255,
255,0,0,0,0,0,0x0a,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0
}));
}

//////////////////////////////////////////////////

private void InitSprites3() {
SPRITE_BACK_ARROW_00 = new MySprite(104, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0x1f,0xf0,0,0,0,0,0,0,0,
0,0,0,0,0x7f,0xe0,0,0,0,0,0,0,0,0,0,0,
0,0x7f,0xf0,0,0,0,0,0,0,0,0,0,0,0x01,255,0xf0,
0,0,0,0,0,0,0,0,0,0,0x03,0xf0,0xf0,0,0,0,
0,0,0,0,0,0,0,0x07,0xe0,0x70,0,0,0,0,0,0,
0,0,0,0,0x0f,0x80,0x7d,0xb6,0xdb,0x6d,0xb6,0xdb,0x68,0,0,0,
0,0x1f,0,0x7f,255,255,255,255,255,0xfc,0,0,0,0,0x3e,0,
0x7f,255,255,255,255,255,0xfc,0,0,0,0,0x7c,0,0,0,0,
0,0,0,0x7c,0,0,0,0,0xf0,0,0,0,0,0,0,0,
0x3c,0,0,0,0x01,0xe0,0,0,0,0,0,0,0,0x3c,0,0,
0,0x03,0xc0,0,0,0,0,0,0,0,0x3c,0,0,0,0x07,0x80,
0,0,0,0,0,0,0,0x3c,0,0,0,0x07,0x80,0,0,0,
0,0,0,0,0x3c,0,0,0,0x07,0xc0,0,0,0,0,0,0,
0,0x3c,0,0,0,0x03,0xe0,0,0,0,0,0,0,0,0x3c,0,
0,0,0x01,0xf0,0,0,0,0,0,0,0,0x3c,0,0,0,0,
0xf8,0,0,0,0,0,0,0,0x3c,0,0,0,0,0x3e,0,0x7f,
255,255,255,255,255,0xf8,0,0,0,0,0x3f,0,0x7f,255,255,255,
255,255,0xfc,0,0,0,0,0x0f,0x80,0x7f,255,255,255,255,255,0xfc,
0,0,0,0,0x07,0xc0,0x70,0,0,0,0,0,0,0,0,0,
0,0x03,0xf0,0x70,0,0,0,0,0,0,0,0,0,0,0x01,0xfd,
0xf0,0,0,0,0,0,0,0,0,0,0,0,255,0xf0,0,0,
0,0,0,0,0,0,0,0,0,0x7f,0xf0,0,0,0,0,0,
0,0,0,0,0,0,0x3f,0xf0,0,0,0,0,0,0,0,0,
0,0,0,0x1f,0xe0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0
}));

SPRITE_BACK_ARROW_01 = new MySprite(104, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0x05,0x50,0,0,0,0,0,0,0,0,0,0,
0,0x1f,0xf0,0,0,0,0,0,0,0,0,0,0,0,0x3f,0xf0,
0,0,0,0,0,0,0,0,0,0,0,255,0xf0,0,0,0,
0,0,0,0,0,0,0,0x01,0xf8,0x70,0,0,0,0,0,0,
0,0,0,0,0x03,0xe0,0x78,0,0,0,0,0,0,0,0,0,
0,0x0f,0xc0,0x7f,255,255,255,255,255,0xf8,0,0,0,0,0x1f,0,
0x7f,255,255,255,255,255,0xf8,0,0,0,0,0x7e,0,0x12,0x49,0x24,
0x92,0x49,0x24,0xf8,0,0,0,0,0xf8,0,0,0,0,0,0,0,
0x38,0,0,0,0x01,0xf0,0,0,0,0,0,0,0,0x38,0,0,
0,0x07,0xc0,0,0,0,0,0,0,0,0x38,0,0,0,0x07,0x80,
0,0,0,0,0,0,0,0x3c,0,0,0,0x07,0x80,0,0,0,
0,0,0,0,0x3c,0,0,0,0x03,0xc0,0,0,0,0,0,0,
0,0x3c,0,0,0,0x03,0xe0,0,0,0,0,0,0,0,0x1c,0,
0,0,0x01,0xf0,0,0,0,0,0,0,0,0x3c,0,0,0,0,
0x78,0,0,0,0,0,0,0,0x3e,0,0,0,0,0x7e,0,255,
255,255,255,255,255,0xfc,0,0,0,0,0x1f,0,0x7f,255,255,255,
255,255,0xfe,0,0,0,0,0x0f,0x80,255,255,255,255,255,255,0xfc,
0,0,0,0,0x07,0xe0,0xf0,0,0,0,0,0,0,0,0,0,
0,0x03,0xf0,0xe0,0,0,0,0,0,0,0,0,0,0,0x01,255,
0xe0,0,0,0,0,0,0,0,0,0,0,0,255,0xe0,0,0,
0,0,0,0,0,0,0,0,0,0x7f,0xe0,0,0,0,0,0,
0,0,0,0,0,0,0x3f,0xe0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0
}));

SPRITE_BACK_ARROW_02 = new MySprite(104, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x05,
0xf0,0,0,0,0,0,0,0,0,0,0,0,0xbe,0xf8,0,0,
0,0,0,0,0,0,0,0,0x0b,0xe0,0x7f,255,255,255,255,255,
0xf0,0,0,0,0,0x7f,0,0,0,0,0,0,0,0xf8,0,0,
0,0x07,0xf0,0,0,0,0,0,0,0,0x3c,0,0,0,0x07,0xc0,
0,0,0,0,0,0,0,0x3c,0,0,0,0x07,0x80,0,0,0,
0,0,0,0,0x1e,0,0,0,0x07,0xe0,0,0,0,0,0,0,
0,0x1e,0,0,0,0x03,0xf8,0,0,0,0,0,0,0,0x3f,0,
0,0,0,0x7f,0,255,255,255,255,255,255,255,0,0,0,0,
0x1f,0xc1,0xf5,0x55,0x55,0x55,0x55,0x55,0x54,0,0,0,0,0x07,255,0xe0,
0,0,0,0,0,0,0,0,0,0,0x01,255,0xe0,0,0,0,
0,0,0,0,0,0,0,0,0x04,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0
}));
SPRITE_BACK_ARROW_03 = new MySprite(104, 37, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0x0a,0,0,0,0x07,0xa0,
0,0,0,0,0,0,0,0x3e,0,0,0,0x07,255,255,255,255,
255,255,255,255,255,0,0,0,0x02,0xaa,0,0,0,0,0,0,
0,0x1e,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0
}));
}

//////////////////////////////////////////////////

private void InitSprites4() {

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SCRIPT INITIALIZATION ///////////////////////////////////////////////////////////////////////////////////////////

/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *      > adding pages
  *      > adding components to the pages
  *      > linking logic and animations
  */
private void InitApplication() {
 // Get a reference to the target panel or crash
    IMyTerminalBlock TextPanel = GridTerminalSystem.GetBlockWithName(TEXT_PANEL_NAME);
    if (TextPanel == null || !(TextPanel is IMyTextPanel)) {
        throw new ArgumentException("Could not find a text panel named [" + TEXT_PANEL_NAME + "]");
   }

 // Apply the settings if so configured
    if (AUTO_APPLY_SCREEN_PARAMETERS) {
        IMyTextPanel TPanel = (IMyTextPanel) TextPanel;
        TPanel.ContentType = ContentType.TEXT_AND_IMAGE;
        TPanel.SetValue<long>("Font", 1147350002);
        TPanel.TextPadding = 0;
        TPanel.Alignment = TextAlignment.LEFT;
        TPanel.FontSize = 0.190f;
    }

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

 // This is where other initialization, such as adding new pages, will happem
    InitOtherStuff();
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

}}