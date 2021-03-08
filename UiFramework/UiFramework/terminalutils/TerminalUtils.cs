using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;

namespace IngameScript.terminal_utils {
    public class TerminalUtils {
        public static IMyTextSurface FindTextSurface(IMyGridTerminalSystem MyGridTerminalSystem, String blockName, int surfaceIndex) {
         // Get all the blocks having the given name
            List<IMyTerminalBlock> allFoundBlocks
                = GetAllBlocksWithName(MyGridTerminalSystem, blockName);

         // Get the first text panel or control seat having a text surface at the given index
            foreach (IMyTerminalBlock block in allFoundBlocks) {
                if (block is IMyTextSurfaceProvider) {
                    IMyTextSurface Surface = ((IMyTextSurfaceProvider)block).GetSurface(surfaceIndex);
                    if (Surface != null) {
                        return Surface;
                    }
                }
            }

         // Let the caller know if nothing was found
            throw new ArgumentException("Could not get text surface nbr [" + surfaceIndex + "] of [" + blockName + "].");
        }

        public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) {
         // Get all the blocks having the given name
            List<IMyTerminalBlock> allFoundBlocks
                = GetAllBlocksWithName(MyGridTerminalSystem, blockName);

         // Return the cast result or crash and burn
            foreach (IMyTerminalBlock block in allFoundBlocks) {
                if (block is T) {
                    return (T)block;
                }
            }
            return default(T);
        }

        private static List<IMyTerminalBlock> GetAllBlocksWithName(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) {
         // Verify the input parameter
            if (blockName == null || blockName.Length == 0) {
                throw new ArgumentException("Invalid block name");
            }

         // Get all the blocks having the given name
            List<IMyTerminalBlock> allFoundBlocks = new List<IMyTerminalBlock>();
            MyGridTerminalSystem.SearchBlocksOfName(blockName, allFoundBlocks);

         // If there's no block with the given name, then throw an exception
            if (allFoundBlocks == null || allFoundBlocks.Count == 0) {
                throw new ArgumentException("Cannot find a block named [" + blockName + "]");
            }

         // Return the result
            return allFoundBlocks;
        }

        public static void SetupTextSurfaceForMatrixDisplay(
            IMyGridTerminalSystem GridTerminalSystem,
            string blockName,
            int surfaceIndex,
            float fontSize
        ) {
            // Get the text panel
               IMyTextSurface TextSurface = TerminalUtils.FindTextSurface(GridTerminalSystem, blockName,surfaceIndex);
               if (TextSurface == null) {
                   throw new ArgumentException("Cannot find a text panel named [" + blockName + "]");
               }

            // Set up the text panel
               TextSurface.ContentType = ContentType.TEXT_AND_IMAGE;
               TextSurface.FontSize = fontSize;
               TextSurface.TextPadding = 0;
            // TextSurface.SetValue<long>("Font", 1147350002);
               TextSurface.Font = "Monospace";
               TextSurface.Alignment = TextAlignment.LEFT;
        }
    }
}
