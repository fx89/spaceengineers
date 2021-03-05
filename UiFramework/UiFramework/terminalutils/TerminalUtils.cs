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
        public static T FindFirstBlockWithName<T>(IMyGridTerminalSystem MyGridTerminalSystem, String blockName) {
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

         // Return the cast result or crash and burn
            foreach (IMyTerminalBlock block in allFoundBlocks) {
                return (T)block;
            }
            return default(T);
        }

        public static void SetupTextPanelForMatrixDisplay(
            IMyGridTerminalSystem GridTerminalSystem,
            string textPanelName,
            float fontSize
        ) {
            // Get the text panel
               IMyTextPanel TextPanel = TerminalUtils.FindFirstBlockWithName<IMyTextPanel>(GridTerminalSystem, textPanelName);
               if (TextPanel == null) {
                   throw new ArgumentException("Cannot find a text panel named [" + textPanelName + "]");
               }

            // Set up the text panel
               TextPanel.ContentType = ContentType.TEXT_AND_IMAGE;
               TextPanel.FontSize = fontSize;
               TextPanel.TextPadding = 0;
               TextPanel.SetValue<long>("Font", 1147350002);
               TextPanel.Alignment = TextAlignment.LEFT;
        }
    }
}
