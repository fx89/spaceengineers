// This class implements IMyGridProgram:
// https://bloc97.github.io/SpaceEngineersModAPIDocs/html/463b6ab6-1cde-294b-2056-95e563450f52.htm

// API doc:
// https://bloc97.github.io/SpaceEngineersModAPIDocs/html/531c215a-1026-ad88-5a6f-3e5bdbd0be1d.htm
// https://bloc97.github.io/SpaceEngineersModAPIDocs/search.html?SearchText=imyentity
// https://bloc97.github.io/SpaceEngineersModAPIDocs/search.html?SearchText=imyshipcontroller
// https://bloc97.github.io/SpaceEngineersModAPIDocs/html/e25bf703-440e-0955-6058-30bbc21cd419.htm

// Better API doc:
// https://github.com/malware-dev/MDK-SE/wiki/Sandbox.ModAPI.Ingame.IMyShipController



///////////////////////////////////////////////////////////////////////////////////////////////////
// CONFIG SECTION /////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



private static FlightAssistantConfig configureFlightAssistant() {
	FlightAssistantConfig config = new FlightAssistantConfig();

	config.preferredControllerName = "Cockpit Flight Seat";

	return config;
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// WIRING /////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



public Program()
{
    // It's recommended to set RuntimeInfo.UpdateFrequency 
    // here, which will allow your script to run itself without a 
    // timer block.

    Runtime.UpdateFrequency |= UpdateFrequency.Update100;

	// All features of the flight assistant are injected here
	flightAssistant = new FlightAssistantBuilder()
								.withConfig(configureFlightAssistant())
								.withFeature(new CruiseControl())
								.withFeature(new AutoLevel())
								.withLogger(new ScriptOutputLogger())
							.build();
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// SCRIPT OBJECTS - MICROFRAMEWORK ////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  The FlightAssistant may have many features, such as cruise control, altitude control,
 *  anti-crash system, barrel roll, split S, etc. Each of these features is defined in
 *  a separate class, which implements this interface, so that the FlightAssistant knows
 *  how to run them.
 */
private interface IFlightAssistantFeature {
	void init(FlightAssistantRegistry registry, Log log, FlightAssistantControls controls, Sensors sensors, FlightAssistantConfig config);

	void tick(string argument);

	String getName();
}

/**
 *  The FlightAssistant will be logging things. By implementing this interface, one can make
 *  it log on the script output, or on some LCD panel, or on many different LCD panels, etc.
 */
private interface IFlightAssistantLogger {
	string getName();

	void init(IMyGridTerminalSystem gridTerminalSystem);

	void info(string info);

	void warning(string warning);

	void error(string error);
}

/**
 *  The FlightAssistant serves as a container for the features. each feature is injected into
 *  the list of features at construction time. At init, the FlightAssistant identifies the
 *  controller and passes on the reference to all the contained features. The FlightAssistant
 *  also keeps track of which feature is enabled and which is disabled. During each iteration
 *  of the script;s update cycle, the FlightAssistant runs an iteration of each enabled
 *  feature that it contains.
 */
private class FlightAssistant {
	/**
	 *  The list of features and their runtime state (i.e. enabled / disabled)
	 */
	private List<FlightAssistantFeatureWrapper> features = new List<FlightAssistantFeatureWrapper>();

	public List<FlightAssistantFeatureWrapper> getFeatures() {
		return features;
	}

	/**
	 *  User preferences (or defaults if not set - the builder takes care of that)
	 */
	private FlightAssistantConfig config = new FlightAssistantConfig();

	public FlightAssistantConfig getConfig() {
		return config;
	}

	/**
	 *  Helper object, providing the logging capability to the FlightAssistant 
	 */
	private Log log = new Log();

	public Log getLog() {
		return log;
	}

	/**
	 *  This registry stores references which are commonly used by all features (i.e.
	 *  a reference to the control seat or the list of thrusters grouped by the
	 *  direction in which the propel the ship
	 */
	private FlightAssistantRegistry registry = new FlightAssistantRegistry();

	private FlightAssistantControls controls = new FlightAssistantControls();

	/**
	 *  The sensors provide a common place where to find information about the ship
	 *  and the world around it - a facade for multiple API calls
	 */
	private Sensors sensors = new Sensors();

	/**
	 *  Store a reference to the GridTerminalSystem, identify the controller and initialize
	 *  the features, if they are enabled / runnable
	 */
	public void init(IMyGridTerminalSystem gridTerminalSystem) {
		// Get references to the GridTerminalSystem and the (preferred) control seat
		registry.gridTerminalSystem = gridTerminalSystem;
		registry.controller = identifyController(gridTerminalSystem, config.preferredControllerName);

		// Get references to the ship's thrusters, grouped by direction
		registry.thrusters = ThrustControl.identifyThrusters(registry);

		// Get references to the ship's gyros
		registry.gyros = GyroControl.identifyGyros(registry);

		// Initialize the log
		log.init(gridTerminalSystem);

		// Initialize the sensors
		sensors.init(registry);

		// Initialize the controls
		controls.init(registry);

		// Log the controller name
		log.info("Controller: " + registry.controller.CustomName);

		// Initialize the features
		features.ForEach(wrappedFeature => {
			if (shouldRunFeature(wrappedFeature)) {
				log.info("Initializing feature [" + wrappedFeature.feature.getName() + "]");
				wrappedFeature.feature.init(registry, log, controls, sensors, config);
				wrappedFeature.nameUCase = wrappedFeature.feature.getName().ToUpper();
			}
		});
	}

	/**
	 *  Tick each enabled / runnable feature
	 */
	public void tick(string argument) {
		// Get the feature name and argument from the command argument (if any)
		string featureName = null;
		string featureArgument = null;
		if (argument != null) {
			String[] parts = argument.ToUpper().Split(':');
			featureName = parts[0].Trim();
			if (parts.Length > 1 && parts[1] != null) {
				featureArgument = parts[1].Trim();
			}
		}

		// Tick all the features, while providing the feature argument to the feature
		// having the gven featureName (if any)
		features.ForEach(wrappedFeature => {
			if (shouldRunFeature(wrappedFeature)) {
				string arg = wrappedFeature.nameUCase.Equals(featureName) ? featureArgument : null;
				wrappedFeature.feature.tick(arg);
			}
		});
	}

	/**
	 *  Features should be run if they are enabled and if they have names,
	 *  so that they can be logged properly
	 */
	private static bool shouldRunFeature(FlightAssistantFeatureWrapper feature) {
		return feature.isEnabled && (feature.feature.getName() != null);
	}

	/**
	 *  If a preferred controller name is configured, then attempt to find the first
	 *  controller with the given name. If a preferred name is not provided or if a
	 *  controller with the given name could not be found, then return the very first 
	 *  controller in the grid. If the grid has no controller, an exception must be
	 *  raised as the script cannot continue without a controller.
	 */
	private static IMyShipController identifyController(IMyGridTerminalSystem gridTerminalSystem, string preferredControllerName) {
		// Get the list of control stations
		List<IMyShipController> gridControllers = new List<IMyShipController>();
		gridTerminalSystem.GetBlocksOfType<IMyShipController>(gridControllers);

		// Raise exception if there is no control station in the grid
		if (gridControllers == null || gridControllers.Count <= 0) {
			throw new ArgumentException("The grid has no usable control stations");
		}

		// If there is no preferred controller name defined, get the first one
		if (preferredControllerName == null) {
			return gridControllers.First();
		}

		// Attempt to identify the controller with the given name
		foreach (var gridController in gridControllers) {
			if (preferredControllerName.Equals(gridController.CustomName)) {
				return gridController;
			}
		}

		// In the event a controller with the preferred name was not found, get the first one
		return gridControllers.First();
	}

}

/**
 *  Something that's lighter to use as key in the dictionary than Vector3I
 *  (i.e. Direction.FORWARD (4 bytes) is liter than Vector3I.Forward (8 / 16 bytes))
 *  Not using VRageMath.Base6Directions.Direction cause it can't be imported and the
 *  fully qualified name is too long
 */
private enum Direction {
	FORWARD,
	BACKWARD,
	LEFT,
	RIGHT,
	UP,
	DOWN,
	NONE
}

/**
 *  Converts from VRageMath.Base6Directions.Direction to the local Direction type, to
 *  avoid using the long fully qualified name of the library type
 */
private static Direction vRageMathDirectionToDirection(VRageMath.Base6Directions.Direction direction) {
	if (direction == VRageMath.Base6Directions.Direction.Forward) return Direction.FORWARD;
	if (direction == VRageMath.Base6Directions.Direction.Backward) return Direction.BACKWARD;
	if (direction == VRageMath.Base6Directions.Direction.Left) return Direction.LEFT;
	if (direction == VRageMath.Base6Directions.Direction.Right) return Direction.RIGHT;
	if (direction == VRageMath.Base6Directions.Direction.Up) return Direction.UP;
	if (direction == VRageMath.Base6Directions.Direction.Down) return Direction.DOWN;

	return Direction.NONE;
}

/**
 *  Transformation utility
 */
private static Vector3D directionToVector3D(Direction direction) {
	if (direction == Direction.FORWARD) return Vector3D.Forward;
	if (direction == Direction.BACKWARD) return Vector3D.Backward;
	if (direction == Direction.LEFT) return Vector3D.Left;
	if (direction == Direction.RIGHT) return Vector3D.Right;
	if (direction == Direction.UP) return Vector3D.Up;
	if (direction == Direction.DOWN) return Vector3D.Down;

	return new Vector3D(0d, 0d, 0d);
}

/**
 *  Transformation utility
 */
private static Vector3I directionToVector3I(Direction direction) {
	if (direction == Direction.FORWARD) return Vector3I.Forward;
	if (direction == Direction.BACKWARD) return Vector3I.Backward;
	if (direction == Direction.LEFT) return Vector3I.Left;
	if (direction == Direction.RIGHT) return Vector3I.Right;
	if (direction == Direction.UP) return Vector3I.Up;
	if (direction == Direction.DOWN) return Vector3I.Down;

	return new Vector3I(0, 0, 0);
}

/**
 *  Transformation utility
 */
private static Vector3D directionToVector3D(VRageMath.Base6Directions.Direction direction) {
	return directionToVector3D(vRageMathDirectionToDirection(direction));
}

/**
 *  Transformation utility
 */
private static Vector3I directionToVector3I(VRageMath.Base6Directions.Direction direction) {
	return directionToVector3I(vRageMathDirectionToDirection(direction));
}

/**
 *  Groups the Forward and Up vectors of an object in world coordinates
 */
private struct WorldOrientation {
	public Vector3D Forward;
	public Vector3D Up;
}

/**
 *  Helper for any FlightAssistantFeature which might want to control a ship's thrusters in
 *  a bulk manner
 */
private class ThrustControl {
	FlightAssistantRegistry registry;

	public void init(FlightAssistantRegistry registry) {
		this.registry = registry;
	}

	/**
	 *  Updates the ThrustOverridePercentage for all thrusters mapped to the given direction,
	 *  effectively making the ship move in that direction (if the thrusters are sufficiently
	 *  powerful to overcome any gravity that might affect the grid)
	 */
	public void overrideThrustPercentage(Direction direction, float pct) {
		// Make sure the given value does not overflow / underflow
		if (pct > 1) pct = 1;
		if (pct < 0) pct = 0;

		// Apply the given value to all thrusters mapped to the given direction
		registry.thrusters[direction].ForEach(thruster => {
			thruster.ThrustOverridePercentage = pct;
		});
	}

	/**
	 *  Throttles down all the thrusters in the grid (unless the pilot uses them)
	 */
	public void zeroThrust() {
		foreach(List<IMyThrust> direction in registry.thrusters.Values) {
			foreach(IMyThrust thruster in direction) {
				thruster.ThrustOverridePercentage = 0;
			};
		};
	}

	/**
	 *  Throttles down all the thrusters mapped to the given direction
	 *  (unless the pilot uses them)
	 */
	public void zeroThrust(Direction direction) {
		registry.thrusters[direction].ForEach(thruster => {
			thruster.ThrustOverridePercentage = 0;
		});
	}

	/**
	 *  Loops through all blocks of type IMyThrust and puts them in a dictionary that
	 *  groups them by the direction in which they move the ship
	 */
	public static Dictionary<Direction, List<IMyThrust>> identifyThrusters(FlightAssistantRegistry registry) {
		// Initialize the dictionary
		Dictionary<Direction, List<IMyThrust>> groupedThrusters = new Dictionary<Direction, List<IMyThrust>>();
		groupedThrusters.Add(Direction.FORWARD , new List<IMyThrust>());
		groupedThrusters.Add(Direction.BACKWARD, new List<IMyThrust>());
		groupedThrusters.Add(Direction.LEFT    , new List<IMyThrust>());
		groupedThrusters.Add(Direction.RIGHT   , new List<IMyThrust>());
		groupedThrusters.Add(Direction.UP      , new List<IMyThrust>());
		groupedThrusters.Add(Direction.DOWN    , new List<IMyThrust>());
		
		// Get the thrusters from the grid
		List<IMyThrust> thrusters = new List<IMyThrust>();
		registry.gridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);

		// Group the thrusters into the dictionary categories
		// NOTE: the direction in which the thruster moves the ship is opposite from
		//       the direction in which the thruster fires in order to move the ship
		thrusters.ForEach(thruster => {
			foreach (Direction direction in groupedThrusters.Keys) {
				matchThrusterToDirectionGroup(thruster,direction, groupedThrusters);
			}
		});

		// Finally, return the populated dictionary
		return groupedThrusters;
	}

	/**
	 * Helper for identifyThrusters() - see above
	 */
	private static void matchThrusterToDirectionGroup(
		IMyThrust thruster,
		Direction direction,
		Dictionary<Direction, List<IMyThrust>> groupedThrusters
	) {
		if (areVectorsOpposite(thruster.GridThrustDirection, directionToVector3D(direction))) {
			groupedThrusters[direction].Add(thruster);
		}
	}
}

/**
 *  
 */
private class GyroControl {
	private FlightAssistantRegistry registry;

	public void init(FlightAssistantRegistry registry) {
		this.registry = registry;
	}

	/**
	 *  Overrides the yaw, pitch and roll velocities of every gyro,
	 *  accounting for the orientation of each gyro in reference to
	 *  the control seat
	 */
	public void startRotation(Vector3D yawPitchRoll) {
		registry.gyros.ForEach(gyro => {
			// Get the rotation matrix of the gyro
			// Transform the vector
			Vector3D yawPitchRollInGyroCoordinates
				= Vector3D.Rotate(yawPitchRoll,gyro.orientationMatrix);

			// Override the gyro's rotation values
			gyro.gyro.GyroOverride = true;
			gyro.gyro.Yaw   = (float) yawPitchRollInGyroCoordinates.X;
			gyro.gyro.Pitch = (float) yawPitchRollInGyroCoordinates.Y;
			gyro.gyro.Roll  = (float) yawPitchRollInGyroCoordinates.Z;
		});
	}

	/**
	 *  Calls the startRotation(Vector3D) method for a vector created
	 * from the given parmeters
	 */
	public void startRotation(double yaw, double pitch, double roll) {
		startRotation(new Vector3D(yaw, pitch, roll));
	}

	/**
	 *  Calls the startRotation(double, double, double) method providing
	 *  only the yaw parameter
	 */
	public void startYaw(double yaw) {
		startRotation(yaw, 0, 0);
	}

	/**
	 *  Calls the startRotation(double, double, double) method providing
	 *  only the pitch parameter
	 */
	public void startPitch(double pitch) {
		startRotation(0, pitch, 0);
	}

	/**
	 *  Calls the startRotation(double, double, double) method providing
	 *  only the roll parameter
	 */
	public void startRoll(double roll) {
		startRotation(0, 0, roll);
	}

	/**
	 *  Gives control back to the user - this will make the gyros
	 *  stop rotating the ship and begin receiving user input
	 */
	public void stopRotation() {
		registry.gyros.ForEach(gyro => {
			gyro.gyro.GyroOverride = false;
		});
	}

	/**
	 *  Creates a list of references to ever gyro in the grid while also computing
	 *  a rotation matrix that can be applied to a yaw/pitch/roll vector to rotate
	 *  it from the control seat's coordinate system into the coordinate system of
	 *  every individual gyro (cause the API doesn't do that for some reason)
	 */
	public static List<MyGyroWrapper> identifyGyros(FlightAssistantRegistry registry) {
		// Get the controller's orientation matrix
		Matrix controllerOrientationMatrix;
		registry.controller.Orientation.GetMatrix(out controllerOrientationMatrix);

		// Compute the inverse of the controller's orientation matrix
		Matrix controllerInverseOrientationMatrix
			= Matrix.Invert(controllerOrientationMatrix);
		
		// List the gyros from the grid
		List<IMyGyro> gyros = new List<IMyGyro>();
		registry.gridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros);

		// Initialize the return list
		List<MyGyroWrapper> ret = new List<MyGyroWrapper>();

		// For each gyro in the grid
		gyros.ForEach(gyro => {
			// Get the orientation matrix of the gyro
			Matrix mtrx;
			gyro.Orientation.GetMatrix(out mtrx);

			// Multiply the rotation matrix of the gyro by the inverse orientation matrix
			// of the controller. The resulting matrix is applied to yaw/pitch/roll vectors
			// to rotate the gyro in reference to the controller rather than the grid
			mtrx = Matrix.Multiply(mtrx, controllerInverseOrientationMatrix);

			// Add the new GyroWrapper to the return list
			ret.Add(new MyGyroWrapper(gyro, mtrx));
		});

		// Return the list
		return ret;
	}
}

/**
 *  Just a place to group the various controls which might be used by features
 */
private class FlightAssistantControls {
	/**
	 *  The ThrustControl is the functional item having to do with handling thrusters.
	 *  This functionality makes the code in the flight assistant features a little
	 *  cleaner and more readable.
	 */
	public ThrustControl thrustControl = new ThrustControl();

	/**
	 *  The GyroControl handles ship rotation by overriding the properties of the
	 *  gyros in the grid, accounting for their orientation in relation to the ship
	 *  controller
	 */
	public GyroControl gyroControl = new GyroControl();

	public void init(FlightAssistantRegistry registry) {
		thrustControl.init(registry);
		gyroControl.init(registry);
	}
}

/**
 *  The sensors provide information such as speed, rotation, elevation (when on planets), etc,
 *  collected from various members of the API
 */
private class Sensors {
	private FlightAssistantRegistry registry;

	public void init(FlightAssistantRegistry registry) {
		this.registry = registry;
	}

	/**
	 *  Get the natural gravity power and direction from the ship's controller
	 */
	public Vector3D getNaturalGravity() {
		return registry.controller.GetNaturalGravity();
	}

	/**
	 *  Get the ship's position as registered by the ship's controller
	 */
	public Vector3D getPosition() {
		return registry.controller.GetPosition();
	}

	/**
	 *  Get the elevation registered by the ship's controller if the ship is
	 *  within a planet's gravity well or return [MaxValue,MaxValue,MaxValue]
	 *  if the ship is not in any planet's gravity well
	 */
	public Vector3D getElevation() {
		// Vector in which to store the elevation
		Vector3D elevation = new Vector3D();

		// Attempt to get the elevation (if within a natural gravity well)
		// If not, then the vector will be set to [0,0,0]
		bool isSuccessful = registry.controller.TryGetPlanetPosition(out elevation);

		// Return the collected elevation or MaxValue if not within a natural gravity well
		return isSuccessful ? elevation : Vector3D.MaxValue;
	}

	/**
	 *  Get the current speed of the ship as registered by the ship's controller
	 */
	public double getShipSpeed() {
		return registry.controller.GetShipSpeed();
	}

	/**
	 *  Get the orientation of the grid in world coordinates, transformed in such a
	 *  way that the UP and FORWARD directions are those of the selected controller
	 */
	public WorldOrientation getControllerWorldOrientationVectors() {
		// Prepare the return type
		WorldOrientation orientation = new WorldOrientation();

		// Get the position of the grid origin in world coordinates
		Vector3D gridOrigin = registry.controller.CubeGrid.GridIntegerToWorld(Vector3I.Zero);

		// Get the directional vectors of the controller in world coordinates
		orientation.Forward = registry.controller.CubeGrid.GridIntegerToWorld(directionToVector3I(registry.controller.Orientation.Forward));
		orientation.Up      = registry.controller.CubeGrid.GridIntegerToWorld(directionToVector3I(registry.controller.Orientation.Up));

		// Translate the directional vectors of the controller to the grid's origin
		orientation.Forward = Vector3D.Subtract(orientation.Forward, gridOrigin);
		orientation.Up      = Vector3D.Subtract(orientation.Up     , gridOrigin);

		// Normalize
		orientation.Forward = Vector3D.Normalize(orientation.Forward);
		orientation.Up      = Vector3D.Normalize(orientation.Up     );

		// return the normalized value, so it can be used in conjunction
		// with other normalized values such as the gravity up vector
		return orientation;
	}

	/**
	 *  If the ship is within a natural gravity well, get the vector pointing from
	 *  the center of the gravity well towards the ship. If the ship is not within
	 *  a natural gravity well, the method returns [0d, 0d, 0d].
	 */
	public Vector3D getGravityUpVector() {
		// Vector for the planet position
		Vector3D planetPosition;

		// Get the planet position in world coordinates (if in a natural gravity well)
		// or return the zero vector (if not in a natural gravity well)
		if (!registry.controller.TryGetPlanetPosition(out planetPosition)) {
			return Vector3D.Zero;
		}

		// Get the ship's position
		Vector3D shipPosition = getPosition();

		// Compute the gravity UP direction (the ship is always above the planet, hopefully,
		// so the gravity up vector should always be pointing towards the ship)
		Vector3D gravityUpVector = Vector3D.Subtract(shipPosition, planetPosition);

		// Return the normalized value of the gravity up vector, so it can be used in
		// conjunction with other values, such as the ship's orientation
		return Vector3D.Normalize(gravityUpVector);
	}
}

/**
 *  Root node of the model used by the FlightAssistant
 *  Holds entities which are commonly used by more than one FlightAssistantFeature
 *  This way, initialization happens only once for all features
 */
private class FlightAssistantRegistry {
	/**
	 *  Keep a reference to the GridTerminalSystem so the features won't have to take it
	 *  from the main script class
	 */
	public IMyGridTerminalSystem gridTerminalSystem;

    /**
	 *  Once the controller is identified, the FlightAssistant must keep a refernece to
	 *  it, so it doesn't have to find it all over again during each tick.
	 */
	public IMyShipController controller;

	/**
	 *  List of the ship's thrusters, grouped by the direction in which they move the ship
	 *  Makes it easier for the features to operate (abstraction of the vector math)
	 */
	public Dictionary<Direction,List<IMyThrust>> thrusters;

    /**
	 *  List of the ship's gyroscopes
	 */
	public List<MyGyroWrapper> gyros;
}

/**
 *  Helper object for the FlightAssistant class - delegate for the logging capability
 *  Just keeping things organized, in case the script grows in size
 */
private class Log {
	/**
	 *  The Log sends messages to each of these loggers (implementations to be injected at construction time)
	 */
	List<IFlightAssistantLogger> loggers = new List<IFlightAssistantLogger>();

	/**
	 *  Some loggers might require the GridTerminalSystem to get started
	 */
	private IMyGridTerminalSystem gridTerminalSystem;

	public List<IFlightAssistantLogger> getLoggers() {
		return loggers;
	}

	public void init(IMyGridTerminalSystem gridTerminalSystem) {
		this.gridTerminalSystem = gridTerminalSystem;

		loggers.ForEach(logger => {
			string loggerName = logger.getName();
			if (loggerName != null) {
				logger.init(gridTerminalSystem);
				logger.info("Logging to [" + loggerName + "]");
			}
		});
	}

	/**
	 * Send the INFO message to each of the applied loggers
	 */
	public void info(string info) {
		loggers.ForEach(logger => {
			if (logger.getName() != null) {
				logger.info(info);
			}
		});
	}

	/**
	 * Send the WARNING message to each of the applied loggers
	 */
	public void warning(string warning) {
		loggers.ForEach(logger => {
			logger.warning(warning);
		});
	}

	/**
	 * Send the ERROR message to each of the applied loggers
	 */
	public void error(string error) {
		loggers.ForEach(logger => {
			logger.error(error);
		});
	}
}

/**
 *  Holds a reference to a flight assistant feature and its runtime state (i.e. enabled / disabled)
 */
private class FlightAssistantFeatureWrapper {
	public IFlightAssistantFeature feature;
	public bool isEnabled = true;
	public string nameUCase = "";

	public FlightAssistantFeatureWrapper(IFlightAssistantFeature feature) {
		this.feature = feature;
	}
}

/**
 *  Holds configurable FlightAssistant properties, such as the name of the preferred controller
 */
private class FlightAssistantConfig {
	public string preferredControllerName = null;

	/**
	 *  The initial target cruising speed in meters per second
	 *  >>> can be modified by running the script with the following parameters:
	 *           - Cruise Control : INCREASE SPEED
	 *           - Cruise Control : DECREASE SPEED
	 */
	public double cruiseControlDefaultCruisingSpeed = 50;

	/**
	 *  Sets the max speed in meters per second for the cruise control
	 *  Useful with mods that remove the speed limit of 100mps
	 */
	public double cruiseControlMaxSpeed = 100;

	/**
	 *  Sets the increment, in meters per second, for the increase speed
	 *  and decrease speed operations of the cruise control
	 */
	public double cruiseControlSpeedIncrement = 10;

	/**
	 *  Maximum thrust that the cruise control will attempt to apply in
	 *  a single burst. During each iteration, the cruise control compares
	 *  the current speed against the target cruising speed. The bigger
	 *  the difference, the higher the thrust applied, so that the cruising
	 *  speed can be reached in a minimum number of iterations. However,
	 *  higher thrust means the cruise control can overshoot the target
	 *  cruising speed, which would result in more frequent corrections,
	 *  effectively making the ship go either faster or slower than the
	 *  actual cruising speed. This behavior is most noticeable in space
	 *  when the inertial dampeners are turned off.
	 */
	public float cruiseControlMaxThrust = 0.5f;
}

private class MyGyroWrapper {
	/**
	 *  Reference to the gyro
	 */
	public IMyGyro gyro;

	/**
	 *  The gyro's orientation matrix is stored here to avoid recomputing
	 *  it during each iteration / tick
	 */
	public Matrix orientationMatrix;

	public MyGyroWrapper(IMyGyro gyro, Matrix orientationMatrix) {
		this.gyro = gyro;
		this.orientationMatrix = orientationMatrix;
	}
}

/**
 *  The FlightAssistantBuilder helps create a FlightAssistant with the given features and
 *  configuration. More complex functionality might be required in the future.
 */
private class FlightAssistantBuilder {
	private FlightAssistant flightAssistant = new FlightAssistant();

	public FlightAssistantBuilder withFeature(IFlightAssistantFeature feature) {
		if (feature != null) {
			flightAssistant.getFeatures().Add(new FlightAssistantFeatureWrapper(feature));
		}
		return this;
	}

	public FlightAssistantBuilder withConfig(FlightAssistantConfig config) {
		if (config != null) {
			FlightAssistantConfig cnf = flightAssistant.getConfig();
			cnf.preferredControllerName = nvl(config.preferredControllerName, cnf.preferredControllerName);
		}
		return this;
	}

	public FlightAssistantBuilder withLogger(IFlightAssistantLogger logger) {
		if (logger != null) {
			flightAssistant.getLog().getLoggers().Add(logger);
		}
		return this;
	}

	public FlightAssistant build() {
		return flightAssistant;
	}
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// SCRIPT OBJECTS - FLIGHT ASSISTANT FEATURES /////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  The cruise control keeps the grid traveling forward at a given velocity.
 *  It takes the following commands:
 *         GO             : start cruising
 *         STOP           : stop cruising
 *         INCREASE SPEED : increase cruising speed
 *         DECREASE SPEED : decrease cruising speed
 */
private class CruiseControl : IFlightAssistantFeature {
	// References to other members of the framework
	private FlightAssistantConfig config;
	private Sensors sensors;
	private ThrustControl thrustControl;
	private FlightAssistantRegistry registry;
	private Log log;

	/**
	 *  The cruise control attempts to keep the ship going at this speed
	 *  by firing thrusters to go forward when the current speed is below
	 *  this number and by firing thrusters to go backwards when the
	 *  current speed is above this number. This number can be altered
	 *  using the INCREASE SPEED and DECREASE SPEED commands
	 */
	private double cruisingSpeed = 0;

	/**
	 *  Set to TRUE by the GO command. Set to FALSE by the STOP command.
	 *  When set to TRUE, the CruiseControl attempts to control the ship's
	 *  speed by firing thrusters to go forward and backwards.
	 */
	private bool isCruising = false;

	public string getName() {
		return "Cruise Control";
	}

	public void init(FlightAssistantRegistry registry, Log log, FlightAssistantControls controls, Sensors sensors, FlightAssistantConfig config) {
		this.config = config;
		this.sensors = sensors;
		this.thrustControl = controls.thrustControl;
		this.registry = registry;
		this.log = log;

		this.cruisingSpeed = config.cruiseControlDefaultCruisingSpeed;
	}

	public void tick(string argument) {
		switch (argument) {
			case "GO":
				isCruising = true;
				break;
			case "STOP":
				isCruising = false;
				idleThrusters();
				break;
			case "INCREASE SPEED":
				cruisingSpeed = addNumbersWithThreshold(cruisingSpeed, config.cruiseControlSpeedIncrement, config.cruiseControlMaxSpeed);
			//	log.info("Cruising speed: " + cruisingSpeed.ToString());
				break;
			case "DECREASE SPEED":
				cruisingSpeed = subtractNumbersWithThreshold(cruisingSpeed, config.cruiseControlSpeedIncrement, 0);
			//	log.info("Cruising speed: " + cruisingSpeed.ToString());
				break;
			default:
				if (isCruising) {
					cruise();
				}
				break;
		}
	}

	private void cruise() {
		// Get the ship's current speed
		double currentSpeed = sensors.getShipSpeed();

		// Compute the amount of thrust to apply for the current burst
		// The further the current speed from the cruising speed, the
		// higher the amount of trust to be applied
		double speedDifference = Math.Abs(cruisingSpeed - currentSpeed);
		float thrust = config.cruiseControlMaxThrust * (float)speedDifference / (float)cruisingSpeed;

		// If the ship is going slower than the cruising speed, speed up
		if (currentSpeed < cruisingSpeed) {
			thrustControl.overrideThrustPercentage(Direction.FORWARD, thrust);
			thrustControl.zeroThrust(Direction.BACKWARD);
			return;
		};

		// If the ship is going faster than the cruising speed, slow down
		if (currentSpeed > cruisingSpeed) {
			thrustControl.overrideThrustPercentage(Direction.BACKWARD, thrust);
			thrustControl.zeroThrust(Direction.FORWARD);
			return;
		}

		// If, by any chance, the current speed is exactly the cruising speed,
		// put the thrusters on idle
		idleThrusters();
	}

	private void idleThrusters() {
		thrustControl.zeroThrust(Direction.FORWARD);
		thrustControl.zeroThrust(Direction.BACKWARD);
	}
}

/**
 *  The auto leveler sets the pitch, roll and yaw of the ship
 *  in such a way as it is aligned with the ground
 *  It takes the following commands:
 *         GO             : start keeping the ship leveled
 *         STOP           : stop keeping the ship leveled
 *         STABILIZE      : bring the ship up to level just once
 */
private class AutoLevel : IFlightAssistantFeature {
	// References to other members of the framework
	private FlightAssistantConfig config;
	private Sensors sensors;
	private ThrustControl thrustControl;
	private GyroControl gyroControl;
	private FlightAssistantRegistry registry;
	private Log log;

	private bool isStabilizing = false;

	private bool isOneShot = true;

	public string getName() {
		return "Auto Level";
	}

	public void init(FlightAssistantRegistry registry, Log log, FlightAssistantControls controls, Sensors sensors, FlightAssistantConfig config) {
		this.config = config;
		this.sensors = sensors;
		this.thrustControl = controls.thrustControl;
		this.gyroControl = controls.gyroControl;
		this.registry = registry;
		this.log = log;
	}

	public void tick(string argument) {
		switch (argument) {
			case "GO":
				isStabilizing = true;
				isOneShot = false;
				break;
			case "STOP":
				isStabilizing = false;
				isOneShot = false;
				stop();
				break;
			case "STABILIZE":
				isStabilizing = true;
				isOneShot = true;
				break;
			default:
				Vector3D normalizedShipAxisVector = sensors.getControllerWorldOrientationVectors().Up;
				Vector3D normalizedGravityVector  = sensors.getGravityUpVector();
				
				log.info("Normalized Forward = " + normalizedShipAxisVector.ToString());
				log.info("==============");
				log.info("Normalized Gravity = " + normalizedGravityVector.ToString());
				log.info("==============");
				log.info("ANGLE = " + getAngleBetweenVectors(normalizedShipAxisVector, normalizedGravityVector));

				if (isStabilizing) {
					if (isStable(normalizedShipAxisVector, normalizedGravityVector)) {
						if (isOneShot) {
							isStabilizing = false;
							stop();
						}
					} else {
						stabilize(normalizedShipAxisVector, normalizedGravityVector);
					}
				}
				break;
		}
	}

	private void stabilize(Vector3D normalizedShipAxisVector, Vector3D normalizedGravityVector) {
		stabilizeRoll(normalizedShipAxisVector, normalizedGravityVector);
	}

	private void stabilizeRoll(Vector3D normalizedShipAxisVector, Vector3D normalizedGravityVector) {
		Vector3D stabilizationVector = new Vector3D();

		if (normalizedShipAxisVector.X < normalizedGravityVector.X) {
			stabilizationVector.Z = (normalizedGravityVector.Z - normalizedShipAxisVector.Z) * 1;
		} else {
			stabilizationVector.Z = (normalizedShipAxisVector.Z + normalizedGravityVector.Z) * 1;
		}

		gyroControl.startRotation(stabilizationVector);
	}

	private bool isStable(Vector3D normalizedShipAxisVector, Vector3D normalizedGravityVector) {
		return isStableOnAxis(normalizedShipAxisVector.X, normalizedGravityVector.X);
//			&& isStableOnAxis(normalizedShipAxisVector.Y, normalizedGravityVector.Y);
//			&& isStableOnAxis(normalizedShipAxisVector.Z, normalizedGravityVector.Z);
	}

	private bool isStableOnAxis(double shipAxisCoord, double gravityVectorCoord) {
		return areValuesCloseEnough(shipAxisCoord, gravityVectorCoord, 0.1);
	}

	private void stop() {
		gyroControl.stopRotation();
	}
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// SCRIPT OBJECTS - LOGGERS ///////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  Logs the stuff to the script output
 */
private class ScriptOutputLogger : IFlightAssistantLogger {
	public string getName() {
		return "ScriptOutputLogger";
	}

	public void init(IMyGridTerminalSystem gridTerminalSystem) {
		// No use for the GridTerminalSystem when writing to the script output
	}

	public void info(string info) {
		echo("INFO: " + info);
	}

	public void warning(string warning) {
		echo("INFO: " + warning);
	}

	public void error(string error) {
		echo("INFO: " + error);
	}
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// COMMONS ////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



private static T nvl<T>(T possibleNull, T replaceNullWith) {
	return possibleNull == null ? replaceNullWith : possibleNull;
}

private static bool areVectorsOpposite(Vector3D v1, Vector3D v2) {
	if (v1 == null || v2 == null) {
		return false;
	}

	if (
		   areNumbersOpposite(v1.X, v2.X)
		&& areNumbersOpposite(v1.Y, v2.Y)
		&& areNumbersOpposite(v1.Z, v2.Z)
	) {
		return true;
	}

	return false;
}

private static bool areNumbersOpposite(double n1, double n2) {
	// NULL resolves to 0 (or at least it did in C++)
	double nvlN1 = nvl(n1, 0d);
	double nvlN2 = nvl(n2, 0d);

	// If both values are 0, then they are both equal and infinitely opposite
	if (nvlN1 == 0d && nvlN2 == 0d) {
		return true;
	}

	// If the values are opposite, well, then they're opposite
	if (nvlN1 == -nvlN2) {
		return true;
	}

	// If they're not, then they're not
	return false;
}

private static double addNumbersWithThreshold(double n1, double n2, double threshold) {
	double ret = n1 + n2;

	if (ret > threshold) {
		ret = threshold;
	}

	return ret;
}

private static double subtractNumbersWithThreshold(double n1, double n2, double threshold) {
	double ret = n1 - n2;

	if (ret < threshold) {
		ret = threshold;
	}

	return ret;
}

private static bool areValuesCloseEnough(double value1, double value2, double errorAmount) {
	return Math.Abs(value1 - value2) < errorAmount;
}

private static double getAngleBetweenVectors(Vector3D v1, Vector3D v2)
{
    if (v1.LengthSquared() == 0 || v2.LengthSquared() == 0)
        return 0;
    else
        return Math.Acos(MathHelper.Clamp(v1.Dot(v2) / v1.Length() / v2.Length(), -1, 1));
}


///////////////////////////////////////////////////////////////////////////////////////////////////
// WORKAROUNDS ////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////


private static MyGridProgram prog;
private static void echo(string str) {
	prog.Echo(str);
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// SCRIPT GLOBALS /////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  The FlightAssistant - with any luck, the only object requiring declaration her
 */
private FlightAssistant flightAssistant;



///////////////////////////////////////////////////////////////////////////////////////////////////
// CUSTOM METHODS ADDED TO THE SCRIPT CLASS ///////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  Custom initialization method - runs during the first tick to allow the components to initialize
 *  while having access to the already built model (i.e. the GridTerminalSystem)
 */
private void initialize(IMyGridTerminalSystem gridTerminalSystem) {
	prog = this;
	flightAssistant.init(gridTerminalSystem);
}

/**
 *  Custom tick method - runs during each tick (each call of Main)
 */
private void tick(string argument, UpdateType updateSource) {
	flightAssistant.tick(argument);
}



///////////////////////////////////////////////////////////////////////////////////////////////////
// CLASS METHODS - INHERITED FROM THE MODAPI //////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////



/**
 *  Used in Main
 */
private bool initialized = false;

/**
 *  Method Main - as defined by the API
 */
public void Main(string argument, UpdateType updateSource) {
	// Initialize during the first tick
	if (!initialized) { initialize(GridTerminalSystem); initialized = true; }

	// In any case, perform the tick
	tick(argument, updateSource);
}

/**
 *  Method Save - as defined by the API
 */
public void Save() {
	// Not sure there's any state to save, but let's keep this here, just in case
}


