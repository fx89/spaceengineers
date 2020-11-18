
// HELPERS FOR CONFIG //////////////////////////////////////////////////////////////////////////////////////////////

private enum GaugeDisplayMode {
    JMP_DRV_VALUE_TEXT,      // display the average charge in the form of "Jump Drives: 100.00%"
    JMP_DRV_VALUE_PRGBAR,    // display the average charge in the form of "JMP DRV: ||||||||||||||||||||||||"
    JMP_DRV_STATUS_TEXT,     // display the overall status of the jump drives as either ONLINE or OFFLINE
    SPD_VALUE_TEXT,          // display the current speed of the grid in m/s - works only when the grid has at least a cockpit or a control block
    SPD_VALUE_PRGBAR,        // display the current speed in the form of "SPEED: ||||||||||||||||||||||||" - works only when the grid has at least a cockpit or a control block
    INTEGRITY_VALUE_TEXT,    // display the current structural integrity in %
    ARMOR_VALUE_TEXT         // display the current armor integrity in %
}

const int TEXT_IS_NOT_MULTILINE = -1;    // use this constant in textDisplayPanelModes configuration
                                         // to allow a gauge use the entire text panel instead of just one line

// Constant values for the jump drive status, to be used in display conditions (see examples below)
const int JMP_DRV_CFG_ERR = -1;
const int JMP_DRV_OFFLINE =  0;
const int JMP_DRV_ONLINE  =  1;




// CONFIG //////////////////////////////////////////////////////////////////////////////////////////////////////////

// Configure environment parameters
const double MAX_SPEED_MPS = 100; // This might have to be changed if working with mods that remove the speed limit

// Set this option to true to have the script change the text color of single line panels
// Set this option to false to keep the assigned color of single line panels
const bool applyStatusColors = true;

// Configure Colors for the average charge display
static Color CLR_JMP_CHARGE__ERR  = new Color(255,   0,   0, 255); // RED   - if there is no jump drive present
static Color CLR_JMP_CHARGE__FULL = new Color(  0, 255,   0, 255); // GREEN - if all of the jump drives are charged
static Color CLR_JMP_CHARGE__PND  = new Color(  0,   0, 255, 255); // BLUE  - if jump drives are charging

// Configure colors for the overall status of jump drives
static Color CLR_JMP_STATUS__OFF  = new Color(  0,   0, 255, 255); // BLUE  - if the jump drives are turned off
static Color CLR_JMP_STATUS__ON   = new Color(255, 255, 255, 255); // WHITE - if the jump drives are turned on
static Color CLR_JMP_STATUS__ERR  = new Color(255,   0,   0, 255); // RED   - if the jump drives are missing or not synchronized

// Configure general purpose colors
static Color CLR_ORANGE = new Color(255, 170, 0);

// Configure the names of the text panels and the type of information (display mode) to be shown on each of them
// If there are multiple text panels having the same name, they will all be updated with the configured information
List<TargetTextPanelConfig> config = new ConfigBuilder()

    .forTextPanel("DISPLAY - LOWER COCKPIT STATUS 1")                                       // Configure text panel named DISPLAY - LOWER COCKPIT STATUS 1
        .atLine(2).addGauge(GaugeDisplayMode.JMP_DRV_STATUS_TEXT)                           // Put the overall status of all jump drives in the 3rd line
        .atLine(3).addGauge(GaugeDisplayMode.JMP_DRV_VALUE_TEXT)                         // Write the average charge in percentage on the 4th line
        .atLine(4).addGauge(GaugeDisplayMode.JMP_DRV_VALUE_PRGBAR)                       // Put the average charge progress bar on the 5th line

    .forTextPanel("DISPLAY - JUMP DRIVE STATION STATUS")                                    // Configure text panel named DISPLAY - JUMP DRIVE STATION STATUS
        .atLine(TEXT_IS_NOT_MULTILINE).addGauge(GaugeDisplayMode.JMP_DRV_STATUS_TEXT)       // Replace the text of the panel with the overall status of the jump drives

    .forTextPanel("DISPLAY - JMP DRV CHARGE")                                                               // - Configure text panel named DISPLAY - JMP DRV CHARGE
        .atLine(TEXT_IS_NOT_MULTILINE)                                                                      //     - Replace the text of the panel
            .addGauge(GaugeDisplayMode.JMP_DRV_VALUE_PRGBAR)                                             //     - Put the progress bar
                .withCondition(gauges => gauges[GaugeDisplayMode.JMP_DRV_STATUS_TEXT] == JMP_DRV_ONLINE)    //         - But only if the jump drives are online
            .addGauge(GaugeDisplayMode.JMP_DRV_STATUS_TEXT)                                                 //     - Or put the jump drive status
                .withCondition(gauges => gauges[GaugeDisplayMode.JMP_DRV_STATUS_TEXT] != JMP_DRV_ONLINE)    //         - But only if the jump drives are not online
                .withAdditionaAction(appCtx => appCtx.forceRefresh[GaugeDisplayMode.JMP_DRV_VALUE_PRGBAR] = true) //    - Once the OFFLINE status is applied,
                                                                                                            //               force the refresh of the progressbar 
                                                                                                            //               when the jump drives come online
    .forTextPanel("DISPLAY - JMP DRV PCT CHARGE")                                           // Do the same thing as above for the average dump drive charge percentage display
        .atLine(TEXT_IS_NOT_MULTILINE)
            .addGauge(GaugeDisplayMode.JMP_DRV_VALUE_TEXT)
                .withCondition(gauges => gauges[GaugeDisplayMode.JMP_DRV_STATUS_TEXT] == JMP_DRV_ONLINE)
            .addGauge(GaugeDisplayMode.JMP_DRV_STATUS_TEXT)
                .withCondition(gauges => gauges[GaugeDisplayMode.JMP_DRV_STATUS_TEXT] != JMP_DRV_ONLINE)
                .withAdditionaAction(appCtx => appCtx.forceRefresh[GaugeDisplayMode.JMP_DRV_VALUE_TEXT] = true)

.build();

// The script runs once every 100 ticks, but it can be configured to skip a few ticks
const int N_TICKS_TO_SKIP = 1;



// END OF CONFIG ///////////////////////////////////////////////////////////////////////////////////////////////////




////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// CONFIG MODEL ////////////////////////////////////////////////////////////////////////////////////////////////////

private class TargetTextPanelConfig {
    public String textPanelName;
    public List<TargetTextPanelLineConfig> lineConfigurations = new List<TargetTextPanelLineConfig>();

    public TargetTextPanelConfig(String textPanelName) {
        this.textPanelName = textPanelName;
    }
}

private class TargetTextPanelLineConfig {
    public int lineNumber;
    public List<TargetTextPanelLineDisplayModeConfig> displayModeConfigurations = new List<TargetTextPanelLineDisplayModeConfig>();

    public TargetTextPanelLineConfig(int lineNumber) {
        this.lineNumber = lineNumber;
    }
}

private class TargetTextPanelLineDisplayModeConfig {
    public GaugeDisplayMode displayMode;
    public Func<Dictionary<GaugeDisplayMode,double>,bool> displayCondition;
    public Action<ApplicationContext> additionalAction;

    public TargetTextPanelLineDisplayModeConfig(GaugeDisplayMode displayMode) {
        this.displayMode = displayMode;
    }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// CONFIG BUILDER //////////////////////////////////////////////////////////////////////////////////////////////////

private class ConfigBuilder {
    private List<TargetTextPanelConfig> config = new List<TargetTextPanelConfig>();

    private TargetTextPanelConfig currentTextPanel;

    private TargetTextPanelLineConfig currentLine;

    private TargetTextPanelLineDisplayModeConfig currentDisplayModeConfig;

    public ConfigBuilder forTextPanel(String textPanelName) {
        currentTextPanel = new TargetTextPanelConfig(textPanelName);
        config.Add(currentTextPanel);

        return this;
    }

    public ConfigBuilder atLine(int lineNumber) {
        if (currentTextPanel == null) {
            throw new Exception("Any call to [atLine] must always be after a call to [forTextPanel]");
        }

        currentLine = new TargetTextPanelLineConfig(lineNumber);
        currentTextPanel.lineConfigurations.Add(currentLine);

        return this;
    }

    public ConfigBuilder addGauge(GaugeDisplayMode gaugeType) {
        if (currentLine == null) {
            throw new Exception("Any call to [addGauge] must always be after a call to [atLine]");
        }

        currentDisplayModeConfig = new TargetTextPanelLineDisplayModeConfig(gaugeType);
        currentLine.displayModeConfigurations.Add(currentDisplayModeConfig);

        return this;
    }

    public ConfigBuilder withCondition(Func<Dictionary<GaugeDisplayMode,double>,bool> condition) {
        if (currentDisplayModeConfig == null) {
            throw new Exception("Any call to [withCondition] must always be after a call to [addGauge]");
        }

        if (currentDisplayModeConfig.displayCondition != null) {
            throw new Exception("A display condition has already been set. There can be only one.");
        }

        currentDisplayModeConfig.displayCondition = condition;

        return this;
    }

    public ConfigBuilder withAdditionaAction(Action<ApplicationContext> additionalAction) {
        if (currentDisplayModeConfig == null) {
            throw new Exception("Any call to [withAdditionaAction] must always be after a call to [addGauge]");
        }

        if (currentDisplayModeConfig.additionalAction != null) {
            throw new Exception("An additional action has already been set. There can be only one.");
        }

        currentDisplayModeConfig.additionalAction = additionalAction;

        return this;
    }

    public List<TargetTextPanelConfig> build() {
        verifyConfiguration();
        return config;
    }

    private void verifyConfiguration() {
        if (config.Count == 0) {
            throw new Exception("Nothing was configured");
        }

        foreach(TargetTextPanelConfig textPanelConfig in config) {
            if (textPanelConfig.textPanelName == null || textPanelConfig.textPanelName == "") {
                throw new Exception("Invalid panel name");
            }

            if (textPanelConfig.lineConfigurations.Count == 0) {
                throw new Exception("No gauge configured for [" + textPanelConfig.textPanelName + "]");
            }

            foreach(TargetTextPanelLineConfig lineConfig in textPanelConfig.lineConfigurations) {
                if (lineConfig == null) {
                    throw new Exception("[" + textPanelConfig.textPanelName + "] has a null line configuration");
                }

                if (lineConfig.lineNumber < TEXT_IS_NOT_MULTILINE) {
                    throw new Exception("The minimum allowed line number is -1 (TEXT_IS_NOT_MULTILINE)");
                }
                
                if (lineConfig.lineNumber > 50) {
                    throw new Exception("The maximum allowed line number is 50");
                }

                if (lineConfig.displayModeConfigurations.Count == 0) {
                    if (lineConfig.lineNumber == TEXT_IS_NOT_MULTILINE) {
                        throw new Exception("No display modes configured for [" + textPanelConfig.textPanelName + "]");
                    } else {
                        throw new Exception("No display modes configured for line [" + lineConfig.lineNumber.ToString() + "] of [" + textPanelConfig.textPanelName + "]");
                    }
                }
            }
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// FOR DEBUGGING ///////////////////////////////////////////////////////////////////////////////////////////////////

public static Program PROGRAM;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// UTILITIES ///////////////////////////////////////////////////////////////////////////////////////////////////////

private const int MISSING_VALUE = -1;
private const int NOT_FOUND     = -1;

private static void execForAllBlocksWithName(
                            IMyGridTerminalSystem GridTerminalSystem,
                            String name,
                            Action<IMyTerminalBlock> theAction
) {
     // Get all blocks having the given name
        List<IMyTerminalBlock> work = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(name, work);

     // Run the referenced function for each block
        for (int i = 0; i < work.Count; i++) {
            theAction(work[i]);
        } 
    }

private static int execForAllBlocksOfType<T>(IMyGridTerminalSystem GridTerminalSystem, Func<T,bool> action)  where T : class
{
     // Get a list of all the objects of the given type in the grid
        List<T> blocks = new List<T>();
        GridTerminalSystem.GetBlocksOfType<T>(blocks); 

     // Return NOT_FOUND in case there are no objects present
        if (blocks.Count == 0) {
            return NOT_FOUND;
        }

     // Execute the referenced action for all objects of the referenced type
        for (int i = 0; i < blocks.Count; i++)  {
            if (blocks[i] is T)  {
                if(action(blocks[i]) == false) break;
            }
        }

     // The success status value is 0
        return 0;
}

private static void execForAllBlocksOfTypeWithName<T> (
                            IMyGridTerminalSystem GridTerminalSystem, 
                            String targetPanelName, 
                            Action<T> theAction
) where T : class 
{
        execForAllBlocksWithName(GridTerminalSystem, targetPanelName, blockTypeCandidate => {
            if (blockTypeCandidate is T) {
                theAction((T)blockTypeCandidate);
            }
        });
    }

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





// MINI FRAMEWORK //////////////////////////////////////////////////////////////////////////////////////////////////

private interface IGaugeDataHandler {
    /**
     *  Called only once, upon initialization. This is where intensive operations
     *  such as extracting object references from the grid may be done.
     */
    void init(IMyGridTerminalSystem GridTerminalSystem);

    /**
     *  Called upon each tick. Extracts the gauge value.
     */
    double computeValue(IMyGridTerminalSystem GridTerminalSystem);
}

private interface IGaugeDisplayHandler {
    void applyValue(IMyTextPanel targetPanel, double value);
    void applyCacheValue(IMyTextPanel targetPanel, int targetLine, double value, String cacheValue);
    String generateCacheValue(double value);
    bool requiresCache();
}

private class StatefulGaugeDisplay {
    private IMyGridTerminalSystem GridTerminalSystem;
    private IGaugeDisplayHandler displayHandler;
    private bool cacheAlreadyComputed = false;
    private string cache;

    public StatefulGaugeDisplay(IGaugeDisplayHandler displayHandler, IMyGridTerminalSystem GridTerminalSystem) {
        this.displayHandler = displayHandler;
        this.GridTerminalSystem = GridTerminalSystem;
    }

    public void prepareForNextIteration() {
        cacheAlreadyComputed = false;
    }

    public void applyValueToAllPanelsWithName(String targetPanelName, int targetLine, double value) {
        if (displayHandler.requiresCache()) {
            if (cacheAlreadyComputed == false) {
                cache = displayHandler.generateCacheValue(value);
                cacheAlreadyComputed = true;
            }
            execForAllBlocksOfTypeWithName<IMyTextPanel>(
                    GridTerminalSystem, targetPanelName, trgPanel => { displayHandler.applyCacheValue(trgPanel,targetLine, value,cache);}
            );
        } else {
            execForAllBlocksOfTypeWithName<IMyTextPanel>(
                    GridTerminalSystem, targetPanelName, trgPanel => { displayHandler.applyCacheValue(trgPanel,targetLine, value,cache);}
            );
        }
    }
}

private class StatefulGaugeManager {
    private IMyGridTerminalSystem GridTerminalSystem;
    private IGaugeDataHandler dataHandler;
    private double currentValue;
    private bool currentValueAlreadyComputed = false;
    private double previousValue;
    private Dictionary<GaugeDisplayMode,StatefulGaugeDisplay> gaugeDisplays = new Dictionary<GaugeDisplayMode,StatefulGaugeDisplay>();
    private ApplicationContext applicationContext;

    public StatefulGaugeManager(
                IMyGridTerminalSystem GridTerminalSystem,
                IGaugeDataHandler dataHandler,
                Dictionary<GaugeDisplayMode,IGaugeDisplayHandler> displayHandlers
           ) {
     // Will be injected further down the stack to provide search functionality
        this.GridTerminalSystem = GridTerminalSystem;

     // Assign the data handler
        this.dataHandler = dataHandler;
   
     // Build the displays for the supported display mode
        foreach (KeyValuePair<GaugeDisplayMode,IGaugeDisplayHandler> entry in displayHandlers) {
            gaugeDisplays.Add(entry.Key, new StatefulGaugeDisplay(entry.Value, GridTerminalSystem));

         // If there's an application context, initialize the force refresh parameters for the given display handlers
            if (applicationContext != null) {
                applicationContext.forceRefresh[entry.Key] = false;
            }
        }
    }

    public void init() {
        dataHandler.init(GridTerminalSystem);
    }

    public void setApplicationContext(ApplicationContext applicationContext) {
        this.applicationContext = applicationContext;

     // Initialize the force refresh parameters for the related display handlers
        foreach(KeyValuePair<GaugeDisplayMode,StatefulGaugeDisplay> entry in gaugeDisplays) {
            applicationContext.forceRefresh[entry.Key] = false;
        }
    }

    public void prepareForNextIteration() {
        previousValue = currentValue;
        currentValueAlreadyComputed = false;

        foreach (KeyValuePair<GaugeDisplayMode,StatefulGaugeDisplay> entry in gaugeDisplays) {
            entry.Value.prepareForNextIteration();
        }
    }

    public bool valueHasChanged() {
        return !(currentValue == previousValue);
    }

    public void applyValue(
                    String targetPanelName,
                    int targetLine,
                    GaugeDisplayMode displayMode,
                    Func<Dictionary<GaugeDisplayMode,double>,bool> condition,
                    Action<ApplicationContext> additionalAction
    ) {
     // If a condition function was given and it evaluates to false, then the current iteration must be skipped
        if (condition != null && condition(applicationContext.gaugeValues) == false) {
            return;
        }

     // The gauge value must be computed if this is the first time the method is called during this tick
        if (currentValueAlreadyComputed == false) {
            currentValue = dataHandler.computeValue(GridTerminalSystem);
            applicationContext.gaugeValues[displayMode] = currentValue;
            currentValueAlreadyComputed = true;
        }

     // If the gauge value has changed or if refresh is forced, then it needs to be applied to all target displays using the referenced display mode
        if (valueHasChanged() || applicationContext.forceRefresh[displayMode]) {
            gaugeDisplays[displayMode].applyValueToAllPanelsWithName(targetPanelName, targetLine, currentValue);

         // Execute any additional action on the recorded gauge values
            if (additionalAction != null) {
                additionalAction(applicationContext);
            }
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





// REUSABLES FOR GAUGES ////////////////////////////////////////////////////////////////////////////////////////////

private static void updateTextPanelTriStateColor(
                IMyTextPanel targetPanel,
                double value, double maxValue,
                Color clrOff, Color clrPending, Color clrOn
) {
    if (value == MISSING_VALUE) { 
        targetPanel.SetValue("FontColor", clrOff);
    }
    else {
        targetPanel.SetValue("FontColor", value == maxValue ? clrOn : clrPending);
    }
}

// Replaces the referenced line of text on the referenced text panel with the given replacement text
// Line numbers start from 0
// Throws exception if the line does not exist
private static String replaceLineInSourceText(String sourceText, int lineNumber, String replacementText) {
 // Get an array of lines from the source text
    String[] lines = sourceText.Split('\n');

 // Throw an exception if the line is not found
    if (lines.Length <= lineNumber) {
        throw new Exception("Unable to find line [" + lineNumber + "] on the target text panel");
    }

 // If the line exists, replace it with the given replacement text
    lines[lineNumber] = replacementText;

 // Rebuild the text
    String newText = "";
    foreach(String line in lines) {
        newText = newText + line + '\n';
    }

 // Return the patched texte
    return newText;
}
 
private static void writeValueToTextPanel(
                IMyTextPanel textPanel, int targetLine,
                double value, double maxValue,
                String cachedValue, String notAvailableMessage,
                Color clrOff, Color clrPending, Color clrOn
)
{
 // Compute the gauge value depending on the state of the value (present or missing)
    String gaugeValue = (value == MISSING_VALUE) ? notAvailableMessage : cachedValue;

 // Compute the text to be applied to the target text panel depending on wether or not the multiline option is used
    String textToWrite
        = (targetLine == TEXT_IS_NOT_MULTILINE) ? gaugeValue
                                                : replaceLineInSourceText(textPanel.GetPublicText(), targetLine, gaugeValue);

 // Update the status color of the text panel if configured to do so
 // This does not apply for multi-line text panels
    if (applyStatusColors && targetLine == TEXT_IS_NOT_MULTILINE) {
        updateTextPanelTriStateColor(textPanel, value, maxValue, clrOff, clrPending, clrOn);
    }

 // Update the display
    textPanel.ShowPublicTextOnScreen();
    textPanel.WritePublicText(textToWrite);
}

private static double readPowerLevelMWh(String strPowerLevel) {
    if (strPowerLevel.IndexOf("kWh") > 0) return Convert.ToDouble(strPowerLevel.Replace(" kWh", "")) / 1000;
    if (strPowerLevel.IndexOf("MWh") > 0) return Convert.ToDouble(strPowerLevel.Replace(" MWh", "")) ;

    throw new Exception("Could not identify measurement unit in string [" + strPowerLevel + "]");
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





// GAUGES //////////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * Computes the average charge of all the jump drives within the current grid
 * while ignoring the already full ones, which can cause the gauge to report the wrong value.
 * The result might also be MISSING_VALUE, which means that there are no jump drives present in the grid.
 */
private class JumpDriveChargeDataHandler : IGaugeDataHandler {
    
    private double getJumpDriveCharge(IMyJumpDrive jumpDrive)
    {
     // Parse the jump drive information for the current power value
        String thirdRow = jumpDrive.DetailedInfo.ToString().Split('\n')[4];   
        String powerValue = thirdRow.Split(':')[1];   

     // Compute the power level in MWh and return the value
        return readPowerLevelMWh(powerValue);
    }

    private double getAverageJumpDriveCharge(IMyGridTerminalSystem GridTerminalSystem, bool ignoreFullyCharged=true)
    {
        const double JUMP_DRIVE_MAX_CHARGE_MWH = 3;

        double totalCharge = 0;
        double maxPower = 0;

     // Compute totalCharge and maxPower by iterating through the jump drives
     // Do not look at any fully charged jump drives if so configured
        int status = execForAllBlocksOfType<IMyJumpDrive>( GridTerminalSystem, jumpDrive => 
        {
            double jumpDriveCharge = getJumpDriveCharge(jumpDrive);

            if (ignoreFullyCharged == false || (ignoreFullyCharged && jumpDriveCharge < JUMP_DRIVE_MAX_CHARGE_MWH)) {
                // PROGRAM.Echo(Convert.ToString(jumpDriveCharge));
                totalCharge += jumpDriveCharge;
                maxPower += JUMP_DRIVE_MAX_CHARGE_MWH;
            }

         // Continue looping
            return true;
        });

     // Return MISSING_VALUE in case there are no jump drives present
     // This will help the main function decide what to write on the target LCD
        if (status == NOT_FOUND) {
            return MISSING_VALUE;
        }
        
     // When ignoring fully charged drives and all drives are charged, one must return 100%
        if (ignoreFullyCharged && totalCharge == 0) return 1;

     // In all other cases, compute and return the average charge
        double avgCharge = totalCharge / maxPower;
        return avgCharge > 1 ? 1 : avgCharge; // CAP at 1 just to make sure it doesn't overflow on the LCD text panel
    }

    public void init(IMyGridTerminalSystem GridTerminalSystem) {
    }

    public double computeValue(IMyGridTerminalSystem GridTerminalSystem) {
        return getAverageJumpDriveCharge(GridTerminalSystem);
    }
}



/**
 * Computes the status of the jump drives in the grid.
 * Possible values:
 *                    0 = OFFLINE
 *                    1 = ONLINE
 *        MISSING_VALUE = Unable to find jump drives OR some of the drives are online, while others are offline
 */
private class JumpDriveStatusDataHandler : IGaugeDataHandler {

    public void init(IMyGridTerminalSystem GridTerminalSystem) {
    }

    public double computeValue(IMyGridTerminalSystem GridTerminalSystem) {
     // Initialize the overall state
        int initialState = MISSING_VALUE;

     // Presume that the states of the jump drives match
        bool statesMatch = true;

     // Loop through the jump drives, get their respective states and figure out if they match or not
     // Also collect the overall state
        int status = execForAllBlocksOfType<IMyJumpDrive>( GridTerminalSystem, jumpDrive => 
        {
         // Get the current jump drive state
            int jdState = TerminalPropertyExtensions.GetValueBool(jumpDrive, "OnOff") ? 1 : 0;

         // Check if the state matches that of the first jump drive
         // If this is the first jump drive, then set the initial state
            if (initialState == MISSING_VALUE) {
                initialState = jdState;
            } else {
                if (jdState != initialState) {
                    statesMatch = false;
                    return false; // Break the loop
                }
            }

         // Continue looping
            return true;
        });

     // If the states of all the jump drives match, then return it. Otherwise, return MISSING_VALUE
        return (statesMatch && status != NOT_FOUND) ? initialState : MISSING_VALUE;
    }
}




/**
 * Extracts the current speed of the ship in meters per second
 */
private class SpeedometerDataHandler : IGaugeDataHandler {
    private IMyShipController controller = null;

    public void init(IMyGridTerminalSystem GridTerminalSystem) {
        // Get the list of control stations
        List<IMyShipController> gridControllers = new List<IMyShipController>();
        GridTerminalSystem.GetBlocksOfType<IMyShipController>(gridControllers);

        // Raise exception if there is no control station in the grid
        if (gridControllers == null || gridControllers.Count <= 0) {
            return;
        }

        //Get the first one
        controller = gridControllers.First();
    }

    public double computeValue(IMyGridTerminalSystem GridTerminalSystem) {
        return (controller == null) ? MISSING_VALUE : controller.GetShipSpeed();
    }
}


/**
 * Measures the structural integrity of the ship by comparing the current
 * non-armor block count to the original non-armor block count
 */
private class StructuralIntegrityDataHandler : IGaugeDataHandler {
    private double initialBlocksCount = 0d;
    
    private IMyGridTerminalSystem GridTerminalSystem;

    public void init(IMyGridTerminalSystem GridTerminalSystem) {
		
		
        this.GridTerminalSystem = GridTerminalSystem;

        this.initialBlocksCount = countBlocks();

        if (initialBlocksCount == 0d) {
            throw new Exception("The ship does not appear to have any blocks");
        }
    }

    public double computeValue(IMyGridTerminalSystem GridTerminalSystem) {
        double currentBlocksCount = countBlocks();
        return currentBlocksCount / initialBlocksCount;
    }

    private int countBlocks() {
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(blocks);
        return blocks.Count;
    }
}



/**
 * Measures the armor health as the average health of all armor blocks in the grid
 */
private class ArmorIntegrityDataHandler : IGaugeDataHandler {
	// Initialization will happen over multiple iterations.
	// This is how many locations in the bounding box will
	// be parsed in any one init iteration.
	private static int N_POSITIONS_PER_INIT_ITERATION = 10000;

	// The cube grid
	private IMyCubeGrid cubeGrid = PROGRAM.Me.CubeGrid;

	// Iterators for the bounding box of the cube grid
	private int x = 0, y = 0, z = 0;

	// Turns TRUE once all the locations in the bounding box have been parsed
	private bool isInitialized = false;

	// Contains the positions of all the armor blocks present in the grid
	// at the time the script started ticking
	List<Vector3I> initialArmorBlockPositions = new List<Vector3I>();


    public void init(IMyGridTerminalSystem GridTerminalSystem) {
		// Initialization is done in the actuallyInit() method, which
		// spreads the initialization process across multiple iterations
		// of the logic loop, to avoid the "script is too complex" error.
		
		x = cubeGrid.Min.X;
		y = cubeGrid.Min.Y;
		z = cubeGrid.Min.Z;
    }

    public double computeValue(IMyGridTerminalSystem GridTerminalSystem) {
		return isInitialized ? actuallyComputeValue() : actuallyInit();
    }

	private double actuallyInit() {
		for (int i = 0 ; i < N_POSITIONS_PER_INIT_ITERATION ; i++) {
			Vector3I vPos = new Vector3I(x,y,z);
			if (gridPosIsArmorBlock(cubeGrid, vPos)) {
				initialArmorBlockPositions.Add(vPos);
			}

			x++; if (x > cubeGrid.Max.X) {
				x = cubeGrid.Min.X;
				y++; if (y > cubeGrid.Max.Y) {
					y = cubeGrid.Min.Y;
					z++; if (z > cubeGrid.Max.Z) {
						PROGRAM.Echo("Initialized");
						isInitialized = true;
						break;
					}
				}
			}
		}

		return NOT_FOUND;
	}

	private double actuallyComputeValue() {
		// Check how many of the original armor blocks are still present in the grid
		double currentArmorBlocksCount = 0d;
		foreach (Vector3I pos in initialArmorBlockPositions) {
			if (gridPosIsArmorBlock(cubeGrid, pos)) {
				currentArmorBlocksCount++;
			}
		}
		PROGRAM.Echo(currentArmorBlocksCount.ToString() + " / " + initialArmorBlockPositions.Count.ToString());
		// Compute the ratio
		return currentArmorBlocksCount / initialArmorBlockPositions.Count;
	}

	/**
	 *  GetCubeBlock() returns an IMySlimBlock if an IMyTerminalBlock resides at the
	 *  specified position. If an armor block is there, then it returns null. On the
	 *  other hand, CubeExists() returns true if any cube resides at the given coor-
	 *  dinates. If there is a cube at the given position and that cube is not and
	 *  IMyTerminalBlock, then it must be an armor block.
	 */
	private static bool gridPosIsArmorBlock(IMyCubeGrid cubeGrid, Vector3I pos) {
		return cubeGrid.CubeExists(pos) && (cubeGrid.GetCubeBlock(pos) == null);
	}
}






/**
 *  Holds the common display method for any given gauge value and the related configuration:
 *         - what to write in front of the value
 *         - what to write if the value is missing
 *         - which colors to use for missing, pending and full
 *         - what is the value for which the gauge can be considered full
 */
private abstract class AbstractColorTextGaugeDisplayHandler {
    protected String title;
    protected String missingValMsg;
    protected Color  clrOff, clrPending, clrOn;
    protected double maxValue;

    public AbstractColorTextGaugeDisplayHandler(String title, String missingValMsg, Color clrOff, Color clrPending, Color clrOn, double maxValue) {
        this.title         = title;
        this.missingValMsg = missingValMsg;
        this.clrOff        = clrOff;
        this.clrPending    = clrPending;
        this.clrOn         = clrOn;
        this.maxValue      = maxValue;
    }

    public void applyCacheValue(IMyTextPanel targetPanel, int targetLine, double value, String cacheValue) {
        writeValueToTextPanel(
            targetPanel, targetLine,
            value, maxValue,
            cacheValue, missingValMsg,
            clrOff, clrPending, clrOn
        );
    }
}



private class PercentTextDisplayHandler : AbstractColorTextGaugeDisplayHandler, IGaugeDisplayHandler {
    private static double MAX_VALUE = 1;

    public PercentTextDisplayHandler(String title, String missingValMsg, Color clrOff, Color clrPending, Color clrOn)
    : base(title, missingValMsg, clrOff, clrPending, clrOn, MAX_VALUE) { }

    public bool requiresCache() {
        return true;
    }

    public void applyValue(IMyTextPanel targetPanel, double value) {
        // Not required because we are using the cached value === requiresCache() returns true
    }

    public String generateCacheValue(double value) {
        return title + (value * 100).ToString("#.00") + "%";
    }
}



private class ProgressBarDisplayHandler : AbstractColorTextGaugeDisplayHandler, IGaugeDisplayHandler {
    public ProgressBarDisplayHandler(String title, String missingValMsg, Color clrOff, Color clrPending, Color clrOn, double maxValue = 1)
    : base(title, missingValMsg, clrOff, clrPending, clrOn, maxValue) { }

    public bool requiresCache() {
        return true;
    }

    public void applyValue(IMyTextPanel targetPanel, double value) {
        // Not required because we are using the cached value === requiresCache() returns true
    }

    public String generateCacheValue(double value) {
        const double maxBars = 24;
        double nLines = maxBars * value;
        String ret = title;

        for (int b = 0 ; b <= nLines ; b++) {
            ret = ret + "|";
        }

        return ret;
    }
}



private class BlockStateDisplayHandler : AbstractColorTextGaugeDisplayHandler, IGaugeDisplayHandler {
    private static double MAX_VALUE = 1;

    public BlockStateDisplayHandler(String title, String missingValMsg, Color clrOff, Color clrPending, Color clrOn)
    : base(title, missingValMsg, clrOff, clrPending, clrOn, MAX_VALUE) { }

    public bool requiresCache() {
        return true;
    }

    public void applyValue(IMyTextPanel targetPanel, double value) {
        // Not required because we are using the cached value === requiresCache() returns true
    }

    private String computeStatusString(double value) {
        if (value == 0) return "OFFLINE";
        if (value == 1) return "ONLINE";
        return "BAD VALUE";
    }

    public String generateCacheValue(double value) {
        return value == MISSING_VALUE ? missingValMsg : title + computeStatusString(value);
    }
}


private class ValueTextDisplayHandler : AbstractColorTextGaugeDisplayHandler, IGaugeDisplayHandler {
    private String unitName;

    public ValueTextDisplayHandler(String title, String missingValMsg, Color clrOff, Color clrPending, Color clrOn, String unitName)
    : base(title, missingValMsg, clrOff, clrPending, clrOn, 1) { 
        this.unitName = unitName;
    }

    public bool requiresCache() {
        return true;
    }

    public void applyValue(IMyTextPanel targetPanel, double value) {
        // Not required because we are using the cached value === requiresCache() returns true
    }

    public String generateCacheValue(double value) {
        return (value == MISSING_VALUE) ? missingValMsg : (title + string.Format("{0:N2}", value) + " " + unitName);
    }
}




////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






//WIRING ///////////////////////////////////////////////////////////////////////////////////////////////////////////

private class ApplicationContext {
    public Dictionary<GaugeDisplayMode,StatefulGaugeManager> gaugeManagers       = new Dictionary<GaugeDisplayMode,StatefulGaugeManager>();
    public List<StatefulGaugeManager>                        uniqueGaugeManagers = new List<StatefulGaugeManager>();
    public Dictionary<GaugeDisplayMode, double>              gaugeValues         = new Dictionary<GaugeDisplayMode, double>();
    public Dictionary<GaugeDisplayMode, bool>                forceRefresh        = new Dictionary<GaugeDisplayMode, bool>();

    public void addGaugeManagerForDisplayMode(GaugeDisplayMode displayMode, StatefulGaugeManager gaugeManager) {
        gaugeManager.setApplicationContext(this);
        gaugeManagers.Add(displayMode, gaugeManager);
        uniqueGaugeManagers.Add(gaugeManager);
        gaugeManager.init();
    }
}



ApplicationContext applicationContext = new ApplicationContext();



private void initWiring() {
 // Gauge manager for monitoring the average charge of jump drives
 // Has two display modes: text and progress bar
    StatefulGaugeManager jumpDriveChargeGaugeManager
        = new StatefulGaugeManager(GridTerminalSystem, 
                                    new JumpDriveChargeDataHandler(),
                                    new Dictionary<GaugeDisplayMode,IGaugeDisplayHandler>() {
                                            {GaugeDisplayMode.JMP_DRV_VALUE_TEXT  , new PercentTextDisplayHandler("Jump Drives: ", "!!! NO JUMP DRIVES !!!" , CLR_JMP_CHARGE__ERR, CLR_JMP_CHARGE__PND, CLR_JMP_CHARGE__FULL)},
                                            {GaugeDisplayMode.JMP_DRV_VALUE_PRGBAR, new ProgressBarDisplayHandler("JMP DRV: "    , "!!! NO JUMP DRIVES !!!" , CLR_JMP_CHARGE__ERR, CLR_JMP_CHARGE__PND, CLR_JMP_CHARGE__FULL)},
                                    });

 // Gauge manager for monitoring the overall status of jump drives
 // Has only one display mode, which says either ONLINE, OFFLINE or CONFIG ERROR
    StatefulGaugeManager jumpDriveStatusGaugeManager
        = new StatefulGaugeManager(GridTerminalSystem, 
                                    new JumpDriveStatusDataHandler(),
                                    new Dictionary<GaugeDisplayMode,IGaugeDisplayHandler>() {
                                            {GaugeDisplayMode.JMP_DRV_STATUS_TEXT   , new BlockStateDisplayHandler ("JMP DRV: "    , "!!! JMP DRV: CFG ERR !!!", CLR_JMP_STATUS__ERR, CLR_JMP_STATUS__OFF, CLR_JMP_STATUS__ON  )}
                                    });

 // Gauge manager for getting the ship's speed, assuming there are control seats or cockpits in the grid
    StatefulGaugeManager shipSpeedGaugeManager
        = new StatefulGaugeManager(GridTerminalSystem,
                                    new SpeedometerDataHandler(),
                                    new Dictionary<GaugeDisplayMode,IGaugeDisplayHandler>() {
                                        {GaugeDisplayMode.SPD_VALUE_TEXT  , new ValueTextDisplayHandler  ("Speed: ", "!!! NO COCKPIT !!!" , CLR_JMP_CHARGE__ERR, CLR_JMP_CHARGE__PND, CLR_JMP_CHARGE__FULL, "m/s")},
                                        {GaugeDisplayMode.SPD_VALUE_PRGBAR, new ProgressBarDisplayHandler("Speed: ", "!!! NO COCKPIT !!!" , CLR_JMP_CHARGE__ERR, CLR_JMP_CHARGE__PND, CLR_JMP_CHARGE__FULL, MAX_SPEED_MPS)},
                                    });

 // Gauge manager for getting the ship's structural integrity
    StatefulGaugeManager shipStructuralIntegrityGaugeManager
        = new StatefulGaugeManager(GridTerminalSystem,
                                    new StructuralIntegrityDataHandler(),
                                    new Dictionary<GaugeDisplayMode,IGaugeDisplayHandler>() {
                                        {GaugeDisplayMode.INTEGRITY_VALUE_TEXT, new PercentTextDisplayHandler("Integrity: ", "!!! CRITICAL DAMAGE !!!" , CLR_JMP_CHARGE__ERR, CLR_JMP_CHARGE__PND, CLR_JMP_CHARGE__FULL)},
                                    });

 // Gauge manager for getting the ship's structural integrity
    StatefulGaugeManager shipArmorIntegrityGaugeManager
        = new StatefulGaugeManager(GridTerminalSystem,
                                    new ArmorIntegrityDataHandler(),
                                    new Dictionary<GaugeDisplayMode,IGaugeDisplayHandler>() {
                                        {GaugeDisplayMode.ARMOR_VALUE_TEXT, new PercentTextDisplayHandler("Armor: ", "Initializing" , CLR_JMP_CHARGE__ERR, CLR_ORANGE, CLR_JMP_CHARGE__FULL)},
                                    });

 // Wiring for the jump drives
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.JMP_DRV_VALUE_TEXT  , jumpDriveChargeGaugeManager);
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.JMP_DRV_VALUE_PRGBAR, jumpDriveChargeGaugeManager);
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.JMP_DRV_STATUS_TEXT , jumpDriveStatusGaugeManager);

 // Wiring for the speedometer
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.SPD_VALUE_TEXT  , shipSpeedGaugeManager);
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.SPD_VALUE_PRGBAR, shipSpeedGaugeManager);
    
 // Wiring for the ship integrity counters
    applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.INTEGRITY_VALUE_TEXT, shipStructuralIntegrityGaugeManager);
	applicationContext.addGaugeManagerForDisplayMode(GaugeDisplayMode.ARMOR_VALUE_TEXT    , shipArmorIntegrityGaugeManager     );
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// BLOCK INIT AND SAVE /////////////////////////////////////////////////////////////////////////////////////////////

public Program() {
 // I wish there was an Update1000 somewhere, so I didn't have to use N_TICKS_TO_SKIP
 // I wish this property was of type int so I could use whatever value I wanted and get an exception if it was outside the valid range
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    PROGRAM = this; // for debugging from other contexts

 // Init the wiring without which nothing will happen
    initWiring();
}



public void Save(){
 // There is no state to be saved
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

private void processDisplayModes() {
 // Run through the target text panels and set the relevant text while also caching it so it doesn't have to be computed more than once
    foreach(TargetTextPanelConfig textPanelConfig in config) {
        foreach(TargetTextPanelLineConfig lineConfig in textPanelConfig.lineConfigurations) {
            foreach(TargetTextPanelLineDisplayModeConfig displayModeConfig in lineConfig.displayModeConfigurations) {
                applicationContext.gaugeManagers[displayModeConfig.displayMode].applyValue(
                                                                textPanelConfig.textPanelName,
                                                                lineConfig.lineNumber,
                                                                displayModeConfig.displayMode,
                                                                displayModeConfig.displayCondition,
                                                                displayModeConfig.additionalAction
                                                            );
            }
        }
    }

 // Prepare the gauge managers for the next iteration
    foreach(StatefulGaugeManager sgMngr in applicationContext.uniqueGaugeManagers) {
        sgMngr.prepareForNextIteration();
    }
}

int nSkippedTicks = 0;

public void Main(string argument, UpdateType updateSource) {
    nSkippedTicks++;
    if (nSkippedTicks > N_TICKS_TO_SKIP) {
        nSkippedTicks = 0;
        processDisplayModes();
    }
}