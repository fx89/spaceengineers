using IngameScript.drawing_framework;
using IngameScript.terminal_utils;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    public class UiFrameworkUtils {
        private const char SCREEN_PIXEL_VALUE_ON = '@';
        private const char SCREEN_PIXEL_VALUE_OFF = ' ';

        public static MyOnScreenApplication InitSingleScreenApplication(IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int resX, int resY, bool mirrorX) {
	        return new MyOnScreenApplication()
				        .WithCanvas(
                            new MyCanvas(resX, resY)
					        )
				        .OnScreen(
					        new MyScreen(
						        TerminalUtils.FindFirstBlockWithName<IMyTextPanel>(MyGridTerminalSystem, textPanelName),
						        SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF,
						        mirrorX
					        )
					        );
        }
    }
}
