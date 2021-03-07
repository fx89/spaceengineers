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




// README //////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIGURATION ///////////////////////////////////////////////////////////////////////////////////////////////////

// Name of the 1x1 LCD panel to which the view should draw
private const string TARGET_LCD_PANEL_NAME = "3D_RENDERING_SCREEN";

// false = white foreground on black background
// true  = black foreground on white background
private static bool INVERT_COLORS = false;

// For how many frames should the script display the "INITIALIZING" message on screen
private const int POST_SCREEN_DURATION = 10;

// Distance of the rendered object from the view
//    --- increase this if you get the "Script too Complex" error while rendering
private const double MODEL_DISTANCE_FROM_VIEW = 6;

// Set this to true to re-compute the object's center after loading
private static bool RECENTER_OBJECT_AFTER_LOADING = true;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.10d;
private const double ROT_SPEED_RAD_PITCH = 0.05d;
private const double ROT_SPEED_RAD_ROLL  = 0.00d;

// Initial rotation angles in radians
private const double INITIAL_ROTATION_RAD_YAW   = 0.00d;
private const double INITIAL_ROTATION_RAD_PITCH = 1.65d;
private const double INITIAL_ROTATION_RAD_ROLL  = 0.00d;


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// WAVEFRONT OBJ LOADING ///////////////////////////////////////////////////////////////////////////////////////////

// This is a simple format which allows defining simple geometry, such as vertices and faces, in text mode.
// This format is available for export in almost every 3D graphics software and it's also easy to understand
// and parse. For more information on the Wavefront OBJ format, please visit this link:
//          https://en.wikipedia.org/wiki/Wavefront_.obj_file

private void InitSprites1() {

// IMPORTANT NOTE:
// ========================================================
// Make sure that none of the lines has leading spaces. The
// parser does not handle leading spaces to avoid running
// into the "Script too Complex" error.
//
ObjFileLines



// REPLACE THIS DATA WITH THE VERTICES AND FACES DEFINED IN YOUR WAVEFRONT OBJ FILE
// DO NOT DELETE THE = @"
= @"
v -0.766712 1.422585 1.950602
v -0.738580 2.692918 1.950602
v -1.000000 1.422585 -0.049398
v -0.738580 2.692918 -0.049398
v 0.766712 1.422585 1.950602
v 0.738580 2.692918 1.950602
v 1.000000 1.422585 -0.049398
v 0.738580 2.692918 -0.049398
v -0.510674 1.540333 2.481572
v -0.534977 2.575170 2.481572
v 0.534977 2.575170 2.481572
v 0.510674 1.540333 2.481572
v -0.522722 2.634621 -0.677365
v -0.522722 1.482215 -0.273197
v 0.522722 1.482215 -0.273197
v 0.522722 2.634621 -0.677365
v -0.501277 2.221321 -2.720361
v -0.501277 1.584532 -2.720361
v 0.501277 1.584532 -2.720361
v 0.501277 2.221321 -2.720361
v -0.443413 2.176197 -2.901042
v -0.443413 1.587400 -2.901042
v 0.443413 1.587400 -2.901042
v 0.443413 2.176197 -2.901042
v -0.171831 1.789464 -3.525297
v -0.171831 1.571181 -3.525297
v 0.171831 1.571181 -3.525297
v 0.171831 1.789464 -3.525297
v -1.000000 1.846029 1.950602
v -1.000000 2.269474 1.950602
v -1.000000 2.269474 -0.049398
v -1.000000 1.846029 -0.049398
v 1.000000 2.269474 -0.049398
v 1.000000 1.846029 -0.049398
v 1.000000 2.269474 1.950602
v 1.000000 1.846029 1.950602
v -0.814619 1.885279 2.481572
v -0.814619 2.230224 2.481572
v 0.814619 2.230224 2.481572
v 0.814619 1.885279 2.481572
v -0.522722 2.162125 -0.399273
v -0.522722 1.953378 -0.325667
v 0.522722 2.162125 -0.399273
v 0.522722 1.953378 -0.325667
v -0.501277 2.009058 -2.720361
v -0.501277 1.796795 -2.720361
v 0.501277 2.009058 -2.720361
v 0.501277 1.796795 -2.720361
v -0.443413 1.979932 -2.901042
v -0.443413 1.783666 -2.901042
v 0.443413 1.979932 -2.901042
v 0.443413 1.783666 -2.901042
v -0.171831 1.716703 -3.525297
v -0.171831 1.643942 -3.525297
v 0.171831 1.716703 -3.525297
v 0.171831 1.643942 -3.525297
v -2.012159 1.819696 1.815113
v -2.012159 2.243140 1.815113
v -2.012159 2.243140 0.215111
v -2.012159 1.819696 0.215111
v 2.012159 2.243140 0.215111
v 2.012159 1.819696 0.215111
v 2.012159 2.243140 1.815113
v 2.012159 1.819696 1.815113
v -3.368881 1.632581 1.585764
v -3.368881 1.948644 1.585764
v -3.368881 1.948643 0.669593
v -3.368881 1.632581 0.669593
v 3.368881 1.948643 0.669593
v 3.368881 1.632581 0.669593
v 3.368881 1.948643 1.585764
v 3.368881 1.632581 1.585764
v -4.092979 1.517624 1.438508
v -4.092979 1.623268 1.438508
v -4.092979 1.623267 0.950850
v -4.092979 1.517624 0.950850
v 4.092979 1.623267 0.950850
v 4.092979 1.517624 0.950850
v 4.092979 1.623267 1.438508
v 4.092979 1.517624 1.438508
f 30/1/1 2/2/1 4/3/1 31/4/1
f 34/5/2 7/6/2 15/7/2 44/8/2
f 33/9/3 8/10/3 6/11/3 35/12/3
f 29/13/4 1/14/4 9/15/4 37/16/4
f 3/17/5 7/6/5 5/18/5 1/19/5
f 8/10/6 4/20/6 2/21/6 6/11/6
f 39/22/7 11/23/7 10/24/7 38/25/7
f 1/19/8 5/18/8 12/26/8 9/27/8
f 6/11/9 2/21/9 10/28/9 11/23/9
f 35/12/10 6/11/10 11/23/10 39/22/10
f 44/8/11 15/7/11 19/29/11 48/30/11
f 7/6/12 3/17/12 14/31/12 15/7/12
f 4/20/13 8/10/13 16/32/13 13/33/13
f 31/4/14 4/3/14 13/34/14 41/35/14
f 17/36/15 20/37/15 24/38/15 21/39/15
f 15/7/16 14/31/16 18/40/16 19/29/16
f 13/33/17 16/32/17 20/37/17 17/36/17
f 41/35/18 13/34/18 17/41/18 45/42/18
f 52/43/19 23/44/19 27/45/19 56/46/19
f 45/42/20 17/41/20 21/47/20 49/48/20
f 48/30/21 19/29/21 23/44/21 52/43/21
f 19/29/22 18/40/22 22/49/22 23/44/22
f 53/50/23 25/51/23 28/52/23 55/53/23
f 23/44/24 22/49/24 26/54/24 27/45/24
f 21/39/25 24/38/25 28/52/25 25/55/25
f 49/48/26 21/47/26 25/51/26 53/50/26
f 22/56/26 50/57/26 54/58/26 26/59/26
f 50/57/26 49/48/26 53/50/26 54/58/26
f 26/59/23 54/58/23 56/46/23 27/45/23
f 54/58/23 53/50/23 55/53/23 56/46/23
f 20/37/21 47/60/21 51/61/21 24/38/21
f 47/60/21 48/30/21 52/43/21 51/61/21
f 18/62/20 46/63/20 50/57/20 22/56/20
f 46/63/20 45/42/20 49/48/20 50/57/20
f 24/38/19 51/61/19 55/53/19 28/52/19
f 51/61/19 52/43/19 56/46/19 55/53/19
f 14/64/27 42/65/27 46/63/27 18/62/27
f 42/65/28 41/35/28 45/42/28 46/63/28
f 3/66/29 32/67/29 42/65/29 14/64/29
f 32/67/30 31/4/30 41/35/30 42/65/30
f 16/32/31 43/68/31 47/60/31 20/37/31
f 43/68/32 44/8/32 48/30/32 47/60/32
f 5/18/33 36/69/33 40/70/33 12/26/33
f 36/69/34 35/12/34 39/22/34 40/70/34
f 12/26/7 40/70/7 37/71/7 9/72/7
f 40/70/7 39/22/7 38/25/7 37/71/7
f 2/2/35 30/1/35 38/73/35 10/74/35
f 30/1/36 29/13/36 37/16/36 38/73/36
f 7/6/37 34/5/37 36/69/37 5/18/37
f 33/9/38 35/12/38 63/75/38 61/76/38
f 8/10/39 33/9/39 43/68/39 16/32/39
f 33/9/40 34/5/40 44/8/40 43/68/40
f 1/14/41 29/13/41 32/67/41 3/66/41
f 30/1/42 31/4/42 59/77/42 58/78/42
f 59/77/43 60/79/43 68/80/43 67/81/43
f 63/75/44 64/82/44 72/83/44 71/84/44
f 35/12/45 36/69/45 64/82/45 63/75/45
f 34/5/46 33/9/46 61/76/46 62/85/46
f 31/4/47 32/67/47 60/79/47 59/77/47
f 29/13/48 30/1/48 58/78/48 57/86/48
f 32/67/49 29/13/49 57/86/49 60/79/49
f 36/69/50 34/5/50 62/85/50 64/82/50
f 69/87/51 71/84/51 79/88/51 77/89/51
f 66/90/52 67/81/52 75/91/52 74/92/52
f 60/79/53 57/86/53 65/93/53 68/80/53
f 64/82/54 62/85/54 70/94/54 72/83/54
f 62/85/55 61/76/55 69/87/55 70/94/55
f 57/86/56 58/78/56 66/90/56 65/93/56
f 58/78/57 59/77/57 67/81/57 66/90/57
f 61/76/58 63/75/58 71/84/58 69/87/58
f 78/95/59 77/89/59 79/88/59 80/96/59
f 73/97/60 74/92/60 75/91/60 76/98/60
f 71/84/61 72/83/61 80/96/61 79/88/61
f 67/81/62 68/80/62 76/98/62 75/91/62
f 68/80/63 65/93/63 73/97/63 76/98/63
f 72/83/64 70/94/64 78/95/64 80/96/64
f 70/94/65 69/87/65 77/89/65 78/95/65
f 65/93/66 66/90/66 74/92/66 73/97/66
"
// DO NOT DELETE THE DOUBLE QUOTES AT THE END
// END OF OBJ FILE //////////////////////////////////////////////////



.Split('\n');
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
    if (RECENTER_OBJECT_AFTER_LOADING) {
        Obj3D.Recenter();
    }

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

    private int iterationNumber = 0;

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

    protected override void Compute(MyCanvas TargetCanvas) {
        
    }

    protected override void Draw(MyCanvas TargetCanvas) {
        if (AttachedModel != null) {
         // Splitting the operation across multiple iterations,
         // to avoid the "Script too Complex" error.
            switch (iterationNumber) {
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

             // The third operation flushes the buffer of the target canvas onto
             // the target screen
                case 2:
                    TargetScreen.FlushBufferToScreen(invertScreenColors);
                    break;

                default:
                    break;
            }

         // Go to the next iteration number
            iterationNumber++;
            if (iterationNumber == 3) {
                iterationNumber = 0;
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

        ret.X = CENTER_X + (((Vertex.X + AttachedModel.Position.X) / z) * SCALE_X) + (z * 30 / SCALE_X);
        ret.Y = CENTER_Y + (((Vertex.Y + AttachedModel.Position.Y) / z) * SCALE_Y) + (z * 30 / SCALE_Y);

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
        double halfPoint = (highVal - lowVal) / 2;
        return halfPoint - highVal;
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

            char componentType = line.Length == 0 ? 'x' : line[0];

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

// This is the resolution of the matrix on the target screen
private const int RES_X = 139, RES_Y =  93;

// This is the String array resulted from loading the Wavefront OBJ
private String[] ObjFileLines;

// The 3D object, which is loaded across multiple iterations to avoid the "Script too Complex" error
private MySimple3DObject Obj3D;

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

/**
 * This is the 3D model view which will rendered the 3D object onto
 * the target LCD panel. It is initialized in the InitSprites1() method.
 */
private My3DModelView TheModelView;

/**
 * Counter for the POST page
 */
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
    TerminalUtils.SetupTextPanelForMatrixDisplay(GridTerminalSystem, TARGET_LCD_PANEL_NAME, 0.190f);

 // Initialized the application with the default POST page
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, TARGET_LCD_PANEL_NAME, RES_X, RES_Y, false)
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after 100 frames
            currFrame++;
            return currFrame >= POST_SCREEN_DURATION;
        })
        .WithoutAutomaticClear() // Don't clear the buffer automatically  - this will only make the 3D model blink
        .WithoutAutomaticFlush() // Don't update the screen automatically - this will also make the 3D model blink
        ;

 // Initialize the main page and add it to the application
    MyPage MainPage = new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Optionally invert the colors of the main page
    if (INVERT_COLORS) {
        MainPage.WithInvertedColors();
    }

 // Create the model view
    TheModelView = new My3DModelView(OnScreenApplication.GetTargetScreens()[0], INVERT_COLORS)
        .WithRotationSpeeds(ROT_SPEED_RAD_YAW, ROT_SPEED_RAD_PITCH, ROT_SPEED_RAD_ROLL);

 // Attach the 3D object to the model view
    TheModelView.AttachModel(Obj3D);
    TheModelView.SetAttachedModelPosition(0, 0, MODEL_DISTANCE_FROM_VIEW);

 // Add the model view to the main page
    MainPage.AddChild(TheModelView);
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
    if (initStepNbr < 6) {
        initStepNbr++;
        if (initStepNbr == 1) InitSprites1();
        if (initStepNbr == 2) InitSprites2();
        if (initStepNbr == 3) InitSprites3();
        if (initStepNbr == 4) InitSprites4();
        if (initStepNbr == 5) InitSprites5();
        if (initStepNbr == 6) InitApplication();
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

