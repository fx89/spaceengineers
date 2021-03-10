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

This script creates a retro-looking tetris game on a text surface referenced by the user. The game is controlled
by switching functional blocks on or off from the toolbar of a control station, flight seat or control seat. By
mapping the functional blocks to the toolbar of the control station, users can control the game using the keyboard.

The game runs best on the screen of a control station, where it can take advantage of the optimal resolution.
However, it can be set up to run on any kind of screen.

The game can be configured by altering the parameters in the following sections, which can be found
in the script:
    - COMMON PARAMETERS USED BY THE FRAMEWORK = Set up the target screen, pixel size and resolution (minimum height = 90 pixels)
    - TETRIS GAME PARAMETERS                  = Set up the speed, the control switches and more
    - PIECES INITIALIZATION                   = Customize the pieces (square matrices only)

The development version of this script can be found here:
    https://github.com/fx89/spaceengineers/tree/main/UiFramework/RetroTetris

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
private const string TARGET_BLOCK_NAME     = "TETRIS_SEAT"; // The name of the LCD panel or control seat where the image will be rendered
private const int    SURFACE_INDEX         = 0;             // Control seats and other similar blocks may have multiple display panels. The first one is 0, the second one is 1, and so on.
private const float  PIXEL_SIZE            = 0.118f;        // The font size to set for the target LCD panel
private const int    RES_X                 = 230;           // Depending on the font size, more or less pixels will fit horizontally
private const int    RES_Y                 =  90;           // Depending on the font size, ore or less pixels will fit vertically
private const bool   MIRROR_X_AXIS         = false;         // If a transparent screen is placed the other way around, the image will have to be mirrored
private const int    POST_SCREEN_DURATION  = 200;           // Set this to 0 to disable the POST screen. Its purpose is mainly to test that the set font size and resolution produce an image of the desired size

// TETRIS GAME PARAMETERS //////////////////////////////////////////////////////////////////////////////////////////

// Set to true for white foreground and black background
// Set to false for black foreground and white background
private static bool INVERT_COLORS = true;

// These are the names of the blocks which will act as switches for the game buttons.
// They can be anything that can be turned on or off. They can be mapped to the toolbar,
// so that they can be activated using the keyboard. The script monitors the states
// of these switches and, if any of them is active, it executes the assigned action and
// then turns the switch off.
private const string SWITCH_LEFT_NAME  = "TETRIS GAME SWITCH LEFT";
private const string SWITCH_UP_NAME    = "TETRIS GAME SWITCH UP";
private const string SWITCH_RIGHT_NAME = "TETRIS GAME SWITCH RIGHT";
private const string SWITCH_DOWN_NAME  = "TETRIS GAME SWITCH DOWN";

// By default, a switch is considered on if its state is on and is considered off if
// its state is off. If the switch is a thruster, or some other block that should be
// kept online at all times, it would be preferrable that the script considers that
// the switch has been pressed if the said thruster is turned off. The toolbar would
// switch the thruster off to signal a key press. The script would then turn the
// thruster back on after a very brief period of time. To achieve this behavior, set
// this parameter to true.
private static bool SWITCH_ACTIVE_STATE_IS_OFF = true;

// This will be the game speed (0 to 20)
private const int GAME_SPEED = 8;

// This is the value at which the score keeper will top up. Any higher score will 
// not be possible. Any further points will be discarded.
private const int MAX_SCORE = 999;

// The splash sceen and the high score screen will ignore key pressing for a given
// number of frames from the moment they are activated. This is so that key-bashing
// user don't start a new game by mistake after suddenly ending the previous one.
private const int IGNORE_KEY_PRESS_FRAMES_COUNT = 100;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// PIECES INITIALIZATION ///////////////////////////////////////////////////////////////////////////////////////////

// These make the matrix look better in code
private const bool _ = false, O = true;

// These are the game pieces. The matrices can have any dimension as long as they are square
// and don't exceed the dimensions of the game screen, which is 10x18. The first parameter is
// the dimension of the matrix. The second parameter is the matrix itself, which has to have
// the specified dimension, or else the game will crash. The matrix needs to be square so it
// can be easily rotated.
private static MyDroppingPiece[] DroppingPieceTemplates = new MyDroppingPiece[]{
    new MyDroppingPiece(3, new bool[,]{
        {_,O,_},
        {_,O,_},
        {_,O,O}
    }),
    new MyDroppingPiece(4, new bool[,]{
        {_,O,_,_},
        {_,O,_,_},
        {_,O,_,_},
        {_,O,_,_}
    }),
    new MyDroppingPiece(3, new bool[,]{
        {_,O,_},
        {O,O,O},
        {_,_,_}
    }),
    new MyDroppingPiece(3, new bool[,]{
        {O,O,_},
        {_,O,O},
        {_,_,_}
    }),
    new MyDroppingPiece(3, new bool[,]{
        {_,O,O},
        {O,O,_},
        {_,_,_}
    }),
    new MyDroppingPiece(2, new bool[,]{
        {O,O},
        {O,O}
    })
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SPECIAL CHARACTER SPRITES ///////////////////////////////////////////////////////////////////////////////////////

private static MySprite CHR_BACK = new MySprite(6, 7, new bool[]{
    _,_,_,_,_,_,
    _,_,O,_,_,_,
    _,O,_,_,_,_,
    O,O,O,O,O,O,
    _,O,_,_,_,_,
    _,_,O,_,_,_,
    _,_,_,_,_,_
});

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// TETRIS PAGE INITIALIZATION //////////////////////////////////////////////////////////////////////////////////////

/**
 * This is responsible for keeping score, as well as serializing / deserializing data,
 * so that the score can be saved into the programmable block's custom data property.
 */
private MyScorekeeper Scorekeeper;

/**
 * This method is called after the InitSprites*() steps.
 * This is where pages and other on-screen objects can be defined.
 */
private void InitPages(MyOnScreenApplication OnScreenApplication) {
 // Initialize the score keeper and load score data from any previous sessions
    Scorekeeper = new MyScorekeeper(MAX_SCORE, Me);
    Scorekeeper.LoadScore();

 // Create the Tetris pages
    MyTetrisPage SplashPage = new MySplashPage(OnScreenApplication).IgnoringKeyPressAtStart(IGNORE_KEY_PRESS_FRAMES_COUNT);
    MyTetrisPage GamePage = new MyGamePage(OnScreenApplication, DroppingPieceTemplates, Scorekeeper);
    MyTetrisPage HighScorePage = new MyHighScorePage(OnScreenApplication, Scorekeeper).IgnoringKeyPressAtStart(IGNORE_KEY_PRESS_FRAMES_COUNT);
    MyTetrisPage NameEntryPage = new MyNameEntryPage(OnScreenApplication, Scorekeeper);

 // Invert colors on the Tetris pages, if the configuration says so
    if (INVERT_COLORS) {
        SplashPage.WithInvertedColors();
        GamePage.WithInvertedColors();
        NameEntryPage.WithInvertedColors();
    } else {
        HighScorePage.WithInvertedColors();
    }

 // Add the Tetris pages to the application
    OnScreenApplication.AddPage(SplashPage);
    OnScreenApplication.AddPage(GamePage);
    OnScreenApplication.AddPage(HighScorePage);
    OnScreenApplication.AddPage(NameEntryPage);

 // Get the switches
    IMyFunctionalBlock Switch1 = TerminalUtils.FindFirstBlockWithName<IMyFunctionalBlock>(GridTerminalSystem, SWITCH_LEFT_NAME);
    IMyFunctionalBlock Switch2 = TerminalUtils.FindFirstBlockWithName<IMyFunctionalBlock>(GridTerminalSystem, SWITCH_UP_NAME);
    IMyFunctionalBlock Switch3 = TerminalUtils.FindFirstBlockWithName<IMyFunctionalBlock>(GridTerminalSystem, SWITCH_RIGHT_NAME);
    IMyFunctionalBlock Switch4 = TerminalUtils.FindFirstBlockWithName<IMyFunctionalBlock>(GridTerminalSystem, SWITCH_DOWN_NAME);

 // Link the switches to the game pages
    SplashPage.WithSwitches(Switch1, Switch2, Switch3, Switch4);
    GamePage.WithSwitches(Switch1, Switch2, Switch3, Switch4);
    HighScorePage.WithSwitches(Switch1, Switch2, Switch3, Switch4);
    NameEntryPage.WithSwitches(Switch1, Switch2, Switch3, Switch4);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// TETRIS SPRITES DECLARATION //////////////////////////////////////////////////////////////////////////////////////

private static MySprite SPRITE_BACKGROUND_IMAGE;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

private const int STATE_SPLASH = 1;
private const int STATE_GAME = 2;
private const int STATE_HIGH_SCORE = 3;
private const int STATE_NAME_ENTRY = 4;

private const int KEY_LEFT = 0;
private const int KEY_UP = 1;
private const int KEY_RIGHT = 2;
private const int KEY_DOWN = 3;

// TETRIS GAME CLASSES /////////////////////////////////////////////////////////////////////////////////////////////

private abstract class MyTetrisPage : MyPage {
 // Helps identify the first iteration, where the background image is being drawn separately
    private int iterationIndex = 0;

 // Needed for special drawing functionality
    protected MyOnScreenApplication OnScreenApplication;

 // Switches
    protected IMyFunctionalBlock SwitchLeft, SwitchUp, SwitchRight, SwitchDown;

    private int ignoreKeyPressFramesCount = 0;
    public int keyPressFrameCounter = 0;

    public MyTetrisPage(MyOnScreenApplication OnScreenApplication) {
        this.OnScreenApplication = OnScreenApplication;
    }

    public MyTetrisPage WithSwitches(
        IMyFunctionalBlock SwitchLeft,
        IMyFunctionalBlock SwitchUp,
        IMyFunctionalBlock SwitchRight,
        IMyFunctionalBlock SwitchDown
    ) {
        this.SwitchLeft  = SwitchLeft;
        this.SwitchUp    = SwitchUp;
        this.SwitchRight = SwitchRight;
        this.SwitchDown  = SwitchDown;

        return this;
    }

    public MyTetrisPage IgnoringKeyPressAtStart(int ignoreKeyPressFramesCount) {
        this.ignoreKeyPressFramesCount = ignoreKeyPressFramesCount;
        return this;
    }

    private void DrawBackgroundLeft(MyCanvas TargetCanvas) {
        int x = 1;
        int y = (RES_Y - SPRITE_BACKGROUND_IMAGE.height) / 2;

        TargetCanvas.BitBlt(
            SPRITE_BACKGROUND_IMAGE,
            0, 0,
            SPRITE_BACKGROUND_IMAGE.width / 2, SPRITE_BACKGROUND_IMAGE.height - 1,
            x, y
        );
    }

    private void DrawBackgroundRight(MyCanvas TargetCanvas) {
        int x = RES_X - SPRITE_BACKGROUND_IMAGE.width / 2 - 5;
        int y = (RES_Y - SPRITE_BACKGROUND_IMAGE.height) / 2;

        TargetCanvas.BitBlt(
            SPRITE_BACKGROUND_IMAGE,
            SPRITE_BACKGROUND_IMAGE.width / 2 + 1, 0, SPRITE_BACKGROUND_IMAGE.width - 1,
            SPRITE_BACKGROUND_IMAGE.height - 1,
            x, y
        );
    }

    protected override void Draw(MyCanvas TargetCanvas) {
        if (iterationIndex <= 3) {
            switch(iterationIndex) {
                case 0:
                    OnScreenApplication.WithoutAutomaticClear();
                    break;
                case 1:
                    DrawBackgroundLeft(TargetCanvas);
                    break;
                case 2:
                    DrawBackgroundRight(TargetCanvas);
                    break;
                case 3:
                    PreDraw(TargetCanvas);
                    break;
            }
            iterationIndex++;
        } else {
         // Subsequent iterations will draw the game
            DrawGame(TargetCanvas);
        }

        base.Draw(TargetCanvas);
    }

    protected override void Compute(MyCanvas TargetCanvas) {
        base.Compute(TargetCanvas);

        if (keyPressFrameCounter > 0) {
            keyPressFrameCounter--;
        }

        if (MonitorSwitch(SwitchLeft) && keyPressFrameCounter <= 0) {
            KeyPressed(KEY_LEFT);
        }

        if (MonitorSwitch(SwitchRight) && keyPressFrameCounter <= 0) {
            KeyPressed(KEY_RIGHT);
        }

        if (MonitorSwitch(SwitchUp) && keyPressFrameCounter <= 0) {
            KeyPressed(KEY_UP);
        }

        if (MonitorSwitch(SwitchDown) && keyPressFrameCounter <= 0) {
            KeyPressed(KEY_DOWN);
        }
    }

    private bool MonitorSwitch(IMyFunctionalBlock Switch) {
        bool ret = SWITCH_ACTIVE_STATE_IS_OFF ? !Switch.Enabled : Switch.Enabled;

        if (ret) {
            Switch.Enabled = SWITCH_ACTIVE_STATE_IS_OFF;
        }

        return ret;
    }

    protected abstract void PreDraw(MyCanvas TargetCanvas);

    protected abstract void DrawGame(MyCanvas TargetCanvas);

    public abstract void KeyPressed(int keyIndex);

    public override void Activate() {
        base.Activate();
        iterationIndex = 0;
        keyPressFrameCounter = ignoreKeyPressFramesCount;
    }
}



private class MyNameEntryPage : MyTetrisPage {
    private MyScorekeeper Scorekeeper;

    private const int PADDING_TOP =  10;
    private const int LINE_SPACING =  3;
    private const int PANEL_WIDTH = 100;

    private int cursorPos = 0;
    private int currCharIdx = 0;

    private const int MAX_READY_TIMEOUT = 80;
    private int nameReadyTimeoutCounter = MAX_READY_TIMEOUT;

    private MyCanvas TargetCanvas;

    public MyNameEntryPage(MyOnScreenApplication OnScreenApplication, MyScorekeeper Scorekeeper)
    : base(OnScreenApplication)
    {
        this.Scorekeeper = Scorekeeper;
    }

    public override void Activate() {
        base.Activate();

        currCharIdx = 0;

        for (int c = 0 ; c < MyScorekeeperEntry.MAX_CHARS_IN_NAME ; c++) {
            Scorekeeper.SetChar(c, '_');
        }

        nameReadyTimeoutCounter = MAX_READY_TIMEOUT;
    }

    public override void KeyPressed(int keyIndex) {
        switch (keyIndex) {
            case KEY_LEFT:
                PopCursorFromScreen();
                MoveCursorLeft();
                PushCursorToScreen();
                break;

            case KEY_UP:
                PopCursorFromScreen();
                MoveCursrorUp();
                PushCursorToScreen();
                break;

            case KEY_RIGHT:
                PopCursorFromScreen();
                MoveCursorRight();
                PushCursorToScreen();
                break;

            case KEY_DOWN:
                Enter();
                break;

            default:
                break;
        }
    }

    private void MoveCursorLeft() {
        if (cursorPos > 0) {
            cursorPos--;
        }
    }

    private void MoveCursorRight() {
        if (cursorPos < 26) {
            cursorPos++;
        }
    }

    private void MoveCursrorUp() {
        if (cursorPos == 26) {
            cursorPos -= 2;
        } else if (cursorPos - 8 >= 0) {
            cursorPos -= 8;
        } else {
            if (cursorPos == 0) {
                cursorPos = 26;
            } else if (cursorPos == 1) {
                cursorPos = 25;
            } else {
                cursorPos += 16;
            }
        }
    }

    private void Enter() {
        if (cursorPos == 26) {
            if (currCharIdx > 0) {
                currCharIdx--;
            }

            Scorekeeper.SetChar(currCharIdx, '_');
        } else {
            if (currCharIdx < MyScorekeeperEntry.MAX_CHARS_IN_NAME) {
                Scorekeeper.SetChar(currCharIdx, GetCharAtCursor());
                currCharIdx++;

                if (currCharIdx == MyScorekeeperEntry.MAX_CHARS_IN_NAME) {
                    nameReadyTimeoutCounter = MAX_READY_TIMEOUT;
                }
            }
        }

        DrawName();
    }

    private char GetCharAtCursor() {
        return (char)(65 + cursorPos);
    }

    private Point ComputeCursorPos() {
        Point ret = new Point();

        int fontHeight = TargetCanvas.GetDefaultFont()['a'].height;
        int fontWidth = TargetCanvas.GetDefaultFont()['a'].width;

        int posY = ComputeTextGridPosY(fontHeight);
        ret.Y = posY + cursorPos / 8 * (fontHeight + LINE_SPACING);

        int posX = ComputeTextGridPosX(TargetCanvas, fontWidth);
        ret.X = posX + ((cursorPos % 8) * fontWidth * 2);

        if (cursorPos == 26) {
            ret.Y += fontHeight + LINE_SPACING;
            ret.X -= fontWidth * 4;
        }

        return ret;
    }

    private void PopCursorFromScreen() {
        DrawCursorInColor(true);
    }

    private void PushCursorToScreen() {
        DrawCursorInColor(false);
    }

    private void DrawCursorInColor(bool color) {
        if (TargetCanvas != null) {
            Point CursorPos = ComputeCursorPos();

            TargetCanvas.DrawRect(
                CursorPos.X - 3, CursorPos.Y - 2,
                CursorPos.X + TargetCanvas.GetDefaultFont()['a'].width + 2,
                CursorPos.Y + TargetCanvas.GetDefaultFont()['a'].height + 1,
                !color, false
            );
        }
    }

    protected void DrawName() {
        if (TargetCanvas != null) {
            int fontHeight = TargetCanvas.GetDefaultFont()['a'].height;
            int fontWidth = TargetCanvas.GetDefaultFont()['a'].width;

            int posX = ComputeTextGridPosX(TargetCanvas, fontWidth);
            int posY = ComputeTextGridPosY(fontHeight);

            posY +=  5 * (fontHeight + LINE_SPACING);
            posX += 10 * fontWidth;

            TargetCanvas.DrawColorText(posX, posY, Scorekeeper.GetCurrentName(), true, false);
        }
    }

    protected override void DrawGame(MyCanvas TargetCanvas) {
        if (currCharIdx == MyScorekeeperEntry.MAX_CHARS_IN_NAME) {
            nameReadyTimeoutCounter--;
            if (nameReadyTimeoutCounter == 0) {
                Scorekeeper.SaveScore();
                OnScreenApplication.SwitchToPage(STATE_HIGH_SCORE);
            }
        }
    }

    private int ComputeTextGridPosY(int fontHeight) {
        return PADDING_TOP + fontHeight + (LINE_SPACING * 2);
    }

    private int ComputeTextGridPosX(MyCanvas TargetCanvas, int fontWidth) {
        return (RES_X - fontWidth * 15) / 2;
    }

    protected override void PreDraw(MyCanvas TargetCanvas) {
        this.TargetCanvas = TargetCanvas;

        TargetCanvas.DrawRect((RES_X - PANEL_WIDTH) / 2, 0, (RES_X - PANEL_WIDTH) / 2 + PANEL_WIDTH, RES_Y, false, true);
        DrawRow(PADDING_TOP, "ENTER INITIALS", TargetCanvas);

        int fontHeight = TargetCanvas.GetDefaultFont()['a'].height;

        int posY = ComputeTextGridPosY(fontHeight);
        DrawRow(posY, "A B C D E F G H", TargetCanvas); posY += LINE_SPACING + fontHeight;
        DrawRow(posY, "I J K L M N O P", TargetCanvas); posY += LINE_SPACING + fontHeight;
        DrawRow(posY, "Q R S T U V W X", TargetCanvas); posY += LINE_SPACING + fontHeight;

        int fontWidth = TargetCanvas.GetDefaultFont()['a'].width;
        int rowLeft = ComputeTextGridPosX(TargetCanvas, fontWidth);

        TargetCanvas.DrawColorText(rowLeft, posY, "Y Z", true, true);
        posY += LINE_SPACING + fontHeight;

        TargetCanvas.BitBltExt(CHR_BACK, rowLeft, posY, true, true);

        PushCursorToScreen();
        DrawName();
    }

    private void DrawRow(int y, String row, MyCanvas TargetCanvas) {
        int strHighScoreWidth = row.Length * TargetCanvas.GetDefaultFont()['a'].width;
        TargetCanvas.DrawColorText((RES_X - strHighScoreWidth) / 2, y, row, true, false);
    }
}



private class MyHighScorePage : MyTetrisPage {
    private const int STAY_ON_MAX_FRAMES = 200;
    private int stayOnFrameCounter = STAY_ON_MAX_FRAMES;

    private MyScorekeeper Scorekeeper;

    private const int PADDING_TOP = 10;
    private const int LINE_SPACING = 3;
    private const int PANEL_WIDTH = 90;

    public MyHighScorePage(MyOnScreenApplication OnScreenApplication, MyScorekeeper Scorekeeper)
    : base(OnScreenApplication)
    {
        this.Scorekeeper = Scorekeeper;
    }

    public override void KeyPressed(int keyIndex) {
        OnScreenApplication.SwitchToPage(STATE_GAME);
    }

    protected override void DrawGame(MyCanvas TargetCanvas) {
        stayOnFrameCounter--;
        if (stayOnFrameCounter <= 0) {
            OnScreenApplication.SwitchToPage(STATE_SPLASH);
        }

        int fontHeight = TargetCanvas.GetDefaultFont()['a'].height;
        int posY = PADDING_TOP + fontHeight + LINE_SPACING;

        foreach(MyScorekeeperEntry Entry in Scorekeeper.Entries) {
            String strEntry = Entry.GetFullStr(Scorekeeper.MaxScore);
            int strEntryWidth = TargetCanvas.GetDefaultFont()['a'].width * strEntry.Length;
            TargetCanvas.DrawColorText((RES_X - strEntryWidth) / 2, posY, strEntry, true, true);

            posY += LINE_SPACING + fontHeight;
        }
    }

    protected override void PreDraw(MyCanvas TargetCanvas) {
        TargetCanvas.DrawRect((RES_X - PANEL_WIDTH) / 2, 0, (RES_X - PANEL_WIDTH) / 2 + PANEL_WIDTH, RES_Y, false, true);

        String strHighScore = "HIGH SCORE";
        int strHighScoreWidth = strHighScore.Length * TargetCanvas.GetDefaultFont()['a'].width;
        TargetCanvas.DrawColorText((RES_X - strHighScoreWidth) / 2, PADDING_TOP, strHighScore, true, false);
    }

    public override void Activate() {
        base.Activate();
        stayOnFrameCounter = STAY_ON_MAX_FRAMES;
    }
}



private class MySplashPage : MyTetrisPage {

    private const int MAX_SWITCH_TO_SCORE_TIMEOUT = 200;
    private int switchToScoreTimeout = MAX_SWITCH_TO_SCORE_TIMEOUT;

    public MySplashPage(MyOnScreenApplication OnScreenApplication) : base(OnScreenApplication) {
        
    }

    public override void Activate() {
        base.Activate();
        switchToScoreTimeout = MAX_SWITCH_TO_SCORE_TIMEOUT;
    }

    public override void KeyPressed(int keyIndex) {
        if (keyPressFrameCounter <= 0) {
            OnScreenApplication.SwitchToPage(STATE_GAME);
        }
    }

    protected override void DrawGame(MyCanvas TargetCanvas) {
        keyPressFrameCounter--;

        if (keyPressFrameCounter >= 0) {
            string msg = "PLEASE WAIT " + PadScore(keyPressFrameCounter, 999);
            int strWidth = TargetCanvas.GetDefaultFont()['a'].width * msg.Length;
            int strHeight = TargetCanvas.GetDefaultFont()['a'].height;

            int x1 = (RES_X - strWidth) / 2;
            int y1 = (RES_Y - strHeight) / 2 + strHeight + 10;
            int x2 = x1 + strWidth;
            int y2 = y1 + strHeight;

            TargetCanvas.DrawRect(x1,y1, x2, y2, true, true);
            TargetCanvas.DrawColorText(x1, y1, msg, false, false);
        }

        if (switchToScoreTimeout > 0) {
            switchToScoreTimeout--;
        } else {
            OnScreenApplication.SwitchToPage(STATE_HIGH_SCORE);
        }
    }

    protected override void PreDraw(MyCanvas TargetCanvas) {
        string text = "PRESS ANY KEY TO BEGIN";

        int charWidth = TargetCanvas.GetDefaultFont()['a'].width;
        int textWidth = text.Length * charWidth;
        int textHeight = TargetCanvas.GetDefaultFont()['a'].height;

        int textX1 = (RES_X - textWidth ) / 2;
        int textY1 = (RES_Y - textHeight) / 2;

        int textX2 = textX1 + textWidth;
        int textY2 = textY1 + textHeight;

        TargetCanvas.DrawRect(textX1 - 3, textY1 - 3, textX2 + 3, textY2 + 3, false, true);
        TargetCanvas.DrawColorText(textX1, textY1, text, true, true);
    }
}


private class MyGamePage : MyTetrisPage {
 // The game matrix dimensions
    private const int MATRIX_WIDTH  = 10;
    private const int MATRIX_HEIGHT = 18;

 // Square sprite dimensions
    private const int SQUARE_WIDTH  = 6;
    private const int SQUARE_HEIGHT = 5;

 // Dimensions of the tetris screen: 10x18 squares board, 6x5 pixels per square
    private const int TETRIS_SCREEN_WIDTH  = 60;
    private const int TETRIS_SCREEN_HEIGHT = 90;

 // Computing coordinates for the background panel based on the dimensions of the tetris screen
    private const int TETRIS_SCREEN_X1 = (RES_X - TETRIS_SCREEN_WIDTH ) / 2;
    private const int TETRIS_SCREEN_Y1 = (RES_Y - TETRIS_SCREEN_HEIGHT) / 2;
    private const int TETRIS_SCREEN_X2 = TETRIS_SCREEN_X1 + TETRIS_SCREEN_WIDTH;
    private const int TETRIS_SCREEN_Y2 = TETRIS_SCREEN_Y1 + TETRIS_SCREEN_HEIGHT;

 // The game matrix
    private bool[,] GameMatrix = new bool[MATRIX_WIDTH,MATRIX_HEIGHT];

 // Templates for the dropping piece
    private MyDroppingPiece[] DroppingPieceTemplates;
    private Random Randomizer = new Random();

 // The dropping piece
    MyDroppingPiece Piece;         // The piece
    int PieceX, PieceY;            // The coordinates

 // The dropping of the pieces will happen once every N frame
    private const int NTH_FRAME = 20 - GAME_SPEED;
    private int nthFrameCounter = 0;

 // The score keeper
    private MyScorekeeper Scorekeeper;

    public MyGamePage(
        MyOnScreenApplication OnScreenApplication,
        MyDroppingPiece[] DroppingPieceTemplates,
        MyScorekeeper Scorekeeper
    ) : base(OnScreenApplication)
    {
        this.DroppingPieceTemplates = DroppingPieceTemplates;
        this.Scorekeeper = Scorekeeper;
        ResetGameMatrix();
        CreateNewPiece();
    }

    public override void Activate() {
        base.Activate();

     // Reset the nth frame counter
        nthFrameCounter = 0;

     // Reset the game matrix upon activation
        ResetGameMatrix();

     // Reset the score
        Scorekeeper.Reset();
    }

/**
  * Draw the background panel, to act as a frame for the tetris game panel.
  * This needs to be drawn only once, so it hoes into the PreDraw() method.
  */
    protected override void PreDraw(MyCanvas TargetCanvas) {
     // Draw the filled rect
        TargetCanvas.DrawRect(
            TETRIS_SCREEN_X1 - 3, TETRIS_SCREEN_Y1 - 3,
            TETRIS_SCREEN_X2 + 3, TETRIS_SCREEN_Y2 + 3,
            false,
            true
        );

     // Draw the non-filled frame
        TargetCanvas.DrawRect(
            TETRIS_SCREEN_X1 - 1, TETRIS_SCREEN_Y1 - 1,
            TETRIS_SCREEN_X2 + 1, TETRIS_SCREEN_Y2 + 1,
            true,
            false
        );
    }

    protected override void DrawGame(MyCanvas TargetCanvas) {
        nthFrameCounter++;

        if (nthFrameCounter == NTH_FRAME) {
            nthFrameCounter = 0;
            NthFrameProcessing();
        } else {
            DrawMatrix(TargetCanvas);
            DrawScore(TargetCanvas);
        }
    }

    private void DrawMatrix(MyCanvas TargetCanvas) {
     // Draw the background frame
        TargetCanvas.DrawRect(TETRIS_SCREEN_X1, TETRIS_SCREEN_Y1, TETRIS_SCREEN_X2, TETRIS_SCREEN_Y2, false, true);

     // Draw the squares
        for(int x = 0 ; x < MATRIX_WIDTH ; x++) {
            for (int y = 0 ; y < MATRIX_HEIGHT ; y++) {
                if (GameMatrix[x,y]) {
                    TargetCanvas.DrawRect(
                        TETRIS_SCREEN_X1 + 1 + x * SQUARE_WIDTH,
                        TETRIS_SCREEN_Y1 + 1 + y * SQUARE_HEIGHT,
                        TETRIS_SCREEN_X1 + 1 + x * SQUARE_WIDTH + SQUARE_WIDTH - 2,
                        TETRIS_SCREEN_Y1 + 1 + y * SQUARE_HEIGHT + SQUARE_HEIGHT - 2,
                        true,
                        true
                    );
                }
            }
        }
    }

    private void DrawScore(MyCanvas TargetCanvas) {
        const string scoreTitle = "SCORE";
        int scoreTitlelWidth = TargetCanvas.GetDefaultFont()['a'].width * scoreTitle.Length;

        string score = Scorekeeper.GetCurrentScoreStr();
        int scoreWidth = TargetCanvas.GetDefaultFont()['a'].width * score.Length;

        int maxWidth = Math.Max(scoreTitlelWidth, scoreWidth);
        int fontHeight = TargetCanvas.GetDefaultFont()['a'].height;
        int totalHeight = (fontHeight + 2) * 2;

        int drawX = TETRIS_SCREEN_X1 - Math.Min(scoreTitlelWidth, scoreWidth) - 19;
        int drawY = TETRIS_SCREEN_Y1 + (TETRIS_SCREEN_Y2 - TETRIS_SCREEN_Y1 - totalHeight) / 2;

        TargetCanvas.DrawRect(drawX - 3, drawY - 3, drawX + maxWidth + 3, drawY + totalHeight + 1, true, true);
        TargetCanvas.DrawRect(drawX - 2, drawY - 2, drawX + maxWidth + 2, drawY + totalHeight, false, true);
        TargetCanvas.DrawColorText(drawX, drawY, scoreTitle, true, false);
        TargetCanvas.DrawColorText(drawX, drawY + fontHeight + 2, score, true, false);
    }

    public override void KeyPressed(int keyIndex) {
        PopPieceFromMatrix();

        switch(keyIndex) {
            case KEY_LEFT:
                MovePiece(false);
                break;
            case KEY_RIGHT:
                MovePiece(true);
                break;
            case KEY_UP:
                RotatePiece();
                break;
            case KEY_DOWN:
                for ( int i = 0 ; i < 16 ; i++) { DropPiece(); }
                BustLines();
                break;
            default:
                break;
        }

        PushPieceToMatrix();
    }

    private void NthFrameProcessing() {
        PopPieceFromMatrix();

     // If the piece is non-existent or down, then a new piece must be created
     // at the top of the game matrix.
        if (Piece == null || PieceCollidesWithMatrix(Piece.Matrix, Piece.dimension, PieceX, PieceY+1)) {
            PushPieceToMatrix();
            CreateNewPiece();

         // If the new piece also colides with the game matrix, even though it's
         // at the top of the screen, it means the game is finished.
            if (PieceCollidesWithMatrix(Piece.Matrix, Piece.dimension, PieceX, PieceY)) {
                GameOver();
            }
        } else {
         // If the piece is not colliding with anything, it means it can be safely dropped.
            DropPiece();
        }

     // Check for busted lines
        BustLines();

        PushPieceToMatrix();
    }

    private void CreateNewPiece() {
        Piece = DroppingPieceTemplates[Randomizer.Next(0, DroppingPieceTemplates.Length)].Copy();
        PieceX = (MATRIX_WIDTH - Piece.dimension) / 2;
        PieceY = 0;
    }

    private void DropPiece() {
        if (!PieceCollidesWithMatrix(Piece.Matrix, Piece.dimension, PieceX, PieceY+1)) {
            PieceY++;
        }
    }

    private void RotatePiece() {
        bool[,] RotatedPieceMatrix = RotatePieceMatrixLeft();

        if (!PieceCollidesWithMatrix(RotatedPieceMatrix, Piece.dimension, PieceX, PieceY)) {
            Piece.Matrix = RotatedPieceMatrix;
        }
    }

    /**
     * Move piece in the given direction (false = left; true = right)
     */
    private void MovePiece(bool direction) {
        if (direction) {
            if (!PieceCollidesWithMatrix(Piece.Matrix, Piece.dimension, PieceX + 1, PieceY)) {
                PieceX++;
            }
        } else {
            if (!PieceCollidesWithMatrix(Piece.Matrix, Piece.dimension, PieceX - 1, PieceY)) {
                PieceX--;
            }
        }
    }

    private void GameOver() {
        if (Scorekeeper.IsHighScore()) {
            Scorekeeper.UpdateScoreBoard();
            OnScreenApplication.SwitchToPage(STATE_NAME_ENTRY);
        } else {
            OnScreenApplication.SwitchToPage(STATE_HIGH_SCORE);
        }
    }

    private void BustLines() {
        for (int y = MATRIX_HEIGHT - 1 ; y >= 0 ; y--) {
            if (EntireRowIsOn(y)) {
                RemoveRowFromMatrix(y);
                Scorekeeper.IncreaseScore();
            }
        }
    }

    private bool EntireRowIsOn(int y) {
        for (int x = 0 ; x < MATRIX_WIDTH ; x++) {
            if (GameMatrix[x, y] == false) {
                return false;
            }
        }

        return true;
    }

    private void RemoveRowFromMatrix(int y) {
        for (int r = y ; r > 0 ; r--) {
            CopyRowFromAbove(r);
        }

        ClearRow(0);
    }

    private void CopyRowFromAbove(int y) {
        for (int x = 0 ; x < MATRIX_WIDTH ; x++) {
            GameMatrix[x, y] = GameMatrix[x, y-1];
        }
    }

    private void ClearRow(int y) {
        for (int x = 0 ; x < MATRIX_WIDTH ; x++) {
            GameMatrix[x, y] = false;
        }
    }

    private bool[,] RotatePieceMatrixLeft() {
        bool[,] RotatedMatrix = new bool[Piece.dimension, Piece.dimension];

        for (int x = 0 ; x < Piece.dimension ; x++) {
            for (int y = 0 ; y < Piece.dimension ; y++) {
                RotatedMatrix[x,y] = Piece.Matrix[y, Piece.dimension - x - 1];
            }
        }

        return RotatedMatrix;
    }

      /**
    * Check if the piece's lit up squares overlap other lit up squares in the game matrix,
    * supposing the piece would be located at the given coordinates.
    */
    private bool PieceCollidesWithMatrix(bool[,] PieceMatrix, int PieceMatrixDimension, int pieceX, int pieceY) {
        for(int x = 0 ; x < PieceMatrixDimension ; x++) {
            for(int y = 0 ; y < PieceMatrixDimension ; y++) {
             // The collision test is valid only for lit up squares of the piece matrix
                if (PieceMatrix[x,y]) {
                 // Check for collisions with the margins of the game matrix
                    if (
                        pieceX + x == MATRIX_WIDTH
                     || pieceY + y == MATRIX_HEIGHT
                     || pieceX + x < 0
                     || pieceY + y < 0
                    ) {
                        return true;
                    }

                 // Check for collisions with the rest of the matrix
                    if (GameMatrix[pieceX + x, pieceY + y]) {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void PushPieceToMatrix() {
        UpdateGameMatrixWithPieceOverlay(true);
    }

    private void PopPieceFromMatrix() {
        UpdateGameMatrixWithPieceOverlay(false);
    }

    private void UpdateGameMatrixWithPieceOverlay(bool replacingValue) {
        for (int x = 0 ; x < Piece.dimension ; x++) {
            for (int y = 0 ; y < Piece.dimension ; y++) {
                if (Piece.Matrix[x, y]) {
                    if (
                        PieceX + x >= 0 && PieceX + x < MATRIX_WIDTH
                     && PieceY + y >= 0 && PieceY + y < MATRIX_HEIGHT
                    ) {
                        GameMatrix[PieceX + x, PieceY + y] = replacingValue;
                    }
                }
            }
        }
    }

    private void ResetGameMatrix() {
        for (int x = 0 ; x < MATRIX_WIDTH ; x++) {
            for (int y = 0 ; y < MATRIX_HEIGHT ; y++) {
                GameMatrix[x,y] = false;
            }
        }
    }
}



private class MyDroppingPiece {
    public bool[,] Matrix;
    public int dimension;

    public MyDroppingPiece(int dimension, bool[,] Matrix) {
        this.dimension = dimension;
        this.Matrix = Matrix;
    }

    public MyDroppingPiece Copy() {
        bool[,] MatrixCopy = new bool[dimension, dimension];

        for (int x = 0 ; x < dimension ; x++) {
            for (int y = 0 ; y < dimension ; y++) {
                MatrixCopy[x,y] = Matrix[x,y];
            }
        }

        return new MyDroppingPiece(dimension, MatrixCopy);
    }
}



private class MyScorekeeper {
    private const int MAX_ENTRIES = 5;
    public int MaxScore;
    private MyScorekeeperEntry CurrentEntry;
    public MyScorekeeperEntry[] Entries = new MyScorekeeperEntry[MAX_ENTRIES];
    private IMyProgrammableBlock DataHolder;

    public MyScorekeeper(int MaxScore, IMyProgrammableBlock DataHolder) {
        this.DataHolder = DataHolder;
        this.MaxScore = MaxScore;

        for (int e = 0 ; e < MAX_ENTRIES ; e++) {
            Entries[e] = new MyScorekeeperEntry("XXX", 0, MaxScore);
        }

        Reset();
    }

    public void Reset() {
        CurrentEntry = new MyScorekeeperEntry("XXX", 0, MaxScore);
    }

    public void IncreaseScore() {
        if (CurrentEntry.score < MaxScore) {
            CurrentEntry.score++;
        }
    }

    public int GetCurrentScore() {
        return CurrentEntry.score;
    }

    public String GetCurrentScoreStr() {
        return CurrentEntry.GetScoreStr(MaxScore);
    }

    public void UpdateScoreBoard() {
        int beatenScoreIndex = GetBeatenScoreIndex();

        if (beatenScoreIndex >= 0) {
            for (int n = Entries.Length-1 ; n > beatenScoreIndex ; n--) {
                Entries[n] = Entries[n-1];
            }

            Entries[beatenScoreIndex] = CurrentEntry;
        }
    }

    public bool IsHighScore() {
        return GetBeatenScoreIndex() >= 0;
    }

    private int GetBeatenScoreIndex() {
        for (int n = 0 ; n < Entries.Length ; n++) {
            if (CurrentEntry.score > Entries[n].score) {
                return n;
            }
        }

        return -1;
    }

    public void SetChar(int index, char value) {
        CurrentEntry.SetChar(index, value);
    }

    public String GetCurrentName() {
        return CurrentEntry.name;
    }

    /**
     * Unfortunately, the System.Text.Json package is not available in Space Engineers,
     * so this has to be done old school (it's going to be a CSV).
     */
    public void SaveScore() {
        String ret = "";

        for (int e = 0 ; e < Entries.Length ; e++) {
            ret += Entries[e].name + "," + Entries[e].score + "\n";
        }

        DataHolder.CustomData = ret;
    }

    /**
     * Unfortunately, the System.Text.Json package is not available in Space Engineers,
     * so this has to be done old school (it's going to be a CSV).
     */
    public void LoadScore() {
        String data = DataHolder.CustomData;

        if (data != null && data.Length > 0) {
            String[] lines = data.Split('\n');
            if (lines.Length - 1 != Entries.Length) {
                return;
            }

            for(int e = 0 ; e < Entries.Length ; e++) {
                String[] cols = lines[e].Split(',');
                if (cols.Length != 2) {
                    return;
                }

                if (cols[0].Length != MyScorekeeperEntry.MAX_CHARS_IN_NAME) {
                    return;
                }

                int score = 0;
                
                try {
                    score = Int32.Parse(cols[1]);
                } catch (Exception exc) {
                    return;
                }

                Entries[e].name = cols[0];
                Entries[e].score = score;
            }
        }
    }
}

private class MyScorekeeperEntry {
    public static int MAX_CHARS_IN_NAME = 3;

    public String name = "   ";
    public int score;

    public MyScorekeeperEntry(String name, int score, int maxScore) {
     // Set the name
        if (name.Length > MAX_CHARS_IN_NAME) {
            this.name = name.Substring(0 ,3);
        } else {
            this.name = name;
            for (int l = this.name.Length ; l < MAX_CHARS_IN_NAME ; l++) {
                this.name += " ";
            }
        }

     // Set the score
        this.score = Math.Min(Math.Max(0, score), maxScore);
    }

    public void SetChar(int charIndex, char value) {
        if (charIndex < 0 || charIndex >= MAX_CHARS_IN_NAME) {
            return;
        }

        char[] chrName = name.ToCharArray();
        chrName[charIndex] = value;
        name = new string(chrName);
    }

    public String GetScoreStr(int maxScore) {
        return PadScore(score, maxScore);
    }

    public String GetFullStr(int maxScore) {
        return name + "   " + GetScoreStr(maxScore);
    }
}

/**
 * Utility method
 */
private static string PadScore(int num, int maxScore) {
    int nChars = maxScore.ToString().Length;
    return num.ToString().PadLeft(nChars, '0');
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SPRITES DEFINITION //////////////////////////////////////////////////////////////////////////////////////////////

/**
  * The InitSprites[1-7]() methods are called from the first 7 loops,
  * each loop calling one method. This helps spread the workload of
  * loading sprites across multiple initialization steps, to avoid
  * "script too complex" errors.
  */
private void InitSprites1() {
SPRITE_BACKGROUND_IMAGE = new MySprite(232, 100, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[]{
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,
255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
255,255,255,255,255,255,255,255,255,0xfc,170,170,170,170,170,170,
170,170,170,170,170,170,170,170,170,170,170,170,170,170,170,170,
170,0xa0,170,170,170,170,0xa8,85,85,85,85,85,85,85,85,85,
85,85,85,85,85,85,85,85,85,85,85,85,84,0,0,0,
0x05,0x50,0x05,84,170,170,170,170,170,170,170,170,170,170,170,170,
170,170,170,170,170,170,170,170,170,170,0,0,0,0,0,0,
0xa8,85,85,85,0x41,0x15,85,85,85,85,85,85,85,85,85,85,
85,85,85,85,85,85,84,0x1f,0xfc,0,0,0,0,84,170,170,
0xa8,0x07,0xc0,0x2a,170,170,170,170,170,170,170,170,170,170,170,170,
170,170,170,0xa8,0x3f,255,255,0xfc,0,0,0x28,255,255,0xf8,0x1d,0xe0,
0x3f,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
0xf8,0x38,0x8b,255,255,255,0xfc,0x04,0,0,0,0x78,0x70,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x30,0x03,
0xc0,0x07,255,255,0,34,34,0x21,0xf1,0x38,34,34,34,34,34,34,
34,34,34,34,34,34,34,34,34,34,0x20,0x62,0x23,0xe2,0x27,0xa2,
0x27,0,0,0,0x07,128,0x1e,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0x60,0x07,128,0x07,128,0x07,0,136,
136,0x1e,0x44,0x47,0,0x08,136,136,136,136,136,136,136,136,136,136,
136,136,136,136,136,0xc8,0x8f,136,0x8f,136,0x8f,128,0,0,0x7c,0,
0x07,128,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0xc0,0x0f,0,0x0f,0,0x0f,128,0x20,0x01,255,0x11,0x1f,0xc0,0x02,
34,34,34,34,34,34,34,34,34,34,34,34,34,34,0x01,0xa2,
0x2f,34,0x3f,34,0x2e,128,0,0x07,0x9f,0,127,0xf0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0x01,128,0x1f,0,0x1e,
0,0x0c,128,136,0x1e,0x4f,0xc4,0xfe,0x78,0x08,136,136,136,136,136,136,
136,136,136,136,136,136,136,136,0x83,0xfd,0xbe,136,0xbe,136,0x9e,0xc0,
0,0x78,0x07,0xc3,0xf8,0x1c,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0x07,255,255,0x50,0x3e,0,0x18,0xc0,0x21,0xf1,0x11,
255,0xf1,0x1e,0x02,34,34,34,34,34,34,34,34,34,34,34,34,
34,34,0x07,255,255,255,0xfe,0xa2,0x3a,0xc0,0x07,128,0,255,128,0x07,
128,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x06,
255,255,255,255,255,0x78,0x40,0x8f,0x44,0x44,0xfe,0x44,0x45,0xc8,136,136,
136,136,136,136,136,136,136,136,136,136,136,136,0x8c,136,0xfe,170,
0xbf,255,0xfa,0xc0,0x07,128,0x03,0xfe,0,0x01,0xe0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0x0c,0,127,85,0x7d,0x5f,0xf0,
0xc0,0x27,0xd1,0x1f,255,0x91,0x17,0xe2,34,34,34,34,34,34,34,34,
34,34,34,34,34,34,0x1a,34,255,170,0xbe,170,0xba,128,0x03,0xf0,
0x3f,0x8f,128,0x1f,0xe0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0x18,0,0xdf,85,0x5f,85,0x59,128,0x8b,0x74,255,0x47,0xe4,
127,0x68,136,136,136,136,136,136,136,136,136,136,136,136,136,128,
0x38,0x89,0xcf,170,0xbe,170,0xbb,128,0x03,0x3f,0xfc,0x01,0xf1,0xfa,0xe0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0x30,0x01,0x8f,
0xd5,0x5f,85,0x5b,0,0x23,0x5f,0xf1,0x11,0xfb,0xf5,0x62,34,34,34,34,
34,34,34,34,34,34,34,34,34,0x20,0x62,0x23,0x87,170,0xaf,170,
0xbf,0,0x01,0x8f,128,0,127,170,0xa0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0x60,0x03,0x87,255,0x5f,85,0x5f,0,255,
0xd7,0xc4,0x44,127,85,0x61,255,255,255,255,255,255,255,255,255,255,
255,255,255,0xe0,0xfa,0x8f,0x03,255,255,0xfa,0xae,0x0c,0x01,0x87,0xc0,0,
0xfe,170,0xb0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,255,255,0x06,0,0x3f,255,0xfe,0,170,0xd7,0xf1,0x11,255,85,0x70,
170,170,170,170,170,170,170,170,170,170,170,170,170,0xa8,255,0xfe,
0x06,0,0,0x1f,0xfc,0x08,0,0xe3,0xf0,0x0f,0xee,170,0xb0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0x75,0x7e,0x0c,0,0,
0,0,0,170,0x77,0xfc,0x5f,0x5f,85,0x70,170,170,170,170,170,170,
170,170,170,170,170,170,170,0xa8,0x6a,0xaf,0x0c,0,0,0,0,0x08,
0,0x3b,0x9e,0x7e,0xae,0xab,0xf0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0x35,87,0x18,0,0,0,0,0,170,0x1f,0xdf,
0xfd,87,87,0xc0,170,170,170,170,170,170,170,170,170,170,170,170,
170,170,0x3a,0xab,0x98,0x2a,170,170,170,0xa8,0,0x0f,0xc7,0xea,0xaf,0xaf,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x1d,
85,0x90,0,0,0,0,0,170,0xa7,0xd7,0xd5,87,0x5c,0,170,170,
170,170,170,170,170,170,170,170,170,170,170,170,0x1a,0xab,0xf0,0x2a,
170,170,170,0xa8,0,0x03,0xc1,170,0xaf,0xf0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0x0d,85,0xf0,0,0,0,0,
0,170,0xa1,0xd5,0xd5,87,0xe0,0,170,170,170,170,170,170,170,170,
170,170,170,170,170,170,0x0f,255,0xe0,0,170,170,170,0xa8,0,0,
225,170,0xaf,128,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,127,0xe0,0,0,0,0,0,170,0xa0,0x35,0xd5,0x5e,
0x0a,170,170,170,170,170,170,170,170,170,170,170,170,170,170,170,
0,0,0xc0,0,170,170,170,0xa8,0,0,0x19,0xea,0xf8,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,255,255,0xfd,0xd5,0xf0,0x1f,255,255,255,255,255,
255,255,255,255,255,255,255,255,255,255,0xf8,0,0,0,0x07,255,
255,0xfc,0,0,0x07,0xeb,0xc0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,136,
136,0x8b,255,0x08,136,136,136,136,136,136,136,136,136,136,136,136,
136,136,136,136,136,0,0,0x7c,0,0x08,136,136,0,0,0x01,0xfc,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0x03,255,128,0,0,0,34,34,0x20,0xf8,0x02,34,34,
34,34,34,34,34,34,34,34,34,34,34,34,34,34,0x20,0,
0x1f,0x23,0xf0,0x02,34,0x20,0,0,0,0x60,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0xfc,0x01,0xf8,
0,0,0,136,136,136,136,0,0,0x08,136,136,136,136,136,136,
136,136,136,136,136,136,136,136,136,0x07,0xc8,0x8b,0xf8,0,136,136,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0x0f,128,0x1f,0xf8,0,0,0,34,34,34,
0,0,0,0x02,34,34,34,34,34,34,34,34,34,34,34,34,
34,34,0x20,0x0f,0xe3,255,0xb8,0x02,34,0x20,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0x0f,0xf7,0xfd,0x58,0,0,0,136,136,136,0,0x17,255,0xf8,0,136,
136,136,136,136,136,136,136,136,136,136,136,136,136,0x8e,255,0xea,
0xbc,0,0x08,136,0,0,0,127,255,255,0xfe,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0x04,0x1f,85,0x5c,0,0,
0,34,34,0x20,0x7b,0x5f,0x11,0x3f,0x02,34,34,34,34,34,34,34,
34,34,34,34,34,34,34,0x06,0x2e,170,0xac,0,0x02,0x20,0,0,
0,0x60,0x0f,0,0x3b,0xc0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0x06,0x0d,85,0x5c,0x02,0,0,136,136,128,0xc4,0x4e,
0x44,0x75,0xe0,0x08,136,136,136,136,136,136,136,136,136,136,136,136,
136,0x06,0x8e,170,0xac,0x1f,0xc0,0x08,0,0,0,0xc0,0x1e,0,0x7a,0xf8,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x06,0x0f,
85,87,0xfd,0xf0,0,34,34,0x20,0x91,0x1f,0x11,0x75,0x7c,34,34,34,
34,34,34,34,34,34,34,34,34,34,34,0x02,0x2e,170,0xaf,0xea,
0xbe,0,0,0,0x01,128,0x3c,0,0x6a,0xac,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0x03,0x07,85,87,0,0x1f,0,136,
136,0x81,0xc4,0x7c,0x44,0xf5,0x56,0x08,136,136,136,136,136,136,136,136,
136,136,136,136,136,0x03,0x8e,170,0xae,170,0xbf,0x08,0,0,0x01,0,
0x3c,0,0xea,0xae,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0x03,0x07,85,87,0x01,255,0,0,0,0x03,0x11,0x39,0x11,0xd5,
0x56,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x03,
0x27,170,0xaf,0xaf,0xfb,128,255,255,0x03,0,0x78,0x01,0xea,0xae,127,255,
255,255,255,255,255,255,255,255,255,255,255,0xfc,0x01,0x83,85,0x5f,
127,0xd5,128,170,170,0x02,0x44,0x7c,0x45,0xd5,0x5c,0x2a,170,170,170,170,
170,170,170,170,170,170,170,170,0xa8,0x01,0x8f,170,255,0xfe,0xab,128,
0,0,0x06,0,0xf0,0x01,170,0xac,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0x01,0xe3,0xd7,255,0xf5,85,128,170,0xa0,0x05,
0x11,0xf1,0x13,0xd5,0x5c,0x2a,170,170,170,170,170,170,170,170,170,170,
170,170,0,0x03,0xfb,0xbf,255,0xea,0xab,128,0,0,0x0e,0,0xe0,0x03,
170,0xb8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0x1f,255,255,255,0xf5,85,128,0xa8,0,0x0f,255,255,255,0xd5,0x58,0x2a,
170,170,170,170,170,170,170,170,170,170,170,0xa0,0,255,255,255,
0xeb,0xea,170,0xc0,0x01,255,255,255,255,255,170,0xb8,0,0,0,0,
0,0,0,0,0,0,0,0,0,0x07,0xe0,127,255,87,0xf5,85,
0xc0,0x81,255,255,255,255,0x5f,0xf5,0x58,0x2a,170,170,170,170,170,170,
170,170,170,170,170,0xa0,0x3f,170,0xbf,0xfa,0xab,0xea,170,0xc0,0x03,0,
0x7a,0xdf,255,0,0xfa,0xb0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0xf8,0,0x3f,0xd5,85,0xf5,85,0xc0,0x83,0x44,0xfc,0x47,255,
0xd5,0x7d,0x70,0x3f,255,255,255,255,255,255,255,255,255,255,255,225,
0xea,0xab,255,0xea,0xab,0xea,170,0xc0,0x06,0,0xf0,0x03,0xbf,0xe0,0x1f,0xb0,
0,0,0,0,0,0,0,0,0,0,0,0,0x01,0xf0,0x07,0xf7,
0xd5,85,0xf5,85,0x40,0x07,0x11,0xf1,0x13,87,0xfd,87,0xe2,34,34,34,
34,34,34,34,34,34,34,34,34,0x21,0xfa,0xbf,0xef,0xea,0xab,0xfa,
170,0xe0,0x06,0x01,0xf0,0x07,0xab,255,255,0xe0,0,0,0,0,0,0,
0,0,0,0,0,0,0x01,0xbd,255,87,0xd5,85,0xf5,87,0xe0,0x8c,
0x45,0xe4,0x47,85,255,255,0xe0,136,136,136,136,136,136,136,136,136,
136,136,136,128,0x8f,0xfa,0xab,0xea,170,0xfa,0xbf,128,0x0c,0x01,0xe0,0x06,
170,0xb8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0xc7,0xd5,87,0xf5,85,0xf5,0xfc,0,0x0d,0x13,0xf1,0x1f,85,0x70,0,
0x02,34,34,34,34,34,34,34,34,34,34,34,34,0x20,0xe3,170,
0xab,0xea,170,0xfb,0xe0,0,0x18,0x03,0xc0,0x0e,170,0xb0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0xc3,0xd5,87,0xf5,85,
127,128,0,0x1c,0x47,0xc4,0x4d,85,0x60,136,136,136,136,136,136,136,
136,136,136,136,136,136,136,136,0xcb,170,0xab,0xea,0xab,0xfc,0,0,
0x10,0x07,0xc0,0x1e,170,0xe0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0x43,0xd5,85,0xf5,87,0xe0,0,0,0x31,0x17,0x91,
0x1d,85,0x62,34,34,34,34,34,34,34,34,34,34,34,34,34,
34,0x20,0x63,170,0xab,0xfa,0xbf,0,0x02,0x20,0x30,0x07,128,0x1a,170,0xc0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0x61,
0xd5,85,0xf5,0xfc,0,0,0,0x7e,0xcf,0xc4,0x7d,85,0xc0,136,136,136,
136,136,136,136,136,136,136,136,136,136,136,136,0x69,0xea,0xab,255,
0xe0,0,0x08,136,127,255,0xa0,0x3a,170,0xc0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0x21,0xd5,85,255,0,0,0,
0,0x3f,255,255,0xf5,85,0xc2,34,34,34,34,34,34,34,34,34,
34,34,34,34,34,0x20,0x33,0xea,0xab,0xfc,0,0x02,34,0x20,0x0f,0x87,
255,0xfe,0xab,128,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0x10,0xd5,87,0xe0,0,0,0,0,0x01,0xe5,0xfc,0x5f,85,
128,136,136,136,136,136,136,136,136,136,136,136,136,136,136,136,
0x9c,0xea,0xbf,0,0x08,136,136,136,0,0x78,0x7e,0x07,0xeb,128,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0x0f,0xf5,0xf8,
0,0,0,0,0,0,0x1f,255,0x91,0xf7,0x02,34,34,34,34,34,
34,34,34,34,34,34,34,34,34,34,0x07,0xef,0xe0,0x02,34,34,
34,0x20,0,0x07,255,225,255,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0x01,255,0,0,0,0,0,0,0,
0,0x03,255,255,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0xf8,0,0,0,0,0,0,255,255,0xe0,0x03,
0xfe,0x1f,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
255,128,0x60,0,0x07,255,255,255,0xfc,85,85,0x40,0,0,0x15,85,
85,85,85,85,85,85,85,85,85,85,85,85,85,85,0,0,
0x01,85,85,85,85,84,170,170,0xa8,0,0,0x0a,170,170,170,170,
170,170,170,170,170,170,170,170,170,170,170,128,0,0x02,170,170,
170,170,0xa8,85,85,85,84,0,0x15,85,85,85,85,85,85,85,
85,85,85,85,85,85,85,85,0,0,0x15,85,85,85,85,84,
170,170,170,170,0,170,170,170,170,170,170,170,170,170,170,170,
170,170,170,170,170,170,170,170,170,170,170,170,0xa8,85,85,85,
85,85,85,85,85,85,85,85,85,85,85,85,85,85,85,85,
85,85,85,85,85,85,85,85,85,84,255,255,255,255,255,255,
255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
255,255,255,255,255,255,0xfc,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0
}));
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