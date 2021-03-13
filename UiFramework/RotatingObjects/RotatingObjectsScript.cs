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

This script displays a rotating 3D wireframe mesh on a 1x1 LCD panel, on a resolution of 325x215 pixels.
The mesh is specified in the custom data field of the programmable block using the Wavefront OBJ format,
which can be exported to by almost every graphics software. Due to script complexity limitations imposed
by the game, the maximum number of lines (lines of text) accepted is 700. To reduce the number of lines,
users can remove any line which doesn't represent a vertex (starting with "v ") or a face (starting with
"f "). Vertex texture coordinates (starting with "vt") and nromals (starting with "vn") can be removed
as well.

To get better results on other types of screens, the display resolution and pixel size can be updated in
the configuration included as part of the script. The initial angle and rotation speed, as well as the
distance of the object from the view can also be updated there.

==========================================================================================================
IMPORTANT NOTE:
==========================================================================================================
In case the script freezes due to a "Script too Complex" error, try modifying the MODEL_DISTANCE_FROM_VIEW
parameter, to move the object farther away.
==========================================================================================================



There are two configuration sections:

    - COMMON PARAMETERS USED BY THE FRAMEWORK
            > setup the target text surface (lcd, control station, etc)
            > setup the resolution and pixel size
            > setup the POST screen duration (0 to go straight to the rendering of the mesh)

    - 3D RENDER CONFIGURATION
            > switch the background and foreground colors
            > move the object closer or farther from the view (too close will result in a "Script too Complex" error)
            > set the initial angle and rotation speeds



Information regarding the Wavefront OBJ format can be found here:
    https://en.wikipedia.org/wiki/Wavefront_.obj_file

The project's development version, as well as a few test meshes, can be found here:
    https://github.com/fx89/spaceengineers/tree/main/UiFramework/RotatingObjects

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
private const float  PIXEL_SIZE            = 0.08056f;     // The font size to set for the target text surface
private const int    RES_X                 = 325;          // Depending on the font size, more or less pixels will fit horizontally
private const int    RES_Y                 = 215;          // Depending on the font size, ore or less pixels will fit vertically
private const int    POST_SCREEN_DURATION  = 100;          // Set this to 0 to disable the POST screen. Its purpose is mainly to test that the set font size and resolution produce an image of the desired size

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// 3D RENDER CONFIGURATION /////////////////////////////////////////////////////////////////////////////////////////

// Set this to true for black foreground on white background
private static bool INVERT_COLORS = false;

// Distance of the rendered object from the view
//    --- increase this if you get the "Script too Complex" error while rendering - !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
private const double MODEL_DISTANCE_FROM_VIEW = 3.0d;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.005d;
private const double ROT_SPEED_RAD_PITCH = 0.010d;
private const double ROT_SPEED_RAD_ROLL  = 0.002d;

// Initial rotation angles in radians
private const double INITIAL_ROTATION_RAD_YAW   = Math.PI;
private const double INITIAL_ROTATION_RAD_PITCH = 0.00d;
private const double INITIAL_ROTATION_RAD_ROLL  = 0.00d;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// WAVEFRONT OBJ LOADING ///////////////////////////////////////////////////////////////////////////////////////////

// This is a simple format which allows defining simple geometry, such as vertices and faces, in text mode.
// This format is available for export in almost every 3D graphics software and it's also easy to understand
// and parse.

private void InitSprites1() {
 // Get the model from the programmable block's custom data
    if (Me.CustomData != null && Me.CustomData.Length > 20) {
        ObjFileLines = Me.CustomData.Split('\n');
    } else {
        ObjFileLines = @"
v -2.918396 0.587113 2.363183
v -2.918396 0.553049 2.250887
v -2.918396 0.497731 2.147394
v -2.918396 0.423285 2.056682
v -2.918397 0.332573 1.982237
v -2.918396 0.229080 1.926919
v -2.918396 0.116784 1.892854
v -2.918396 -0.000000 1.881352
v -2.918396 0.598616 2.479967
v -2.918396 0.587113 2.596751
v -2.918396 0.553049 2.709048
v -2.918396 0.497731 2.812540
v -2.918396 0.423285 2.903253
v -2.918397 0.332573 2.977698
v -2.918396 0.229080 3.033016
v -2.918396 0.116784 3.067080
v -2.918396 -0.000000 3.078583
v -2.918397 -0.116784 3.067080
v -2.918397 -0.229081 3.033016
v -2.918397 -0.332573 2.977698
v -2.918397 -0.423286 2.903252
v -2.918397 -0.497731 2.812540
v -2.918397 -0.553049 2.709047
v -2.918397 -0.587114 2.596751
v -2.918397 -0.598616 2.479967
v -2.918397 -0.587113 2.363183
v -2.918397 -0.553049 2.250886
v -2.918397 -0.497730 2.147394
v -2.918397 -0.423285 2.056682
v -2.918397 -0.332572 1.982236
v -2.918397 -0.229080 1.926918
v -2.918397 -0.116783 1.892854
v -2.158842 0.587113 2.363183
v -2.158842 0.553049 2.250887
v -2.158842 0.497731 2.147394
v -2.158842 0.423285 2.056682
v -2.158842 0.332573 1.982237
v -2.158842 0.229080 1.926919
v -2.158842 0.116784 1.892854
v -2.158842 -0.000000 1.881352
v -2.158842 0.598616 2.479967
v -2.158842 0.587113 2.596751
v -2.158842 0.553049 2.709048
v -2.158842 0.497731 2.812540
v -2.158842 0.423285 2.903253
v -2.158842 0.332573 2.977698
v -2.158842 0.229080 3.033016
v -2.158842 0.116784 3.067080
v -2.158842 -0.000000 3.078583
v -2.158842 -0.116784 3.067080
v -2.158842 -0.229081 3.033016
v -2.158842 -0.332573 2.977698
v -2.158842 -0.423286 2.903252
v -2.158842 -0.497731 2.812540
v -2.158842 -0.553049 2.709047
v -2.158842 -0.587114 2.596751
v -2.158842 -0.598616 2.479967
v -2.158842 -0.587113 2.363183
v -2.158842 -0.553049 2.250886
v -2.158842 -0.497730 2.147394
v -2.158842 -0.423285 2.056682
v -2.158842 -0.332572 1.982236
v -2.158842 -0.229080 1.926918
v -2.158842 -0.116783 1.892854
v -1.366821 0.587113 2.363183
v -1.366821 0.553049 2.250887
v -1.366821 0.497731 2.147394
v -1.366821 0.423285 2.056682
v -1.366821 0.332573 1.982237
v -1.366821 0.229080 1.926919
v -1.366821 0.116784 1.892854
v -1.366821 -0.000000 1.881352
v -1.366821 0.598616 2.479967
v -1.366821 0.587113 2.596751
v -1.366821 0.553049 2.709048
v -1.366821 0.497731 2.812540
v -1.366821 0.423285 2.903253
v -1.366821 0.332573 2.977698
v -1.366821 0.229080 3.033016
v -1.366821 0.116784 3.067080
v -1.366821 -0.000000 3.078583
v -1.366821 -0.116784 3.067080
v -1.366821 -0.229081 3.033016
v -1.366821 -0.332573 2.977698
v -1.366821 -0.423286 2.903252
v -1.366821 -0.497731 2.812540
v -1.366821 -0.553049 2.709047
v -1.366821 -0.587114 2.596751
v -1.366821 -0.598616 2.479967
v -1.366821 -0.587113 2.363183
v -1.366821 -0.553049 2.250886
v -1.366821 -0.497730 2.147394
v -1.366821 -0.423285 2.056682
v -1.366821 -0.332572 1.982236
v -1.366821 -0.229080 1.926918
v -1.366821 -0.116783 1.892854
v -0.692922 0.587113 2.363183
v -0.692922 0.553049 2.250887
v -0.692922 0.497731 2.147394
v -0.692922 0.423285 2.056682
v -0.692922 0.332573 1.982237
v -0.692922 0.229080 1.926919
v -0.692922 0.116784 1.892854
v -0.692922 -0.000000 1.881352
v -0.692922 0.598616 2.479967
v -0.692922 0.587113 2.596751
v -0.692922 0.553049 2.709048
v -0.692922 0.497731 2.812540
v -0.692922 0.423285 2.903253
v -0.692922 0.332573 2.977698
v -0.692922 0.229080 3.033016
v -0.692922 0.116784 3.067080
v -0.692922 -0.000000 3.078583
v -0.692922 -0.116784 3.067080
v -0.692922 -0.229081 3.033016
v -0.692922 -0.332573 2.977698
v -0.692922 -0.423286 2.903252
v -0.692922 -0.497731 2.812540
v -0.692922 -0.553049 2.709047
v -0.692922 -0.587114 2.596751
v -0.692922 -0.598616 2.479967
v -0.692922 -0.587113 2.363183
v -0.692922 -0.553049 2.250886
v -0.692922 -0.497730 2.147394
v -0.692922 -0.423285 2.056682
v -0.692922 -0.332572 1.982236
v -0.692922 -0.229080 1.926918
v -0.692922 -0.116783 1.892854
v -0.007361 0.587113 2.363183
v -0.007361 0.553049 2.250887
v -0.007361 0.497731 2.147394
v -0.007361 0.423285 2.056682
v -0.007361 0.332573 1.982237
v -0.007361 0.229080 1.926919
v -0.007361 0.116784 1.892854
v -0.007361 -0.000000 1.881352
v -0.007361 0.598616 2.479967
v -0.007361 0.587113 2.596751
v -0.007361 0.553049 2.709048
v -0.007361 0.497731 2.812540
v -0.007361 0.423285 2.903253
v -0.007361 0.332573 2.977698
v -0.007361 0.229080 3.033016
v -0.007361 0.116784 3.067080
v -0.007361 -0.000000 3.078583
v -0.007361 -0.116784 3.067080
v -0.007361 -0.229081 3.033016
v -0.007361 -0.332573 2.977698
v -0.007361 -0.423286 2.903252
v -0.007361 -0.497731 2.812540
v -0.007361 -0.553049 2.709047
v -0.007361 -0.587114 2.596751
v -0.007361 -0.598616 2.479967
v -0.007361 -0.587113 2.363183
v -0.007361 -0.553049 2.250886
v -0.007361 -0.497730 2.147394
v -0.007361 -0.423285 2.056682
v -0.007361 -0.332572 1.982236
v -0.007361 -0.229080 1.926918
v -0.007361 -0.116783 1.892854
v 0.712269 0.587113 2.363183
v 0.712269 0.553049 2.250887
v 0.712269 0.497731 2.147394
v 0.712269 0.423285 2.056682
v 0.712269 0.332573 1.982237
v 0.712269 0.229080 1.926919
v 0.712269 0.116784 1.892854
v 0.712269 -0.000000 1.881352
v 0.712269 0.598616 2.479967
v 0.712269 0.587113 2.596751
v 0.712269 0.553049 2.709048
v 0.712269 0.497731 2.812540
v 0.712269 0.423285 2.903253
v 0.712269 0.332573 2.977698
v 0.712269 0.229080 3.033016
v 0.712269 0.116784 3.067080
v 0.712269 -0.000000 3.078583
v 0.712269 -0.116784 3.067080
v 0.712269 -0.229081 3.033016
v 0.712269 -0.332573 2.977698
v 0.712269 -0.423286 2.903252
v 0.712269 -0.497731 2.812540
v 0.712269 -0.553049 2.709047
v 0.712269 -0.587114 2.596751
v 0.712269 -0.598616 2.479967
v 0.712269 -0.587113 2.363183
v 0.712269 -0.553049 2.250886
v 0.712269 -0.497730 2.147394
v 0.712269 -0.423285 2.056682
v 0.712269 -0.332572 1.982236
v 0.712269 -0.229080 1.926918
v 0.712269 -0.116783 1.892854
v -2.918396 0.587113 -2.800842
v -2.918396 0.553049 -2.913138
v -2.918396 0.497731 -3.016630
v -2.918396 0.423285 -3.107343
v -2.918397 0.332573 -3.181788
v -2.918396 0.229080 -3.237106
v -2.918396 0.116784 -3.271171
v -2.918396 -0.000000 -3.282673
v -2.918396 0.598616 -2.684057
v -2.918396 0.587113 -2.567273
v -2.918396 0.553049 -2.454977
v -2.918396 0.497731 -2.351485
v -2.918396 0.423285 -2.260772
v -2.918397 0.332573 -2.186327
v -2.918396 0.229080 -2.131009
v -2.918396 0.116784 -2.096944
v -2.918396 -0.000000 -2.085442
v -2.918397 -0.116784 -2.096944
v -2.918397 -0.229081 -2.131009
v -2.918397 -0.332573 -2.186327
v -2.918397 -0.423286 -2.260773
v -2.918397 -0.497731 -2.351485
v -2.918397 -0.553049 -2.454978
v -2.918397 -0.587114 -2.567274
v -2.918397 -0.598616 -2.684058
v -2.918397 -0.587113 -2.800842
v -2.918397 -0.553049 -2.913138
v -2.918397 -0.497730 -3.016631
v -2.918397 -0.423285 -3.107343
v -2.918397 -0.332572 -3.181788
v -2.918397 -0.229080 -3.237107
v -2.918397 -0.116783 -3.271171
v -2.158842 0.587113 -2.800842
v -2.158842 0.553049 -2.913138
v -2.158842 0.497731 -3.016630
v -2.158842 0.423285 -3.107343
v -2.158842 0.332573 -3.181788
v -2.158842 0.229080 -3.237106
v -2.158842 0.116784 -3.271171
v -2.158842 -0.000000 -3.282673
v -2.158842 0.598616 -2.684057
v -2.158842 0.587113 -2.567273
v -2.158842 0.553049 -2.454977
v -2.158842 0.497731 -2.351485
v -2.158842 0.423285 -2.260772
v -2.158842 0.332573 -2.186327
v -2.158842 0.229080 -2.131009
v -2.158842 0.116784 -2.096944
v -2.158842 -0.000000 -2.085442
v -2.158842 -0.116784 -2.096944
v -2.158842 -0.229081 -2.131009
v -2.158842 -0.332573 -2.186327
v -2.158842 -0.423286 -2.260773
v -2.158842 -0.497731 -2.351485
v -2.158842 -0.553049 -2.454978
v -2.158842 -0.587114 -2.567274
v -2.158842 -0.598616 -2.684058
v -2.158842 -0.587113 -2.800842
v -2.158842 -0.553049 -2.913138
v -2.158842 -0.497730 -3.016631
v -2.158842 -0.423285 -3.107343
v -2.158842 -0.332572 -3.181788
v -2.158842 -0.229080 -3.237107
v -2.158842 -0.116783 -3.271171
v -1.366821 0.587113 -2.800842
v -1.366821 0.553049 -2.913138
v -1.366821 0.497731 -3.016630
v -1.366821 0.423285 -3.107343
v -1.366821 0.332573 -3.181788
v -1.366821 0.229080 -3.237106
v -1.366821 0.116784 -3.271171
v -1.366821 -0.000000 -3.282673
v -1.366821 0.598616 -2.684057
v -1.366821 0.587113 -2.567273
v -1.366821 0.553049 -2.454977
v -1.366821 0.497731 -2.351485
v -1.366821 0.423285 -2.260772
v -1.366821 0.332573 -2.186327
v -1.366821 0.229080 -2.131009
v -1.366821 0.116784 -2.096944
v -1.366821 -0.000000 -2.085442
v -1.366821 -0.116784 -2.096944
v -1.366821 -0.229081 -2.131009
v -1.366821 -0.332573 -2.186327
v -1.366821 -0.423286 -2.260773
v -1.366821 -0.497731 -2.351485
v -1.366821 -0.553049 -2.454978
v -1.366821 -0.587114 -2.567274
v -1.366821 -0.598616 -2.684058
v -1.366821 -0.587113 -2.800842
v -1.366821 -0.553049 -2.913138
v -1.366821 -0.497730 -3.016631
v -1.366821 -0.423285 -3.107343
v -1.366821 -0.332572 -3.181788
v -1.366821 -0.229080 -3.237107
v -1.366821 -0.116783 -3.271171
v -0.692922 0.587113 -2.800842
v -0.692922 0.553049 -2.913138
v -0.692922 0.497731 -3.016630
v -0.692922 0.423285 -3.107343
v -0.692922 0.332573 -3.181788
v -0.692922 0.229080 -3.237106
v -0.692922 0.116784 -3.271171
v -0.692922 -0.000000 -3.282673
v -0.692922 0.598616 -2.684057
v -0.692922 0.587113 -2.567273
v -0.692922 0.553049 -2.454977
v -0.692922 0.497731 -2.351485
v -0.692922 0.423285 -2.260772
v -0.692922 0.332573 -2.186327
v -0.692922 0.229080 -2.131009
v -0.692922 0.116784 -2.096944
v -0.692922 -0.000000 -2.085442
v -0.692922 -0.116784 -2.096944
v -0.692922 -0.229081 -2.131009
v -0.692922 -0.332573 -2.186327
v -0.692922 -0.423286 -2.260773
v -0.692922 -0.497731 -2.351485
v -0.692922 -0.553049 -2.454978
v -0.692922 -0.587114 -2.567274
v -0.692922 -0.598616 -2.684058
v -0.692922 -0.587113 -2.800842
v -0.692922 -0.553049 -2.913138
v -0.692922 -0.497730 -3.016631
v -0.692922 -0.423285 -3.107343
v -0.692922 -0.332572 -3.181788
v -0.692922 -0.229080 -3.237107
v -0.692922 -0.116783 -3.271171
v -0.007361 0.587113 -2.800842
v -0.007361 0.553049 -2.913138
v -0.007361 0.497731 -3.016630
v -0.007361 0.423285 -3.107343
v -0.007361 0.332573 -3.181788
v -0.007361 0.229080 -3.237106
v -0.007361 0.116784 -3.271171
v -0.007361 -0.000000 -3.282673
v -0.007361 0.598616 -2.684057
v -0.007361 0.587113 -2.567273
v -0.007361 0.553049 -2.454977
v -0.007361 0.497731 -2.351485
v -0.007361 0.423285 -2.260772
v -0.007361 0.332573 -2.186327
v -0.007361 0.229080 -2.131009
v -0.007361 0.116784 -2.096944
v -0.007361 -0.000000 -2.085442
v -0.007361 -0.116784 -2.096944
v -0.007361 -0.229081 -2.131009
v -0.007361 -0.332573 -2.186327
v -0.007361 -0.423286 -2.260773
v -0.007361 -0.497731 -2.351485
v -0.007361 -0.553049 -2.454978
v -0.007361 -0.587114 -2.567274
v -0.007361 -0.598616 -2.684058
v -0.007361 -0.587113 -2.800842
v -0.007361 -0.553049 -2.913138
v -0.007361 -0.497730 -3.016631
v -0.007361 -0.423285 -3.107343
v -0.007361 -0.332572 -3.181788
v -0.007361 -0.229080 -3.237107
v -0.007361 -0.116783 -3.271171
v 0.712269 0.587113 -2.800842
v 0.712269 0.553049 -2.913138
v 0.712269 0.497731 -3.016630
v 0.712269 0.423285 -3.107343
v 0.712269 0.332573 -3.181788
v 0.712269 0.229080 -3.237106
v 0.712269 0.116784 -3.271171
v 0.712269 -0.000000 -3.282673
v 0.712269 0.598616 -2.684057
v 0.712269 0.587113 -2.567273
v 0.712269 0.553049 -2.454977
v 0.712269 0.497731 -2.351485
v 0.712269 0.423285 -2.260772
v 0.712269 0.332573 -2.186327
v 0.712269 0.229080 -2.131009
v 0.712269 0.116784 -2.096944
v 0.712269 -0.000000 -2.085442
v 0.712269 -0.116784 -2.096944
v 0.712269 -0.229081 -2.131009
v 0.712269 -0.332573 -2.186327
v 0.712269 -0.423286 -2.260773
v 0.712269 -0.497731 -2.351485
v 0.712269 -0.553049 -2.454978
v 0.712269 -0.587114 -2.567274
v 0.712269 -0.598616 -2.684058
v 0.712269 -0.587113 -2.800842
v 0.712269 -0.553049 -2.913138
v 0.712269 -0.497730 -3.016631
v 0.712269 -0.423285 -3.107343
v 0.712269 -0.332572 -3.181788
v 0.712269 -0.229080 -3.237107
v 0.712269 -0.116783 -3.271171
v -1.540566 -0.393580 0.696022
v -1.540566 0.393580 0.696022
v -1.540566 -0.393580 -0.696022
v -1.540566 0.393580 -0.696022
v -0.148522 -0.393580 0.696022
v -0.148522 0.393580 0.696022
v -0.148522 -0.393580 -0.696022
v -0.148522 0.393580 -0.696022
v -1.540566 0.000000 0.892637
v -1.540566 0.000000 -0.892637
v -0.148522 0.000000 -0.892637
v -0.148522 0.000000 0.892637
v 1.099250 0.000000 -0.892637
v 1.099250 -0.393580 -0.696022
v 1.099250 -0.393580 0.696022
v 1.099250 0.393580 -0.696022
v 1.099250 0.393580 0.696022
v 1.099250 0.000000 0.892637
v 2.075451 -0.048657 -0.711144
v 2.075451 -0.362213 -0.554506
v 2.075451 -0.362213 0.554506
v 2.075451 0.264900 -0.554506
v 2.075451 0.264900 0.554506
v 2.075451 -0.048657 0.711144
v 2.790668 -0.105833 -0.485225
v 2.790668 -0.319777 -0.378348
v 2.790668 -0.319777 0.378348
v 2.790668 0.108112 -0.378348
v 2.790668 0.108112 0.378348
v 2.790668 -0.105833 0.485225
v 3.133705 -0.194989 -0.281347
v 3.133705 -0.319040 -0.219377
v 3.133705 -0.319040 0.219377
v 3.133705 -0.070938 -0.219377
v 3.133705 -0.070938 0.219377
v 3.133705 -0.194989 0.281347
v -0.148522 0.000000 -1.459092
v -0.148522 -0.393580 -1.137708
v -0.148522 0.000000 1.459092
v -0.148522 -0.393580 1.137708
v -0.148522 0.393580 -1.137708
v -0.148522 0.393580 1.137708
v 1.099250 0.000000 -1.459092
v 1.099250 -0.393580 -1.137708
v 1.099250 0.000000 1.459092
v 1.099250 -0.393580 1.137708
v 1.099250 0.393580 -1.137708
v 1.099250 0.393580 1.137708
f 401/1/1 390/2/1 426/3/1 432/4/1
f 394/5/2 388/6/2 392/7/2 395/8/2
f 397/9/3 400/10/3 406/11/3 403/12/3
f 396/13/4 390/2/4 386/14/4 393/15/4
f 387/16/5 391/17/5 389/18/5 385/19/5
f 392/7/1 388/20/1 386/21/1 390/2/1
f 13/22/6 12/23/6 11/24/6 10/25/6 9/26/6 1/27/6 2/28/6 3/29/6 4/30/6 5/31/6 6/32/6 7/33/6 8/34/6 32/35/6 31/36/6 30/37/6 29/38/6 28/39/6 27/40/6 26/41/6 25/42/6 24/43/6 23/44/6 22/45/6 21/46/6 20/47/6 19/48/6 18/49/6 17/50/6 16/51/6 15/52/6 14/53/6
f 45/54/6 44/55/6 43/56/6 42/57/6 41/58/6 33/59/6 34/60/6 35/61/6 36/62/6 37/63/6 38/64/6 39/65/6 40/66/6 64/67/6 63/68/6 62/69/6 61/70/6 60/71/6 59/72/6 58/73/6 57/74/6 56/75/6 55/76/6 54/77/6 53/78/6 52/79/6 51/80/6 50/81/6 49/82/6 48/83/6 47/84/6 46/85/6
f 77/86/6 76/87/6 75/88/6 74/89/6 73/90/6 65/91/6 66/92/6 67/93/6 68/94/6 69/95/6 70/96/6 71/97/6 72/98/6 96/99/6 95/100/6 94/101/6 93/102/6 92/103/6 91/104/6 90/105/6 89/106/6 88/107/6 87/108/6 86/109/6 85/110/6 84/111/6 83/112/6 82/113/6 81/114/6 80/115/6 79/116/6 78/117/6
f 109/118/6 108/119/6 107/120/6 106/121/6 105/122/6 97/123/6 98/124/6 99/125/6 100/126/6 101/127/6 102/128/6 103/129/6 104/130/6 128/131/6 127/132/6 126/133/6 125/134/6 124/135/6 123/136/6 122/137/6 121/138/6 120/139/6 119/140/6 118/141/6 117/142/6 116/143/6 115/144/6 114/145/6 113/146/6 112/147/6 111/148/6 110/149/6
f 141/150/6 140/151/6 139/152/6 138/153/6 137/154/6 129/155/6 130/156/6 131/157/6 132/158/6 133/159/6 134/160/6 135/161/6 136/162/6 160/163/6 159/164/6 158/165/6 157/166/6 156/167/6 155/168/6 154/169/6 153/170/6 152/171/6 151/172/6 150/173/6 149/174/6 148/175/6 147/176/6 146/177/6 145/178/6 144/179/6 143/180/6 142/181/6
f 173/182/6 172/183/6 171/184/6 170/185/6 169/186/6 161/187/6 162/188/6 163/189/6 164/190/6 165/191/6 166/192/6 167/193/6 168/194/6 192/195/6 191/196/6 190/197/6 189/198/6 188/199/6 187/200/6 186/201/6 185/202/6 184/203/6 183/204/6 182/205/6 181/206/6 180/207/6 179/208/6 178/209/6 177/210/6 176/211/6 175/212/6 174/213/6
f 365/214/6 364/215/6 363/216/6 362/217/6 361/218/6 353/219/6 354/220/6 355/221/6 356/222/6 357/223/6 358/224/6 359/225/6 360/226/6 384/227/6 383/228/6 382/229/6 381/230/6 380/231/6 379/232/6 378/233/6 377/234/6 376/235/6 375/236/6 374/237/6 373/238/6 372/239/6 371/240/6 370/241/6 369/242/6 368/243/6 367/244/6 366/245/6
f 269/246/6 268/247/6 267/248/6 266/249/6 265/250/6 257/251/6 258/252/6 259/253/6 260/254/6 261/255/6 262/256/6 263/257/6 264/258/6 288/259/6 287/260/6 286/261/6 285/262/6 284/263/6 283/264/6 282/265/6 281/266/6 280/267/6 279/268/6 278/269/6 277/270/6 276/271/6 275/272/6 274/273/6 273/274/6 272/275/6 271/276/6 270/277/6
f 333/278/6 332/279/6 331/280/6 330/281/6 329/282/6 321/283/6 322/284/6 323/285/6 324/286/6 325/287/6 326/288/6 327/289/6 328/290/6 352/291/6 351/292/6 350/293/6 349/294/6 348/295/6 347/296/6 346/297/6 345/298/6 344/299/6 343/300/6 342/301/6 341/302/6 340/303/6 339/304/6 338/305/6 337/306/6 336/307/6 335/308/6 334/309/6
f 237/310/6 236/311/6 235/312/6 234/313/6 233/314/6 225/315/6 226/316/6 227/317/6 228/318/6 229/319/6 230/320/6 231/321/6 232/322/6 256/323/6 255/324/6 254/325/6 253/326/6 252/327/6 251/328/6 250/329/6 249/330/6 248/331/6 247/332/6 246/333/6 245/334/6 244/335/6 243/336/6 242/337/6 241/338/6 240/339/6 239/340/6 238/341/6
f 205/342/6 204/343/6 203/344/6 202/345/6 201/346/6 193/347/6 194/348/6 195/349/6 196/350/6 197/351/6 198/352/6 199/353/6 200/354/6 224/355/6 223/356/6 222/357/6 221/358/6 220/359/6 219/360/6 218/361/6 217/362/6 216/363/6 215/364/6 214/365/6 213/366/6 212/367/6 211/368/6 210/369/6 209/370/6 208/371/6 207/372/6 206/373/6
f 301/374/6 300/375/6 299/376/6 298/377/6 297/378/6 289/379/6 290/380/6 291/381/6 292/382/6 293/383/6 294/384/6 295/385/6 296/386/6 320/387/6 319/388/6 318/389/6 317/390/6 316/391/6 315/392/6 314/393/6 313/394/6 312/395/6 311/396/6 310/397/6 309/398/6 308/399/6 307/400/6 306/401/6 305/402/6 304/403/6 303/404/6 302/405/6
f 389/18/7 396/13/7 393/15/7 385/406/7
f 400/10/8 397/9/8 427/407/8 431/408/8
f 387/409/9 394/5/9 395/8/9 391/17/9
f 389/18/5 391/17/5 398/410/5 399/411/5
f 392/7/1 390/2/1 401/1/1 400/10/1
f 390/2/6 396/13/6 423/412/6 426/3/6
f 399/411/8 402/413/8 429/414/8 430/415/8
f 407/416/10 408/417/10 414/418/10 413/419/10
f 400/10/11 401/1/11 407/416/11 406/11/11
f 401/1/12 402/413/12 408/417/12 407/416/12
f 402/413/13 399/411/13 405/420/13 408/417/13
f 399/411/14 398/410/14 404/421/14 405/420/14
f 398/410/15 397/9/15 403/12/15 404/421/15
f 409/422/16 412/423/16 418/424/16 415/425/16
f 408/417/17 405/420/17 411/426/17 414/418/17
f 405/420/18 404/421/18 410/427/18 411/426/18
f 403/12/19 406/11/19 412/423/19 409/422/19
f 406/11/20 407/416/20 413/419/20 412/423/20
f 404/421/21 403/12/21 409/422/21 410/427/21
f 416/428/8 415/425/8 418/424/8 419/429/8 420/430/8 417/431/8
f 412/423/22 413/419/22 419/429/22 418/424/22
f 410/427/23 409/422/23 415/425/23 416/428/23
f 413/419/24 414/418/24 420/430/24 419/429/24
f 414/418/25 411/426/25 417/431/25 420/430/25
f 411/426/26 410/427/26 416/428/26 417/431/26
f 421/432/27 425/433/27 431/408/27 427/407/27
f 423/412/28 424/434/28 430/415/28 429/414/28
f 422/435/29 421/432/29 427/407/29 428/436/29
f 426/3/30 423/412/30 429/414/30 432/4/30
f 398/410/5 391/17/5 422/435/5 428/436/5
f 396/13/6 389/18/6 424/434/6 423/412/6
f 391/17/6 395/8/6 421/432/6 422/435/6
f 402/413/8 401/1/8 432/4/8 429/414/8
f 392/7/1 400/10/1 431/408/1 425/433/1
f 397/9/8 398/410/8 428/436/8 427/407/8
f 389/18/5 399/411/5 430/415/5 424/434/5
f 395/8/6 392/7/6 425/433/6 421/432/6"
.Split('\n');
    }    
}
private void InitSprites2() {
 // Make sure the model is not too complex for the script to handle
 // In effect, try to avoid the "Script too Complex" error as much as possible
    if (ObjFileLines.Length > 700) {
        throw new ArgumentException("The Wavefront OBJ you are trying to load has more than 700 lines. This makes it too complex for the script to handle.");
    }

 // Create the 3D object
    Obj3D = new MySimple3DObject();

 // Parse the first 231 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 0, 230);    
}
private void InitSprites3() {
 // Parse the next 230 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 231, 460);
}
private void InitSprites4() {
 // Parse the last 240 lines of the Wavefront OBJ file into the 3D object
 // The loader will not do anything if the line index is outside the available range
    MySimpleWavefrontObjLoader.LoadFromArray(ObjFileLines, Obj3D, 461, 700);

 // One may also create a simple model form code and attach it to the model view, like this:
 /*
    TheModelView.AttachModel(
        new MySimple3DObject()
              // Bottom plane
                .WithVertex(-1, -1, -1)
                .WithVertex(-1, -1,  1)
                .WithVertex( 1, -1,  1)
                .WithVertex( 1, -1, -1)
              // Top plane
                .WithVertex(-1,  1, -1)
                .WithVertex(-1,  1,  1)
                .WithVertex( 1,  1,  1)
                .WithVertex( 1,  1, -1)
              // Faces
                .WithFace(new int[]{0,1,2,3}) // bottom
                .WithFace(new int[]{4,5,6,7}) // top
                .WithFace(new int[]{0,4,7,3}) // front
                .WithFace(new int[]{1,5,6,2}) // back
                .WithFace(new int[]{0,4,5,1}) // left
                .WithFace(new int[]{3,7,6,2}) // right
    );
*/
}

private void InitSprites5() {
 // Re-center the object, if this is required
    Obj3D.Recenter();
}

private void InitSprites6(){
 // Normalize the object (only works after re-centering)
    Obj3D.Normalize(OBJ_MAX_SCALE);
}
private void InitSprites7(){
 // Set the initial angle of the object
    Obj3D.Rotate(INITIAL_ROTATION_RAD_YAW, INITIAL_ROTATION_RAD_PITCH, INITIAL_ROTATION_RAD_ROLL);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SMALL 3D FRAMEWORK //////////////////////////////////////////////////////////////////////////////////////////////

private class My3DModelView : MyOnScreenObject {

    private const int CENTER_X = RES_X / 2;
    private const int CENTER_Y = RES_Y / 2;

    private const double SCALE_X = 35d;
    private const double SCALE_Y = 20d;

    private MySimple3DObject AttachedModel;

    private MyScreen TargetScreen;

    private bool invertScreenColors = false;

    private double rotYaw   = 0.01d;
    private double rotPitch = 0.10d;
    private double rotRoll  = 0.03d;

    public My3DModelView(MyScreen TargetScreen, bool invertScreenColors) : base(null, 0, 0, true) {
        this.TargetScreen = TargetScreen;
        this.invertScreenColors = invertScreenColors;
    }

    public My3DModelView WithRotationSpeeds(double rotYaw, double rotPitch, double rotRoll) {
        this.rotYaw   = rotYaw;
        this.rotPitch = rotPitch;
        this.rotRoll  = rotRoll;

        return this;
    }

    public override int GetHeight() {
        return RES_Y;
    }

    public override int GetWidth() {
        return RES_X;
    }

    protected override void Compute(MyCanvas TargetCanvas, int currIteration) {
        
    }

    protected override void Draw(MyCanvas TargetCanvas, int currIteation) {
        if (AttachedModel != null) {
         // Splitting the operation across multiple iterations,
         // to avoid the "Script too Complex" error.
            switch (currIteation) {
             // The first iteration rotates the object
             // The canvas is also cleared at this point, but it is not yet
             // flushed to the target screen, to avoid flickering
                case 0:
                    AttachedModel.Rotate(rotYaw, rotPitch, rotRoll);
                    TargetCanvas.Clear();
                    break;

             // The second iteration draws the object
                case 1:
                    foreach (MyFace Face in AttachedModel.Faces) {
                        DrawFace(Face, TargetCanvas);
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void DrawFace(MyFace Face, MyCanvas TargetCanvas) {
     // These will be the screen coordinates of the first vertex, which will be needed at the
     // end of the process, for closing the polygon
        MyPoint2D Origin = null;

     // This will hold the screen coordinates of the previous vertex, which will serve as the
     // origin point when drawing the edges of the polygon.
        MyPoint2D PrevScreenPos = null;

        foreach(MyPoint3D Vertex in Face.Vertices) {
         // If this is the first vertex, then there is no edge to draw yer.
         // However, the screen coordinates mist be computed and stored for the next iteration.
         // They will also be needed for closing the polygon at the end of the process.
            if(Origin == null) {
                Origin = WorldToScreenCoordinates(Vertex);
                PrevScreenPos = Origin;
            } else {
             // Get the screen coordinates of the vertex
                MyPoint2D ScreenPos = WorldToScreenCoordinates(Vertex);

             // Draw the line between the screen coordinates of the previous vertex and those of the current one
                TargetCanvas.DrawLine(
                    (int)PrevScreenPos.X, (int)PrevScreenPos.Y,
                    (int)ScreenPos.X, (int)ScreenPos.Y, true
                );

             // The screen coordinates of the previous vertex now become those of the current one
                PrevScreenPos = ScreenPos;
            }
        }

     // Finally, draw a line from the last vertex back to the first vertex, to close the polygon
        TargetCanvas.DrawLine(
            (int)PrevScreenPos.X, (int)PrevScreenPos.Y,
            (int)Origin.X, (int)Origin.Y, true
        );
    }

    /**
     * Appies a blunt transformation between 3D coordinates
     * and screen coordinates
     */
    private MyPoint2D WorldToScreenCoordinates(MyPoint3D Vertex) {
        MyPoint2D ret = new MyPoint2D(0, 0);

        double z = Vertex.Z + AttachedModel.Position.Z;
        if (z == 0) {
            z = 1;
        }

		double unit = 0.1 * Math.Pow(2, z / 2);
        ret.X = CENTER_X + (((Vertex.X + AttachedModel.Position.X) / unit) * SCALE_X);
        ret.Y = CENTER_Y + (((Vertex.Y + AttachedModel.Position.Y) / unit) * SCALE_Y);

        return ret;
    }

    protected override void Init() {
        
    }

    public void AttachModel(MySimple3DObject Model) {
        AttachedModel = Model;
    }

    public void SetAttachedModelPosition(double x, double y, double z) {
        if (AttachedModel != null) {
            AttachedModel.Position.X = x;
            AttachedModel.Position.Y = y;
            AttachedModel.Position.Z = z;
        }
    }
}

/////////////////////////////////////////////////////

/**
 * Using custom type instead of the already available Vecor3D because that is
 * a struct and causes unwanted issues with the linkage between faces and vertices.
 */
private class MyPoint3D {
    public double X, Y, Z;

    public MyPoint3D(double x, double y, double z) {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

/**
 * This will hold screen coordinates of vertices in the 3D environment
 */
private class MyPoint2D {
    public double X, Y;

    public MyPoint2D(double x, double y) {
        this.X = x;
        this.Y = y;
    }
}

/**
 * Stores references to vertices. This is why the Vector3D struct is not
 * usable. For this to work, we need to store references to the vertices,
 * so that when the vertices rotate, the faces will rotate automatically.
 */
private class MyFace {
    public List<MyPoint3D> Vertices = new List<MyPoint3D>();
}

/**
 * Stores simple geometry (vertices and faces)
 */
private class MySimple3DObject {
/**
  * The object's position in 3D space
  */
    public Vector3D Position = new Vector3D(0, 0, 0);

/**
  * List of vertices (mostly useful for loading faces)
  */
    public List<MyPoint3D> Vertices = new List<MyPoint3D>();

/**
  * List of faces, which will be required for drawing
  */
    public List<MyFace> Faces = new List<MyFace>();

/**
  * The corners of the object (might be required for re-centering)
  */
    public MyPoint3D NegativeCorner = new MyPoint3D(0,0,0);
    public MyPoint3D PositiveCorner = new MyPoint3D(0,0,0);

    public MySimple3DObject WithVertex(double x, double y, double z) {
     // Add the vertex
        Vertices.Add(new MyPoint3D(x, y, z));

     // Update the corner coordinates
        if (x < NegativeCorner.X) NegativeCorner.X = x;
        if (y < NegativeCorner.Y) NegativeCorner.Y = y;
        if (z < NegativeCorner.Z) NegativeCorner.Z = z;
        if (x > PositiveCorner.X) PositiveCorner.X = x;
        if (y > PositiveCorner.Y) PositiveCorner.Y = y;
        if (z > PositiveCorner.Z) PositiveCorner.Z = z;

     // Return a reference to the current object, so it can be use as a builder
        return this;
    }

    public MySimple3DObject WithFace(int[] vertIndexes) {
     // Validate the vertex indexes array
        if (vertIndexes == null || vertIndexes.Length < 3) {
            throw new ArgumentException("The face must have at least 3 vertices");
        }

     // Create a new face
        MyFace Face = new MyFace();

     // Link the vertices to the face
        foreach (int vIdx in vertIndexes) {
            MyPoint3D vertex = null;
            try {
                vertex = Vertices[vIdx];
            } catch (Exception exc) {
                throw new ArgumentException("Vertex number [" + vIdx + "] is not yet defined. " + exc.Message);
            }

            try {
                Face.Vertices.Add(vertex);
            } catch (Exception exc) {
                throw new ArgumentException("Could not add vertex to face: " + exc.Message);
            }
        }

     // Add the face to the list of faces
        Faces.Add(Face);

     // Return a reference to the current object, so it can be use as a builder
        return this;
    }

    public MySimple3DObject WithFace(List<int> vertIndexes) {
        return WithFace(vertIndexes.ToArray());
    }

/**
  * Applies the given transformation matrix to all the vertices owned by the object
  */
    public void Transform(MatrixD TransformationMatrix) {
        foreach (MyPoint3D Vertex in Vertices) {
            VertexTransform(Vertex, ref TransformationMatrix);
        }
    }

/**
  * Rotates all the vertices of the object using Vrage.Math, which
  * is provided by the Space Engineers API.
  */
    public void Rotate(double yaw, double pitch, double roll) {
     // Create the rotation matrix
        MatrixD RotationMatrix = MatrixD.CreateFromYawPitchRoll(yaw, pitch, roll);

     // Rotate the vertices
        Transform(RotationMatrix);
    }

/**
  * Translates all the vertices of the object using Vrage.Math, which
  * is provided by the Space Engineers API.
  */
    public void Translate(double x, double y, double z) {
     // Create the translation matrix
        MatrixD TranslationMatrix = MatrixD.CreateTranslation(x, y, z);

     // Translate the vertices
        Transform(TranslationMatrix);
    }

/**
  * Translates all the vertices of the object so that the center point
  * resides half way between the positive corner and the negative corner
  */
    public void Recenter() {
     // See how much the object needs to be translated in each direction
     // to have the object's center half way between the positive corner
     // and the negative corner
        double xCorrection = ComputeCorrection(PositiveCorner.X, NegativeCorner.X);
        double yCorrection = ComputeCorrection(PositiveCorner.Y, NegativeCorner.Y);
        double zCorrection = ComputeCorrection(PositiveCorner.Z, NegativeCorner.Z);

     // Translate the object's vertices by the computed amount
        Translate(xCorrection, yCorrection, zCorrection);
    }

    private double ComputeCorrection(double lowVal, double highVal) {
        double halfPoint = (highVal - lowVal) / 2d;
        return -(lowVal + halfPoint);
    }

 /**
   * If any one of the object's dimensions exceeds the given length,
   * then the object will be shrinked down to specs.
   * !!!
   * Due to complexity constraints, this does not call the re-center
   * method before it does its work. The re-center method has to be
   * called from a previous loop if this is to work properly.
   */
    public void Normalize(double maxSize) {
     // Compute the max dimension
        double maxDim
            = Math.Max(PositiveCorner.X - NegativeCorner.X,
              Math.Max(PositiveCorner.Y - NegativeCorner.Y,
                       PositiveCorner.Z - NegativeCorner.Z));

     // If the max dimension exceeds the given max size, then resize
     // each vertex, including the positive and negative corners
        if (maxDim > maxSize) {
         // Compute the shrink factor and create the scale matrix
            double shrinkFactor = maxDim / maxSize;
            MatrixD ScaleMatrix = MatrixD.CreateScale(1 / shrinkFactor);

         // Scale the positive and negative corners
            VertexTransform(PositiveCorner, ref ScaleMatrix);
            VertexTransform(NegativeCorner, ref ScaleMatrix);

         // Scale the vertices
            Transform(ScaleMatrix);
        }
    }
}

/** Transform the vertex using a copy of the logic of multiplying the vector by the 
  * transformation matrix which can be found in VRage.Math.Vector3D.
  * This is necessary because the API does not provide a way to update the Vector3Ds
  * in a loop without having to copy their values into and back from intermediary
  * variables a couple of times (once when num1, num2 and num3 are created, and a
  * second time because the compiler doesn't allow passing iterators as "byref"
  * parameters).
  */
private static void VertexTransform(MyPoint3D Vertex, ref MatrixD TransformationMatrix) {
    double num1 = Vertex.X * TransformationMatrix.M11 + Vertex.Y * TransformationMatrix.M21 + Vertex.Z * TransformationMatrix.M31 + TransformationMatrix.M41;
    double num2 = Vertex.X * TransformationMatrix.M12 + Vertex.Y * TransformationMatrix.M22 + Vertex.Z * TransformationMatrix.M32 + TransformationMatrix.M42;
    double num3 = Vertex.X * TransformationMatrix.M13 + Vertex.Y * TransformationMatrix.M23 + Vertex.Z * TransformationMatrix.M33 + TransformationMatrix.M43;
    Vertex.X = num1;
    Vertex.Y = num2;
    Vertex.Z = num3;
}

/////////////////////////////////////////////////////

/**
 * Simple Wavefront OBJ loader capable of loading vertices and faces,
 * without texture coordinates, normals, materials, etc, which are not
 * going to be used in the script anyway
 */
private class MySimpleWavefrontObjLoader {

    public static void LoadFromArray(String[] array, MySimple3DObject Obj3D, int lineStart, int lineEnd) {
        if (array == null || array.Length < 4) {
            throw new ArgumentException("The object definition must contain at least 4 lines (3 vertices and one face)");
        }

        for (int lNumber = lineStart ; lNumber <= lineEnd ; lNumber++) {
            if (lNumber >= array.Length) {
                break;
            }

            String line = array[lNumber];

         // Get the component type (expected v or f)
            char componentType = line.Length == 0 ? 'x' : line[0];

         // If the component type is not one letter long (for instance if it's vt),
         // then go to the next line
            if (line.Length < 2 || line[1] != ' ') {
                continue;
            }

            if (componentType == 'v') {
                String[] vertDef = line.Split(' ');
                if (vertDef.Length >= 4) {
                    try {
                        double x = double.Parse(vertDef[1], System.Globalization.CultureInfo.InvariantCulture);
                        double y = double.Parse(vertDef[2], System.Globalization.CultureInfo.InvariantCulture);
                        double z = double.Parse(vertDef[3], System.Globalization.CultureInfo.InvariantCulture);
                        Obj3D.WithVertex(x,y,z);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read vertex data from [" + line + "]: "  + exc.Message);
                    }
                    
                } else {
                    throw new ArgumentException("Vertex information incomplete at line [" + line + "]");
                }
            }
            else if (componentType == 'f') {
                String[] faceDef = line.Split(' ');
                if (faceDef.Length >= 4) {
                    try {
                        int [] vertIndexes = new int[faceDef.Length - 1];
                        for (int i = 1 ; i < faceDef.Length ; i++) {
                            vertIndexes[i-1] = Int32.Parse(faceDef[i].Split('/')[0]) - 1;
                        }
                        Obj3D.WithFace(vertIndexes);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read face data at line [" + line + "]: " + exc.Message);
                    }
                } else {
                    throw new ArgumentException("Face information incomplete at line [" + line + "]");
                }
            }
        }
    }  
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// APPLICATION INIT ////////////////////////////////////////////////////////////////////////////////////////////////

// The script will become too complex if the X axis is mirrored
private const bool MIRROR_X_AXIS = false;

// The My3DModelView class doesn't currently handle N_COMPUTE_FRAMES <> 2
private const int N_COMPUTE_FRAMES = 2; // The minimum for this is 2 (1 = rotating the mesh, 2 = drawing the mesh onto the backbuffer)
private const int N_RENDER_FRAMES  = 2; // Rendering the target screen in two separate frames to accommodate the slighty higher resolution

// The max scale of the object - objects will be shrinked to conform to this specification,
// otherwise they might be too big and not be drawn. The script could even crash with the
// dreaded "Script too Complex" error, because the line algorythm will have too many pixels to set.
private const double OBJ_MAX_SCALE = 2.0d;

// This is the String array resulted from loading the Wavefront OBJ
private String[] ObjFileLines;

// The 3D object, which is loaded across multiple iterations to avoid the "Script too Complex" error
private MySimple3DObject Obj3D;

/**
 * This is the 3D model view which will rendered the 3D object onto
 * the target LCD panel. It is initialized in the InitSprites1() method.
 */
private My3DModelView TheModelView;

private void InitPages(MyOnScreenApplication OnScreenApplication) {
 // Initialize the main page and add it to the application
    MyPage MainPage = new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Instruct the application to not clear the back buffer before rendering
 // This will be done by this script, when it's time
    OnScreenApplication.WithoutAutomaticClear();

 // Optionally invert the colors of the main page
    if (INVERT_COLORS) {
        MainPage.WithInvertedColors();
    }

 // Create the model view
    TheModelView = new My3DModelView(OnScreenApplication.GetTargetScreen(), INVERT_COLORS)
        .WithRotationSpeeds(ROT_SPEED_RAD_YAW, ROT_SPEED_RAD_PITCH, ROT_SPEED_RAD_ROLL);

 // Attach the 3D object to the model view
    TheModelView.AttachModel(Obj3D);
    TheModelView.SetAttachedModelPosition(0, 0, MODEL_DISTANCE_FROM_VIEW);

 // Add the model view to the main page
    MainPage.AddChild(TheModelView);
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
        MIRROR_X_AXIS,                                        // Rendering option
        N_COMPUTE_FRAMES,                                     // The number of compute iterations
        N_RENDER_FRAMES                                       // The number of draw iterations
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

