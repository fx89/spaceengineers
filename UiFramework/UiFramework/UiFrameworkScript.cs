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
  * This is called from the constructor of the program.
  * It should be used for initializing the application.
  *	  > adding pages
  *	  > adding components to the pages
  *	  > linking logic and animations
  */
private void Init() {
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

	// Initialize the script
	Init();
}

public void Save() {
	// There is no state to be saved
}

public void Main(string argument, UpdateType updateSource) {
	OnScreenApplication.Cycle();
}

}}