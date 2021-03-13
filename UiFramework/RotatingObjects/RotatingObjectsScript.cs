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
private const double MODEL_DISTANCE_FROM_VIEW = 5.0d;

// Set this to true to re-compute the object's center after loading
private static bool RECENTER_OBJECT_AFTER_LOADING = true;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.015d;
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
v 1.000000 1.000000 -1.000000
v 1.000000 -1.000000 -1.000000
v 1.000000 1.000000 1.000000
v 1.000000 -1.000000 1.000000
v -1.000000 1.000000 -1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 1.000000 1.000000
v -1.000000 -1.000000 1.000000
v -1.000000 -1.000000 -0.333333
v -1.000000 -1.000000 0.333333
v 1.000000 1.000000 -0.333333
v 1.000000 1.000000 0.333333
v -1.000000 1.000000 0.333333
v -1.000000 1.000000 -0.333333
v 1.000000 -1.000000 0.333333
v 1.000000 -1.000000 -0.333333
v 0.333333 -1.000000 -1.000000
v -0.333333 -1.000000 -1.000000
v 0.333333 1.000000 1.000000
v -0.333333 1.000000 1.000000
v -0.333333 -1.000000 1.000000
v 0.333333 -1.000000 1.000000
v -0.333333 1.000000 -1.000000
v 0.333333 1.000000 -1.000000
v 0.333333 -1.000000 -0.333333
v -0.333333 -1.000000 -0.333333
v 0.333333 -1.000000 0.333333
v -0.333333 -1.000000 0.333333
v -0.333333 1.000000 -0.333333
v 0.333333 1.000000 -0.333333
v -0.333333 1.000000 0.333333
v 0.333333 1.000000 0.333333
v 1.000000 0.333333 -1.000000
v 1.000000 -0.333333 -1.000000
v -1.000000 -0.333333 1.000000
v -1.000000 0.333333 1.000000
v 1.000000 0.333333 1.000000
v 1.000000 -0.333333 1.000000
v -1.000000 0.333333 -1.000000
v -1.000000 -0.333333 -1.000000
v 1.000000 0.333333 -0.333333
v 1.000000 -0.333333 -0.333333
v 1.000000 0.333333 0.333333
v 1.000000 -0.333333 0.333333
v -1.000000 0.333333 0.333333
v -1.000000 -0.333333 0.333333
v -1.000000 0.333333 -0.333333
v -1.000000 -0.333333 -0.333333
v -0.333333 0.333333 -1.000000
v -0.333333 -0.333333 -1.000000
v 0.333333 0.333333 -1.000000
v 0.333333 -0.333333 -1.000000
v 0.333333 0.333333 1.000000
v 0.333333 -0.333333 1.000000
v -0.333333 0.333333 1.000000
v -0.333333 -0.333333 1.000000
v 1.000000 0.289372 -0.289372
v 1.000000 -0.289372 -0.289372
v 1.000000 0.289372 0.289372
v 1.000000 -0.289372 0.289372
v 1.000000 0.250434 -0.250434
v 1.000000 -0.250434 -0.250434
v 1.000000 0.250434 0.250434
v 1.000000 -0.250434 0.250434
v 1.000000 0.205577 -0.205577
v 1.000000 -0.205577 -0.205577
v 1.000000 0.205577 0.205577
v 1.000000 -0.205577 0.205577
v 1.000000 0.157181 -0.157181
v 1.000000 -0.157181 -0.157181
v 1.000000 0.157181 0.157181
v 1.000000 -0.157181 0.157181
v 1.000000 0.117845 -0.117845
v 1.000000 -0.117845 -0.117845
v 1.000000 0.117845 0.117845
v 1.000000 -0.117845 0.117845
v 1.000000 0.080535 -0.080535
v 1.000000 -0.080535 -0.080535
v 1.000000 0.080535 0.080535
v 1.000000 -0.080535 0.080535
v 1.000000 0.049049 -0.049049
v 1.000000 -0.049049 -0.049049
v 1.000000 0.049049 0.049049
v 1.000000 -0.049049 0.049049
f 31/1/1 13/2/1 7/3/1 20/4/1
f 55/5/2 20/6/2 7/7/2 36/8/2
f 47/9/3 14/10/3 5/11/3 39/12/3
f 27/13/4 15/14/4 4/15/4 22/16/4
f 43/17/5 12/18/5 3/19/5 37/20/5
f 51/21/6 24/22/6 1/23/6 33/24/6
f 33/24/5 1/23/5 11/25/5 41/26/5
f 41/26/5 11/25/5 12/18/5 43/17/5
f 17/27/4 2/28/4 16/29/4 25/30/4
f 25/30/4 16/29/4 15/14/4 27/13/4
f 36/31/3 7/32/3 13/33/3 45/34/3
f 45/34/3 13/33/3 14/10/3 47/9/3
f 23/35/1 5/36/1 14/37/1 29/38/1
f 29/38/1 14/37/1 13/2/1 31/1/1
f 11/25/1 30/39/1 32/40/1 12/18/1
f 30/39/1 29/38/1 31/1/1 32/40/1
f 1/23/1 24/41/1 30/39/1 11/25/1
f 24/41/1 23/35/1 29/38/1 30/39/1
f 9/42/4 26/43/4 28/44/4 10/45/4
f 26/43/4 25/30/4 27/13/4 28/44/4
f 6/46/4 18/47/4 26/43/4 9/42/4
f 18/47/4 17/27/4 25/30/4 26/43/4
f 39/12/6 5/11/6 23/48/6 49/49/6
f 49/49/6 23/48/6 24/22/6 51/21/6
f 10/45/4 28/44/4 21/50/4 8/51/4
f 28/44/4 27/13/4 22/16/4 21/50/4
f 37/20/2 3/19/2 19/52/2 53/53/2
f 53/53/2 19/52/2 20/6/2 55/5/2
f 12/18/1 32/40/1 19/54/1 3/19/1
f 32/40/1 31/1/1 20/4/1 19/54/1
f 22/55/2 54/56/2 56/57/2 21/58/2
f 54/56/2 53/53/2 55/5/2 56/57/2
f 4/15/2 38/59/2 54/56/2 22/55/2
f 38/59/2 37/20/2 53/53/2 54/56/2
f 18/60/6 50/61/6 52/62/6 17/63/6
f 50/61/6 49/49/6 51/21/6 52/62/6
f 6/64/6 40/65/6 50/61/6 18/60/6
f 40/65/6 39/12/6 49/49/6 50/61/6
f 10/66/3 46/67/3 48/68/3 9/69/3
f 46/67/3 45/34/3 47/9/3 48/68/3
f 8/70/3 35/71/3 46/67/3 10/66/3
f 35/71/3 36/31/3 45/34/3 46/67/3
f 16/29/5 42/72/5 44/73/5 15/14/5
f 41/26/5 43/17/5 59/74/5 57/75/5
f 2/28/5 34/76/5 42/72/5 16/29/5
f 34/76/5 33/24/5 41/26/5 42/72/5
f 17/63/6 52/62/6 34/76/6 2/28/6
f 52/62/6 51/21/6 33/24/6 34/76/6
f 15/14/5 44/73/5 38/59/5 4/15/5
f 44/73/5 43/17/5 37/20/5 38/59/5
f 9/69/3 48/68/3 40/65/3 6/64/3
f 48/68/3 47/9/3 39/12/3 40/65/3
f 21/58/2 56/57/2 35/77/2 8/78/2
f 56/57/2 55/5/2 36/8/2 35/77/2
f 60/79/5 58/80/5 62/81/5 64/82/5
f 43/17/5 44/73/5 60/79/5 59/74/5
f 42/72/5 41/26/5 57/75/5 58/80/5
f 44/73/5 42/72/5 58/80/5 60/79/5
f 61/83/5 63/84/5 67/85/5 65/86/5
f 58/80/5 57/75/5 61/83/5 62/81/5
f 57/75/5 59/74/5 63/84/5 61/83/5
f 59/74/5 60/79/5 64/82/5 63/84/5
f 68/87/5 66/88/5 70/89/5 72/90/5
f 63/84/5 64/82/5 68/87/5 67/85/5
f 64/82/5 62/81/5 66/88/5 68/87/5
f 62/81/5 61/83/5 65/86/5 66/88/5
f 70/89/5 69/91/5 73/92/5 74/93/5
f 66/88/5 65/86/5 69/91/5 70/89/5
f 65/86/5 67/85/5 71/94/5 69/91/5
f 67/85/5 68/87/5 72/90/5 71/94/5
f 73/92/5 75/95/5 79/96/5 77/97/5
f 69/91/5 71/94/5 75/95/5 73/92/5
f 71/94/5 72/90/5 76/98/5 75/95/5
f 72/90/5 70/89/5 74/93/5 76/98/5
f 80/99/5 78/100/5 82/101/5 84/102/5
f 75/95/5 76/98/5 80/99/5 79/96/5
f 76/98/5 74/93/5 78/100/5 80/99/5
f 74/93/5 73/92/5 77/97/5 78/100/5
f 82/101/5 81/103/5 83/104/5 84/102/5
f 78/100/5 77/97/5 81/103/5 82/101/5
f 77/97/5 79/96/5 83/104/5 81/103/5
f 79/96/5 80/99/5 84/102/5 83/104/5"
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
    if (RECENTER_OBJECT_AFTER_LOADING) {
        Obj3D.Recenter();
    }

 // Set the initial angle of the object
    Obj3D.Rotate(INITIAL_ROTATION_RAD_YAW, INITIAL_ROTATION_RAD_PITCH, INITIAL_ROTATION_RAD_ROLL);
}

private void InitSprites6(){}
private void InitSprites7(){}

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

