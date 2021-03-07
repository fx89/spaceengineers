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

private const string TARGET_LCD_PANEL_NAME = "3D_RENDERING_SCREEN";
private static bool INVERT_COLORS = false;
private const int POST_SCREEN_DURATION = 10;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// WAVEFRONT OBJ LOADING ///////////////////////////////////////////////////////////////////////////////////////////

// This is a simple format which allows defining simple geometry, such as vertices and faces, in text mode.
// This format is available for export in almost every 3D graphics software and it's also easy to understand
// and parse. For more information on the Wavefront OBJ format, please visit this link:
//          https://en.wikipedia.org/wiki/Wavefront_.obj_file

/**
 * The InitSprites[1-4]() methods are called from the first 4 loops,
 * each loop calling one method. This helps spread the workload of
 * loading sprites across multiple initialization steps, to avoid
 * "script too complex" erros.
 */
private void InitSprites1() {
 // Attach a new cube to the model view
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

 // Set the object's position in 3D space
    TheModelView.SetAttachedModelPosition(0, 0, 2);
}
private void InitSprites2() {

}
private void InitSprites3() {

}
private void InitSprites4() {

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// SMALL 3D FRAMEWORK //////////////////////////////////////////////////////////////////////////////////////////////

private class My3DModelView : MyOnScreenObject {

    private const int CENTER_X = RES_X / 2;
    private const int CENTER_Y = RES_Y / 2;

    private const double SCALE_X = 35d;
    private const double SCALE_Y = 20d;

    private MySimple3DObject AttachedModel;

    public My3DModelView() : base(null, 0, 0, true) {
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
         // Rotate the model
            AttachedModel.Rotate(0.5, 0.3, 1.2);

         // TODO: if the wireframe doesn't look good, a z-buffer will be required,
         //       to sort faces and draw only the visible ones

         // Draw the model's faces
            foreach (MyFace Face in AttachedModel.Faces) {
                DrawFace(Face, TargetCanvas);
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
            MyPoint3D vertex = Vertices[vIdx];

         // If any of the provided vertex numbers has not yet been defined, raise an error
            if (vertex == null) {
                throw new ArgumentException("Vertex number [" + vIdx + "] is not yet defined");
            }

         // Add the vertex reference to the face
            Face.Vertices.Add(vertex);
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
    * Rotates all the vertices of the object using Vrage.Math, which
    * is provided by the Space Engineers API.
    */
    public void Rotate(double yaw, double pitch, double roll) {
     // Create the rotation matrix
        MatrixD RotationMatrix = MatrixD.CreateFromYawPitchRoll(yaw, pitch, roll);

     // Rotate the vertices
        foreach (MyPoint3D Vertex in Vertices) {
            VertexTransform(Vertex, ref RotationMatrix);
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
    public static MySimple3DObject LoadFromString(String document) {
        if (document == null || document.Length == 0) {
            throw new ArgumentException("The provided Wavefront OBJ is empty");
        }
        return LoadFromArray(document.Split('\n'));
    }

    public static MySimple3DObject LoadFromArray(String[] array) {
        if (array == null || array.Length < 4) {
            throw new ArgumentException("The object definition must contain at least 4 lines (3 vertices and one face)");
        }

        MySimple3DObject obj3D = new MySimple3DObject();

        foreach (String line in array) {
            char componentType = line.Length == 0 ? 'x' : line[0];

            if (componentType == 'v') {
                String[] vertDef = line.Split(' ');
                if (vertDef.Length >= 4) {
                    try {
                        double x = double.Parse(vertDef[1], System.Globalization.CultureInfo.InvariantCulture);
                        double y = double.Parse(vertDef[2], System.Globalization.CultureInfo.InvariantCulture);
                        double z = double.Parse(vertDef[3], System.Globalization.CultureInfo.InvariantCulture);
                        obj3D.WithVertex(x,y,z);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read vertex data from [" + line + "]");
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
                            vertIndexes[i-1] = Int32.Parse(faceDef[i].Split('/')[0]);
                        }
                        obj3D.WithFace(vertIndexes);
                    } catch (Exception exc) {
                        throw new ArgumentException("Cannot read face data at line [" + line + "]");
                    }
                } else {
                    throw new ArgumentException("Face information incomplete at line [" + line + "]");
                }
            }
        }

        return obj3D;
    }  
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// APPLICATION INIT ////////////////////////////////////////////////////////////////////////////////////////////////

// This is the resolution of the matrix on the target screen
private const int RES_X = 139, RES_Y =  93;

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

/**
 * This is the 3D model view which will rendered the 3D object onto
 * the target LCD panel. It is initialized in the InitSprites1() method.
 */
private My3DModelView TheModelView = new My3DModelView();

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
        });

 // Initialize the main page and add it to the application
    MyPage MainPage = new MyPage();
    OnScreenApplication.AddPage(MainPage);

 // Optionally invert the colors of the main page
    if (INVERT_COLORS) {
        MainPage.WithInvertedColors();
    }

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

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////







////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}}

