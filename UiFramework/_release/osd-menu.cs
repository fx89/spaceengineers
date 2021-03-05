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
private const string TEXT_PANEL_NAME = "OSD_MENU_SCREEN";

// The display resolution (if it's too large, the script will crash with a "Script too Complex" error)
// These values work with a text panel having the following properties:
//	  CONTENT      = TEXT AND IMAGES
//	  FONT         = MONOSPACE
//	  TEXT PADDING = 0.0%
//	  ALIGNMENT	   = LEFT
//	  FONT SIZE	   = 0.190
// The above parameters will be applied automatically to the target text panel if the
// AUTO_APPLY_SCREEN_PARAMETERS is set to true. The default parameters will work
// well on a 1x1 display. For other types of text panels or for custom setup, such
// as displaying the menu on only the top half of the panel because the bottom half
// is covered by the console, the AUTO_APPLY_SCREEN_PARAMETERS parameter will have
// to be set to false and the panel settings will have to be adjusted manually.
private const bool AUTO_APPLY_SCREEN_PARAMETERS = true;
private const int  RESOLUTION_WIDTH	            = 139;
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
//	 > The script looks for the status of these switches (On / Off).
//	   If the switch is on, then the proper action is executed and
//	   the switch is automatically turned off immediately after.
//	 > The actions are:
//			 - CURSOR_DOWN - selects the next item in the list
//			 - CURSOR_UP   - selects the previous item in the list
//			 - CURSOR_IN   - activates the selected item, triggering
//							 an action or navigating to the sub-menu,
//							 depending on how the OSD menu was initialized
//			 - CURSOR_OUT  - goes back to the parent menu, but only if a
//							 sub-menu is currently displayed
//	 > These switch status blocks can be of any type as long as they ca be
//	   turned on or off. It is preferable that they are lighting blocks,
//	   so that they may help identify script errors. If an error occurs,
//	   the switch status block is not turned off automatically (because of
//	   the crash) and, in the case of a lighting block, the light will remain
//	   on, so that it's easy to spot.
private const string CURSOR_DOWN_SWITCH_STATUS_BLOCK_NAME = "OSD MENU SWITCH 1";
private const string CURSOR_UP_SWITCH_STATUS_BLOCK_NAME   = "OSD MENU SWITCH 2";
private const string CURSOR_IN_SWITCH_STATUS_BLOCK_NAME   = "OSD MENU SWITCH 3";
private const string CURSOR_OUT_SWITCH_STATUS_BLOCK_NAME  = "OSD MENU SWITCH 4";

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
//	 > Here it's best to use monochrome bmp files, which can be created using Windows Paint, amongts others
//	 > The following options should be applied on the site, under the Output section:
//			   - Code output format = plain bytes
//			   - Draw mode: Horizontal - 1 bit per pixel
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
_,_,_,O,O,O,_,_,_
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



// OSD MENU CLASS //////////////////////////////////////////////////////////////////////////////////////////////////

class MyOsdMenu : MyOnScreenObject {
	private const int TITLE_ROW_HEIGHT  = 14;
	private const int PAGE_PADDING	  =  3;
	private const int BOTTOM_ROW_HEIGHT = 17;
	private const int TITLE_LABEL_POS_Y =  4;

	private int resX, resY;

  /**
	* The title label will be displayed on top of the menu
	*/
	private MyTextLabel TitleLabel;

  /**
	* Turns TRUE when the End() method is called.
	* Ensures that the CurrentMenu is set to the TopLevelMenu and that
	* the CurrentOption is set to the first option of te TopLevelMenu.
	*/
	private bool isConstructed = false;

  /**
	* This is the list that the OSD menu will display initially
	*/
	private MyList TopLevelMenu;

  /**
	* This is the list currently being displayed. Upon clicking options with mapped sub-menus,
	* this property will point to the list representing the submenu mapped to the clicked option.
	*/
	private MyList CurrentMenu;

   /**
	 * Required for construction purposes (i.e. knowing to which key to map a sub-menu)
	 */
	 private MyOnScreenObject CurrentOption;

   /**
	 * Required for construction (i.e. when switching back from the sub-menu to the parent menu)
	 */
	 private Dictionary<MyList, MyOnScreenObject> MenuCurrentOptionsDictionary = new Dictionary<MyList, MyOnScreenObject>();

  /**
	* Required for navigation (i.e. when the back button is clicked, the previous list must be displayed)
	*/
	private Dictionary<MyList, MyList> ParentMenusDictionary = new Dictionary<MyList, MyList>();

  /**
	* Required for navigation (i.e. when an option is clicked, the list mapped to it appears)
	*/
	private Dictionary<MyOnScreenObject, MyList> SubmenusDictionary = new Dictionary<MyOnScreenObject, MyList>();

  /**
	* When an option is clicked, if there's no submenu assigned to it in the SubmenusDictionary,
	* then the assigned action is executed (if any).
	*/
	private Dictionary<MyOnScreenObject, Action> ActionsDictionary = new Dictionary<MyOnScreenObject, Action>();

	private Dictionary<MyList, string> MenuTitlesDictionary = new Dictionary<MyList, string>();

	private MyBlinkingIcon UpArrow, DownArrow, InArrow, OutArrow;

	public MyOsdMenu(string title, int resX, int resY) : base(null, 0, 0, true) {
		if (title == null) {
			throw new ArgumentException("The title must not be null");
		}

		if (resX <=0 || resY <= 0) {
			throw new ArgumentException("Invalid resolution");
		}

		this.resX = resX;
		this.resY = resY;

		TopLevelMenu = CreateNewInvisibleList();
		MenuTitlesDictionary[TopLevelMenu] = title;
		TopLevelMenu.isVisible = true;
		AddChild(TopLevelMenu);
		CurrentMenu = TopLevelMenu;

		TitleLabel = new MyTextLabel(title, 3, TITLE_LABEL_POS_Y);
		AddChild(TitleLabel);

		int bottomIconsLeftMargin = resX / 2 - (StockSprites.SPRITE_UP.width * 2);
		int bottomIconsY = resY - BOTTOM_ROW_HEIGHT;

		DownArrow = new MyBlinkingIcon(bottomIconsLeftMargin , bottomIconsY, StockSprites.SPRITE_DOWN);
		UpArrow   = new MyBlinkingIcon(bottomIconsLeftMargin + StockSprites.SPRITE_UP.width , bottomIconsY, StockSprites.SPRITE_UP);
		InArrow   = new MyBlinkingIcon(bottomIconsLeftMargin + (StockSprites.SPRITE_UP.width * 2), bottomIconsY, StockSprites.SPRITE_UPDOWN);
		OutArrow  = new MyBlinkingIcon(bottomIconsLeftMargin + (StockSprites.SPRITE_UP.width * 3), bottomIconsY, StockSprites.SPRITE_REVERSE);

		AddChild(UpArrow);
		AddChild(DownArrow);
		AddChild(InArrow);
		AddChild(OutArrow);
	}

  /**
	* Tells the current menu or sub-menu to display just one item per page
	*/
	public MyOsdMenu WithOneItemPerPage() {
		CurrentMenu.WithOneItemPerPage();
		return this;
	}

  /**
	* Sets the alignment for the current menu (top menu or sub-menu)
	* Expected values:
	*	 0 = LEFT
	*	 1 = CENTER
	*	 2 = RIGHT
	* Any other value will default to LEFT
	* Also see the contsants:
	*	 HORIZONTAL_ALIGN_LEFT
	*	 HORIZONTAL_ALIGN_CENTER
	*	 HORIZONTAL_ALIGN_RIGHT
	*/
	public MyOsdMenu WithCustomHorizontalAlignment(int horizontalAlignment) {
		CurrentMenu.WithHorizontalAlignment(horizontalAlignment);
		return this;
	}

	public MyOsdMenu WithPictureOption(MySprite[] AnimationFrames) {
		CurrentOption = new MyStatefulAnimatedSprite(0, 0)
			.WithState("Default", new MyStatefulAnimatedSpriteState(AnimationFrames));

		CurrentMenu.AddChild(CurrentOption);

		return this;
	}

	public MyOsdMenu WithTextOption(string text) {
		CurrentOption = new MyTextLabel(text, 0, 0);
		CurrentMenu.AddChild(CurrentOption);
		return this;
	}

	public MyOsdMenu WithIconOption(string text, MySprite[] Frames, int floatingIconPosition) {
		MyIconLabel IconLabel = new MyIconLabel(0, 0, text, Frames);
		IconLabel.WithFloatingIconPosition(floatingIconPosition);
		CurrentOption = IconLabel;
		CurrentMenu.AddChild(CurrentOption);
		return this;
	}

	public MyOsdMenu WithAggregatedOptions(Action<MyOsdMenu> Act) {
		if (Act != null) {
			Act(this);
		}

		return this;
	}

	public MyOsdMenu WithAction(Action action) {
		ActionsDictionary[CurrentOption] = action;
		return this;
	}

	public MyOsdMenu WithSubMenu(string title) {
		if (title == null) {
			throw new ArgumentException("The title must not be null");
		}

		MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
		MyList ParentMenu = CurrentMenu;
		CurrentMenu = CreateNewInvisibleList();
		AddChild(CurrentMenu);
		SubmenusDictionary[CurrentOption] = CurrentMenu;
		ParentMenusDictionary[CurrentMenu] = ParentMenu;
		CurrentOption = null;
		MenuTitlesDictionary[CurrentMenu] = title;

		return this;
	}

	public MyOsdMenu EndSubMenu() {
		CurrentMenu = ParentMenusDictionary.GetValueOrDefault(CurrentMenu, null);
		CurrentOption = MenuCurrentOptionsDictionary.GetValueOrDefault(CurrentMenu, null);
		return this;
	}

	public MyOsdMenu End() {
		if (TopLevelMenu.GetItems().Count() == 0) {
			throw new InvalidOperationException("The top level menu must have at least one item");
		}

		CurrentMenu = TopLevelMenu;
		CurrentOption = TopLevelMenu.GetItems()[0];

		SetTitle(MenuTitlesDictionary[CurrentMenu]);

		isConstructed = true;

		return this;
	}

	public void CursorUp() {
		if (isConstructed) {
			if (CurrentMenu != null) {
				UpArrow.Blink(3);
				CurrentOption = CurrentMenu.SelectPreviousItem();
			}
		} else {
			throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
		}
	}

	public void CursorDown() {
		if (isConstructed) {
			if (CurrentMenu != null) {
				DownArrow.Blink(3);
				CurrentOption = CurrentMenu.SelectNextItem();
			}
		} else {
			throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
		}
	}

	public void CursorIn() {
		if (isConstructed) {
			InArrow.Blink(3);
			MyList Submenu = SubmenusDictionary.GetValueOrDefault(CurrentOption, null);
			if (Submenu == null) {
				Action AssignedAction = ActionsDictionary.GetValueOrDefault(CurrentOption, null);
				if (AssignedAction != null) {
					AssignedAction();
				}
			} else {
				MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
				CurrentMenu.isVisible = false;
				CurrentMenu = Submenu;
				CurrentMenu.isVisible = true;
				CurrentOption = CurrentMenu.SelectFirstItem();
				SetTitle(MenuTitlesDictionary[CurrentMenu]);
			}
		} else {
			throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
		}
	}

	public void CursorOut() {
		if (isConstructed) {
			OutArrow.Blink(3);
			MyList ParentMenu = ParentMenusDictionary.GetValueOrDefault(CurrentMenu, null);
			if (ParentMenu != null) {
				CurrentMenu.isVisible = false;
				CurrentMenu = ParentMenu;
				CurrentMenu.isVisible = true;
				CurrentOption = CurrentMenu.GetSelectedItem();
				MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
				SetTitle(MenuTitlesDictionary[CurrentMenu]);
			}
		} else {
			throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
		}
	}

	protected override void Compute(MyCanvas TargetCanvas) {
		// Nothing to do here
	}

	protected override void Draw(MyCanvas TargetCanvas) {
		// Nothing to do here
	}

	protected override void Init() {
	 // This is the first stage where the Canvas and its default font are available,
	 // making it possible to compute the text width so that it may be properly centered.
		SetTitle(MenuTitlesDictionary[CurrentMenu]);
	}

	public override int GetWidth() {
		return resX;
	}

	public override int GetHeight() {
		return resY;
	}

	private MyList CreateNewInvisibleList() {
		MyList ret = new MyList(
			PAGE_PADDING,
			TITLE_ROW_HEIGHT,
			resX - (PAGE_PADDING * 2),
			resY - PAGE_PADDING - TITLE_ROW_HEIGHT - BOTTOM_ROW_HEIGHT + 2
		);

		ret.isVisible = false;

		return ret;
	}

	private void SetTitle(string title) {
		TitleLabel.SetText(title);
		TitleLabel.x = (resX - TitleLabel.GetWidth()) / 2;
	}
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// SCRIPT INITIALIZATION ///////////////////////////////////////////////////////////////////////////////////////////

/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *	  > adding pages
  *	  > adding components to the pages
  *	  > linking logic and animations
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



// MINIFIED UI FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////
public static Program PROGRAM;public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update1; PROGRAM = this;}public void Save() {}private int initStepNbr = 0;public void Main(string argument, UpdateType updateSource) { if (initStepNbr < 5) {initStepNbr++;if (initStepNbr == 1) InitSprites1();if (initStepNbr == 2) InitSprites2();if (initStepNbr == 3) InitSprites3();if (initStepNbr == 4) InitSprites4();if (initStepNbr == 5) InitApplication(); } else {OnScreenApplication.Cycle(); }}}public class Constants { public const int FLOATING_POSITION_TOP = 0, FLOATING_POSITION_LEFT = 1, FLOATING_POSITION_RIGHT = 2, FLOATING_POSITION_BOTTOM = 3; public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2;}public class DefaultMonospaceFont { private static MySprite CreateFontSprite(byte[] bytes) { MyCanvas Cvs = new MyCanvas(6, 7); Cvs.BitBlt(new MySprite(8, 7, DrawingFrameworkUtils.ByteArrayToBoolArray(bytes)), 0, 0); return new MySprite(6, 7, Cvs.GetBuffer()); } private static MySprite SPRITE_A = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_B = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0 }); private static MySprite SPRITE_C = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_D = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0x88,0x88,0x88,0xf0 }); private static MySprite SPRITE_E = CreateFontSprite(new byte[] { 0xf0,0x80,0x80,0xf0,0x80,0x80,0xf0 }); private static MySprite SPRITE_F = CreateFontSprite(new byte[] { 0xf8,0x80,0x80,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_G = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x98,0x88,0x88,0x78 }); private static MySprite SPRITE_H = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_I = CreateFontSprite(new byte[] { 0x70,0x20,0x20,0x20,0x20,0x20,0x70 }); private static MySprite SPRITE_J = CreateFontSprite(new byte[] { 0x10,0x10,0x10,0x10,0x10,0x10,0x60 }); private static MySprite SPRITE_K = CreateFontSprite(new byte[] { 0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88 }); private static MySprite SPRITE_L = CreateFontSprite(new byte[] { 0x80,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_M = CreateFontSprite(new byte[] { 0x88,0xd8,0xa8,0x88,0x88,0x88,0x88 }); private static MySprite SPRITE_N = CreateFontSprite(new byte[] { 0x88,0xc8,0xa8,0x98,0x88,0x88,0x88 }); private static MySprite SPRITE_O = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_P = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_Q = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0xa8,0x90,0x68 }); private static MySprite SPRITE_R = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0xa0,0x90,0x88 }); private static MySprite SPRITE_S = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_T = CreateFontSprite(new byte[] { 0xf8,0x20,0x20,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_U = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_V = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x50,0x20 }); private static MySprite SPRITE_W = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0xa8,0x50 }); private static MySprite SPRITE_X = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x50,0x88,0x88 }); private static MySprite SPRITE_Y = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_Z = CreateFontSprite(new byte[] { 0xf8,0x08,0x10,0x20,0x40,0x80,0xf8 }); private static MySprite SPRITE_1 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x10,0x10,0x10,0x38 }); private static MySprite SPRITE_2 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x08,0x70,0x40,0x78 }); private static MySprite SPRITE_3 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x30,0x08,0x48,0x30 }); private static MySprite SPRITE_4 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x90,0xf8,0x10,0x10 }); private static MySprite SPRITE_5 = CreateFontSprite(new byte[] { 0x78,0x40,0x40,0x70,0x08,0x08,0x70 }); private static MySprite SPRITE_6 = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0xf0,0x88,0x88,0x70 }); private static MySprite SPRITE_7 = CreateFontSprite(new byte[] { 0xf8,0x08,0x08,0x10,0x20,0x40,0x40 }); private static MySprite SPRITE_8 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x88,0x88,0x70 }); private static MySprite SPRITE_9 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_0 = CreateFontSprite(new byte[] { 0x70,0x88,0x98,0xa8,0xc8,0x88,0x70 }); private static MySprite SPRITE_DASH = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0x00,0x00,0x00,0xf8 }); private static MySprite SPRITE_HYPHEN = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0xf8,0x00,0x00,0x00 }); private static MySprite SPRITE_GT = CreateFontSprite(new byte[] { 0x40,0x20,0x10,0x08,0x10,0x20,0x40 }); private static MySprite SPRITE_LT = CreateFontSprite(new byte[] { 0x08,0x10,0x20,0x40,0x20,0x10,0x08 }); private static MySprite SPRITE_EQ = CreateFontSprite(new byte[] { 0x00,0x00,0xf8,0x00,0xf8,0x00,0x00 }); private static MySprite SPRITE_PCT = CreateFontSprite(new byte[] { 0xc0,0xc8,0x10,0x20,0x40,0x98,0x18 }); private static MySprite[] Create() { MySprite[] BitmapFont = new MySprite[256]; BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A; BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B; BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C; BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D; BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E; BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F; BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G; BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H; BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I; BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J; BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K; BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L; BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M; BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N; BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O; BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P; BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q; BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R; BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S; BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T; BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U; BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V; BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W; BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X; BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y; BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z; BitmapFont['1'] = SPRITE_1; BitmapFont['2'] = SPRITE_2; BitmapFont['3'] = SPRITE_3; BitmapFont['4'] = SPRITE_4; BitmapFont['5'] = SPRITE_5; BitmapFont['6'] = SPRITE_6; BitmapFont['7'] = SPRITE_7; BitmapFont['8'] = SPRITE_8; BitmapFont['9'] = SPRITE_9; BitmapFont['0'] = SPRITE_0; BitmapFont['_'] = SPRITE_DASH; BitmapFont['-'] = SPRITE_HYPHEN; BitmapFont['<'] = SPRITE_LT; BitmapFont['>'] = SPRITE_GT; BitmapFont['='] = SPRITE_EQ; BitmapFont['%'] = SPRITE_PCT; return BitmapFont; } public static MySprite[] BitmapFont = Create();}public class DrawingFrameworkConstants { public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2; public const int VERTICAL_ALIGN_TOP = 0, VERTICAL_ALIGN_MIDDLE = 1, VERTICAL_ALIGN_BOTTOM = 2;}public class DrawingFrameworkUtils { private static readonly byte[] BYTE_POW = new byte[]{ 128,64,32,16,8,4,2,1 }; public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) { if (BoolArray == null || BoolArray.Count() == 0) { return null; } bool[] ret = new bool[BoolArray.Count()]; for (int i = 0; i < BoolArray.Count(); i++) { ret[i] = negate ? !BoolArray[i] : BoolArray[i]; } return ret; } public static bool[] NegateBoolArray(bool[] BoolArray) { return CopyBoolArray(BoolArray, true); } public static bool[] ByteArrayToBoolArray(byte [] byteArray) { if (byteArray == null || byteArray.Length == 0) { return new bool[]{}; } bool[] ret = new bool[byteArray.Length * 8]; int retIdx = 0; for (int bIdx = 0 ; bIdx < byteArray.Length ; bIdx++) { byte b = byteArray[bIdx]; for (int divIdx = 0 ; divIdx < 8 ; divIdx++) { ret[retIdx] = (b / BYTE_POW[divIdx] > 0); b = (byte)(b % BYTE_POW[divIdx]); retIdx++; } } return ret; } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight) { return ResizeSpriteCanvas(Sprite, newWidth, newHeight, DrawingFrameworkConstants.HORIZONTAL_ALIGN_CENTER, DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE); } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight, int horizontalAlignment, int verticalAlignment) { if (Sprite == null || newWidth < 1 || newHeight < 1) { return null; } MyCanvas NewCanvas = new MyCanvas(newWidth, newHeight); int posX = ComputePos(Sprite.width, newWidth, horizontalAlignment); int posY = ComputePos(Sprite.height, newHeight, verticalAlignment); NewCanvas.BitBlt(Sprite, posX, posY); return new MySprite(newWidth, newHeight, NewCanvas.GetBuffer()); } private static int ComputePos(int origSize, int newSize, int alignemnt) { if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE) { return (newSize - origSize) / 2; } if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_BOTTOM) { return newSize - 1 - origSize; } return 0; }}public class MyBlinkingIcon : MyOnScreenObject { private MyStatefulAnimatedSprite Sprite; private int blinkingInterval = 3; private int blinkTimeout = 0; private bool isOn = false; private bool isBlinking = false; private int nBlinkTimes = 0; public MyBlinkingIcon(int x, int y, MySprite Graphics) : base(null, x, y, true) { Sprite = new MyStatefulAnimatedSprite(0, 0) .WithState("Off", new MyStatefulAnimatedSpriteState(new MySprite[]{ Graphics })) .WithState("On" , new MyStatefulAnimatedSpriteState(new MySprite[]{ new MySprite(Graphics.width, Graphics.height, DrawingFrameworkUtils.NegateBoolArray(Graphics.data)) })); AddChild(Sprite); } public MyBlinkingIcon WithBlinkingInterval(int blinkingInterval) { this.blinkingInterval = blinkingInterval; return this; } public override int GetWidth() { return Sprite.GetWidth(); } public override int GetHeight() { return Sprite.GetHeight(); } protected override void Init() { } private void LocalSwitchOn() { Sprite.SetState("On"); isOn = true; } private void LocalSwitchOff() { Sprite.SetState("Off"); isOn = false; } private void LocalSwitch() { if (isOn) { LocalSwitchOff(); } else { LocalSwitchOn(); } } public void SwitchOn() { LocalSwitchOn(); stopBlinking(); } public void SwitchOff() { LocalSwitchOff(); stopBlinking(); } public void Switch() { LocalSwitch(); stopBlinking(); } public void Blink(int nTimes) { SwitchOn(); nBlinkTimes = nTimes; isBlinking = true; blinkTimeout = 0; } public void stopBlinking() { isBlinking = false; nBlinkTimes = 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (isBlinking) { blinkTimeout++; if (blinkTimeout >= blinkingInterval) { blinkTimeout = 0; LocalSwitch(); nBlinkTimes--; if (nBlinkTimes == 0) { SwitchOff(); } } } }}public class MyCanvas { private bool[] Buffer; private int resX; private int resY; private int length; private MySprite[] DefaultFont; public MyCanvas(int resX, int resY) { this.resX = resX; this.resY = resY; length = resY * resX; Buffer = new bool[length]; this.DefaultFont = DefaultMonospaceFont.BitmapFont; } public bool[] GetBuffer() { return Buffer; } public int GetResX() { return resX; } public int GetResY() { return resY; } public bool[] GetBufferCopy() { return DrawingFrameworkUtils.CopyBoolArray(Buffer, false); } public void SetDefaultFont(MySprite[] DefaultFont) { this.DefaultFont = DefaultFont; } public MySprite[] GetDefaultFont() { return DefaultFont; } public MyCanvas WithDefaultFont(MySprite[] DefaultFont) { SetDefaultFont(DefaultFont); return this; } public void Clear(bool value = false) { for (int x = 0; x < length; x++) { Buffer[x] = value; } } private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) { if (invertColors) { if (transparentBackground) { return targetPixelValue && !sourcePixelValue; } else { return !sourcePixelValue; } } else { if (transparentBackground) { return sourcePixelValue || targetPixelValue; } else { return sourcePixelValue; } } } public void BitBlt(MySprite sprite, int x, int y) { BitBltExt(sprite, x, y, false, false); } public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) { if (x < 0 || y < 0) { return; } int screenPos = resX * y + x; int spriteLength = sprite.height * sprite.width; int spritePosX = 0; for (int spritePos = 0; spritePos < spriteLength; spritePos++) { try { Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground); screenPos++; } catch (Exception exc) { } if (screenPos >= length - 1) { return; } spritePosX++; if (spritePosX == sprite.width) { spritePosX = 0; screenPos += resX - sprite.width; } } } public void DrawText(int x, int y, String text) { DrawColorText(x, y, text, false, false); } public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) { if (DefaultFont == null || text == null) { return; } char[] textChars = text.ToCharArray(); int screenPosX = x; int prevSpacing = 7; foreach (char chr in textChars) { MySprite CharSprite = DefaultFont[chr]; if (CharSprite != null) { BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground); prevSpacing = CharSprite.width; } screenPosX += prevSpacing; if (screenPosX >= resX) { return; } } } public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) { int actualX1 = x1 > x2 ? x2 : x1; int actualY1 = y1 > y2 ? y2 : y1; int actualX2 = x1 > x2 ? x1 : x2; int actualY2 = y1 > y2 ? y1 : y2; if (actualX1 < 0) actualX1 = 0; if (actualY1 < 0) actualY1 = 0; if (actualX2 >= resX - 1) actualX2 = resX - 1; if (actualY2 >= resY - 1) actualY2 = resY - 1; int rectWidth = actualX2 - actualX1; int screenPosY = actualY1; while (screenPosY <= actualY2) { int screenPos = screenPosY * resX + actualX1; if (screenPos >= length) { return; } bool targetColor = !invertColors; Buffer[screenPos] = targetColor; Buffer[screenPos + rectWidth - 1] = targetColor; if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) { for (int innerPos = screenPos; innerPos < screenPos + rectWidth; innerPos++) { Buffer[innerPos] = targetColor; } } screenPos += resX; screenPosY++; } }}public class MyIconLabel : MyOnScreenObject { private MyStatefulAnimatedSprite AnimatedSprite; private MyTextLabel TextLabel; private int width; private int height; private int floatingIconPosition = Constants.FLOATING_POSITION_TOP; private int spacing = 3; public MyIconLabel(int x, int y, string text, MySprite[] Frames) :base(null, x, y, true){ if (text == null) { throw new ArgumentException("The text of the MyIconLabel must not be null"); } if (Frames == null || Frames.Length == 0) { throw new ArgumentException("There has to be at least one frame if the picture is to be displayed by the MyIconLabel"); } int frameWidth = Frames[0].width; int frameHeight = Frames[0].height; foreach (MySprite Frame in Frames) { if (Frame.width != frameWidth || Frame.height != frameHeight) { throw new ArgumentException("All the frames of the MyIconLabel must have the same width and height"); } } AnimatedSprite = new MyStatefulAnimatedSprite(0,0).WithState("Default", new MyStatefulAnimatedSpriteState(Frames)); AddChild(AnimatedSprite); TextLabel = new MyTextLabel(text, 0,0); AddChild(TextLabel); } public MyIconLabel WithFloatingIconPosition(int floatingIconPosition) { this.floatingIconPosition = floatingIconPosition; return this; } public MyIconLabel WithSpaceing(int spacing) { this.spacing = spacing; return this; } public override int GetHeight() { return height; } public override int GetWidth() { return width; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { int spriteWidth = AnimatedSprite.GetWidth(); int spriteHeight = AnimatedSprite.GetHeight(); int textWidth = TextLabel.GetWidth(); int textHeight = TextLabel.GetHeight(); if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { width = spriteWidth + spacing + textWidth; height = spriteHeight > textHeight ? spriteHeight : textHeight; } else { width = spriteWidth > textWidth ? spriteWidth : textWidth; height = spriteHeight + spacing + textHeight; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT) { AnimatedSprite.x = 0; TextLabel.x = spriteWidth + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { AnimatedSprite.x = width - spriteWidth; TextLabel.x = AnimatedSprite.x - spacing - textWidth; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP || floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { AnimatedSprite.x = (width - spriteWidth) / 2; TextLabel.x = (width - textWidth) / 2; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { TextLabel.y = (height - textHeight) / 2; AnimatedSprite.y = (height - spriteHeight) / 2; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP) { AnimatedSprite.y = 0; TextLabel.y = spriteHeight + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { TextLabel.y = 0; AnimatedSprite.y = textHeight + spacing; } }}public class MyList : MyOnScreenObject { private MyPanel Panel; private MyScrollbar Scrollbar; private MyPanel SelectionBackground; private MyOnScreenObject SelectedItem; private bool oneItemPerPage = false; private int selectedItemIndex; private int startPosY; private int padding = 2; private int horizontalAlignment = Constants.HORIZONTAL_ALIGN_LEFT; private List<MyOnScreenObject> Items = new List<MyOnScreenObject>(); public MyList(int x, int y, int width, int height) : base(null, x, y, true) { Panel = new MyPanel(0, 0, width, height); base.AddChild(Panel); Scrollbar = new MyScrollbar(Panel); SelectionBackground = new MyPanel(0, 0, width, 1).WithOptionalParameters(true, true, false); base.AddChild(SelectionBackground); } public MyList WithCustomScrollbarWidth(int scrollbarWidth) { Scrollbar.WithCustomWidth(scrollbarWidth); return this; } public MyList WithOneItemPerPage() { this.oneItemPerPage = true; return this; } public MyList WithPadding(int padding) { this.padding = padding; return this; } public MyList WithHorizontalAlignment(int horizontalAlignment) { this.horizontalAlignment = horizontalAlignment; return this; } public override void AddChild(MyOnScreenObject Item) { base.AddChild(Item); if (SelectedItem == null) { SetInitialSelectedItem(Item); UpdateScrollbarPosition(); } Items.Add(Item); UpdateItemPositions(); } public override void RemoveChild(MyOnScreenObject Item) { base.RemoveChild(Item); if (Item == SelectedItem) { SetInitialSelectedItem(ChildObjects[0]); UpdateScrollbarPosition(); } Items.Remove(Item); UpdateItemPositions(); } private void SetInitialSelectedItem(MyOnScreenObject Item) { SelectedItem = Item; selectedItemIndex = 0; startPosY = padding; } private int ComputeItemHorizontalPosition(MyOnScreenObject Item) { if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_RIGHT) { return GetWidth() - Scrollbar.GetWidth() - padding - Item.GetWidth(); } else if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_CENTER) { return (GetWidth() - Scrollbar.GetWidth() - Item.GetWidth()) / 2; } else { return padding; } } private void UpdateItemPositions() { if (Items.Count() == 0) { return; } if (oneItemPerPage) { foreach (MyOnScreenObject Item in Items) { if (Item == SelectedItem) { Item.isVisible = true; Item.x = ComputeItemHorizontalPosition(Item); Item.y = (Panel.GetHeight() - Item.GetHeight()) / 2; } else { Item.isVisible = false; } Item.invertColors = false; } SelectionBackground.isVisible = false; } else { int listMaxHeight = GetHeight() - (padding * 2); int selItemY = startPosY; for (int idx = 0 ; idx < selectedItemIndex ; idx++) { selItemY += Items[idx].GetHeight(); } if (selItemY < padding) { startPosY += padding - selItemY; } else if (selItemY + SelectedItem.GetHeight() > listMaxHeight) { startPosY -= selItemY + SelectedItem.GetHeight() - listMaxHeight; } int currPosY = startPosY; foreach (MyOnScreenObject Item in Items) { Item.y = currPosY; Item.x = ComputeItemHorizontalPosition(Item); currPosY += Item.GetHeight(); Item.isVisible = Item.y >= padding && Item.y + Item.GetHeight() <= listMaxHeight; Item.invertColors = Item == SelectedItem; } SelectionBackground.x = padding; SelectionBackground.y = SelectedItem.y; SelectionBackground.SetWidth(GetWidth() - Scrollbar.GetWidth() - (padding * 2)); SelectionBackground.SetHeight(SelectedItem.GetHeight()); SelectionBackground.isVisible = true; } } private void UpdateScrollbarPosition() { Scrollbar.SetPosPct(Items.Count() == 0 ? 0f : ((float)selectedItemIndex / ((float)Items.Count() - 1))); } public MyOnScreenObject SelectNextItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex >= 0 && selectedItemIndex < Items.Count() - 1) { selectedItemIndex++; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject SelectPreviousItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex > 0) { selectedItemIndex--; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject GetSelectedItem() { return SelectedItem; } public MyOnScreenObject SelectFirstItem() { SetInitialSelectedItem(Items[0]); return SelectedItem; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } public override int GetWidth() { return Panel.GetWidth(); } public override int GetHeight() { return Panel.GetHeight(); } public List<MyOnScreenObject> GetItems() { return Items; } protected override void Init() { UpdateItemPositions(); }}public class MyOnScreenApplication { private List<MyScreen> TargetScreens = new List<MyScreen>(); private List<MyPage> Pages = new List<MyPage>(); private MyPage CurrentPage; private MyCanvas Canvas; private int currIteration, nIterations; public MyOnScreenApplication() { currIteration = 0; nIterations = 1; } public MyOnScreenApplication WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyOnScreenApplication OnScreen(MyScreen TargetScreen) { return OnScreen(TargetScreen, 0, 0, Canvas.GetResX(), Canvas.GetResY()); } public MyOnScreenApplication OnScreen(MyScreen TargetScreen, int clipRectX1, int clipRectY1, int clipRectX2, int clipRectY2) { if (Canvas == null) { throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen()."); } TargetScreen.WithCanvas(Canvas); TargetScreen.WithClippedBuffer(clipRectX1, clipRectY1, clipRectX2, clipRectY2); TargetScreens.Add(TargetScreen); nIterations++; return this; } public MyOnScreenApplication WithDefaultPostPage(Func<MyOnScreenApplication,bool> initializationMonitoringFunction) { if (Pages.Count() > 0) { throw new InvalidOperationException("The POST page must be the first page ever added to the application"); } if (Canvas == null) { throw new InvalidOperationException("Please call WithCanvas() before calling WithDefaultPostPage()"); } if (initializationMonitoringFunction == null) { throw new ArgumentException("The initialization monitoring function must be a lambda taking in a MyOnScreenObject and returning a bool"); } MyPage POSTPage = (MyPage) new MyPage().WithClientCycleMethod((MyOnScreenObject obj) => { if (initializationMonitoringFunction(this)) { SwitchToPage(1); } return 0; }); this.AddPage(POSTPage); MyPanel Panel = new MyPanel(0, 0, Canvas.GetResX(), Canvas.GetResY()).WithOptionalParameters(true, true, false); POSTPage.AddChild(Panel); MyPanel TextBackgroundPanel = new MyPanel(0, 0, 2, 2).WithOptionalParameters(true, true, true); POSTPage.AddChild(TextBackgroundPanel); MyTextLabel TextLabel = new MyTextLabel("INITIALIZING", 1, 1).WithOptionalParameters(true, false, false); POSTPage.AddChild(TextLabel); int textLabelWidth = TextLabel.GetWidth(); int textLabelHeight = TextLabel.GetHeight(); TextLabel.x = (Canvas.GetResX() - textLabelWidth) / 2; TextLabel.y = (Canvas.GetResY() - textLabelHeight) / 2 - 3; TextBackgroundPanel.x = TextLabel.x - 3; TextBackgroundPanel.y = TextLabel.y - 2; TextBackgroundPanel.SetWidth(textLabelWidth + 6); TextBackgroundPanel.SetHeight(textLabelHeight + 3); POSTPage.AddChild( new MyPanel(TextBackgroundPanel.x, TextBackgroundPanel.y + TextBackgroundPanel.GetHeight() + 2, 7, 4) .WithOptionalParameters(true, true, true) .WithClientCycleMethod((MyOnScreenObject obj) => { obj.x++; if (obj.x > TextBackgroundPanel.x + TextBackgroundPanel.GetWidth() - 7) { obj.x = TextBackgroundPanel.x; } return 0; }) ); return this; } public void AddPage(MyPage Page) { Pages.Add(Page); Page.SetApplication(this); if (CurrentPage == null) { CurrentPage = Page; } } public void SwitchToPage(MyPage Page) { foreach (MyPage Pg in Pages) { if (Pg == Page) { CurrentPage = Pg; } } } public void SwitchToPage(int pageNumber) { if (pageNumber < 0 || pageNumber >= Pages.Count) { return; } CurrentPage = Pages[pageNumber]; } public MyPage GetCurrentPage() { return CurrentPage; } public void Cycle() { if (currIteration == 0) { Canvas.Clear(); CurrentPage.Cycle(Canvas); } else { TargetScreens[currIteration - 1].FlushBufferToScreen(CurrentPage.invertColors); } currIteration++; if (currIteration >= nIterations) { currIteration = 0; } } public MyCanvas GetCanvas() { return Canvas; }}public abstract class MyOnScreenObject { public int x; public int y; public bool isVisible = true; private bool _invertColors = false; public bool invertColors { get { return _invertColors; } set { _invertColors = value; foreach(MyOnScreenObject Child in ChildObjects) { Child.invertColors = value; } } } public MyOnScreenObject ParentObject; public List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>(); private Func<MyOnScreenObject, int> ClientCycleMethod; private Func<MyCanvas, int> ClientDrawMethod; private bool isObjectNotInitialized = true; public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) { this.SetParent(ParentObject); this.x = x; this.y = y; this.isVisible = isVisible; if (ParentObject != null) { ParentObject.AddChild(this); } } public MyOnScreenObject WithClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { SetClientCycleMethod(ClientCycleMethod); return this; } public void SetClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { this.ClientCycleMethod = ClientCycleMethod; } public MyOnScreenObject WithClientDrawMethod(Func<MyCanvas, int> ClientDrawMethod) { this.ClientDrawMethod = ClientDrawMethod; return this; } public virtual void AddChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Add(ChildObject); ChildObject.SetParent(this); } } public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) { if (ChildObject != null) { ChildObject.SetPosition(x, y); AddChild(ChildObject); } } public void SetPosition(int x, int y) { this.x = x; this.y = y; } public virtual void RemoveChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Remove(ChildObject); ChildObject.RemoveParent(); } } public virtual void SetParent(MyOnScreenObject ParentObject) { if (ParentObject != null) { this.ParentObject = ParentObject; } } public void RemoveParent() { this.ParentObject = null; } public MyOnScreenObject GetTopLevelParent() { if (ParentObject == null) { return this; } return ParentObject.GetTopLevelParent(); } public bool IsObjectVisible() { return isVisible && (ParentObject == null || ParentObject.IsObjectVisible()); } public virtual void Cycle(MyCanvas TargetCanvas) { Compute(TargetCanvas); if (ClientCycleMethod != null) { ClientCycleMethod(this); } foreach (MyOnScreenObject ChildObject in ChildObjects) { ChildObject.Cycle(TargetCanvas); } if (isObjectNotInitialized) { Init(); isObjectNotInitialized = true; } if (IsObjectVisible()) { if (ClientDrawMethod != null) { ClientDrawMethod(TargetCanvas); } Draw(TargetCanvas); } } public int GetAbsoluteX() { return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX()); } public int GetAbsoluteY() { return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY()); } protected abstract void Compute(MyCanvas TargetCanvas); protected abstract void Draw(MyCanvas TargetCanvas); public abstract int GetWidth(); public abstract int GetHeight(); protected abstract void Init();}public class MyPage : MyOnScreenObject { private MyOnScreenApplication OnScreenApplication; public MyPage() : base(null, 0, 0, true) { } public void SetApplication(MyOnScreenApplication OnScreenApplication) { this.OnScreenApplication = OnScreenApplication; } public MyPage WithInvertedColors() { this.invertColors = true; return this; } public MyOnScreenApplication GetApplication() { return OnScreenApplication; } public override int GetHeight() { return 0; } public override int GetWidth() { return 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { }}public class MyPanel : MyOnScreenObject { private int width; private int height; private bool isFilled = false; public MyPanel(int x, int y, int width, int height) : base(null, x, y, true) { this.width = width; this.height = height; this.isFilled = false; } public MyPanel WithOptionalParameters(bool isVisible, bool isFilled, bool invertColors) { this.isVisible = isVisible; this.isFilled = isFilled; this.invertColors = invertColors; return this; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int absoluteX = GetAbsoluteX(); int absoluteY = GetAbsoluteY(); TargetCanvas.DrawRect( absoluteX, absoluteY, absoluteX + width, absoluteY + height, invertColors, isFilled ); } public override int GetWidth() { return width; } public override int GetHeight() { return height; } public void SetWidth(int width) { this.width = width; } public void SetHeight(int height) { this.height = height; } protected override void Init() { }}public class MyScreen { private IMyTextPanel TargetLCD; private MyCanvas Canvas; private bool mirrorX; private char pixelValueOn, pixelValueOff; private int clipRectX1, clipRectY1, clipRectX2, clipRectY2; private bool isClipping = false; public MyScreen(IMyTextPanel TargetLCD, char pixelValueOn, char pixelValueOff, bool mirrorX) { this.TargetLCD = TargetLCD; this.mirrorX = mirrorX; this.pixelValueOn = pixelValueOn; this.pixelValueOff = pixelValueOff; } public MyScreen WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) { clipRectX1 = x1 > x2 ? x2 : x1; clipRectY1 = y1 > y2 ? y2 : y1; clipRectX2 = x1 > x2 ? x1 : x2; clipRectY2 = y1 > y2 ? y1 : y2; isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY(); return this; } private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) { int length = Buffer.Count(); bool[] MirroredBuffer = new bool[length]; int mirrorPosX = resX - 1; int mirrorPos = mirrorPosX; for (int sourcePos = 0; sourcePos < length; sourcePos++) { MirroredBuffer[mirrorPos] = Buffer[sourcePos]; mirrorPos--; mirrorPosX--; if (mirrorPosX == -1) { mirrorPosX = resX - 1; mirrorPos += resX * 2; } } return MirroredBuffer; } private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) { int rectX1 = x1 > x2 ? x2 : x1; int rectY1 = y1 > y2 ? y2 : y1; int rectX2 = x1 > x2 ? x1 : x2; int rectY2 = y1 > y2 ? y1 : y2; if (rectX1 < 0) rectX1 = 0; if (rectY1 < 0) rectY1 = 0; if (rectX2 > resX) rectX2 = resX; if (rectY2 > resY) rectY2 = resY; bool[] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1]; int srcCursor = rectY1 * resX + rectX1; int trgCursor = 0; for (int srcY = rectY1; srcY < rectY2; srcY++) { for (int srcX = rectX1; srcX < rectX2; srcX++) { ret[trgCursor] = Buffer[srcCursor]; ret[trgCursor] = true; trgCursor++; srcCursor++; } srcCursor += rectX1; } return ret; } public void FlushBufferToScreen(bool invertColors) { bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer(); int length = Buffer.Count(); int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX(); int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY(); bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer; StringBuilder renderedBuffer = new StringBuilder(length + resY + 1); char pxValOn = invertColors ? pixelValueOff : pixelValueOn; char pxValOff = invertColors ? pixelValueOn : pixelValueOff; int currXPos = 0; for (int idx = 0; idx < length; idx++) { renderedBuffer.Append(SourceBuffer[idx] ? pxValOn : pxValOff); currXPos++; if (currXPos == resX) { renderedBuffer.Append('\n'); currXPos = 0; } } TargetLCD.WriteText(renderedBuffer.ToString()); } public MyCanvas GetCanvas() { return Canvas; }}public class MyScrollbar : MyOnScreenObject { private int width = 7; private int height = 10; private float posPct = 0.5f; private bool snapToParent = true; public MyScrollbar(MyOnScreenObject ParentObject) : base(ParentObject, 0, 0, true) { } public MyScrollbar DetachedFromParent(int height) { this.snapToParent = false; this.height = height; return this; } public MyScrollbar WithCustomWidth(int width) { this.width = width; return this; } public MyScrollbar AtCoordinates(int x, int y) { this.x = x; this.y = y; return this; } public void SetPosPct(float posPct) { this.posPct = posPct; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int x1 = snapToParent ? ParentObject.GetAbsoluteX() + ResolveClientX() : GetAbsoluteX(); int y1 = snapToParent ? ParentObject.GetAbsoluteY() : GetAbsoluteY(); int x2 = x1 + width; int actualHeight = GetHeight(); int y2 = y1 + actualHeight; TargetCanvas.DrawRect(x1, y1, x2, y2, invertColors, false); int sliderX = x1 + 1; int sliderY = (int)(y1 + 1 + (posPct * ((actualHeight - 5 - 2)))); TargetCanvas.BitBltExt( StockSprites.SCROLLBAR_SLIDER, sliderX, sliderY, invertColors ? (!this.invertColors) : this.invertColors, false ); } private int ResolveHight() { if (ParentObject is MyPanel) { return ((MyPanel)ParentObject).GetHeight(); } return height; } private int ResolveClientX() { if (ParentObject is MyPanel) { return ParentObject.GetWidth() - this.width; } return 0; } public override int GetWidth() { return width; } public override int GetHeight() { return snapToParent ? ResolveHight() : height; } protected override void Init() { }}public class MySprite { public int width; public int height; public bool[] data; public MySprite(int width, int height, bool[] data) { this.width = width; this.height = height; this.data = data; }}public class MyStatefulAnimatedSprite : MyOnScreenObject { private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>(); private MyStatefulAnimatedSpriteState CurrentState; private bool isBackgroundTransparet = true; private MySprite CurrentFrame; public MyStatefulAnimatedSprite(int x, int y) : base(null, x, y, true) { } public MyStatefulAnimatedSprite WithState(string stateName, MyStatefulAnimatedSpriteState State) { if (stateName == null) { throw new ArgumentException("Each state must have a name"); } if (State == null) { throw new ArgumentException("The state may not be null"); } States.Add(stateName, State); if (CurrentState == null) { SetStateObject(State); } return this; } public void SetState(String stateName) { if (stateName == null) { return; } MyStatefulAnimatedSpriteState State; States.TryGetValue(stateName, out State); if (State != null) { SetStateObject(State); } } private void SetStateObject(MyStatefulAnimatedSpriteState State) { CurrentState = State; isBackgroundTransparet = State.IsBackgroundTransparent(); } protected override void Compute(MyCanvas TargetCanvas) { CurrentFrame = CurrentState.GetFrame(); } protected override void Draw(MyCanvas TargetCanvas) { TargetCanvas.BitBltExt( CurrentFrame, GetAbsoluteX(), GetAbsoluteY(), invertColors, CurrentState.IsBackgroundTransparent() ); } public override int GetWidth() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.width; } } public override int GetHeight() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.height; } } protected override void Init() { }}public class MyStatefulAnimatedSpriteState { private MySprite[] Frames; private int nFrames; private int currFrame = 0; private bool transparentBackground = true; public MyStatefulAnimatedSpriteState(MySprite[] Frames) { if (Frames == null || Frames.Count() == 0) { throw new ArgumentException("The frames array must have at least one frame"); } this.Frames = Frames; this.nFrames = Frames.Count(); } public bool IsBackgroundTransparent() { return transparentBackground; } public MyStatefulAnimatedSpriteState WithOpaqueBackground() { transparentBackground = false; return this; } public MySprite GetFrame() { MySprite ret = Frames[currFrame]; currFrame++; if (currFrame >= nFrames) { currFrame = 0; } return ret; }}public class MyTextLabel : MyOnScreenObject { private String text; private bool transparentBackground; private MySprite[] Font; private int padding = 1; public MyTextLabel(string text, int x, int y) : base(null, x, y, true) { this.text = text; this.transparentBackground = true; } public MyTextLabel WithOptionalParameters(bool isVisible, bool invertColors, bool transparentBackground) { this.isVisible = isVisible; this.invertColors = invertColors; this.transparentBackground = transparentBackground; return this; } public MyTextLabel WithCustomFont(MySprite[] CustomFont) { this.Font = CustomFont; return this; } public MyTextLabel WithPadding(int padding) { this.padding = padding; return this; } public void SetText(string text) { this.text = text; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (Font == null) { Font = TargetCanvas.GetDefaultFont(); } TargetCanvas.DrawColorText( GetAbsoluteX() + padding, GetAbsoluteY() + padding, text, invertColors, transparentBackground ); } private MySprite[] ResolveFont() { if (Font == null) { MyOnScreenObject TopLevelParent = GetTopLevelParent(); if (TopLevelParent is MyPage) { return ((MyPage) TopLevelParent).GetApplication().GetCanvas().GetDefaultFont(); } else { return null; } } else { return Font; } } public override int GetWidth() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].width * text.Length + (2 * padding); } public override int GetHeight() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].height + (2 * padding); } protected override void Init() { }}public class StockSprites { private const bool O = true; private const bool _ = false; public static MySprite SPRITE_PWR = new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0x02,0,0x1a,0xc0,0x22,0x20,0x22,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x1f,0xc0,0,0 })); public static MySprite SPRITE_UP = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_DOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_LEFTRIGHT = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0d,0x80,0x1d,0xc0,0x3d,0xe0,0x7d,0xf0,0x3d,0xe0,0x1d,0xc0,0x0d,0x80,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_UPDOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0,0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0 })), 14, 10, 0, 0); public static MySprite SPRITE_REVERSE = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0f,0xe0,0x10,0x10,0x7c,0x10,0x38,0x10,0x10,0x10,0,0x10,0x10,0x10,0x0f,0xe0 })), 14, 10, 0, 0); public static MySprite SCROLLBAR_SLIDER = new MySprite(5, 5, new bool[] { _,O,O,O,_, O,O,O,O,O, O,O,_,_,O, O,O,_,_,O, _,O,O,O,_ }); public static MySprite SPRITE_SMILE_SAD = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0x7e,0,0xc0,0,0xc1,0x81, 0x81,0x80,0,0x62,0,0x43,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_NEUTRAL = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0,0,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_HAPPY = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x82,0,0x40,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0x7e,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 }));}public class TerminalUtils { public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { if (blockName == null || blockName.Length == 0) { throw new ArgumentException("Invalid block name"); } List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>(); MyGridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks); if (allFoundBlocks == null || allFoundBlocks.Count == 0) { throw new ArgumentException("Cannot find a block named [" + blockName + "]"); } foreach (IMyTerminalBlock block in allFoundBlocks) { return (T)block; } return default(T); }}public class UiFrameworkUtils { private const char SCREEN_PIXEL_VALUE_ON = '@'; private const char SCREEN_PIXEL_VALUE_OFF = ' '; public static MyOnScreenApplication InitSingleScreenApplication(IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int resX, int resY, bool mirrorX) { return new MyOnScreenApplication() .WithCanvas( new MyCanvas(resX, resY) ) .OnScreen( new MyScreen( TerminalUtils.FindFirstBlockWithName<IMyTextPanel>(MyGridTerminalSystem, textPanelName), SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF, mirrorX ) ); }
