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

        public static MyOnScreenApplication InitSingleScreenApplication(
            IMyGridTerminalSystem MyGridTerminalSystem, String textPanelName, int surfaceIndex,
            int resX, int resY, bool mirrorX,
            int nComputeIterations = 1, int nDrawIterations = 1
        ) {
            return new MyOnScreenApplication(nComputeIterations, nDrawIterations)
                        .WithCanvas(
                            new MyCanvas(resX, resY)
                            )
                        .OnScreen(
                            new MyScreen(
                                TerminalUtils.FindTextSurface(MyGridTerminalSystem, textPanelName, surfaceIndex),
                                SCREEN_PIXEL_VALUE_ON, SCREEN_PIXEL_VALUE_OFF,
                                mirrorX
                            )
                            );
        }
    }
}
