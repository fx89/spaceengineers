/******************************************************************************************************************/
/*                                                                                                                */
/*                     STROBE LIGHTS SCRIPT - FOR AUTOMATING THE PARAMETERS OF LIGHTING BLOCKS                    */
/*                                                                                                                */
/******************************************************************************************************************/
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


// Search for // CONFIG // to get to the configuration section !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


// README //////////////////////////////////////////////////////////////////////////////////////////////////////////
/*
 *   ABOUT:
 *      This script allows automating the parameters of lighting blocks using parameterized mathematical functions.
 *
 *      This allows for smooth transitions between light states (for example on / off).
 *
 *      The script is configurable - one may any combination of transitions with distinct options to any parameter
 *      of any lighting block (interior lights, corner lights or spotlights) accessible through the IMyLightingBlock
 *      interface. The transitions may run only once, or they may repeat as many times as needed, or they can run
 *      forever. 
 *
 *      The script is extensible - one may easily add new transition functions and/or new methods of application for
 *      any other parameter of a given lighting block
 *
 *
 *   CONFIGURATION:
 *      To configure the script, one must modify the configStrobeLightsManager() method of this program. This method
 *      may be found below, in the CONFIG section (after this README). 
 *
 *      Follow the examples in the CONFIG section to build a configuration that's suitable for your grid. 
 *
 *      The following methods are available in the StrobleLightsManagerConfigurator
 *            - forLightsWithName(name) => the transitions below this line will apply to all lighting blocks having
 *                                         the given name
 *
 *            - beginTransition(type)   => begin configuring a transition of the given type
 *
 *            - ofLength(nbr of ticks)  => the script ticks once every 10 game ticks - the transition will last as
 *                                         long as the given numbr of ticks (script ticks) after which it will repeat
 *                                         unless configured otherwise
 *
 *            - repeatableFor(n times)  => the upon completion, the transition resets and repeats for the given number
 *                                         of times --- the transition will repeat forever if this method is not called
 *
 *            - betweenValues(min, max) => sets the values between which the parameters of lighting block will oscilate
 *
 *            - forParameter(parameter) => sets the target parameter for the transition
 *
 *            - endTransition()         => the transition is configured and the configurator is ready for a new
 *                                         transition for the same group of lighting blocks
 *
 *      !!! Do not call forLightsWithName() before endTransition() as the transition will not be created
 *          Do not call beginTransition() before endTransition() as the transition will not be created
 *          The transition builder will throw exceptions if a transition is not configured properly
 *          The order of the instructions given to the configurator is not verified
 *
 *      The following table shows the minimum and maximum possible values for each parameter,
 *      which are in line with those applicable when configuring lights via the UI:
 *          LightParameter.INTENSITY : between 0.0 and  10.0
 *          LightParameter.RADIUS    : between 1.0 and  20.0
 *          LightParameter.FALLOFF   : between 0.0 and   3.0
 *          LightParameter.COLOR_*   : between 0.0 and 255.0
 *
 *      The following table describes the graphs of the stock transition functions:
 *          Transition.LINEAR     : ////////////////// - it's actually sawtooth
 *          Transition.TRIANGULAR : /\/\/\/\/\/\/\/\/\
 *          Transition.QUADRATIC  : \_/\_/\_/\_/\_/\_/
 *          Transition.SINUSOIDAL : \_/-\_/-\_/-\_/-\_
 *          Transition.RANDOM     : random pulses of random intensity and without any pattern
 *
 *
 *   ADDING NEW TRANSITION FUNCTIONS:
 *      Search for "// UTILITIES FOR THE CONFIG BUILDER //" and add the new transition type to the Transition enum
 *      Search for "// TRANSITION FUNCTIONS //" to get to the transitions section
 *      Copy one of the existing functions and implement the new mathematical equation: f(index) = value
 *      Add the new function to the transitionFunctions dictionary which can be found at the end of the section
 *
 *
 *   ADDING NEW VALUE APPLIERS - to extend the list of paramters editable by the script:
 *      Search for "// UTILITIES FOR THE CONFIG BUILDER //" and add the new paramter type to the LightParameter enum
 *      Search for "// VALUE APPLIERS //" to get to that section
 *      Copy one of the existing action and modify it for the new parameter
 *      Add the newly created action to the parameterValueAppliers dictionary which is found near the end of the section
 *      Add the validator function for the new parameter to the parameterValueValidators dictionary
 *
 */
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIG //////////////////////////////////////////////////////////////////////////////////////////////////////////

private static void configStrobeLightsManager(StrobeLightsManager manager) {
new StrobleLightsManagerConfigurator(manager)
//==================================================================================================================
// DO NOT EDIT ABOVE THIS LINE






// !!! This is an example config. It should be replaced with the configuration required for your grid. 



 // For all lights named "TEST RED ALERT LIGHT", add the following transitions:
   .forLightsWithName("TEST RED ALERT LIGHT")
     // Configure a sinusoidal transition of the intensity parameter between 0.0 and 10.0 over 100 ticks
     // and which repeats 5 times
       .beginTransition(Transition.SINUSOIDAL)
           .ofLength      (100)
           .repeatableFor (5)
           .betweenValues (0.0, 10.0)
           .forParameter  (LightParameter.INTENSITY)
       .endTransition()



 // For all lights named "TEST STROBE LIGHT", add the following transitions:
   .forLightsWithName("TEST DISCO LIGHT")
     // Configure a linear transition of the intensity of 
     // The transition will take 400 ticks of the script to complete and will rewind and restart after completion
     // The transition will start with 0 intensity and will go all the way up to 10
       .beginTransition(Transition.LINEAR)
           .ofLength      (10)
           .betweenValues (0.0, 10.0)
           .forParameter  (LightParameter.INTENSITY)
       .endTransition()

     // Configure a linear transition of the RED color channel from the value 0 to the value 255 over 50 ticks
       .beginTransition(Transition.LINEAR)
           .ofLength      (50)
           .betweenValues (0, 255)
           .forParameter  (LightParameter.COLOR_RED)
       .endTransition()

     // Configure a linear transition of the GREEN color channel from the value 0 to the value 255 over 100 ticks
       .beginTransition(Transition.LINEAR)
           .ofLength      (100)
           .betweenValues (0, 255)
           .forParameter  (LightParameter.COLOR_GREEN)
       .endTransition()

     // Configure a linear transition of the BLUE color channel from the value 0 to the value 255 over 25 ticks
       .beginTransition(Transition.LINEAR)
           .ofLength      (25)
           .betweenValues (0, 255)
           .forParameter  (LightParameter.COLOR_BLUE)
       .endTransition()

     // Configure a random transition of the radius from the value 1 to the value 20
     // The length does not matter cause it's random
       .beginTransition(Transition.RANDOM)
           .betweenValues (1.0, 20.0)
           .forParameter  (LightParameter.RADIUS)
       .endTransition()

     // Configure a triangular transition of the falloff from the value 0 to the value 3 over 60 ticks
       .beginTransition(Transition.TRIANGULAR)
           .ofLength      (60)
           .betweenValues (0.0, 3.0)
           .forParameter  (LightParameter.FALLOFF)
       .endTransition()








// DO NOT EDIT BELOW THIS LINE !!!
//==================================================================================================================
;}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// UTILITIES FOR THE CONFIG BUILDER ////////////////////////////////////////////////////////////////////////////////

/**
 * A list of parameters that can be automated by this script
 */
private enum LightParameter {
    INTENSITY
   ,RADIUS
   ,FALLOFF
   ,COLOR_RED
   ,COLOR_GREEN
   ,COLOR_BLUE
}

/**
 * A list of transition functions that can be applied for parameter automation
 */
private enum Transition {
    LINEAR
   ,TRIANGULAR
   ,QUADRATIC
   ,SINUSOIDAL
   ,RANDOM
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// CONFIG BUILDER //////////////////////////////////////////////////////////////////////////////////////////////////

private class StrobleLightsManagerConfigurator {
    private StrobeLightsManager       manager;
    private StrobeLight               light;
    private PropertyTransitionBuilder transitionBuilder;

    public StrobleLightsManagerConfigurator(StrobeLightsManager manager) {
        this.manager = manager;
    }

    public StrobleLightsManagerConfigurator forLightsWithName(String name) {
        light = manager.addStrobeLight(name);
        return this;
    }

    public StrobleLightsManagerConfigurator beginTransition(Transition transition) {
        transitionBuilder = new PropertyTransitionBuilder().withTransition(transition);
        return this;
    }

    public StrobleLightsManagerConfigurator ofLength(int length) {
        transitionBuilder.ofLength(length);
        return this;
    }

    public StrobleLightsManagerConfigurator repeatableFor(int nTimes) {
        transitionBuilder.repeatableFor(nTimes);
        return this;
    }

    public StrobleLightsManagerConfigurator betweenValues(double min, double max) {
        transitionBuilder.betweenValues(min, max);
        return this;
    }
    
    public StrobleLightsManagerConfigurator forParameter(LightParameter parameter) {
        transitionBuilder.forParameter(parameter);
        return this;
    }

    public StrobleLightsManagerConfigurator endTransition() {
        light.addTransition(transitionBuilder.build());
        return this;
    }
}

private class PropertyTransitionBuilder {
    private double minValue      =   0;
    private double maxValue      =  50;
    private int    nTicks        = 100;
    private int    nRepeatCycles =   0;
    private Func<int,int,double,double,double> progressionFunction;
    private Action<IMyLightingBlock,double> valueApplier;
    private LightParameter parameter; // for the verification method

    public PropertyTransitionBuilder forParameter(LightParameter parameter) {
        this.parameter = parameter; // for the verification method
        valueApplier   = parameterValueAppliers[parameter];       
        return this;
    }

    public PropertyTransitionBuilder withTransition(Transition transition) {
        progressionFunction = transitionFunctions[transition];
        return this;
    }

    public PropertyTransitionBuilder ofLength(int nTicks) {
        this.nTicks = nTicks;
        return this;
    }

    public PropertyTransitionBuilder betweenValues(double min, double max) {
        minValue = min;
        maxValue = max;
        return this;
    }

    public PropertyTransitionBuilder repeatableFor(int nCycles) {
        nRepeatCycles = nCycles;
        return this;
    }

    public PropertyTransition build() {
        verifyConfig();
        return new PropertyTransition(minValue, maxValue, nTicks, nRepeatCycles, progressionFunction, valueApplier);
    }

    private void verifyConfig() {
        if (parameterValueValidators[parameter](minValue) == false) {
            throw new Exception("Invalid minimum value");
        }

        if (parameterValueValidators[parameter](maxValue) == false) {
            throw new Exception("Invalid maximum value");
        }

        if (minValue >= maxValue) {
            throw new Exception("The maximum value must be greater than the minimum value");
        }

        if (nTicks < 10 || nTicks > 500) {
            throw new Exception("Invalid transition length - must be between 10 ticks and 500 ticks");
        }

        if (progressionFunction == null) {
            throw new Exception("Missing transition function - call withTransition() before build()");
        }

        if (valueApplier == null) {
            throw new Exception("Missing parameter specification - call forParameter() before build()");
        }

        if (nRepeatCycles < 0) {
            throw new Exception("The repeatableFor() method was called with a negative parameter");
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// UTILITIES ///////////////////////////////////////////////////////////////////////////////////////////////////////

private const int NOT_FOUND     = -1;

private static T findBlockOfTypeWithName<T>(IMyGridTerminalSystem GridTerminalSystem, String blockName)
where T : class {
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks);

    foreach (IMyTerminalBlock block in blocks) {
        if (blockName.Equals(block.CustomName)) {
            return (T)block;
        }
    }

    throw new Exception("Unable to find a " + typeof(T).Name + " named [" + blockName + "]");
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

private static void DebugListProperties(IMyTerminalBlock block) {
    List<ITerminalProperty> properties = new List<ITerminalProperty>();
    block.GetProperties(properties);

    foreach(ITerminalProperty property in properties) {
        PROGRAM.Echo("[" + property.Id + "]:[" + property.TypeName + "]");
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




// STROBE LIGHTS FRAMEWORK /////////////////////////////////////////////////////////////////////////////////////////

/**
 * Specification for custom transitions
 * Not really needed now, but maybe in the future the PropertyTransition will not provide enough generalization
 */
private interface IStrobeLightTransition {
 // Called only once, when the transition is added to the target lighting block
 // Modifies the parameters of the target lighting block to a desirable states
    void resetLightingBlock(IMyLightingBlock lightingBlock);

 // Called during every tick
 // Gradually modifies the parameters of the referenced lighting block according to
 // the implemented transition function
    void tick(IMyLightingBlock lightingBlock);
}

/**
 * Applies the configured transitions to all of the given lighting blocks
 */
private class StrobeLight {
 // The given lighting blocks
    private List<IMyLightingBlock> lightingBlocks = new List<IMyLightingBlock>();

 // The configured transitions
    private List<IStrobeLightTransition> transitions = new List<IStrobeLightTransition>();

 // Adds a lighting block to the list of given lighting blocks
    public void addLightingBlock(IMyLightingBlock lightingBlock) {
        lightingBlocks.Add(lightingBlock);
    }

 // It is recommended to call this method only after all required calls to addLightingBlock() are done
 // This method adds a new transition to the list of configured transitions and calls the reset method
 // of the transition for the all of the currently added block
    public StrobeLight addTransition(IStrobeLightTransition transition) {
        transitions.Add(transition);

        foreach(IMyLightingBlock lightingBlock in lightingBlocks) {
            transition.resetLightingBlock(lightingBlock);
        }

        return this;
    }

 // Applies each of the configured transitions to each of the given lighting blocks
    public void tick() {
        foreach(IMyLightingBlock lightingBlock in lightingBlocks) {
            foreach(IStrobeLightTransition transition in transitions) {
                transition.tick(lightingBlock);
            }
        }
    }
}

/**
 * Container for the configured StrobeLights
 * Identifies lighting blocks in the terminal system
 * Forwards the tick() command to each of the configured ligts
 */
private class StrobeLightsManager {
    private IMyGridTerminalSystem gridTerminalSystem;
    private List<StrobeLight> strobeLights = new List<StrobeLight>();

    public StrobeLightsManager(IMyGridTerminalSystem gridTerminalSystem) {
        this.gridTerminalSystem = gridTerminalSystem;
    }

    public StrobeLight addStrobeLight(String lightName) {
        StrobeLight strobeLight = new StrobeLight();
        strobeLights.Add(strobeLight);

        execForAllBlocksOfType<IMyLightingBlock>(gridTerminalSystem, lightingBlock => {
            if (lightName.Equals(lightingBlock.CustomName)) {
                strobeLight.addLightingBlock(lightingBlock);
            }
            return true;
        });

        return strobeLight;
    }

    public void tick() {
        foreach(StrobeLight strobeLight in strobeLights) {
            strobeLight.tick();
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// TRANSITIONS /////////////////////////////////////////////////////////////////////////////////////////////////////

/**
 * Modifies the intensity of a lighting block based on its interval length
 */
private class PropertyTransition : IStrobeLightTransition {
 // Cap the intensity value so that it does not overflow
    private double minValue;
    private double maxValue;

 // f(tickIndex,maxIndex,minValue,maxValue) = value
    private Func<int,int,double,double,double> progressionFunction;

 // The index value will cycle within the domain of the progressionFunction
    private int index = 0;

 // The domain of the progressionFunction is between 0 and maxIndex
    private int maxIndex;

 // How many times to repeat the transition --- 0 = forever
    private int nRepeatCycles;
    private const int REPEAT_FOREVER = 0;

 // Counter for the current repeat cycle
    private int repeatCycle = 0;

 // Applies the value to computed by the progressionFunction to one or more
 // of the properties of the referenced lighting block
    private Action<IMyLightingBlock,double> valueApplier;

    public PropertyTransition(
                double minValue, double maxValue,
                int maxIndex,
                int nRepeatCycles,
                Func<int,int,double,double,double> progressionFunction,
                Action<IMyLightingBlock,double> valueApplier
    ) {
        this.minValue            = minValue;
        this.maxValue            = maxValue;
        this.maxIndex            = maxIndex;
        this.progressionFunction = progressionFunction;
        this.valueApplier        = valueApplier;
        this.nRepeatCycles       = nRepeatCycles;
    }

    public void resetLightingBlock(IMyLightingBlock lightingBlock) {
        valueApplier(lightingBlock, minValue);
    }

    public void tick(IMyLightingBlock lightingBlock) {
        if (canRepeat()) {
            cycleIndex();
            double value = progressionFunction(index, maxIndex, minValue, maxValue);
            valueApplier(lightingBlock, value);
        }
    }

    private bool canRepeat() {
        return nRepeatCycles == REPEAT_FOREVER
            || repeatCycle   <  nRepeatCycles
        ;
    }

    private void cycleIndex() {
        index++;

        if (index > maxIndex) {
            index = 0;

            repeatCycle++;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// UTILITIES FOR TRANSITION FUNCTIONS //////////////////////////////////////////////////////////////////////////////

/**
 * Cap the value to either the minimum value or the maximum value, depending on the value :D
 * Call using types that support the < and > operators
 */
private static double cap(double value, double min, double max) {
    if (value < min) return min;
    if (value > max) return max;
                     return value;
}

/**
 * Used, for example, by the RandomStrobe function
 */
private static Random RANDOM = new Random();

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// TRANSITION FUNCTIONS ////////////////////////////////////////////////////////////////////////////////////////////

private static Func<int,int,double,double,double> LinearStrobe = (index,maxIndex,minValue,maxValue) => {
    double valueRange = maxValue   - minValue;
    double valueDelta = valueRange / maxIndex;
    double value      = minValue + (valueDelta * index);

    return cap(value, minValue, maxValue);
};

private static Func<int,int,double,double,double> TriangularStrobe = (index,maxIndex,minValue,maxValue) => {
    double valueRange = maxValue  - minValue;
    int     timeRange = maxIndex   / 2;
    double valueDelta = valueRange / timeRange;

    double  value = minValue + (valueDelta * (index <= timeRange ? index : maxIndex - index));
    
    return cap(value, minValue, maxValue);
};

private static Func<int,int,double,double,double> QuadraticStrobe = (index,maxIndex,minValue,maxValue) => {
    double translatedIndex = index - (maxIndex / 2);
    double value           = - (maxValue - ((translatedIndex * translatedIndex) / (maxValue - minValue)));
    
    return cap(value, minValue, maxValue);
};

private static Func<int,int,double,double,double> SinusoidalStrobe = (index,maxIndex,minValue,maxValue) => {
    double angle = (((Math.PI * 2) * (double)index) / (double)maxIndex) - Math.PI;
    double value = (Math.Cos(angle) * (maxValue - minValue)) + minValue;
    
    return cap(value, minValue, maxValue);
};

private static Func<int,int,double,double,double> RandomStrobe = (index,maxIndex,minValue,maxValue) => {
    double value = RANDOM.NextDouble() * (maxValue - minValue) + minValue;
    return cap(value, minValue, maxValue);
};

/**
 * Needed for the config builder
 */
private static Dictionary<Transition,Func<int,int,double,double,double>> transitionFunctions
    = new Dictionary<Transition,Func<int,int,double,double,double>>() {
        { Transition.LINEAR     , LinearStrobe     }
       ,{ Transition.TRIANGULAR , TriangularStrobe }
       ,{ Transition.QUADRATIC  , QuadraticStrobe  }
       ,{ Transition.SINUSOIDAL , SinusoidalStrobe }
       ,{ Transition.RANDOM     , RandomStrobe     }
    }
;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// VALUE APPLIERS //////////////////////////////////////////////////////////////////////////////////////////////////

private static Action<IMyLightingBlock,double> IntensityValueApplier = (lightingBlock, value) => {
    lightingBlock.SetValue<float>("Intensity", (float)value);
};

private static Action<IMyLightingBlock,double> RadiusValueApplier = (lightingBlock, value) => {
    lightingBlock.SetValue<float>("Radius", (float)value);
};

private static Action<IMyLightingBlock,double> FalloffValueApplier = (lightingBlock, value) => {
    lightingBlock.SetValue<float>("Falloff", (float)value);
};

private static Action<IMyLightingBlock,Color> ColorValueApplier = (lightingBlock, value) => {
    lightingBlock.Color = value;
};

private static Action<IMyLightingBlock,double> RedColorChannelValueApplier = (lightingBlock, value) => {
    lightingBlock.Color = new Color((byte)value, lightingBlock.Color.G, lightingBlock.Color.B, 255);
};

private static Action<IMyLightingBlock,double> GreenColorChannelValueApplier = (lightingBlock, value) => {
    lightingBlock.Color = new Color(lightingBlock.Color.R, (byte)value, lightingBlock.Color.B, 255);
};

private static Action<IMyLightingBlock,double> BlueColorChannelValueApplier = (lightingBlock, value) => {
    lightingBlock.Color = new Color(lightingBlock.Color.R, lightingBlock.Color.G, (byte)value, 255);
};

/**
 * Needed for the config builder
 */
private static Dictionary<LightParameter,Action<IMyLightingBlock,double>> parameterValueAppliers
    = new Dictionary<LightParameter,Action<IMyLightingBlock,double>>() {
        { LightParameter.INTENSITY  , IntensityValueApplier         }
       ,{ LightParameter.RADIUS     , RadiusValueApplier            }
       ,{ LightParameter.FALLOFF    , FalloffValueApplier           }
       ,{ LightParameter.COLOR_RED  , RedColorChannelValueApplier   }
       ,{ LightParameter.COLOR_GREEN, GreenColorChannelValueApplier }
       ,{ LightParameter.COLOR_BLUE , BlueColorChannelValueApplier  }
    }
;

/**
 * Needed for the verification of the MIN / MAX paramters in the config builder
 */
private static Dictionary<LightParameter,Func<double,bool>> parameterValueValidators
  = new Dictionary<LightParameter,Func<double,bool>>() {
        { LightParameter.INTENSITY  , value => 0.0d <= value && value <=  10.0d }
       ,{ LightParameter.RADIUS     , value => 1.0d <= value && value <=  20.0d }
       ,{ LightParameter.FALLOFF    , value => 0.0d <= value && value <=   3.0d }
       ,{ LightParameter.COLOR_RED  , value => 0.0d <= value && value <= 255.0d }
       ,{ LightParameter.COLOR_GREEN, value => 0.0d <= value && value <= 255.0d }
       ,{ LightParameter.COLOR_BLUE , value => 0.0d <= value && value <= 255.0d }
    };

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

private static Program PROGRAM;

private StrobeLightsManager strobeLightsManager;



public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    PROGRAM = this;

    strobeLightsManager = new StrobeLightsManager(GridTerminalSystem);
    configStrobeLightsManager(strobeLightsManager);
}

public void Main(string argument, UpdateType updateSource) {
 // debugEchoTickNumber();
    strobeLightsManager.tick();
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

private int tickNumber = 0;

private void debugEchoTickNumber() {
    PROGRAM.Echo("tick = " + tickNumber.ToString());
    tickNumber++;

    if (tickNumber > 32000) {
        tickNumber = 0;
    }
}
