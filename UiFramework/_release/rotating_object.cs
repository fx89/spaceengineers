/* README //////////////////////////////////////////////////////////////////////////////////////////////////////////

This script displays a rotating wireframe mesh on a 1x1 LCD panel, on a resolution of 139x93 pixels. The mesh
is specified in the Wavefront OBJ format, which can be exported to by almost every graphics software. Information
regarding the Wavefront OBJ format can be found here:
    https://en.wikipedia.org/wiki/Wavefront_.obj_file



Updating the WAVEFRONT OBJ LOADING section:
====================================================================================================================
To load the mesh, simply export it as a Wavefront OBJ, open the file using a text editor and update the
WAVEFRONT OBJ LOADING section, which can be found below, with the content of the file. To avoid errors caused
by the complexity limitations imposed by the game, please make sure that only the vertices (lines starting with
the letter v) and faces (lines starting with the letter f) are copied into the WAVEFRONT OBJ LOADING section.

The maximum allowed number of lines is 700.
====================================================================================================================
====================================================================================================================
====================================================================================================================



Updating the CONFIGURATION section:
====================================================================================================================
The CONFIGURATION section allows setting the rotation speed on each axis, as well as the initial orientation of
the object on screen. Please pay special attention to the MODEL_DISTANCE_FROM_VIEW property. If the model is too
close, it will force the script to draw lines outside the screen. This will result in a "Script too Complex"
error and the script will freeze. If this happens, then increase the distance until the script becomes stable.
====================================================================================================================
====================================================================================================================
====================================================================================================================



This is a showcase of the UI Framework, which enables low resolution graphics on text panels by dumping the frame
buffer into a string, which is then set as the text property of a given text panel, which is set up to use the
Monospace font and very small font size. The UI framework is minified in this script. The development version can be
found here: https://github.com/fx89/spaceengineers/tree/main/UiFramework/UiFramework

For a working example, please visit the following workshop item:
    https://steamcommunity.com/sharedfiles/filedetails/?id=2415572447

Development and partial building is done using MDK-SE: https://github.com/malware-dev/MDK-SE

*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIGURATION ///////////////////////////////////////////////////////////////////////////////////////////////////

// Name of the 1x1 LCD panel to which the view should draw
private const string TARGET_LCD_PANEL_NAME = "3D_RENDERING_SCREEN_2";

// false = white foreground on black background
// true  = black foreground on white background
private static bool INVERT_COLORS = false;

// For how many frames should the script display the "INITIALIZING" message on screen
private const int POST_SCREEN_DURATION = 10;

// Distance of the rendered object from the view
//    --- increase this if you get the "Script too Complex" error while rendering - !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
private const double MODEL_DISTANCE_FROM_VIEW = 1.7d;

// Set this to true to re-compute the object's center after loading
private static bool RECENTER_OBJECT_AFTER_LOADING = true;

// Rotation angles in radians (how much should the object turn each frame)
private const double ROT_SPEED_RAD_YAW   = 0.015d;
private const double ROT_SPEED_RAD_PITCH = 0.000d;
private const double ROT_SPEED_RAD_ROLL  = 0.000d;

// Initial rotation angles in radians
private const double INITIAL_ROTATION_RAD_YAW   = 0.00d;
private const double INITIAL_ROTATION_RAD_PITCH = Math.PI;
private const double INITIAL_ROTATION_RAD_ROLL  = 0.00d;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// WAVEFRONT OBJ LOADING ///////////////////////////////////////////////////////////////////////////////////////////

// This is a simple format which allows defining simple geometry, such as vertices and faces, in text mode.
// This format is available for export in almost every 3D graphics software and it's also easy to understand
// and parse. For more information on the Wavefront OBJ format, please visit this link:
//          https://en.wikipedia.org/wiki/Wavefront_.obj_file

private void InitSprites1() {

}
private void InitSprites2() {
 // Get the model from the programmable block's custom data
    ObjFileLines = Me.CustomData.Split('\n');

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
    TerminalUtils.SetupTextSurfaceForMatrixDisplay(GridTerminalSystem, TARGET_LCD_PANEL_NAME, 0, 0.190f);

 // Initialized the application with the default POST page
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, TARGET_LCD_PANEL_NAME, 0, RES_X, RES_Y, false)
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



// MINIFIED UI FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////
}public class Constants { public const int FLOATING_POSITION_TOP = 0, FLOATING_POSITION_LEFT = 1, FLOATING_POSITION_RIGHT = 2, FLOATING_POSITION_BOTTOM = 3; public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2;}public class DefaultMonospaceFont { private static MySprite CreateFontSprite(byte[] bytes) { MyCanvas Cvs = new MyCanvas(6, 7); Cvs.BitBlt(new MySprite(8, 7, DrawingFrameworkUtils.ByteArrayToBoolArray(bytes)), 0, 0); return new MySprite(6, 7, Cvs.GetBuffer()); } private static MySprite SPRITE_A = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_B = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x88,0x88,0xf0 }); private static MySprite SPRITE_C = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_D = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0x88,0x88,0x88,0xf0 }); private static MySprite SPRITE_E = CreateFontSprite(new byte[] { 0xf0,0x80,0x80,0xf0,0x80,0x80,0xf0 }); private static MySprite SPRITE_F = CreateFontSprite(new byte[] { 0xf8,0x80,0x80,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_G = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x98,0x88,0x88,0x78 }); private static MySprite SPRITE_H = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0xf8,0x88,0x88,0x88 }); private static MySprite SPRITE_I = CreateFontSprite(new byte[] { 0x70,0x20,0x20,0x20,0x20,0x20,0x70 }); private static MySprite SPRITE_J = CreateFontSprite(new byte[] { 0x10,0x10,0x10,0x10,0x10,0x10,0x60 }); private static MySprite SPRITE_K = CreateFontSprite(new byte[] { 0x88,0x90,0xa0,0xc0,0xa0,0x90,0x88 }); private static MySprite SPRITE_L = CreateFontSprite(new byte[] { 0x80,0x80,0x80,0x80,0x80,0x80,0x78 }); private static MySprite SPRITE_M = CreateFontSprite(new byte[] { 0x88,0xd8,0xa8,0x88,0x88,0x88,0x88 }); private static MySprite SPRITE_N = CreateFontSprite(new byte[] { 0x88,0xc8,0xa8,0x98,0x88,0x88,0x88 }); private static MySprite SPRITE_O = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_P = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0x80,0x80,0x80 }); private static MySprite SPRITE_Q = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x88,0xa8,0x90,0x68 }); private static MySprite SPRITE_R = CreateFontSprite(new byte[] { 0xf0,0x88,0x88,0xf0,0xa0,0x90,0x88 }); private static MySprite SPRITE_S = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_T = CreateFontSprite(new byte[] { 0xf8,0x20,0x20,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_U = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x88,0x70 }); private static MySprite SPRITE_V = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0x50,0x20 }); private static MySprite SPRITE_W = CreateFontSprite(new byte[] { 0x88,0x88,0x88,0x88,0x88,0xa8,0x50 }); private static MySprite SPRITE_X = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x50,0x88,0x88 }); private static MySprite SPRITE_Y = CreateFontSprite(new byte[] { 0x88,0x88,0x50,0x20,0x20,0x20,0x20 }); private static MySprite SPRITE_Z = CreateFontSprite(new byte[] { 0xf8,0x08,0x10,0x20,0x40,0x80,0xf8 }); private static MySprite SPRITE_1 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x10,0x10,0x10,0x38 }); private static MySprite SPRITE_2 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x08,0x70,0x40,0x78 }); private static MySprite SPRITE_3 = CreateFontSprite(new byte[] { 0x30,0x48,0x08,0x30,0x08,0x48,0x30 }); private static MySprite SPRITE_4 = CreateFontSprite(new byte[] { 0x10,0x30,0x50,0x90,0xf8,0x10,0x10 }); private static MySprite SPRITE_5 = CreateFontSprite(new byte[] { 0x78,0x40,0x40,0x70,0x08,0x08,0x70 }); private static MySprite SPRITE_6 = CreateFontSprite(new byte[] { 0x78,0x80,0x80,0xf0,0x88,0x88,0x70 }); private static MySprite SPRITE_7 = CreateFontSprite(new byte[] { 0xf8,0x08,0x08,0x10,0x20,0x40,0x40 }); private static MySprite SPRITE_8 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x88,0x88,0x70 }); private static MySprite SPRITE_9 = CreateFontSprite(new byte[] { 0x70,0x88,0x88,0x70,0x08,0x08,0xf0 }); private static MySprite SPRITE_0 = CreateFontSprite(new byte[] { 0x70,0x88,0x98,0xa8,0xc8,0x88,0x70 }); private static MySprite SPRITE_DASH = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0x00,0x00,0x00,0xf8 }); private static MySprite SPRITE_HYPHEN = CreateFontSprite(new byte[] { 0x00,0x00,0x00,0xf8,0x00,0x00,0x00 }); private static MySprite SPRITE_GT = CreateFontSprite(new byte[] { 0x40,0x20,0x10,0x08,0x10,0x20,0x40 }); private static MySprite SPRITE_LT = CreateFontSprite(new byte[] { 0x08,0x10,0x20,0x40,0x20,0x10,0x08 }); private static MySprite SPRITE_EQ = CreateFontSprite(new byte[] { 0x00,0x00,0xf8,0x00,0xf8,0x00,0x00 }); private static MySprite SPRITE_PCT = CreateFontSprite(new byte[] { 0xc0,0xc8,0x10,0x20,0x40,0x98,0x18 }); private static MySprite SPRITE_SPC = new MySprite(6, 7, new bool[42]); private static MySprite[] Create() { MySprite[] BitmapFont = new MySprite[256]; BitmapFont['a'] = SPRITE_A; BitmapFont['A'] = SPRITE_A; BitmapFont['b'] = SPRITE_B; BitmapFont['B'] = SPRITE_B; BitmapFont['c'] = SPRITE_C; BitmapFont['C'] = SPRITE_C; BitmapFont['d'] = SPRITE_D; BitmapFont['D'] = SPRITE_D; BitmapFont['e'] = SPRITE_E; BitmapFont['E'] = SPRITE_E; BitmapFont['f'] = SPRITE_F; BitmapFont['F'] = SPRITE_F; BitmapFont['g'] = SPRITE_G; BitmapFont['G'] = SPRITE_G; BitmapFont['h'] = SPRITE_H; BitmapFont['H'] = SPRITE_H; BitmapFont['i'] = SPRITE_I; BitmapFont['I'] = SPRITE_I; BitmapFont['j'] = SPRITE_J; BitmapFont['J'] = SPRITE_J; BitmapFont['k'] = SPRITE_K; BitmapFont['K'] = SPRITE_K; BitmapFont['l'] = SPRITE_L; BitmapFont['L'] = SPRITE_L; BitmapFont['m'] = SPRITE_M; BitmapFont['M'] = SPRITE_M; BitmapFont['n'] = SPRITE_N; BitmapFont['N'] = SPRITE_N; BitmapFont['o'] = SPRITE_O; BitmapFont['O'] = SPRITE_O; BitmapFont['p'] = SPRITE_P; BitmapFont['P'] = SPRITE_P; BitmapFont['q'] = SPRITE_Q; BitmapFont['Q'] = SPRITE_Q; BitmapFont['r'] = SPRITE_R; BitmapFont['R'] = SPRITE_R; BitmapFont['s'] = SPRITE_S; BitmapFont['S'] = SPRITE_S; BitmapFont['t'] = SPRITE_T; BitmapFont['T'] = SPRITE_T; BitmapFont['u'] = SPRITE_U; BitmapFont['U'] = SPRITE_U; BitmapFont['v'] = SPRITE_V; BitmapFont['V'] = SPRITE_V; BitmapFont['w'] = SPRITE_W; BitmapFont['W'] = SPRITE_W; BitmapFont['x'] = SPRITE_X; BitmapFont['X'] = SPRITE_X; BitmapFont['y'] = SPRITE_Y; BitmapFont['Y'] = SPRITE_Y; BitmapFont['z'] = SPRITE_Z; BitmapFont['Z'] = SPRITE_Z; BitmapFont['1'] = SPRITE_1; BitmapFont['2'] = SPRITE_2; BitmapFont['3'] = SPRITE_3; BitmapFont['4'] = SPRITE_4; BitmapFont['5'] = SPRITE_5; BitmapFont['6'] = SPRITE_6; BitmapFont['7'] = SPRITE_7; BitmapFont['8'] = SPRITE_8; BitmapFont['9'] = SPRITE_9; BitmapFont['0'] = SPRITE_0; BitmapFont['_'] = SPRITE_DASH; BitmapFont['-'] = SPRITE_HYPHEN; BitmapFont['<'] = SPRITE_LT; BitmapFont['>'] = SPRITE_GT; BitmapFont['='] = SPRITE_EQ; BitmapFont['%'] = SPRITE_PCT; BitmapFont[' '] = SPRITE_SPC; return BitmapFont; } public static MySprite[] BitmapFont = Create();}public class DrawingFrameworkConstants { public const int HORIZONTAL_ALIGN_LEFT = 0, HORIZONTAL_ALIGN_CENTER = 1, HORIZONTAL_ALIGN_RIGHT = 2; public const int VERTICAL_ALIGN_TOP = 0, VERTICAL_ALIGN_MIDDLE = 1, VERTICAL_ALIGN_BOTTOM = 2;}public class DrawingFrameworkUtils { private static readonly byte[] BYTE_POW = new byte[]{ 128,64,32,16,8,4,2,1 }; public static bool[] CopyBoolArray(bool[] BoolArray, bool negate) { if (BoolArray == null || BoolArray.Count() == 0) { return null; } bool[] ret = new bool[BoolArray.Count()]; for (int i = 0; i < BoolArray.Count(); i++) { ret[i] = negate ? !BoolArray[i] : BoolArray[i]; } return ret; } public static bool[] NegateBoolArray(bool[] BoolArray) { return CopyBoolArray(BoolArray, true); } public static bool[] ByteArrayToBoolArray(byte [] byteArray) { if (byteArray == null || byteArray.Length == 0) { return new bool[]{}; } bool[] ret = new bool[byteArray.Length * 8]; int retIdx = 0; for (int bIdx = 0 ; bIdx < byteArray.Length ; bIdx++) { byte b = byteArray[bIdx]; for (int divIdx = 0 ; divIdx < 8 ; divIdx++) { ret[retIdx] = (b / BYTE_POW[divIdx] > 0); b = (byte)(b % BYTE_POW[divIdx]); retIdx++; } } return ret; } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight) { return ResizeSpriteCanvas(Sprite, newWidth, newHeight, DrawingFrameworkConstants.HORIZONTAL_ALIGN_CENTER, DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE); } public static MySprite ResizeSpriteCanvas(MySprite Sprite, int newWidth, int newHeight, int horizontalAlignment, int verticalAlignment) { if (Sprite == null || newWidth < 1 || newHeight < 1) { return null; } MyCanvas NewCanvas = new MyCanvas(newWidth, newHeight); int posX = ComputePos(Sprite.width, newWidth, horizontalAlignment); int posY = ComputePos(Sprite.height, newHeight, verticalAlignment); NewCanvas.BitBlt(Sprite, posX, posY); return new MySprite(newWidth, newHeight, NewCanvas.GetBuffer()); } private static int ComputePos(int origSize, int newSize, int alignemnt) { if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_MIDDLE) { return (newSize - origSize) / 2; } if (alignemnt == DrawingFrameworkConstants.VERTICAL_ALIGN_BOTTOM) { return newSize - 1 - origSize; } return 0; }}public class MyBlinkingIcon : MyOnScreenObject { private MyStatefulAnimatedSprite Sprite; private int blinkingInterval = 3; private int blinkTimeout = 0; private bool isOn = false; private bool isBlinking = false; private int nBlinkTimes = 0; public MyBlinkingIcon(int x, int y, MySprite Graphics) : base(null, x, y, true) { Sprite = new MyStatefulAnimatedSprite(0, 0) .WithState("Off", new MyStatefulAnimatedSpriteState(new MySprite[]{ Graphics })) .WithState("On" , new MyStatefulAnimatedSpriteState(new MySprite[]{ new MySprite(Graphics.width, Graphics.height, DrawingFrameworkUtils.NegateBoolArray(Graphics.data)) })); AddChild(Sprite); } public MyBlinkingIcon WithBlinkingInterval(int blinkingInterval) { this.blinkingInterval = blinkingInterval; return this; } public override int GetWidth() { return Sprite.GetWidth(); } public override int GetHeight() { return Sprite.GetHeight(); } protected override void Init() { } private void LocalSwitchOn() { Sprite.SetState("On"); isOn = true; } private void LocalSwitchOff() { Sprite.SetState("Off"); isOn = false; } private void LocalSwitch() { if (isOn) { LocalSwitchOff(); } else { LocalSwitchOn(); } } public void SwitchOn() { LocalSwitchOn(); stopBlinking(); } public void SwitchOff() { LocalSwitchOff(); stopBlinking(); } public void Switch() { LocalSwitch(); stopBlinking(); } public void Blink(int nTimes) { SwitchOn(); nBlinkTimes = nTimes; isBlinking = true; blinkTimeout = 0; } public void stopBlinking() { isBlinking = false; nBlinkTimes = 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (isBlinking) { blinkTimeout++; if (blinkTimeout >= blinkingInterval) { blinkTimeout = 0; LocalSwitch(); nBlinkTimes--; if (nBlinkTimes == 0) { SwitchOff(); } } } }}public class MyCanvas { private bool[] Buffer; private int resX; private int resY; private int length; private MySprite[] DefaultFont; public MyCanvas(int resX, int resY) { this.resX = resX; this.resY = resY; length = resY * resX; Buffer = new bool[length]; this.DefaultFont = DefaultMonospaceFont.BitmapFont; } public bool[] GetBuffer() { return Buffer; } public int GetResX() { return resX; } public int GetResY() { return resY; } public bool[] GetBufferCopy() { return DrawingFrameworkUtils.CopyBoolArray(Buffer, false); } public void SetDefaultFont(MySprite[] DefaultFont) { this.DefaultFont = DefaultFont; } public MySprite[] GetDefaultFont() { return DefaultFont; } public MyCanvas WithDefaultFont(MySprite[] DefaultFont) { SetDefaultFont(DefaultFont); return this; } public void Clear(bool value = false) { for (int x = 0; x < length; x++) { Buffer[x] = value; } } private bool TransformSourcePixelValue(bool sourcePixelValue, bool targetPixelValue, bool invertColors, bool transparentBackground) { if (invertColors) { if (transparentBackground) { return targetPixelValue && !sourcePixelValue; } else { return !sourcePixelValue; } } else { if (transparentBackground) { return sourcePixelValue || targetPixelValue; } else { return sourcePixelValue; } } } public void BitBlt(MySprite sprite, int x, int y) { BitBltExt(sprite, x, y, false, false); } public void BitBltExt(MySprite sprite, int x, int y, bool invertColors, bool transparentBackground) { if (x < 0 || y < 0) { return; } int screenPos = resX * y + x; int spriteLength = sprite.height * sprite.width; int spritePosX = 0; for (int spritePos = 0; spritePos < spriteLength; spritePos++) { try { Buffer[screenPos] = TransformSourcePixelValue(sprite.data[spritePos], Buffer[screenPos], invertColors, transparentBackground); screenPos++; } catch (Exception exc) { } if (screenPos >= length - 1) { return; } spritePosX++; if (spritePosX == sprite.width) { spritePosX = 0; screenPos += resX - sprite.width; } } } public void BitBlt( MySprite Sprite, int cropX1, int cropY1, int cropX2, int cropY2, int x, int y ) { int trgX = x; for (int srcX = cropX1 ; srcX <= cropX2 ; srcX++) { int trgY = y; for (int srcY = cropY1 ; srcY <= cropY2 ; srcY++) { SetPixel(trgX, trgY, Sprite.data[srcY * Sprite.width + srcX]); trgY++;} trgX++;} } public void DrawText(int x, int y, String text) { DrawColorText(x, y, text, false, false); } public void DrawColorText(int x, int y, String text, bool invertColors, bool transparentBackground) { if (DefaultFont == null || text == null) { return; } char[] textChars = text.ToCharArray(); int screenPosX = x; int prevSpacing = 7; foreach (char chr in textChars) { MySprite CharSprite = DefaultFont[chr]; if (CharSprite != null) { BitBltExt(CharSprite, screenPosX, y, invertColors, transparentBackground); prevSpacing = CharSprite.width; } screenPosX += prevSpacing; if (screenPosX >= resX) { return; } } } public void DrawRect(int x1, int y1, int x2, int y2, bool invertColors, bool fillRect) { int actualX1 = x1 > x2 ? x2 : x1; int actualY1 = y1 > y2 ? y2 : y1; int actualX2 = x1 > x2 ? x1 : x2; int actualY2 = y1 > y2 ? y1 : y2; if (actualX1 < 0) actualX1 = 0; if (actualY1 < 0) actualY1 = 0; if (actualX2 >= resX - 1) actualX2 = resX - 1; if (actualY2 >= resY - 1) actualY2 = resY - 1; int rectWidth = actualX2 - actualX1; int screenPosY = actualY1; while (screenPosY <= actualY2) { int screenPos = screenPosY * resX + actualX1; if (screenPos >= length) { return; } bool targetColor = !invertColors; Buffer[screenPos] = targetColor; Buffer[screenPos + rectWidth - 1] = targetColor; if (fillRect || screenPosY == actualY1 || screenPosY == actualY2) { for (int innerPos = screenPos; innerPos < screenPos + rectWidth; innerPos++) { Buffer[innerPos] = targetColor; } } screenPos += resX; screenPosY++; } } public void DrawLine(int x1, int y1, int x2, int y2, bool color) { if (x1 == x2) { int incY = y1 < y2 ? 1 : -1; for (int y = y1 ; y != y2 ; y += incY) { SetPixel(x1, y, color); } } else { float a = (float)(y2 - y1) / (float)(x2 - x1); float b = ((float)y2 - (a * x2)); int incX = x1 <= x2 ? 1 : -1; for (int x = x1 ; x != x2 ; x += incX) { int y = (int)((a * x) + b); SetPixel(x, y, color); } } } public void SetPixel(int x, int y, bool color) { if (x >= 0 && x < resX - 1 && y >= 0 && y < resY - 1) { Buffer[y * resX + x] = color; } }}public class MyIconLabel : MyOnScreenObject { private MyStatefulAnimatedSprite AnimatedSprite; private MyTextLabel TextLabel; private int width; private int height; private int floatingIconPosition = Constants.FLOATING_POSITION_TOP; private int spacing = 3; public MyIconLabel(int x, int y, string text, MySprite[] Frames) :base(null, x, y, true){ if (text == null) { throw new ArgumentException("The text of the MyIconLabel must not be null"); } if (Frames == null || Frames.Length == 0) { throw new ArgumentException("There has to be at least one frame if the picture is to be displayed by the MyIconLabel"); } int frameWidth = Frames[0].width; int frameHeight = Frames[0].height; foreach (MySprite Frame in Frames) { if (Frame.width != frameWidth || Frame.height != frameHeight) { throw new ArgumentException("All the frames of the MyIconLabel must have the same width and height"); } } AnimatedSprite = new MyStatefulAnimatedSprite(0,0).WithState("Default", new MyStatefulAnimatedSpriteState(Frames)); AddChild(AnimatedSprite); TextLabel = new MyTextLabel(text, 0,0); AddChild(TextLabel); } public MyIconLabel WithFloatingIconPosition(int floatingIconPosition) { this.floatingIconPosition = floatingIconPosition; return this; } public MyIconLabel WithSpaceing(int spacing) { this.spacing = spacing; return this; } public override int GetHeight() { return height; } public override int GetWidth() { return width; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { int spriteWidth = AnimatedSprite.GetWidth(); int spriteHeight = AnimatedSprite.GetHeight(); int textWidth = TextLabel.GetWidth(); int textHeight = TextLabel.GetHeight(); if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { width = spriteWidth + spacing + textWidth; height = spriteHeight > textHeight ? spriteHeight : textHeight; } else { width = spriteWidth > textWidth ? spriteWidth : textWidth; height = spriteHeight + spacing + textHeight; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT) { AnimatedSprite.x = 0; TextLabel.x = spriteWidth + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { AnimatedSprite.x = width - spriteWidth; TextLabel.x = AnimatedSprite.x - spacing - textWidth; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP || floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { AnimatedSprite.x = (width - spriteWidth) / 2; TextLabel.x = (width - textWidth) / 2; } if (floatingIconPosition == Constants.FLOATING_POSITION_LEFT || floatingIconPosition == Constants.FLOATING_POSITION_RIGHT) { TextLabel.y = (height - textHeight) / 2; AnimatedSprite.y = (height - spriteHeight) / 2; } else if (floatingIconPosition == Constants.FLOATING_POSITION_TOP) { AnimatedSprite.y = 0; TextLabel.y = spriteHeight + spacing; } else if (floatingIconPosition == Constants.FLOATING_POSITION_BOTTOM) { TextLabel.y = 0; AnimatedSprite.y = textHeight + spacing; } }}public class MyList : MyOnScreenObject { private MyPanel Panel; private MyScrollbar Scrollbar; private MyPanel SelectionBackground; private MyOnScreenObject SelectedItem; private bool oneItemPerPage = false; private int selectedItemIndex; private int startPosY; private int padding = 2; private int horizontalAlignment = Constants.HORIZONTAL_ALIGN_LEFT; private List<MyOnScreenObject> Items = new List<MyOnScreenObject>(); public MyList(int x, int y, int width, int height) : base(null, x, y, true) { Panel = new MyPanel(0, 0, width, height); base.AddChild(Panel); Scrollbar = new MyScrollbar(Panel); SelectionBackground = new MyPanel(0, 0, width, 1).WithOptionalParameters(true, true, false); base.AddChild(SelectionBackground); } public MyList WithCustomScrollbarWidth(int scrollbarWidth) { Scrollbar.WithCustomWidth(scrollbarWidth); return this; } public MyList WithOneItemPerPage() { this.oneItemPerPage = true; return this; } public MyList WithPadding(int padding) { this.padding = padding; return this; } public MyList WithHorizontalAlignment(int horizontalAlignment) { this.horizontalAlignment = horizontalAlignment; return this; } public override void AddChild(MyOnScreenObject Item) { base.AddChild(Item); if (SelectedItem == null) { SetInitialSelectedItem(Item); UpdateScrollbarPosition(); } Items.Add(Item); UpdateItemPositions(); } public override void RemoveChild(MyOnScreenObject Item) { base.RemoveChild(Item); if (Item == SelectedItem) { SetInitialSelectedItem(ChildObjects[0]); UpdateScrollbarPosition(); } Items.Remove(Item); UpdateItemPositions(); } private void SetInitialSelectedItem(MyOnScreenObject Item) { SelectedItem = Item; selectedItemIndex = 0; startPosY = padding; } private int ComputeItemHorizontalPosition(MyOnScreenObject Item) { if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_RIGHT) { return GetWidth() - Scrollbar.GetWidth() - padding - Item.GetWidth(); } else if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_CENTER) { return (GetWidth() - Scrollbar.GetWidth() - Item.GetWidth()) / 2; } else { return padding; } } private void UpdateItemPositions() { if (Items.Count() == 0) { return; } if (oneItemPerPage) { foreach (MyOnScreenObject Item in Items) { if (Item == SelectedItem) { Item.isVisible = true; Item.x = ComputeItemHorizontalPosition(Item); Item.y = (Panel.GetHeight() - Item.GetHeight()) / 2; } else { Item.isVisible = false; } Item.invertColors = false; } SelectionBackground.isVisible = false; } else { int listMaxHeight = GetHeight() - (padding * 2); int selItemY = startPosY; for (int idx = 0 ; idx < selectedItemIndex ; idx++) { selItemY += Items[idx].GetHeight(); } if (selItemY < padding) { startPosY += padding - selItemY; } else if (selItemY + SelectedItem.GetHeight() > listMaxHeight) { startPosY -= selItemY + SelectedItem.GetHeight() - listMaxHeight; } int currPosY = startPosY; foreach (MyOnScreenObject Item in Items) { Item.y = currPosY; Item.x = ComputeItemHorizontalPosition(Item); currPosY += Item.GetHeight(); Item.isVisible = Item.y >= padding && Item.y + Item.GetHeight() <= listMaxHeight; Item.invertColors = Item == SelectedItem; } SelectionBackground.x = padding; SelectionBackground.y = SelectedItem.y; SelectionBackground.SetWidth(GetWidth() - Scrollbar.GetWidth() - (padding * 2)); SelectionBackground.SetHeight(SelectedItem.GetHeight()); SelectionBackground.isVisible = true; } } private void UpdateScrollbarPosition() { Scrollbar.SetPosPct(Items.Count() == 0 ? 0f : ((float)selectedItemIndex / ((float)Items.Count() - 1))); } public MyOnScreenObject SelectNextItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex >= 0 && selectedItemIndex < Items.Count() - 1) { selectedItemIndex++; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject SelectPreviousItem() { if (SelectedItem != null) { selectedItemIndex = Items.IndexOf(SelectedItem); if (selectedItemIndex > 0) { selectedItemIndex--; SelectedItem = Items[selectedItemIndex]; UpdateItemPositions(); UpdateScrollbarPosition(); } } return SelectedItem; } public MyOnScreenObject GetSelectedItem() { return SelectedItem; } public MyOnScreenObject SelectFirstItem() { SetInitialSelectedItem(Items[0]); return SelectedItem; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } public override int GetWidth() { return Panel.GetWidth(); } public override int GetHeight() { return Panel.GetHeight(); } public List<MyOnScreenObject> GetItems() { return Items; } protected override void Init() { UpdateItemPositions(); }}public class MyOnScreenApplication { private List<MyScreen> TargetScreens = new List<MyScreen>(); private List<MyPage> Pages = new List<MyPage>(); private MyPage CurrentPage; private MyCanvas Canvas; private bool autoClearScreen = true; private bool autoFlushBuffer = true; private int currIteration, nIterations; public MyOnScreenApplication() { currIteration = 0; nIterations = 1; } public MyOnScreenApplication WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyOnScreenApplication OnScreen(MyScreen TargetScreen) { return OnScreen(TargetScreen, 0, 0, Canvas.GetResX(), Canvas.GetResY()); } public MyOnScreenApplication OnScreen(MyScreen TargetScreen, int clipRectX1, int clipRectY1, int clipRectX2, int clipRectY2) { if (Canvas == null) { throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen()."); } TargetScreen.WithCanvas(Canvas); TargetScreen.WithClippedBuffer(clipRectX1, clipRectY1, clipRectX2, clipRectY2); TargetScreens.Add(TargetScreen); nIterations++; return this; } public MyOnScreenApplication WithDefaultPostPage(Func<MyOnScreenApplication,bool> initializationMonitoringFunction) { if (Pages.Count() > 0) { throw new InvalidOperationException("The POST page must be the first page ever added to the application"); } if (Canvas == null) { throw new InvalidOperationException("Please call WithCanvas() before calling WithDefaultPostPage()"); } if (initializationMonitoringFunction == null) { throw new ArgumentException("The initialization monitoring function must be a lambda taking in a MyOnScreenObject and returning a bool"); } MyPage POSTPage = (MyPage) new MyPage().WithClientCycleMethod((MyOnScreenObject obj) => { if (initializationMonitoringFunction(this)) { SwitchToPage(1); } return 0; }); this.AddPage(POSTPage); MyPanel Panel = new MyPanel(0, 0, Canvas.GetResX(), Canvas.GetResY()).WithOptionalParameters(true, true, false); POSTPage.AddChild(Panel); MyPanel TextBackgroundPanel = new MyPanel(0, 0, 2, 2).WithOptionalParameters(true, true, true); POSTPage.AddChild(TextBackgroundPanel); MyTextLabel TextLabel = new MyTextLabel("INITIALIZING", 1, 1).WithOptionalParameters(true, false, false); POSTPage.AddChild(TextLabel); int textLabelWidth = TextLabel.GetWidth(); int textLabelHeight = TextLabel.GetHeight(); TextLabel.x = (Canvas.GetResX() - textLabelWidth) / 2; TextLabel.y = (Canvas.GetResY() - textLabelHeight) / 2 - 3; TextBackgroundPanel.x = TextLabel.x - 3; TextBackgroundPanel.y = TextLabel.y - 2; TextBackgroundPanel.SetWidth(textLabelWidth + 6); TextBackgroundPanel.SetHeight(textLabelHeight + 3); POSTPage.AddChild( new MyPanel(TextBackgroundPanel.x, TextBackgroundPanel.y + TextBackgroundPanel.GetHeight() + 2, 7, 4) .WithOptionalParameters(true, true, true) .WithClientCycleMethod((MyOnScreenObject obj) => { obj.x++; if (obj.x > TextBackgroundPanel.x + TextBackgroundPanel.GetWidth() - 7) { obj.x = TextBackgroundPanel.x; } return 0; }) ); return this; } public MyOnScreenApplication WithoutAutomaticClear() { autoClearScreen = false; return this; } public MyOnScreenApplication WithoutAutomaticFlush() { autoFlushBuffer = false; return this; } public void AddPage(MyPage Page) { Pages.Add(Page); Page.SetApplication(this); if (CurrentPage == null) { CurrentPage = Page; } } public void SwitchToPage(MyPage Page) { foreach (MyPage Pg in Pages) { if (Pg == Page) { CurrentPage = Pg; CurrentPage.Activate(); } } } public void SwitchToPage(int pageNumber) { if (pageNumber < 0 || pageNumber >= Pages.Count) { return; } CurrentPage = Pages[pageNumber]; CurrentPage.Activate(); } public MyPage GetCurrentPage() { return CurrentPage; } public void Cycle() { if (currIteration == 0) { if (autoClearScreen) { Canvas.Clear(); } CurrentPage.Cycle(Canvas); } else { if (autoFlushBuffer) { TargetScreens[currIteration - 1].FlushBufferToScreen(CurrentPage.invertColors); } } currIteration++; if (currIteration >= nIterations) { currIteration = 0; } } public MyCanvas GetCanvas() { return Canvas; } public List<MyScreen> GetTargetScreens() { return TargetScreens; }}public abstract class MyOnScreenObject { public int x; public int y; public bool isVisible = true; private bool _invertColors = false; public bool invertColors { get { return _invertColors; } set { _invertColors = value; foreach(MyOnScreenObject Child in ChildObjects) { Child.invertColors = value; } } } public MyOnScreenObject ParentObject; public List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>(); private Func<MyOnScreenObject, int> ClientCycleMethod; private Func<MyCanvas, int> ClientDrawMethod; private bool isObjectNotInitialized = true; public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) { this.SetParent(ParentObject); this.x = x; this.y = y; this.isVisible = isVisible; if (ParentObject != null) { ParentObject.AddChild(this); } } public MyOnScreenObject WithClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { SetClientCycleMethod(ClientCycleMethod); return this; } public void SetClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) { this.ClientCycleMethod = ClientCycleMethod; } public MyOnScreenObject WithClientDrawMethod(Func<MyCanvas, int> ClientDrawMethod) { this.ClientDrawMethod = ClientDrawMethod; return this; } public virtual void AddChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Add(ChildObject); ChildObject.SetParent(this); } } public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) { if (ChildObject != null) { ChildObject.SetPosition(x, y); AddChild(ChildObject); } } public void SetPosition(int x, int y) { this.x = x; this.y = y; } public virtual void RemoveChild(MyOnScreenObject ChildObject) { if (ChildObject != null) { ChildObjects.Remove(ChildObject); ChildObject.RemoveParent(); } } public virtual void SetParent(MyOnScreenObject ParentObject) { if (ParentObject != null) { this.ParentObject = ParentObject; } } public void RemoveParent() { this.ParentObject = null; } public MyOnScreenObject GetTopLevelParent() { if (ParentObject == null) { return this; } return ParentObject.GetTopLevelParent(); } public bool IsObjectVisible() { return isVisible && (ParentObject == null || ParentObject.IsObjectVisible()); } public virtual void Cycle(MyCanvas TargetCanvas) { Compute(TargetCanvas); if (ClientCycleMethod != null) { ClientCycleMethod(this); } foreach (MyOnScreenObject ChildObject in ChildObjects) { ChildObject.Cycle(TargetCanvas); } if (isObjectNotInitialized) { Init(); isObjectNotInitialized = true; } if (IsObjectVisible()) { if (ClientDrawMethod != null) { ClientDrawMethod(TargetCanvas); } Draw(TargetCanvas); } } public int GetAbsoluteX() { return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX()); } public int GetAbsoluteY() { return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY()); } protected abstract void Compute(MyCanvas TargetCanvas); protected abstract void Draw(MyCanvas TargetCanvas); public abstract int GetWidth(); public abstract int GetHeight(); protected abstract void Init();}public class MyPage : MyOnScreenObject { private MyOnScreenApplication OnScreenApplication; public MyPage() : base(null, 0, 0, true) { } public void SetApplication(MyOnScreenApplication OnScreenApplication) { this.OnScreenApplication = OnScreenApplication; } public MyPage WithInvertedColors() { this.invertColors = true; return this; } public MyOnScreenApplication GetApplication() { return OnScreenApplication; } public override int GetHeight() { return 0; } public override int GetWidth() { return 0; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { } protected override void Init() { } public virtual void Activate() {}}public class MyPanel : MyOnScreenObject { private int width; private int height; private bool isFilled = false; public MyPanel(int x, int y, int width, int height) : base(null, x, y, true) { this.width = width; this.height = height; this.isFilled = false; } public MyPanel WithOptionalParameters(bool isVisible, bool isFilled, bool invertColors) { this.isVisible = isVisible; this.isFilled = isFilled; this.invertColors = invertColors; return this; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int absoluteX = GetAbsoluteX(); int absoluteY = GetAbsoluteY(); TargetCanvas.DrawRect( absoluteX, absoluteY, absoluteX + width, absoluteY + height, invertColors, isFilled ); } public override int GetWidth() { return width; } public override int GetHeight() { return height; } public void SetWidth(int width) { this.width = width; } public void SetHeight(int height) { this.height = height; } protected override void Init() { }}public class MyScreen { public IMyTextSurface TargetSurface; private MyCanvas Canvas; private bool mirrorX; private char pixelValueOn, pixelValueOff; private int clipRectX1, clipRectY1, clipRectX2, clipRectY2; private bool isClipping = false; public MyScreen(IMyTextSurface TargetSurface, char pixelValueOn, char pixelValueOff, bool mirrorX) { this.TargetSurface = TargetSurface; this.mirrorX = mirrorX; this.pixelValueOn = pixelValueOn; this.pixelValueOff = pixelValueOff; } public MyScreen WithCanvas(MyCanvas Canvas) { this.Canvas = Canvas; return this; } public MyScreen WithClippedBuffer(int x1, int y1, int x2, int y2) { clipRectX1 = x1 > x2 ? x2 : x1; clipRectY1 = y1 > y2 ? y2 : y1; clipRectX2 = x1 > x2 ? x1 : x2; clipRectY2 = y1 > y2 ? y1 : y2; isClipping = clipRectX1 > 0 || clipRectY1 > 0 || clipRectX2 < Canvas.GetResX() || clipRectY2 < Canvas.GetResY(); return this; } private bool[] MirrorBufferOnXAxis(bool[] Buffer, int resX, int resY) { int length = Buffer.Count(); bool[] MirroredBuffer = new bool[length]; int mirrorPosX = resX - 1; int mirrorPos = mirrorPosX; for (int sourcePos = 0; sourcePos < length; sourcePos++) { MirroredBuffer[mirrorPos] = Buffer[sourcePos]; mirrorPos--; mirrorPosX--; if (mirrorPosX == -1) { mirrorPosX = resX - 1; mirrorPos += resX * 2; } } return MirroredBuffer; } private bool[] ClipBuffer(bool[] Buffer, int x1, int y1, int x2, int y2, int resX, int resY) { int rectX1 = x1 > x2 ? x2 : x1; int rectY1 = y1 > y2 ? y2 : y1; int rectX2 = x1 > x2 ? x1 : x2; int rectY2 = y1 > y2 ? y1 : y2; if (rectX1 < 0) rectX1 = 0; if (rectY1 < 0) rectY1 = 0; if (rectX2 > resX) rectX2 = resX; if (rectY2 > resY) rectY2 = resY; bool[] ret = new bool[(rectX2 - rectX1) * (rectY2 - rectY1) + 1]; int srcCursor = rectY1 * resX + rectX1; int trgCursor = 0; for (int srcY = rectY1; srcY < rectY2; srcY++) { for (int srcX = rectX1; srcX < rectX2; srcX++) { ret[trgCursor] = Buffer[srcCursor]; ret[trgCursor] = true; trgCursor++; srcCursor++; } srcCursor += rectX1; } return ret; } public void FlushBufferToScreen(bool invertColors) { bool[] Buffer = isClipping ? ClipBuffer(Canvas.GetBuffer(), clipRectX1, clipRectY1, clipRectX2, clipRectY2, Canvas.GetResX(), Canvas.GetResY()) : Canvas.GetBuffer(); int length = Buffer.Count(); int resX = isClipping ? clipRectX2 - clipRectX1 : Canvas.GetResX(); int resY = isClipping ? clipRectY2 - clipRectY1 : Canvas.GetResY(); bool[] SourceBuffer = mirrorX ? MirrorBufferOnXAxis(Buffer, resX, resY) : Buffer; StringBuilder renderedBuffer = new StringBuilder(length + resY + 1); char pxValOn = invertColors ? pixelValueOff : pixelValueOn; char pxValOff = invertColors ? pixelValueOn : pixelValueOff; int currXPos = 0; for (int idx = 0; idx < length; idx++) { renderedBuffer.Append(SourceBuffer[idx] ? pxValOn : pxValOff); currXPos++; if (currXPos == resX) { renderedBuffer.Append('\n'); currXPos = 0; } } TargetSurface.WriteText(renderedBuffer.ToString()); } public MyCanvas GetCanvas() { return Canvas; }}public class MyScrollbar : MyOnScreenObject { private int width = 7; private int height = 10; private float posPct = 0.5f; private bool snapToParent = true; public MyScrollbar(MyOnScreenObject ParentObject) : base(ParentObject, 0, 0, true) { } public MyScrollbar DetachedFromParent(int height) { this.snapToParent = false; this.height = height; return this; } public MyScrollbar WithCustomWidth(int width) { this.width = width; return this; } public MyScrollbar AtCoordinates(int x, int y) { this.x = x; this.y = y; return this; } public void SetPosPct(float posPct) { this.posPct = posPct; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { int x1 = snapToParent ? ParentObject.GetAbsoluteX() + ResolveClientX() : GetAbsoluteX(); int y1 = snapToParent ? ParentObject.GetAbsoluteY() : GetAbsoluteY(); int x2 = x1 + width; int actualHeight = GetHeight(); int y2 = y1 + actualHeight; TargetCanvas.DrawRect(x1, y1, x2, y2, invertColors, false); int sliderX = x1 + 1; int sliderY = (int)(y1 + 1 + (posPct * ((actualHeight - 5 - 2)))); TargetCanvas.BitBltExt( StockSprites.SCROLLBAR_SLIDER, sliderX, sliderY, invertColors ? (!this.invertColors) : this.invertColors, false ); } private int ResolveHight() { if (ParentObject is MyPanel) { return ((MyPanel)ParentObject).GetHeight(); } return height; } private int ResolveClientX() { if (ParentObject is MyPanel) { return ParentObject.GetWidth() - this.width; } return 0; } public override int GetWidth() { return width; } public override int GetHeight() { return snapToParent ? ResolveHight() : height; } protected override void Init() { }}public class MySprite { public int width; public int height; public bool[] data; public MySprite(int width, int height, bool[] data) { this.width = width; this.height = height; this.data = data; }}public class MyStatefulAnimatedSprite : MyOnScreenObject { private Dictionary<string, MyStatefulAnimatedSpriteState> States = new Dictionary<string, MyStatefulAnimatedSpriteState>(); private MyStatefulAnimatedSpriteState CurrentState; private bool isBackgroundTransparet = true; private MySprite CurrentFrame; public MyStatefulAnimatedSprite(int x, int y) : base(null, x, y, true) { } public MyStatefulAnimatedSprite WithState(string stateName, MyStatefulAnimatedSpriteState State) { if (stateName == null) { throw new ArgumentException("Each state must have a name"); } if (State == null) { throw new ArgumentException("The state may not be null"); } States.Add(stateName, State); if (CurrentState == null) { SetStateObject(State); } return this; } public void SetState(String stateName) { if (stateName == null) { return; } MyStatefulAnimatedSpriteState State; States.TryGetValue(stateName, out State); if (State != null) { SetStateObject(State); } } private void SetStateObject(MyStatefulAnimatedSpriteState State) { CurrentState = State; isBackgroundTransparet = State.IsBackgroundTransparent(); } protected override void Compute(MyCanvas TargetCanvas) { CurrentFrame = CurrentState.GetFrame(); } protected override void Draw(MyCanvas TargetCanvas) { TargetCanvas.BitBltExt( CurrentFrame, GetAbsoluteX(), GetAbsoluteY(), invertColors, CurrentState.IsBackgroundTransparent() ); } public override int GetWidth() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.width; } } public override int GetHeight() { if (CurrentFrame == null) { return 0; } else { return CurrentFrame.height; } } protected override void Init() { } public MyStatefulAnimatedSpriteState GetState(string stateName) { if (stateName == null || stateName.Length == 0) { throw new ArgumentException("Invalid state name"); } MyStatefulAnimatedSpriteState ret = null; States.TryGetValue(stateName, out ret); if (ret == null) { throw new ArgumentException("Cannot find a state named [" + stateName + "]"); } return ret; }}public class MyStatefulAnimatedSpriteState { private MySprite[] Frames; private int nFrames; private int currFrame = 0; private bool transparentBackground = true; public MyStatefulAnimatedSpriteState(MySprite[] Frames) { if (Frames == null || Frames.Count() == 0) { throw new ArgumentException("The frames array must have at least one frame"); } this.Frames = Frames; this.nFrames = Frames.Count(); } public bool IsBackgroundTransparent() { return transparentBackground; } public MyStatefulAnimatedSpriteState WithOpaqueBackground() { transparentBackground = false; return this; } public MySprite GetFrame() { MySprite ret = Frames[currFrame]; currFrame++; if (currFrame >= nFrames) { currFrame = 0; } return ret; }}public class MyTextLabel : MyOnScreenObject { private String text; private bool transparentBackground; private MySprite[] Font; private int padding = 1; public MyTextLabel(string text, int x, int y) : base(null, x, y, true) { this.text = text; this.transparentBackground = true; } public MyTextLabel WithOptionalParameters(bool isVisible, bool invertColors, bool transparentBackground) { this.isVisible = isVisible; this.invertColors = invertColors; this.transparentBackground = transparentBackground; return this; } public MyTextLabel WithCustomFont(MySprite[] CustomFont) { this.Font = CustomFont; return this; } public MyTextLabel WithPadding(int padding) { this.padding = padding; return this; } public void SetText(string text) { this.text = text; } protected override void Compute(MyCanvas TargetCanvas) { } protected override void Draw(MyCanvas TargetCanvas) { if (Font == null) { Font = TargetCanvas.GetDefaultFont(); } TargetCanvas.DrawColorText( GetAbsoluteX() + padding, GetAbsoluteY() + padding, text, invertColors, transparentBackground ); } private MySprite[] ResolveFont() { if (Font == null) { MyOnScreenObject TopLevelParent = GetTopLevelParent(); if (TopLevelParent is MyPage) { return ((MyPage) TopLevelParent).GetApplication().GetCanvas().GetDefaultFont(); } else { return null; } } else { return Font; } } public override int GetWidth() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].width * text.Length + (2 * padding); } public override int GetHeight() { MySprite[] Font = ResolveFont(); if (Font == null || Font['a'] == null) { return 0; } return Font['a'].height + (2 * padding); } protected override void Init() { }}public class StockSprites { private const bool O = true; private const bool _ = false; public static MySprite SPRITE_PWR = new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0x02,0,0x1a,0xc0,0x22,0x20,0x22,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x20,0x1f,0xc0,0,0 })); public static MySprite SPRITE_UP = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_DOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x3f,0xe0,0x3f,0xe0,0x3f,0xe0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_LEFTRIGHT = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0d,0x80,0x1d,0xc0,0x3d,0xe0,0x7d,0xf0,0x3d,0xe0,0x1d,0xc0,0x0d,0x80,0,0 })), 14, 10, 0, 0); public static MySprite SPRITE_UPDOWN = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x02,0,0x07,0,0x0f,0x80,0x1f,0xc0,0,0,0x1f,0xc0,0x0f,0x80,0x07,0,0x02,0 })), 14, 10, 0, 0); public static MySprite SPRITE_REVERSE = DrawingFrameworkUtils.ResizeSpriteCanvas(new MySprite(16, 10, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0,0,0x0f,0xe0,0x10,0x10,0x7c,0x10,0x38,0x10,0x10,0x10,0,0x10,0x10,0x10,0x0f,0xe0 })), 14, 10, 0, 0); public static MySprite SCROLLBAR_SLIDER = new MySprite(5, 5, new bool[] { _,O,O,O,_, O,O,O,O,O, O,O,_,_,O, O,O,_,_,O, _,O,O,O,_ }); public static MySprite SPRITE_SMILE_SAD = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0x7e,0,0xc0,0,0xc1,0x81, 0x81,0x80,0,0x62,0,0x43,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_NEUTRAL = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x80,0,0,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 })); public static MySprite SPRITE_SMILE_HAPPY = new MySprite(40, 15, DrawingFrameworkUtils.ByteArrayToBoolArray(new byte[] { 0,0,0x7f,0,0,0,0x07,0xc1,0xf0,0,0,0x1e,0,0x3c,0,0, 0x38,0,0x0e,0,0,0x61,0x81,0x83,0,0,0xc3,0xc3,0xc1,0x80,0x01,0x80, 0,0,0xc0,0x01,0x80,0,0,0xc0,0x01,0x82,0,0x40,0xc0,0,0xc1,255, 0x81,0x80,0,0x60,0x7e,0x03,0,0,0x38,0,0x0e,0,0,0x1e,0,0x3c, 0,0,0x07,0xc1,0xf0,0,0,0,0x7f,0,0 }));}public class TerminalUtils { public static IMyTextSurface FindTextSurface(IMyGridTerminalSystem MyGridTerminalSystem, String blockName, int surfaceIndex) { List<IMyTerminalBlock> allFoundBlocks = GetAllBlocksWithName(MyGridTerminalSystem, blockName); foreach (IMyTerminalBlock block in allFoundBlocks) { if (block is IMyTextSurfaceProvider) { IMyTextSurface Surface = ((IMyTextSurfaceProvider)block).GetSurface(surfaceIndex); if (Surface != null) { return Surface; } } } throw new ArgumentException("Could not get text surface nbr [" + surfaceIndex + "] of [" + blockName + "]."); } public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { List<IMyTerminalBlock> allFoundBlocks = GetAllBlocksWithName(MyGridTerminalSystem, blockName); foreach (IMyTerminalBlock block in allFoundBlocks) { if (block is T) { return (T)block; } } return default(T); } private static List<IMyTerminalBlock> GetAllBlocksWithName(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) { if (blockName == null || blockName.Length == 0) { throw new ArgumentException("Invalid block name"); } List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>(); MyGridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks); if (allFoundBlocks == null || allFoundBlocks.Count == 0) { throw new ArgumentException("Cannot find a block named [" + blockName + "]"); } return allFoundBlocks; } public static void SetupTextSurfaceForMatrixDisplay( IMyGridTerminalSystem GridTerminalSystem, string blockName, int surfaceIndex, float fontSize ) { IMyTextSurface TextSurface = TerminalUtils.FindTextSurface(GridTerminalSystem, blockName,surfaceIndex); if (TextSurface == null) { throw new ArgumentException("Cannot find a text panel named [" + blockName + "]"); } TextSurface.ContentType = ContentType.TEXT_AND_IMAGE; TextSurface.FontSize = fontSize; TextSurface.TextPadding = 0; TextSurface.Font = "Monospace"; TextSurface.Alignment = TextAlignment.LEFT; }}public class UiFrameworkUtils { private const char SCREEN_PIXEL_VALUE_ON = '@'; private const char SCREEN_PIXEL_VALUE_OFF = ' '; public static MyOnScreenApplication InitSingleScreenApplication( IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int surfaceIndex, int resX, int resY, bool mirrorX ) { return new MyOnScreenApplication() .WithCanvas( new MyCanvas(resX, resY) ) .OnScreen( new MyScreen( TerminalUtils.FindTextSurface(MyGridTerminalSystem, textPanelName, surfaceIndex), SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF, mirrorX ) ); }
