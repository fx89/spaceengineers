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



// MINIFIED UI FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////
}public class Constants { public const int FLOATING_POSITION_TOP = 0, FLOATING_POSITION_LEFT = 1, FLOATING_POSITION_RIGHT = 2, FLOATING_POSITION_BOTTOM = 3; public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2;}public class DefaultMonospaceFont { private static MySprite CreateFontSprite(byte[] bytes) { MyCanvas Cvs = new MyCanvas(6, 7); Cvs.BitBlt(new MySprite(8, 7, DrawingFrameworkUtils.ByteArrayToBoolArray(bytes)), 0, 0); return new MySprite(6, 7, Cvs.GetBuffer()); } private static MySprite SPRITE_A = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_B = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0 }); private static MySprite SPRITE_C = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_D = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0x88,0x88,0x88,0xf0 }); private static MySprite SPRITE_E = CreateFontSprite(new byte[] { 0xf0,0x80,0x80,0xf0,0x80,0x80,0xf0 }); private static MySprite SPRITE_F = CreateFontSprite(new byte[] { 0xf8,0x80,0x80,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_G = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x98,0x88,0x88,0x78 }); private static MySprite SPRITE_H = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_I = CreateFontSprite(new byte[] { 0x70,0x20,0x20,0x20,0x20,0x20,0x70 }); private static MySprite SPRITE_J = CreateFontSprite(new byte[] { 0x10,0x10,0x10,0x10,0x10,0x10,0x60 }); private static MySprite SPRITE_K = CreateFontSprite(new byte[] { 0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88 }); private static MySprite SPRITE_L = CreateFontSprite(new byte[] { 0x80,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_M = CreateFontSprite(new byte[] { 0x88,0xd8,0xa8,0x88,0x88,0x88,0x88 }); private static MySprite SPRITE_N = CreateFontSprite(new byte[] { 0x88,0xc8,0xa8,0x98,0x88,0x88,0x88 }); private static MySprite SPRITE_O = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_P = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_Q = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0xa8,0x90,0x68 }); private static MySprite SPRITE_R = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0xa0,0x90,0x88 }); private static MySprite SPRITE_S = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_T = CreateFontSprite(new byte[] { 0xf8,0x20,0x20,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_U = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_V = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x50,0x20 }); private static MySprite SPRITE_W = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0xa8,0x50 }); private static MySprite SPRITE_X = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x50,0x88,0x88 }); private static MySprite SPRITE_Y = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_Z = CreateFontSprite(new byte[] { 0xf8,0x08,0x10,0x20,0x40,0x80,0xf8 }); private static MySprite SPRITE_1 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x10,0x10,0x10,0x38 }); private static MySprite SPRITE_2 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x08,0x70,0x40,0x78 }); private static MySprite SPRITE_3 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x30,0x08,0x48,0x30 }); private static MySprite SPRITE_4 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x90,0xf8,0x10,0x10 }); private static MySprite SPRITE_5 = CreateFontSprite(new byte[] { 0x78,0x40,0x40,0x70,0x08,0x08,0x70 }); private static MySprite SPRITE_6 = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0xf0,0x88,0x88,0x70 }); private static MySprite SPRITE_7 = CreateFontSprite(new byte[] { 0xf8,0x08,0x08,0x10,0x20,0x40,0x40 }); private static MySprite SPRITE_8 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x88,0x88,0x70 }); private static MySprite SPRITE_9 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_0 = CreateFontSprite(new byte[] { 0x70,0x88,0x98,0xa8,0xc8,0x88,0x70 }); private static MySprite SPRITE_DASH = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0x00,0x00,0x00,0xf8 }); private static MySprite SPRITE_HYPHEN = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0xf8,0x00,0x00,0x00 }); private static MySprite SPRITE_GT = CreateFontSprite(new byte[] { 0x40,0x20,0x10,0x08,0x10,0x20,0x40 }); private static MySprite SPRITE_LT = CreateFontSprite(new byte[] { 0x08,0x10,0x20,0x40,0x20,0x10,0x08 }); private static MySprite SPRITE_EQ = CreateFontSprite(new byte[] { 0x00,0x00,0xf8,0x00,0xf8,0x00,0x00 }); private static MySprite SPRITE_PCT = CreateFontSprite(new byte[] { 0xc0,0xc8,0x10,0x20,0x40,0x98,0x18 }); private static MySprite SPRITE_SPC = new MySprite(6, 7, new bool[42]); private static MySprite[] Create() { MySprite[] BitmapFont = new MySprite[256]; BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A; BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B; BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C; BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D; BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E; BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F; BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G; BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H; BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I; BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J; BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K; BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L; BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M; BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N; BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O; BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P; BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q; BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R; BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S; BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T; BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U; BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V; BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W; BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X; BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y; BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z; BitmapFont['1'] = SPRITE_1; BitmapFont['2'] = SPRITE_2; BitmapFont['3'] = SPRITE_3; BitmapFont['4'] = SPRITE_4; BitmapFont['5'] = SPRITE_5; BitmapFont['6'] = SPRITE_6; BitmapFont['7'] = SPRITE_7; BitmapFont['8'] = SPRITE_8; BitmapFont['9'] = SPRITE_9; BitmapFont['0'] = SPRITE_0; BitmapFont['_'] = SPRITE_DASH; BitmapFont['-'] = SPRITE_HYPHEN; BitmapFont['<'] = SPRITE_LT; BitmapFont['>'] = SPRITE_GT; BitmapFont['='] = SPRITE_EQ; BitmapFont['%'] = SPRITE_PCT; BitmapFont[' '] = SPRITE_SPC; return BitmapFont; } public static MySprite[] BitmapFont = Create();}public class DrawingFrameworkConstants { public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2; public const int VERTICAL_ALIGN_TOP = 0, VERTICAL_ALIGN_MIDDLE = 1, VERTICAL_ALIGN_BOTTOM = 2;}public class DrawingFrameworkUtils { private static readonly byte[] BYTE_POW = new byte[]{ 128,64,32,16,8,4,2,1 }; public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) { if (BoolArray == null || BoolArray.Count() == 0) { return null; } bool[] ret = new bool[BoolArray.Count()]; for (int i = 0; i < BoolArray.Count(); i++) { ret[i] = negate ? !BoolArray[i] : BoolArray[i]; } return ret; } public static bool[] NegateBoolArray(bool[] BoolArray) { return CopyBoolArray(BoolArray, true); } public static bool[] ByteArrayToBoolArray(byte [] byteArray) { if (byteArray == null || byteArray.Length == 0) { return new bool[]{}; } bool[] ret = new bool[byteArray.Length * 8]; int retIdx = 0; for (int bIdx = 0 ; bIdx < byteArray.Length ; bIdx++) { byte b = byteArray[bIdx]; for (int divIdx = 0 ; divIdx < 8 ; divIdx++) { ret[retIdx] = (b / BYTE_POW[divIdx] > 0); b = (byte)(b % BYTE_POW[divIdx]); retIdx++; } } return ret; } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight) { return ResizeSpriteCanvas(Sprite, newWidth, newHeight, DrawingFrameworkConstants.HORIZONTAL_ALIGN_CENTER, DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE); } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight, int horizontalAlignment, int verticalAlignment) { if (Sprite == null || newWidth < 1 || newHeight < 1) { return null; } MyCanvas NewCanvas = new MyCanvas(newWidth, newHeight); int posX = ComputePos(Sprite.width, newWidth, horizontalAlignment); int posY = ComputePos(Sprite.height, newHeight, verticalAlignment); NewCanvas.BitBlt(Sprite, posX, posY); return new MySprite(newWidth, newHeight, NewCanvas.GetBuffer()); } private static int ComputePos(int origSize, int newSize, int alignemnt) { if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE) { return (newSize - origSize) / 2; } if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_BOTTOM) { return newSize - 1 - origSize; } return 0; }}public class MyBlinkingIcon : MyOnScreenObject { private MyStatefulAnimatedSprite Sprite; private int blinkingInterval = 3; private int blinkTimeout = 0; private bool isOn = false; private bool isBlinking = false; private int nBlinkTimes = 0; public MyBlinkingIcon(int x, int y, MySprite Graphics) : base(null, x, y, true) { Sprite = new MyStatefulAnimatedSprite(0, 0) .WithState("Off", new MyStatefulAnimatedSpriteState(new MySprite[]{ Graphics })) .WithState("On" , new MyStatefulAnimatedSpriteState(new MySprite[]{ new MySprite(Graphics.width, Graphics.height, DrawingFrameworkUtils.NegateBoolArray(Graphics.data)) })); AddChild(Sprite); } public MyBlinkingIcon WithBlinkingInterval(int blinkingInterval) { this.blinkingInterval = blinkingInterval; return this; } public override int GetWidth() { return Sprite.GetWidth(); } public override int GetHeight() { return Sprite.GetHeight(); } protected override void Init() { } private void LocalSwitchOn() { Sprite.SetState("On"); isOn = true; } private void LocalSwitchOff() { Sprite.SetState("Off"); isOn = false; } private void LocalSwitch() { if (isOn) { LocalSwitchOff(); } else { LocalSwitchOn(); } } public void SwitchOn() { LocalSwitchOn(); stopBlinking(); } public void SwitchOff() { LocalSwitchOff(); stopBlinking(); } public void Switch() { LocalSwitch(); stopBlinking(); } public void Blink(int nTimes) { SwitchOn(); nBlinkTimes = nTimes; isBlinking = true; blinkTimeout = 0; } public void stopBlinking() { isBlinking = false; nBlinkTimes = 0; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { if (isBlinking) { blinkTimeout++; if (blinkTimeout >= blinkingInterval) { blinkTimeout = 0; LocalSwitch(); nBlinkTimes--; if (nBlinkTimes == 0) { SwitchOff(); } } } }}public class MyCanvas { private bool[] Buffer; private int resX; private int resY; private int length; private MySprite[] DefaultFont; public MyCanvas(int resX, int resY) { this.resX = resX; this.resY = resY; length = resY * resX; Buffer = new bool[length]; this.DefaultFont = DefaultMonospaceFont.BitmapFont; } public bool[] GetBuffer() { return Buffer; } public int GetResX() { return resX; } public int GetResY() { return resY; } public bool[] GetBufferCopy() { return DrawingFrameworkUtils.CopyBoolArray(Buffer, false); } public void SetDefaultFont(MySprite[] DefaultFont) { this.DefaultFont = DefaultFont; } public MySprite[] GetDefaultFont() { return DefaultFont; } public MyCanvas WithDefaultFont(MySprite[] DefaultFont) { SetDefaultFont(DefaultFont); return this; } public void Clear(bool value = false) { Buffer = Enumerable.Repeat(value, length).ToArray(); } private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) { if (invertColors) { if (transparentBackground) { return targetPixelValue && !sourcePixelValue; } else { return !sourcePixelValue; } } else { if (transparentBackground) { return sourcePixelValue || targetPixelValue; } else { return sourcePixelValue; } } } public void BitBlt(MySprite sprite, int x, int y) { BitBltExt(sprite, x, y, false, false); } public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) { if (x < 0 || y < 0) { return; } int screenPos = resX * y + x; int spriteLength = sprite.height * sprite.width; int spritePosX = 0; for (int spritePos = 0; spritePos < spriteLength; spritePos++) { try { Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground); screenPos++; } catch (Exception exc) { } if (screenPos >= length - 1) { return; } spritePosX++; if (spritePosX == sprite.width) { spritePosX = 0; screenPos += resX - sprite.width; } } } public void BitBlt( MySprite Sprite, int cropX1, int cropY1, int cropX2, int cropY2, int x, int y ) { int trgX = x; for (int srcX = cropX1 ; srcX <= cropX2 ; srcX++) { int trgY = y; for (int srcY = cropY1 ; srcY <= cropY2 ; srcY++) { SetPixel(trgX, trgY, Sprite.data[srcY * Sprite.width + srcX]); trgY++;} trgX++;} } public void DrawText(int x, int y, String text) { DrawColorText(x, y, text, false, false); } public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) { if (DefaultFont == null || text == null) { return; } char[] textChars = text.ToCharArray(); int screenPosX = x; int prevSpacing = 7; foreach (char chr in textChars) { MySprite CharSprite = DefaultFont[chr]; if (CharSprite != null) { BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground); prevSpacing = CharSprite.width; } screenPosX += prevSpacing; if (screenPosX >= resX) { return; } } } public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) { int actualX1 = x1 > x2 ? x2 : x1; int actualY1 = y1 > y2 ? y2 : y1; int actualX2 = x1 > x2 ? x1 : x2; int actualY2 = y1 > y2 ? y1 : y2; if (actualX1 < 0) actualX1 = 0; if (actualY1 < 0) actualY1 = 0; if (actualX2 >= resX - 1) actualX2 = resX - 1; if (actualY2 >= resY - 1) actualY2 = resY - 1; int rectWidth = actualX2 - actualX1; int screenPosY = actualY1; while (screenPosY <= actualY2) { int screenPos = screenPosY * resX + actualX1; if (screenPos >= length) { return; } bool targetColor = !invertColors; Buffer[screenPos] = targetColor; Buffer[screenPos + rectWidth - 1] = targetColor; if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) { for (int innerPos = screenPos; innerPos < screenPos + rectWidth; innerPos++) { Buffer[innerPos] = targetColor; } } screenPos += resX; screenPosY++; } } public void DrawLine(int x1, int y1, int x2, int y2, bool color) { if (x1 == x2) { int incY = y1 < y2 ? 1 : -1; for (int y = y1 ; y != y2 ; y += incY) { SetPixel(x1, y, color); } } else { float a = (float)(y2 - y1) / (float)(x2 - x1); float b = ((float)y2 - (a * x2)); int incX = x1 <= x2 ? 1 : -1; for (int x = x1 ; x != x2 ; x += incX) { int y = (int)((a * x) + b); SetPixel(x, y, color); } } } public void SetPixel(int x, int y, bool color) { if (x >= 0 && x < resX - 1 && y >= 0 && y < resY - 1) { Buffer[y * resX + x] = color; } }}public class MyIconLabel : MyOnScreenObject { private MyStatefulAnimatedSprite AnimatedSprite; private MyTextLabel TextLabel; private int width; private int height; private int floatingIconPosition = Constants.FLOATING_POSITION_TOP; private int spacing = 3; public MyIconLabel(int x, int y, string text, MySprite[] Frames) :base(null, x, y, true){ if (text == null) { throw new ArgumentException("The text of the MyIconLabel must not be null"); } if (Frames == null || Frames.Length == 0) { throw new ArgumentException("There has to be at least one frame if the picture is to be displayed by the MyIconLabel"); } int frameWidth = Frames[0].width; int frameHeight = Frames[0].height; foreach (MySprite Frame in Frames) { if (Frame.width != frameWidth || Frame.height != frameHeight) { throw new ArgumentException("All the frames of the MyIconLabel must have the same width and height"); } } AnimatedSprite = new MyStatefulAnimatedSprite(0,0).WithState("Default", new MyStatefulAnimatedSpriteState(Frames)); AddChild(AnimatedSprite); TextLabel = new MyTextLabel(text, 0,0); AddChild(TextLabel); } public MyIconLabel WithFloatingIconPosition(int floatingIconPosition) { this.floatingIconPosition = floatingIconPosition; return this; } public MyIconLabel WithSpaceing(int spacing) { this.spacing = spacing; return this; } public override int GetHeight() { return height; } public override int GetWidth() { return width; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { } protected override void Init() { int spriteWidth = AnimatedSprite.GetWidth(); int spriteHeight = AnimatedSprite.GetHeight(); int textWidth = TextLabel.GetWidth(); int textHeight = TextLabel.GetHeight(); if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { width = spriteWidth + spacing + textWidth; height = spriteHeight > textHeight ? spriteHeight : textHeight; } else { width = spriteWidth > textWidth ? spriteWidth : textWidth; height = spriteHeight + spacing + textHeight; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT) { AnimatedSprite.x = 0; TextLabel.x = spriteWidth + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { AnimatedSprite.x = width - spriteWidth; TextLabel.x = AnimatedSprite.x - spacing - textWidth; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP || floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { AnimatedSprite.x = (width - spriteWidth) / 2; TextLabel.x = (width - textWidth) / 2; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { TextLabel.y = (height - textHeight) / 2; AnimatedSprite.y = (height - spriteHeight) / 2; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP) { AnimatedSprite.y = 0; TextLabel.y = spriteHeight + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { TextLabel.y = 0; AnimatedSprite.y = textHeight + spacing; } }}public class MyList : MyOnScreenObject { private MyPanel Panel; private MyScrollbar Scrollbar; private MyPanel SelectionBackground; private MyOnScreenObject SelectedItem; private bool oneItemPerPage = false; private int selectedItemIndex; private int startPosY; private int padding = 2; private int horizontalAlignment = Constants.HORIZONTAL_ALIGN_LEFT; private List<MyOnScreenObject> Items = new List<MyOnScreenObject>(); public MyList(int x, int y, int width, int height) : base(null, x, y, true) { Panel = new MyPanel(0, 0, width, height); base.AddChild(Panel); Scrollbar = new MyScrollbar(Panel); SelectionBackground = new MyPanel(0, 0, width, 1).WithOptionalParameters(true, true, false); base.AddChild(SelectionBackground); } public MyList WithCustomScrollbarWidth(int scrollbarWidth) { Scrollbar.WithCustomWidth(scrollbarWidth); return this; } public MyList WithOneItemPerPage() { this.oneItemPerPage = true; return this; } public MyList WithPadding(int padding) { this.padding = padding; return this; } public MyList WithHorizontalAlignment(int horizontalAlignment) { this.horizontalAlignment = horizontalAlignment; return this; } public override void AddChild(MyOnScreenObject Item) { base.AddChild(Item); if (SelectedItem == null) { SetInitialSelectedItem(Item); UpdateScrollbarPosition(); } Items.Add(Item); UpdateItemPositions(); } public override void RemoveChild(MyOnScreenObject Item) { base.RemoveChild(Item); if (Item == SelectedItem) { SetInitialSelectedItem(ChildObjects[0]); UpdateScrollbarPosition(); } Items.Remove(Item); UpdateItemPositions(); } private void SetInitialSelectedItem(MyOnScreenObject Item) { SelectedItem = Item; selectedItemIndex = 0; startPosY = padding; } private int ComputeItemHorizontalPosition(MyOnScreenObject Item) { if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_RIGHT) { return GetWidth() - Scrollbar.GetWidth() - padding - Item.GetWidth(); } else if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_CENTER) { return (GetWidth() - Scrollbar.GetWidth() - Item.GetWidth()) / 2; } else { return padding; } } private void UpdateItemPositions() { if (Items.Count() == 0) { return; } if (oneItemPerPage) { foreach (MyOnScreenObject Item in Items) { if (Item == SelectedItem) { Item.isVisible = true; Item.x = ComputeItemHorizontalPosition(Item); Item.y = (Panel.GetHeight() - Item.GetHeight()) / 2; } else { Item.isVisible = false; } Item.invertColors = false; } SelectionBackground.isVisible = false; } else { int listMaxHeight = GetHeight() - (padding * 2); int selItemY = startPosY; for (int idx = 0 ; idx < selectedItemIndex ; idx++) { selItemY += Items[idx].GetHeight(); } if (selItemY < padding) { startPosY += padding - selItemY; } else if (selItemY + SelectedItem.GetHeight() > listMaxHeight) { startPosY -= selItemY + SelectedItem.GetHeight() - listMaxHeight; } int currPosY = startPosY; foreach (MyOnScreenObject Item in Items) { Item.y = currPosY; Item.x = ComputeItemHorizontalPosition(Item); currPosY += Item.GetHeight(); Item.isVisible = Item.y >= padding && Item.y + Item.GetHeight() <= listMaxHeight; Item.invertColors = Item == SelectedItem; } SelectionBackground.x = padding; SelectionBackground.y = SelectedItem.y; SelectionBackground.SetWidth(GetWidth() - Scrollbar.GetWidth() - (padding * 2)); SelectionBackground.SetHeight(SelectedItem.GetHeight()); SelectionBackground.isVisible = true; } } private void UpdateScrollbarPosition() { Scrollbar.SetPosPct(Items.Count() == 0 ? 0f : ((float)selectedItemIndex / ((float)Items.Count() - 1))); } public MyOnScreenObject SelectNextItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex >= 0 && selectedItemIndex < Items.Count() - 1) { selectedItemIndex++; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject SelectPreviousItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex > 0) { selectedItemIndex--; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject GetSelectedItem() { return SelectedItem; } public MyOnScreenObject SelectFirstItem() { SetInitialSelectedItem(Items[0]); return SelectedItem; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { } public override int GetWidth() { return Panel.GetWidth(); } public override int GetHeight() { return Panel.GetHeight(); } public List<MyOnScreenObject> GetItems() { return Items; } protected override void Init() { UpdateItemPositions(); }}public class MyOnScreenApplication { private MyScreen TargetScreen; private List<MyPage> Pages = new List<MyPage>(); private MyPage CurrentPage; private MyCanvas Canvas; private bool autoClearScreen = true; private bool autoFlushBuffer = true; private int currIteration; private readonly int nIterations; private readonly int nComputeIterations; private readonly int nDrawIterations; public MyOnScreenApplication(int nComputeIterations, int nDrawIterations) { currIteration = 0; this.nComputeIterations = nComputeIterations; nIterations = nComputeIterations + nDrawIterations; this.nDrawIterations = nDrawIterations; } public MyOnScreenApplication WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyOnScreenApplication OnScreen(MyScreen TargetScreen) { if (Canvas == null) { throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen()."); } TargetScreen.WithCanvas(Canvas); this.TargetScreen = TargetScreen; return this; } public MyOnScreenApplication WithDefaultPostPage(Func<MyOnScreenApplication,bool> initializationMonitoringFunction) { if (Pages.Count() > 0) { throw new InvalidOperationException("The POST page must be the first page ever added to the application"); } if (Canvas == null) { throw new InvalidOperationException("Please call WithCanvas() before calling WithDefaultPostPage()"); } if (initializationMonitoringFunction == null) { throw new ArgumentException("The initialization monitoring function must be a lambda taking in a MyOnScreenObject and returning a bool"); } MyPage POSTPage = (MyPage) new MyPage() .WithInvertedColors() .WithClientPreDrawMethod((MyCanvas TargetCanvas, int iterationIndex) => { TargetCanvas.Clear(); }) .WithClientCycleMethod((MyOnScreenObject obj, int iterationIndex) => { if (initializationMonitoringFunction(this)) { SwitchToPage(1); } }); this.AddPage(POSTPage); MyPanel TextBackgroundPanel = new MyPanel(0, 0, 2, 2).WithOptionalParameters(true, true, false); POSTPage.AddChild(TextBackgroundPanel); MyTextLabel TextLabel = new MyTextLabel("INITIALIZING", 1, 1).WithOptionalParameters(true, true, true); POSTPage.AddChild(TextLabel); int textLabelWidth = TextLabel.GetWidth(); int textLabelHeight = TextLabel.GetHeight(); TextLabel.x = (Canvas.GetResX() - textLabelWidth) / 2; TextLabel.y = (Canvas.GetResY() - textLabelHeight) / 2 - 3; TextBackgroundPanel.x = TextLabel.x - 3; TextBackgroundPanel.y = TextLabel.y - 2; TextBackgroundPanel.SetWidth(textLabelWidth + 6); TextBackgroundPanel.SetHeight(textLabelHeight + 3); POSTPage.AddChild( new MyPanel(TextBackgroundPanel.x, TextBackgroundPanel.y + TextBackgroundPanel.GetHeight() + 2, 7, 4) .WithOptionalParameters(true, true, false) .WithClientCycleMethod((MyOnScreenObject obj, int iterationIndex) => { obj.x++; if (obj.x > TextBackgroundPanel.x + TextBackgroundPanel.GetWidth() - 7) { obj.x = TextBackgroundPanel.x; } }) ); return this; } public MyOnScreenApplication WithoutAutomaticClear() { autoClearScreen = false; return this; } public MyOnScreenApplication WithoutAutomaticFlush() { autoFlushBuffer = false; return this; } public void AddPage(MyPage Page) { Pages.Add(Page); Page.SetApplication(this); if (CurrentPage == null) { CurrentPage = Page; } } public void SwitchToPage(MyPage Page) { foreach (MyPage Pg in Pages) { if (Pg == Page) { CurrentPage = Pg; CurrentPage.Activate(); } } } public void SwitchToPage(int pageNumber) { if (pageNumber < 0 || pageNumber >= Pages.Count) { return; } CurrentPage = Pages[pageNumber]; CurrentPage.Activate(); } public MyPage GetCurrentPage() { return CurrentPage; } public void Cycle() { if (currIteration < nComputeIterations) { if (autoClearScreen) { Canvas.Clear(); } CurrentPage.Cycle(Canvas, currIteration); } else { if (autoFlushBuffer) { TargetScreen .FlushBufferToScreen( CurrentPage.invertColors, currIteration - nComputeIterations, nDrawIterations ); } } currIteration++; if (currIteration >= nIterations) { currIteration = 0; } } public MyCanvas GetCanvas() { return Canvas; } public MyScreen GetTargetScreen() { return TargetScreen; }}public abstract class MyOnScreenObject { public int x; public int y; public bool isVisible = true; private bool _invertColors = false; public bool invertColors { get { return _invertColors; } set { _invertColors = value; foreach(MyOnScreenObject Child in ChildObjects) { Child.invertColors = value; } } } public MyOnScreenObject ParentObject; public List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>(); private Action<MyOnScreenObject, int> ClientCycleMethod; private Action<MyCanvas, int> ClientDrawMethod, ClientPreDrawMethod; private bool isObjectNotInitialized = true; public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) { this.SetParent(ParentObject); this.x = x; this.y = y; this.isVisible = isVisible; if (ParentObject != null) { ParentObject.AddChild(this); } } public MyOnScreenObject WithClientCycleMethod(Action<MyOnScreenObject, int> ClientCycleMethod) { SetClientCycleMethod(ClientCycleMethod); return this; } public void SetClientCycleMethod(Action<MyOnScreenObject, int> ClientCycleMethod) { this.ClientCycleMethod = ClientCycleMethod; } public MyOnScreenObject WithClientDrawMethod(Action<MyCanvas, int> ClientDrawMethod) { this.ClientDrawMethod = ClientDrawMethod; return this; } public MyOnScreenObject WithClientPreDrawMethod(Action<MyCanvas, int> ClientPreDrawMethod) { this.ClientPreDrawMethod = ClientPreDrawMethod; return this; } public virtual void AddChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Add(ChildObject); ChildObject.SetParent(this); } } public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) { if (ChildObject != null) { ChildObject.SetPosition(x, y); AddChild(ChildObject); } } public void SetPosition(int x, int y) { this.x = x; this.y = y; } public virtual void RemoveChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Remove(ChildObject); ChildObject.RemoveParent(); } } public virtual void SetParent(MyOnScreenObject ParentObject) { if (ParentObject != null) { this.ParentObject = ParentObject; } } public void RemoveParent() { this.ParentObject = null; } public MyOnScreenObject GetTopLevelParent() { if (ParentObject == null) { return this; } return ParentObject.GetTopLevelParent(); } public bool IsObjectVisible() { return isVisible && (ParentObject == null || ParentObject.IsObjectVisible()); } public virtual void Cycle(MyCanvas TargetCanvas, int iterationIndex) { Compute(TargetCanvas, iterationIndex); if (ClientCycleMethod != null) { ClientCycleMethod(this, iterationIndex); } if (ClientPreDrawMethod != null) { ClientPreDrawMethod(TargetCanvas, iterationIndex); } foreach (MyOnScreenObject ChildObject in ChildObjects) { ChildObject.Cycle(TargetCanvas, iterationIndex); } if (isObjectNotInitialized) { Init(); isObjectNotInitialized = false; } if (IsObjectVisible()) { if (ClientDrawMethod != null) { ClientDrawMethod(TargetCanvas, iterationIndex); } Draw(TargetCanvas, iterationIndex); } } public int GetAbsoluteX() { return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX()); } public int GetAbsoluteY() { return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY()); } protected abstract void Compute(MyCanvas TargetCanvas, int iterationIndex); protected abstract void Draw(MyCanvas TargetCanvas, int iterationIndex); public abstract int GetWidth(); public abstract int GetHeight(); protected abstract void Init();}public class MyPage : MyOnScreenObject { private MyOnScreenApplication OnScreenApplication; public MyPage() : base(null, 0, 0, true) { } public void SetApplication(MyOnScreenApplication OnScreenApplication) { this.OnScreenApplication = OnScreenApplication; } public MyPage WithInvertedColors() { this.invertColors = true; return this; } public MyOnScreenApplication GetApplication() { return OnScreenApplication; } public override int GetHeight() { return 0; } public override int GetWidth() { return 0; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { } protected override void Init() { } public virtual void Activate() {}}public class MyPanel : MyOnScreenObject { private int width; private int height; private bool isFilled = false; public MyPanel(int x, int y, int width, int height) : base(null, x, y, true) { this.width = width; this.height = height; this.isFilled = false; } public MyPanel WithOptionalParameters(bool isVisible, bool isFilled, bool invertColors) { this.isVisible = isVisible; this.isFilled = isFilled; this.invertColors = invertColors; return this; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { int absoluteX = GetAbsoluteX(); int absoluteY = GetAbsoluteY(); TargetCanvas.DrawRect( absoluteX, absoluteY, absoluteX + width, absoluteY + height, invertColors, isFilled ); } public override int GetWidth() { return width; } public override int GetHeight() { return height; } public void SetWidth(int width) { this.width = width; } public void SetHeight(int height) { this.height = height; } protected override void Init() { }}public class MyScreen { public IMyTextSurface TargetSurface; private MyCanvas Canvas; private bool mirrorX; private StringBuilder RenderedBuffer; private char pixelValueOn, pixelValueOff; private int clipRectX1, clipRectY1, clipRectX2, clipRectY2; private bool isClipping = false; public MyScreen(IMyTextSurface TargetSurface, char pixelValueOn, char pixelValueOff, bool mirrorX) { this.TargetSurface = TargetSurface; this.mirrorX = mirrorX; this.pixelValueOn = pixelValueOn; this.pixelValueOff = pixelValueOff; } public MyScreen WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) { clipRectX1 = x1 > x2 ? x2 : x1; clipRectY1 = y1 > y2 ? y2 : y1; clipRectX2 = x1 > x2 ? x1 : x2; clipRectY2 = y1 > y2 ? y1 : y2; isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY(); return this; } private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) { int length = Buffer.Count(); bool[] MirroredBuffer = new bool[length]; int mirrorPosX = resX - 1; int mirrorPos = mirrorPosX; for (int sourcePos = 0; sourcePos < length; sourcePos++) { MirroredBuffer[mirrorPos] = Buffer[sourcePos]; mirrorPos--; mirrorPosX--; if (mirrorPosX == -1) { mirrorPosX = resX - 1; mirrorPos += resX * 2; } } return MirroredBuffer; } private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) { int rectX1 = x1 > x2 ? x2 : x1; int rectY1 = y1 > y2 ? y2 : y1; int rectX2 = x1 > x2 ? x1 : x2; int rectY2 = y1 > y2 ? y1 : y2; if (rectX1 < 0) rectX1 = 0; if (rectY1 < 0) rectY1 = 0; if (rectX2 > resX) rectX2 = resX; if (rectY2 > resY) rectY2 = resY; bool[] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1]; int srcCursor = rectY1 * resX + rectX1; int trgCursor = 0; for (int srcY = rectY1; srcY < rectY2; srcY++) { for (int srcX = rectX1; srcX < rectX2; srcX++) { ret[trgCursor] = Buffer[srcCursor]; ret[trgCursor] = true; trgCursor++; srcCursor++; } srcCursor += rectX1; } return ret; } public void FlushBufferToScreen(bool invertColors, int currentBlock, int nBlocks) { bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer(); int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX(); int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY(); bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer; int blockSize = (resY / nBlocks); int lowY = currentBlock * blockSize; int highY = (currentBlock == nBlocks - 1) ? resY - 1 : lowY + blockSize; int capacity = resY * resX + resY + 1; if (currentBlock == 0) { if (RenderedBuffer != null) { TargetSurface.WriteText(RenderedBuffer); } RenderedBuffer = new StringBuilder(capacity); RenderedBuffer.EnsureCapacity(capacity); } char pxValOn = invertColors ? pixelValueOff : pixelValueOn; char pxValOff = invertColors ? pixelValueOn : pixelValueOff; for (int y = lowY ; y < highY ; y++) { for (int x = 0 ; x < resX ; x++) { RenderedBuffer.Append(SourceBuffer[y * resX + x] ? pxValOn : pxValOff); } RenderedBuffer.Append('\n'); } } public MyCanvas GetCanvas() { return Canvas; }}public class MyScrollbar : MyOnScreenObject { private int width = 7; private int height = 10; private float posPct = 0.5f; private bool snapToParent = true; public MyScrollbar(MyOnScreenObject ParentObject) : base(ParentObject, 0, 0, true) { } public MyScrollbar DetachedFromParent(int height) { this.snapToParent = false; this.height = height; return this; } public MyScrollbar WithCustomWidth(int width) { this.width = width; return this; } public MyScrollbar AtCoordinates(int x, int y) { this.x = x; this.y = y; return this; } public void SetPosPct(float posPct) { this.posPct = posPct; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { int x1 = snapToParent ? ParentObject.GetAbsoluteX() + ResolveClientX() : GetAbsoluteX(); int y1 = snapToParent ? ParentObject.GetAbsoluteY() : GetAbsoluteY(); int x2 = x1 + width; int actualHeight = GetHeight(); int y2 = y1 + actualHeight; TargetCanvas.DrawRect(x1, y1, x2, y2, invertColors, false); int sliderX = x1 + 1; int sliderY = (int)(y1 + 1 + (posPct * ((actualHeight - 5 - 2)))); TargetCanvas.BitBltExt( StockSprites.SCROLLBAR_SLIDER, sliderX, sliderY, invertColors ? (!this.invertColors) : this.invertColors, false ); } private int ResolveHight() { if (ParentObject is MyPanel) { return ((MyPanel)ParentObject).GetHeight(); } return height; } private int ResolveClientX() { if (ParentObject is MyPanel) { return ParentObject.GetWidth() - this.width; } return 0; } public override int GetWidth() { return width; } public override int GetHeight() { return snapToParent ? ResolveHight() : height; } protected override void Init() { }}public class MySprite { public int width; public int height; public bool[] data; public MySprite(int width, int height, bool[] data) { this.width = width; this.height = height; this.data = data; }}public class MyStatefulAnimatedSprite : MyOnScreenObject { private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>(); private MyStatefulAnimatedSpriteState CurrentState; private bool isBackgroundTransparet = true; private MySprite CurrentFrame; public MyStatefulAnimatedSprite(int x, int y) : base(null, x, y, true) { } public MyStatefulAnimatedSprite WithState(string stateName, MyStatefulAnimatedSpriteState State) { if (stateName == null) { throw new ArgumentException("Each state must have a name"); } if (State == null) { throw new ArgumentException("The state may not be null"); } States.Add(stateName, State); if (CurrentState == null) { SetStateObject(State); } return this; } public void SetState(String stateName) { if (stateName == null) { return; } MyStatefulAnimatedSpriteState State; States.TryGetValue(stateName, out State); if (State != null) { SetStateObject(State); } } private void SetStateObject(MyStatefulAnimatedSpriteState State) { CurrentState = State; isBackgroundTransparet = State.IsBackgroundTransparent(); } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { CurrentFrame = CurrentState.GetFrame(); } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { TargetCanvas.BitBltExt( CurrentFrame, GetAbsoluteX(), GetAbsoluteY(), invertColors, CurrentState.IsBackgroundTransparent() ); } public override int GetWidth() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.width; } } public override int GetHeight() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.height; } } protected override void Init() { } public MyStatefulAnimatedSpriteState GetState(string stateName) { if (stateName == null || stateName.Length == 0) { throw new ArgumentException("Invalid state name"); } MyStatefulAnimatedSpriteState ret = null; States.TryGetValue(stateName, out ret); if (ret == null) { throw new ArgumentException("Cannot find a state named [" + stateName + "]"); } return ret; }}public class MyStatefulAnimatedSpriteState { private MySprite[] Frames; private int nFrames; private int currFrame = 0; private bool transparentBackground = true; public MyStatefulAnimatedSpriteState(MySprite[] Frames) { if (Frames == null || Frames.Count() == 0) { throw new ArgumentException("The frames array must have at least one frame"); } this.Frames = Frames; this.nFrames = Frames.Count(); } public bool IsBackgroundTransparent() { return transparentBackground; } public MyStatefulAnimatedSpriteState WithOpaqueBackground() { transparentBackground = false; return this; } public MySprite GetFrame() { MySprite ret = Frames[currFrame]; currFrame++; if (currFrame >= nFrames) { currFrame = 0; } return ret; }}public class MyTextLabel : MyOnScreenObject { private String text; private bool transparentBackground; private MySprite[] Font; private int padding = 1; public MyTextLabel(string text, int x, int y) : base(null, x, y, true) { this.text = text; this.transparentBackground = true; } public MyTextLabel WithOptionalParameters(bool isVisible, bool invertColors, bool transparentBackground) { this.isVisible = isVisible; this.invertColors = invertColors; this.transparentBackground = transparentBackground; return this; } public MyTextLabel WithCustomFont(MySprite[] CustomFont) { this.Font = CustomFont; return this; } public MyTextLabel WithPadding(int padding) { this.padding = padding; return this; } public void SetText(string text) { this.text = text; } protected override void Compute(MyCanvas TargetCanvas, int currIteration) { } protected override void Draw(MyCanvas TargetCanvas, int currIteration) { if (Font == null) { Font = TargetCanvas.GetDefaultFont(); } TargetCanvas.DrawColorText( GetAbsoluteX() + padding, GetAbsoluteY() + padding, text, invertColors, transparentBackground ); } private MySprite[] ResolveFont() { if (Font == null) { MyOnScreenObject TopLevelParent = GetTopLevelParent(); if (TopLevelParent is MyPage) { return ((MyPage) TopLevelParent).GetApplication().GetCanvas().GetDefaultFont(); } else { return null; } } else { return Font; } } public override int GetWidth() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].width * text.Length + (2 * padding); } public override int GetHeight() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].height + (2 * padding); } protected override void Init() { }}public class StockSprites { private const bool O = true; private const bool _ = false; public static MySprite SPRITE_PWR = new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0x02,0,0x1a,0xc0,0x22,0x20,0x22,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x1f,0xc0,0,0 })); public static MySprite SPRITE_UP = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_DOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_LEFTRIGHT = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0d,0x80,0x1d,0xc0,0x3d,0xe0,0x7d,0xf0,0x3d,0xe0,0x1d,0xc0,0x0d,0x80,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_UPDOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0,0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0 })), 14, 10, 0, 0); public static MySprite SPRITE_REVERSE = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0f,0xe0,0x10,0x10,0x7c,0x10,0x38,0x10,0x10,0x10,0,0x10,0x10,0x10,0x0f,0xe0 })), 14, 10, 0, 0); public static MySprite SCROLLBAR_SLIDER = new MySprite(5, 5, new bool[] { _,O,O,O,_, O,O,O,O,O, O,O,_,_,O, O,O,_,_,O, _,O,O,O,_ }); public static MySprite SPRITE_SMILE_SAD = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0x7e,0,0xc0,0,0xc1,0x81, 0x81,0x80,0,0x62,0,0x43,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_NEUTRAL = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0,0,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_HAPPY = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x82,0,0x40,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0x7e,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 }));}public class TerminalUtils { public static IMyTextSurface FindTextSurface(IMyGridTerminalSystem MyGridTerminalSystem, String blockName, int surfaceIndex) { List<IMyTerminalBlock> allFoundBlocks = GetAllBlocksWithName(MyGridTerminalSystem, blockName); foreach (IMyTerminalBlock block in allFoundBlocks) { if (block is IMyTextSurfaceProvider) { IMyTextSurface Surface = ((IMyTextSurfaceProvider)block).GetSurface(surfaceIndex); if (Surface != null) { return Surface; } } } throw new ArgumentException("Could not get text surface nbr [" + surfaceIndex + "] of [" + blockName + "]."); } public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { List<IMyTerminalBlock> allFoundBlocks = GetAllBlocksWithName(MyGridTerminalSystem, blockName); foreach (IMyTerminalBlock block in allFoundBlocks) { if (block is T) { return (T)block; } } return default(T); } private static List<IMyTerminalBlock> GetAllBlocksWithName(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { if (blockName == null || blockName.Length == 0) { throw new ArgumentException("Invalid block name"); } List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>(); MyGridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks); if (allFoundBlocks == null || allFoundBlocks.Count == 0) { throw new ArgumentException("Cannot find a block named [" + blockName + "]"); } return allFoundBlocks; } public static void SetupTextSurfaceForMatrixDisplay( IMyGridTerminalSystem GridTerminalSystem, string blockName, int surfaceIndex, float fontSize ) { IMyTextSurface TextSurface = TerminalUtils.FindTextSurface(GridTerminalSystem, blockName,surfaceIndex); if (TextSurface == null) { throw new ArgumentException("Cannot find a text panel named [" + blockName + "]"); } TextSurface.ContentType = ContentType.TEXT_AND_IMAGE; TextSurface.FontSize = fontSize; TextSurface.TextPadding = 0; TextSurface.Font = "Monospace"; TextSurface.Alignment = TextAlignment.LEFT; }}public class UiFrameworkUtils { private const char SCREEN_PIXEL_VALUE_ON = '@'; private const char SCREEN_PIXEL_VALUE_OFF = ' '; public static MyOnScreenApplication InitSingleScreenApplication( IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int surfaceIndex, int resX, int resY, bool mirrorX, int nComputeIterations = 1, int nDrawIterations = 1 ) { return new MyOnScreenApplication(nComputeIterations, nDrawIterations) .WithCanvas( new MyCanvas(resX, resY) ) .OnScreen( new MyScreen( TerminalUtils.FindTextSurface(MyGridTerminalSystem, textPanelName, surfaceIndex), SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF, mirrorX ) ); }
