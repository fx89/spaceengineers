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

/**
 * This will be the interface to the application.
 */
private MyOnScreenApplication OnScreenApplication;

int currFrame = 0;

/**
 * The InitSprites[1-4]() methods are called from the first 4 loops,
 * each loop calling one method. This helps spread the workload of
 * loading sprites across multiple initialization steps, to avoid
 * "script too complex" erros.
 */
private void InitSprites1() {
    
}
private void InitSprites2() {

}
private void InitSprites3() {

}
private void InitSprites4() {

}


/**
  * This is called from the 5th loop
  * It should be used for initializing the application.
  *      > adding pages
  *      > adding components to the pages
  *      > linking logic and animations
  */
private void InitApplication() {
    OnScreenApplication = UiFrameworkUtils.InitSingleScreenApplication(GridTerminalSystem, "Text Panel", 139, 93, false)
        .WithDefaultPostPage((MyOnScreenApplication app) => {
         // The POST page should disappear after 100 frames
            currFrame++;
            return currFrame >= 100;
        });

    // TODO: Add more pages
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



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