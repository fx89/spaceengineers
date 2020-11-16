/******************************************************************************************************************/
/*                                                                                                                */
/*               DOOR AUTOMATION SCRIPT - FOR CREATING AIRLOCKS AND MAKING DOORS CLOSE BY THEMSELVES              */
/*                                                                                                                */
/******************************************************************************************************************/
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// CONFIG //////////////////////////////////////////////////////////////////////////////////////////////////////////

// This constant defines the number of cycles which must pass until an auto-closing door
// or auto-closing airlock system is automatically closed
const int AUTO_CLOSE_INTERVAL_CYCLES = 10;

// This is where airlock systems are defined. An airlock system is a pair of doors which
// operate together so that when one door opens, the other closes automatically
//
// !!! IMPORTANT NOTE:
//     If there are two or more doors with the same name in the grid, the airlock system
//     will use only the first door that was added to the grid
//
private static void InitAirlocks(AirlockManager airlockManager) {
/*  EXAMPLES OF TWO-DOOR AIRLOCK SYSTEM:
    airlockManager.createAirlock("AIRLOCK01 DOOR A", "AIRLOCK01 DOOR B"                 ); // Creates an airlock system using the referenced doors
    airlockManager.createAirlock("AIRLOCK02 DOOR A", "AIRLOCK02 DOOR B", AUTO_CLOSE_TRUE); // Creates an auto-closing airlock system using the referenced doors

    EXAMPLE OF THREE-DOOR AIRLOCK SYSTEM:
    airlockManager.createAirlock("AIRLOCK03 DOOR A", "AIRLOCK03 DOOR B", "AIRLOCK03 DOOR C"); // Creates an auto-closing airlock system using the referenced doors

    EXAMPLE OF A MULTIPLE DOORS AIRLOCK SYSTEM:
    airlockManager.createAirlock("AIRLOCK04 DOOR A", "AIRLOCK04 DOOR B"); // When door B opens, door A closes
    airlockManager.createAirlock("AIRLOCK04 DOOR B", "AIRLOCK04 DOOR C"); // When door C opens, door B closes
    airlockManager.createAirlock("AIRLOCK04 DOOR C", "AIRLOCK04 DOOR D"); // When door D opens, door C closes
    airlockManager.createAirlock("AIRLOCK04 DOOR D", "AIRLOCK04 DOOR A"); // When door A opens, door D closes -- VERY IMPORTANT !!! when two people pass through the airlock one after another in the same direction, it is important that the second finds that last door closed

    EXAMPLE OF AN AUTO-SEALING TWO DOOR AIRLOCK SYSTEM:
    airlockManager.createAirlock("AIRLOCK05 DOOR A", "AIRLOCK05 DOOR B", AUTO_CLOSE_TRUE, AUTO_SEAL_TRUE);
*/
}

// This is where the auto-closing doors are configured
// Either one of the following configurations are accepted:
//     - a list of doors which should auto-close
//     - all the doors should auto-close
// The two configurations are mutually exclusive
//
// IMPORTANT NOTE:
// Doors which are already part of airlock systems will be ignored even if they are referenced in the list
//
private static void InitAutoClosingDoors(AutoClosingDoorsManager autoClosingDoorsManager) {
/*  EXAMPLE 1: A list of doors which should auto-close
    autoClosingDoorsManager.addAutoClosingDoor("TEST AUTOCLOSING DOOR 1");
    autoClosingDoorsManager.addAutoClosingDoor("TEST AUTOCLOSING DOOR 2");
    autoClosingDoorsManager.addAutoClosingDoor("TEST AUTOCLOSING DOOR 3");
*/
/*  EXAMPLE 2: All the doors should auto-close, except the ones named "TEST EXCLUDED DOOR"
    autoClosingDoorsManager.excludeDoor("TEST EXCLUDED DOOR");
    autoClosingDoorsManager.addAllDoorsNotPartOfAirlocks();
*/
    autoClosingDoorsManager.excludeDoor("EXCL"); // Exclude any door who's name begins with EXCL
    autoClosingDoorsManager.addAllDoorsNotPartOfAirlocks(); // Make all doors auto-close by default
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




// AIRLOCK FRAMEWORK ///////////////////////////////////////////////////////////////////////////////////////////////

// These constants make the calls in the InitAirlocks() method easier to understand
const bool AUTO_CLOSE_TRUE  = true ;
const bool AUTO_CLOSE_FALSE = false;

const bool AUTO_SEAL_TRUE  = true ;
const bool AUTO_SEAL_FALSE = false;

// Used internally when calling IMyDoor.ApplyAction()
const String PWR_STATE_OFF = "OnOff_Off";
const String PWR_STATE_ON  = "OnOff_On" ;

private enum DoorEvent {
    ERROR,
    OPENING,
    CLOSING,
    JUST_OPENED,
    JUST_CLOSED
}

/**
 * Watches a given door for a change in status and
 * fires an event when the change occurs
 */
private class DoorEventWatcher {
    IMyDoor door;
  
    DoorStatus currentState;
    DoorStatus previousState;

    Action<DoorEvent> eventHandler;

    private DoorStatus getDoorState() {
        return door.Status;
     // return TerminalPropertyExtensions.GetValueBool(door, "Open");
    }

    public DoorEventWatcher(IMyDoor door, Action<DoorEvent> eventHandler) {
        this.door         = door;
        currentState      = getDoorState();
        previousState     = currentState;
        this.eventHandler = eventHandler;
    }

    private DoorEvent computeEvent() {
        if (currentState == DoorStatus.Opening) return DoorEvent.OPENING;
        if (currentState == DoorStatus.Closing) return DoorEvent.CLOSING;
        if (currentState == DoorStatus.Open   ) return DoorEvent.JUST_OPENED;
        if (currentState == DoorStatus.Closed ) return DoorEvent.JUST_CLOSED;
                                                return DoorEvent.ERROR;
    }

    public void tick() {
        currentState = getDoorState();

        if (currentState != previousState) {
            eventHandler(computeEvent());
        }

        previousState = currentState;
    }
}

/**
 * Employs two DoorEventWatchers, one for each of the two given doors, 
 * to watch for changes in the state of each door and to close one door
 * when the other opens
 */
private class Airlock {
    private bool hasAutoCloseFeature;
    private bool hasAutoSealFeature;
    private int  autoCloseCycles = 0;
    private bool isOpen;

    private IMyDoor doorA, doorB;

    private DoorEventWatcher doorAWatcher, doorBWatcher;

    public Airlock(IMyDoor doorA, IMyDoor doorB, bool hasAutoCloseFeature, bool hasAutoSealFeature = false) {
        this.doorA = doorA;
        this.doorB = doorB;

        this.hasAutoCloseFeature = hasAutoCloseFeature;
        this.hasAutoSealFeature = hasAutoSealFeature;

        doorAWatcher = createDoorWatcher(doorA, doorB);
        doorBWatcher = createDoorWatcher(doorB, doorA);
    }

    private DoorEventWatcher createDoorWatcher(IMyDoor door, IMyDoor otherDoor) {
        return new DoorEventWatcher( door,
                                evt => {
                                    if (evt == DoorEvent.OPENING){
                                        setPowerState(PWR_STATE_ON);    // Make sure the doors are turned on so that they can close
                                        otherDoor.CloseDoor();          // When a door opens, close the other door
                                        isOpen = true;                  // Note that the airlock is open - used by the auto-closing feature
                                        autoCloseCycles = 0;            // The auto-close count down must always start from the moment the last door opens
                                        if (hasAutoSealFeature) sealDoor(otherDoor); // Seal the other door if the airlock is auto-sealing
                                    }
                                    if (evt == DoorEvent.JUST_CLOSED){
                                        enableDoor(otherDoor);
                                    }
                                } 
                           );
    }

    private void setPowerState(String powerState) {
        doorA.ApplyAction(powerState);
        doorB.ApplyAction(powerState);
    }

    public void close() {
        doorA.CloseDoor();
        doorB.CloseDoor();
        isOpen = false;
    }

    public void seal() {
        close();
        setPowerState(PWR_STATE_OFF);
    }

    private void sealDoor(IMyDoor door) {
        door.CloseDoor();
        door.ApplyAction(PWR_STATE_OFF);
    }

    private void enableDoor(IMyDoor door) {
        door.ApplyAction(PWR_STATE_ON);
    }

    public void enable() {
        setPowerState(PWR_STATE_ON);
    }


    private void tickAutoClose() {
        if (isOpen) {
            autoCloseCycles++;

            if (autoCloseCycles >= AUTO_CLOSE_INTERVAL_CYCLES) {
                autoCloseCycles = 0;
                close();
            }
        }
    }

    public void tick() {
        doorAWatcher.tick();
        doorBWatcher.tick();

        if (hasAutoCloseFeature) {
            tickAutoClose();
        }
    }

    public IMyDoor getDoorA() {
        return doorA;
    }

    public IMyDoor getDoorB() {
        return doorB;
    }
}


/**
 * Container for multiple airlocks
 */
private class AirlockManager {
    private IMyGridTerminalSystem gridTerminalSystem;
    private List<Airlock> airlocks = new List<Airlock>();

    public AirlockManager(IMyGridTerminalSystem gridTerminalSystem) {
        this.gridTerminalSystem = gridTerminalSystem;
    }

    public void createAirlock(String nameOfDoorA, String nameOfDoorB, String nameOfDoorC, bool autoClose = false) {
        createAirlock(nameOfDoorA, nameOfDoorB, autoClose);
        createAirlock(nameOfDoorB, nameOfDoorC, autoClose);
        createAirlock(nameOfDoorC, nameOfDoorA, autoClose);
    }

    public void createAirlock(String nameOfDoorA, String nameOfDoorB, bool autoClose = false, bool autoSealing = false) {
     // Get references to both door
        IMyDoor doorA = findBlockOfTypeWithName<IMyDoor>(gridTerminalSystem, nameOfDoorA);
        IMyDoor doorB = findBlockOfTypeWithName<IMyDoor>(gridTerminalSystem, nameOfDoorB);

     // Check that the doors are not already part of more than one airlock system
        int nAirlocksWithDoorA = 0;
        int nAirlocksWithDoorB = 0;

        foreach(Airlock airlock in airlocks) {
            if (airlockContainsDoor(airlock, doorA)) nAirlocksWithDoorA++; 
            if (airlockContainsDoor(airlock, doorB)) nAirlocksWithDoorB++;
        }

        if (nAirlocksWithDoorA > 1) {
            throw new Exception(
                        "Cannot create the airlock [" + nameOfDoorA + "/" + nameOfDoorB + "] " +
                        "because door [" + nameOfDoorA + "] is already managed by two airlocks"
                      );
        }

        if (nAirlocksWithDoorB > 1) {
            throw new Exception(
                        "Cannot create the airlock [" + nameOfDoorA + "/" + nameOfDoorB + "] " +
                        "because door [" + nameOfDoorB + "] is already managed by two airlocks"
                      );
        }


     // Create a new airlock system with the referenced doors and close both doors
        Airlock newAirlock = new Airlock(doorA, doorB, autoClose, autoSealing);
        newAirlock.close();
        airlocks.Add(newAirlock);
    }

    private bool airlockContainsDoor(Airlock airlock, IMyDoor door) {
        return airlock.getDoorA().Equals(door) || airlock.getDoorB().Equals(door);
    }

 // Tells if a referenced door is contained by any airlock
    public bool doorUsedInAirlock(IMyDoor door) {
        foreach(Airlock airlock in airlocks) {
            if (airlockContainsDoor(airlock, door)) {
                return true;
            }
        }

        return false;
    }

    public void tick() {
        foreach(Airlock airlock in airlocks) {
            airlock.tick();
        }
    }
}


private class AutoClosingDoor {
    IMyDoor door;
    private int autoCloseCycles = 0;
    private bool isOpen;
    DoorEventWatcher eventWatcher;

    public AutoClosingDoor(IMyDoor door) {
        this.door = door;

        eventWatcher = new DoorEventWatcher( door, evt => {
                                    if (evt == DoorEvent.OPENING){
                                        isOpen = true;
                                        autoCloseCycles = 0;
                                    }
                                });
    }

    private void setPowerState(String powerState) {
        door.ApplyAction(powerState);
    }

    public void close() {
        setPowerState(PWR_STATE_ON);
        door.CloseDoor();
        isOpen = false;
    }

    private void tickAutoClose() {
        if (isOpen) {
            autoCloseCycles++;
            if (autoCloseCycles >= AUTO_CLOSE_INTERVAL_CYCLES) {
                close();
            }
        }
    }

    public IMyDoor getDoor() {
        return door;
    }

    public void tick() {
        eventWatcher.tick();
        tickAutoClose();
    }
}


private class AutoClosingDoorsManager {
    private IMyGridTerminalSystem gridTerminalSystem;
    private AirlockManager airlockManager;
    private List<AutoClosingDoor> doors = new List<AutoClosingDoor>();
    private bool addedAll = false;
    private List<String> namesExcludedFromAutoclosingList = new List<String>();

    public AutoClosingDoorsManager(IMyGridTerminalSystem gridTerminalSystem, AirlockManager airlockManager) {
        this.airlockManager = airlockManager;
        this.gridTerminalSystem = gridTerminalSystem;
    }

    private bool doorAlreadyAdded(IMyDoor door) {
        foreach(AutoClosingDoor acDoor in doors) {
            if (acDoor.getDoor().Equals(door)) {
                return true;
            }
        }

        return false;
    }

    private bool doorToBeExcludedFromAutoclosing(IMyDoor door) {
        foreach(String excludedDoorName in namesExcludedFromAutoclosingList) {
            if (door.CustomName.StartsWith(excludedDoorName)) {
                return true;
            }
        }

        return false;
    }

    private void addDoor(IMyDoor door) {
        if (doorToBeExcludedFromAutoclosing(door)) {
            PROGRAM.Echo("The door [" + door.CustomName + "] has been manually excluded from the auto-closing doors list");
        } else {
            AutoClosingDoor acDoor = new AutoClosingDoor(door);
            doors.Add(acDoor);
            acDoor.close();
        }
    }

    public void excludeDoor(String doorName) {
        namesExcludedFromAutoclosingList.Add(doorName);
    }

    public void addAutoClosingDoor(String doorName) {
        if (addedAll) {
            throw new Exception("The method addAutoClosingDoor cannot be called after addAllDoorsNotPartOfAirlocks");
        }

        IMyDoor door = findBlockOfTypeWithName<IMyDoor>(gridTerminalSystem, doorName);

        if (airlockManager.doorUsedInAirlock(door)) {
            throw new Exception("The door named [" + doorName + "] is being used in an airlock system and auto-closing is managed by the airlock");
        }

        if (doorAlreadyAdded(door)) {
            throw new Exception("The door named [" + doorName + "] has already been set up as auto-closing");
        }

        addDoor(door);
    }

    public void addAllDoorsNotPartOfAirlocks() {
        if (doors.Count > 0) {
            throw new Exception("The method addAllDoorsNotPartOfAirlocks cannot be called after addAutoClosingDoor");
        }

        execForAllBlocksOfType<IMyDoor>(gridTerminalSystem, door => {
            if (airlockManager.doorUsedInAirlock(door) == false) {
                addDoor(door);
            }
            return true;
        });

        addedAll = true;
    }

    public void tick() {
        foreach(AutoClosingDoor door in doors) {
            door.tick();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



// MAIN ////////////////////////////////////////////////////////////////////////////////////////////////////////////

private static Program PROGRAM;

AirlockManager airlockManager;
AutoClosingDoorsManager autoClosingDoorsManager;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    PROGRAM = this;

    airlockManager = new AirlockManager(GridTerminalSystem);
    InitAirlocks(airlockManager);

    autoClosingDoorsManager = new AutoClosingDoorsManager(GridTerminalSystem, airlockManager);
    InitAutoClosingDoors(autoClosingDoorsManager);
}

public void Main(string argument, UpdateType updateSource) {
    airlockManager.tick();
    autoClosingDoorsManager.tick();
}
