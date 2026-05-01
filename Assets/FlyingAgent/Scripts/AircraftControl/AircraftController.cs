using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(AircraftCurrentValues)), RequireComponent(typeof(AircraftInput))]
public class AircraftController : MonoBehaviour
{
    /// <summary>
    /// Reference to the component where the player input is received and processed by the New Input System into normalised values for aircraft control.
    /// </summary>
    [field: SerializeField]
    public AircraftInput InputControls
    { get; private set; }

    /// <summary>
    /// The values of the aircraft, from it's Flight Controls to the acting forces on the aircraft.
    /// </summary>
    [field: SerializeField]
    public AircraftCurrentValues CurrentValues
    { get; private set; }

    /// <summary>
    /// The rigidbody to be manipulated by the calculated forces acting on the aircraft.
    /// </summary>
    [field: SerializeField]
    public Rigidbody PlaneRigidBody
    { get; private set; }

    /// <summary>
    /// For the sake of controlling the centre of mass of the aircraft.
    /// </summary>
    [field: SerializeField]
    public Transform CentreOfMassTransform
    { get; private set; }

    private void Awake()
    {
        // Rigidbody null checking.
        if (PlaneRigidBody == null)
        {
            PlaneRigidBody = GetComponent<Rigidbody>();

            if (PlaneRigidBody == null)
            {
                Debug.LogError("Unable to locate a rigidbody on the aircraft for physics control.");
                return;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlaneRigidBody.mass = CurrentValues.FlightForces.Weight;

        if (CentreOfMassTransform != null)
        {
            PlaneRigidBody.centerOfMass = CentreOfMassTransform.transform.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FixedUpdate()
    {
        UpdatePlaneState(CurrentValues.ValuesHolder);
        InputToFlightControls(InputControls, CurrentValues.FlightControls);
        ForcesAndFlightControlsToForces(CurrentValues.FlightControls, CurrentValues.FlightForces);
        ForcesToRigidBody(CurrentValues.FlightForces, PlaneRigidBody);
        UpdateDisplayInformation(CurrentValues.ValuesHolder);

        Debug.DrawLine(transform.position, transform.position + PlaneRigidBody.linearVelocity.normalized * 10f, Color.blue);
    }

    /// <summary>
    /// Called every frame, it updates the class dedicated to holding information used to calculate information used accross multiple methods.
    /// </summary>
    /// <param name="valuesHolder"></param>
    public void UpdatePlaneState(AircraftValuesHolder valuesHolder)
    {
        // Local velocity and angular velocity values stored in value holder.
        Quaternion invRotation = Quaternion.Inverse(PlaneRigidBody.rotation);

        CurrentValues.ValuesHolder.CurrentVelocityLocal = invRotation * PlaneRigidBody.linearVelocity;
        CurrentValues.ValuesHolder.CurrentAngularVelocityLocal = invRotation * PlaneRigidBody.angularVelocity;

        // Angle of attack calculation.
        if (valuesHolder.CurrentVelocityLocal.sqrMagnitude < 0.1f)
        {
            valuesHolder.AngleOfAttack = 0;
            valuesHolder.AngleOfAttackYaw = 0;
        }
        else
        {
            valuesHolder.AngleOfAttack = Mathf.Rad2Deg * Mathf.Atan2(-valuesHolder.CurrentVelocityLocal.y, valuesHolder.CurrentVelocityLocal.z);
            valuesHolder.AngleOfAttackYaw = Mathf.Rad2Deg * Mathf.Atan2(valuesHolder.CurrentVelocityLocal.x, valuesHolder.CurrentVelocityLocal.z);
        }

        // Update the manually calculated centre of mass if required.
        if (PlaneRigidBody.centerOfMass != CentreOfMassTransform.transform.localPosition)
        {
            PlaneRigidBody.centerOfMass = CentreOfMassTransform.transform.localPosition;
        }

        valuesHolder.LevelOfFlight = CurrentValues.ValuesHolder.CalculateLevelOfFlightDotProduct(PlaneRigidBody.gameObject);
    }

    /// <summary>
    /// Transfers the players normalised input values to the elements of the flight control systems, such as throttle and control surfaces.
    /// </summary>
    /// <param name="inputControls"></param>
    /// <param name="flightControls"></param>
    public void InputToFlightControls(AircraftInput inputControls, PrimaryFlightControls flightControls)
    {
        // Check for the current throttle usage from either joystick or not due to different processing of value.
        if (InputControls.IsJoytickControl == true)
        {
            // Normalise the Joystick input and then set the throttle value to move towards the normalised input value directly.
            float normalisedJoystickInput = (inputControls.ThrottleInput + 1f) / 2f;

            float newThrottleValue = Mathf.MoveTowards(flightControls.ThrottleValue, normalisedJoystickInput, Time.fixedDeltaTime * CurrentValues.BaseValues.ThrottleSpeedOfChange);

            flightControls.AdjustThrottleValue(newThrottleValue);
        }
        else
        {
            // Calculating the throttles values after any changes in input by the player.
            float newThrottleValue = flightControls.ThrottleValue + (inputControls.ThrottleInput * Time.fixedDeltaTime * CurrentValues.BaseValues.ThrottleSpeedOfChange);

            flightControls.AdjustThrottleValue(Mathf.Clamp01(newThrottleValue));
        }

        // Calculating the elevator normalised angles of degrees.
        flightControls.ElevatorDegree = PrimaryFlightControls.CalculateCurrentRotation(inputControls.ElevatorInput, CurrentValues.FlightControls.ElevatorDegree, CurrentValues.BaseValues.ElevatorDegreeLimit, CurrentValues.BaseValues.ElevatorSpeedOfRotation);

        // Calculating the ailerons normalised angles of degrees.
        flightControls.AileronDegree_Left = PrimaryFlightControls.CalculateCurrentRotation(inputControls.AileronInput, CurrentValues.FlightControls.AileronDegree_Left, CurrentValues.BaseValues.AileronDegreeLimit, CurrentValues.BaseValues.AileronSpeedOfRotation);
        flightControls.AileronDegree_Right = -flightControls.AileronDegree_Left;

        // Calculating the rudder normalised angles of degrees.
        flightControls.RudderDegree = PrimaryFlightControls.CalculateCurrentRotation(inputControls.RudderInput, CurrentValues.FlightControls.RudderDegree, CurrentValues.BaseValues.RudderDegreeLimit, CurrentValues.BaseValues.RudderSpeedOfRotation);




        // Properller spin value is based on the current throttle level (a normalised value) multiplied by the max rotations per second allowed.
        flightControls.PropellerRotationsPerSecond = flightControls.ThrottleValue * CurrentValues.BaseValues.PropellerRotationPerSecondAtMaxThrottle;
    }

    /// <summary>
    /// Transfers the flight controls systems values to the forces being applied to the aircraft.
    /// </summary>
    /// <param name="flightControls"></param>
    /// <param name="flightForces"></param>
    public void ForcesAndFlightControlsToForces(PrimaryFlightControls flightControls, ForcesOnFlight flightForces)
    {
        // Calculate the rotation of the aircraft based upon the control surfaces simulation.
        flightForces.AngularRotationForce = ForcesOnFlight.CalculateAngularRotationForce(CurrentValues.ValuesHolder, InputControls, CurrentValues);

        // Converting the throttle value to the thrust output.
        flightForces.Thrust = ForcesOnFlight.CalculateThrustForce(flightControls.ThrottleValue, CurrentValues.BaseValues.ThrustMax);

        // Converting the current.0 plane velocity based on the thrust through a to a drag equation
        flightForces.Drag = ForcesOnFlight.CalculateDragVelocity(PlaneRigidBody.transform, PlaneRigidBody.linearVelocity, CurrentValues.BaseValues.DragCoefficientValues, CurrentValues.ValuesHolder);

        // Converting the planes current velocity and Angle of attack to the lift being generated.
        if (CurrentValues.ValuesHolder.CurrentVelocityLocal.sqrMagnitude >= 1f)
        {
            flightForces.Lift = ForcesOnFlight.CalculateLiftVector(CurrentValues.ValuesHolder.AngleOfAttack, Vector3.right, CurrentValues.BaseValues.LiftPower, CurrentValues.BaseValues.LiftCurve, CurrentValues.ValuesHolder, out float lifeCoefficent, out Vector3 liftVelocity);
            flightForces.Lift += ForcesOnFlight.CalculateLiftVector(CurrentValues.ValuesHolder.AngleOfAttackYaw, Vector3.up, CurrentValues.BaseValues.LiftPowerVertical, CurrentValues.BaseValues.LiftCurveVertical, CurrentValues.ValuesHolder, out float lifeCoefficentYaw, out Vector3 liftVelocityYaw);

            flightForces.Lift += ForcesOnFlight.CalculateInducedDrag(lifeCoefficent, liftVelocity, CurrentValues.BaseValues.InducedDrag);
            flightForces.Lift += ForcesOnFlight.CalculateInducedDrag(lifeCoefficentYaw, liftVelocityYaw, CurrentValues.BaseValues.InducedDrag);
        }
    }

    /// <summary>
    /// Applies the calculated forces to the aircrafts rigidbody.
    /// </summary>
    /// <param name="flightForces"></param>
    /// <param name="rigidBody"></param>
    public void ForcesToRigidBody(ForcesOnFlight flightForces, Rigidbody rigidBody)
    {
        // Applying the forces value to the aircraft rigidbody.
        rigidBody.AddRelativeTorque(flightForces.AngularRotationForce, ForceMode.VelocityChange);
        rigidBody.AddForce(transform.forward * flightForces.Thrust, ForceMode.Force);
        rigidBody.AddForce(flightForces.Drag, ForceMode.Force);
        rigidBody.AddRelativeForce(flightForces.Lift, ForceMode.Force);

        // Debug.Log($"Thrust: {transform.forward * flightForces.Thrust}\tDrag: {flightForces.Drag}");
        // Debug.Log($"Lift: {flightForces.Lift}\tGravity Newtons: {flightForces.Weight * Physics.gravity}");
    }

    /// <summary>
    /// Updates the variable containing data simply used for display purposes.
    /// </summary>
    /// <param name="valuesHolder"></param>
    public void UpdateDisplayInformation(AircraftValuesHolder valuesHolder)
    {
        // Updating display information.
        valuesHolder.AirSpeed = Mathf.Max(0f, Vector3.Dot(PlaneRigidBody.linearVelocity, transform.forward));

        // Updating display information for player to read.
        valuesHolder.HeightAboveSeaLevel = PlaneRigidBody.transform.position.y;
    }

}
