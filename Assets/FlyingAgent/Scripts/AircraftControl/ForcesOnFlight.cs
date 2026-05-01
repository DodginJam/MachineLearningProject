using UnityEngine;

/// <summary>
/// The representation of the four main forces acting on a fixed wing aircraft.
/// </summary>
public class ForcesOnFlight
{
    /// <summary>
    /// The weight of the aircraft - an unchanging value.
    /// </summary>
    public float Weight
    { get; set; } = 0;

    /// <summary>
    /// The calcualted lift the aircraft experiances as a result of calculated airflow under the wings.
    /// </summary>
    public Vector3 Lift
    { get; set; }

    /// <summary>
    /// The calulated forward force produced by the engine.
    /// </summary>
    public float Thrust
    { get; set; } = 0;

    /// <summary>
    /// The calulated resistance to forward thrust (a backwards acting force) determined by the speed at which the aircraft is moving through the air.
    /// </summary>
    public Vector3 Drag
    { get; set; }

    /// <summary>
    /// The force of the angularRotation applied the aircraft from the control surfaces simulating lift to their respective areas of the aircraft.
    /// </summary>
    public Vector3 AngularRotationForce
    { get; set; }

    /// <summary>
    /// Calculate the thrust force output from the inputted throttle value and the maximum available thrust.
    /// </summary>
    /// <param name="throttleValue"></param>
    /// <param name="maximumThrust"></param>
    /// <returns></returns>
    public static float CalculateThrustForce(float throttleValue, float maximumThrust)
    {
        // Converting the throttle value to the thrust output.
        return throttleValue * maximumThrust;
    }

    /// <summary>
    /// Calculate the drag on the given transform object based on it's current world velocity 
    /// </summary>
    /// <param name="planeTransform"></param>
    /// <param name="currentWorldVelocity"></param>
    /// <returns></returns>
    public static Vector3 CalculateDragVelocity(Transform planeTransform, Vector3 currentWorldVelocity, DragCoefficient dragCoefficient, AircraftValuesHolder valuesHolder)
    {
        Vector3 dragForceLocal = new Vector3(
            valuesHolder.CurrentVelocityLocal.x * ((valuesHolder.CurrentVelocityLocal.x > 0) ? dragCoefficient.AxisX_Pos.Evaluate(valuesHolder.CurrentVelocityLocal.x) : dragCoefficient.AxisX_Neg.Evaluate(valuesHolder.CurrentVelocityLocal.x)),
            valuesHolder.CurrentVelocityLocal.y * ((valuesHolder.CurrentVelocityLocal.y > 0) ? dragCoefficient.AxisY_Pos.Evaluate(valuesHolder.CurrentVelocityLocal.y) : dragCoefficient.AxisY_Neg.Evaluate(valuesHolder.CurrentVelocityLocal.y)),
            valuesHolder.CurrentVelocityLocal.z * ((valuesHolder.CurrentVelocityLocal.z > 0) ? dragCoefficient.AxisZ_Pos.Evaluate(valuesHolder.CurrentVelocityLocal.z) : dragCoefficient.AxisZ_Neg.Evaluate(valuesHolder.CurrentVelocityLocal.z))
        );

        float dragForceLength = dragForceLocal.magnitude;

        if (dragForceLength < 0.0001f)
        {
            return Vector3.zero;
        }

        Vector3 dragDirectionLocal = -dragForceLocal / dragForceLength;
        float dragMagnitude = dragForceLocal.sqrMagnitude; // more stable than v^3

        Vector3 dragDirectionWorld = planeTransform.TransformDirection(dragDirectionLocal);
        Vector3 totalDragForce = dragDirectionWorld * dragMagnitude;

        return totalDragForce;
    }


    /// <summary>
    /// Calculating the lift through taking the calculated AngleOfAttack and sampling it's position on the aircrafts starting values AngleOfAttack to Lift Coefficent Animation Curve.
    /// </summary>
    /// <param name="angleOfAttack"></param>
    /// <param name="rightAxis"></param>
    /// <param name="liftPower"></param>
    /// <param name="angleOfAttackCurve"></param>
    /// <param name="valuesHolder"></param>
    /// <returns></returns>
    public static Vector3 CalculateLiftVector(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve angleOfAttackCurve, AircraftValuesHolder valuesHolder, out float liftCoefficent, out Vector3 liftVelocity)
    {
        // Project velocity onto YZ plane
        liftVelocity = Vector3.ProjectOnPlane(valuesHolder.CurrentVelocityLocal, rightAxis);

        // Square of velocity
        var v2 = liftVelocity.sqrMagnitude;

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        liftCoefficent = angleOfAttackCurve.Evaluate(angleOfAttack);
        var liftForce = v2 * liftCoefficent * liftPower;

        // Lift is perpendicular to velocity
        // Lift direction is up
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);

        var lift = liftDirection * liftForce;

        return lift;
    }

    public static Vector3 CalculateInducedDrag(float liftCoefficent, Vector3 liftVelocity, float inducedDragValue)
    {
        float dragForce = liftCoefficent * liftCoefficent * inducedDragValue;
        Vector3 dragDirection = -liftVelocity.normalized;
        Vector3 inducedDrag = dragDirection * liftVelocity.sqrMagnitude * dragForce;

        return inducedDrag;
    }

    public static Vector3 CalculateAngularRotationForce(AircraftValuesHolder valuesHolder, AircraftInput controlInputs, AircraftCurrentValues currentValues)
    {
        float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
        {
            var error = targetVelocity - angularVelocity;
            var accel = acceleration * dt;
            return Mathf.Clamp(error, -accel, accel);
        }

        var speed = Mathf.Max(0, currentValues.ValuesHolder.CurrentVelocityLocal.z);
        var steeringPower = currentValues.BaseValues.ElevatorTurnSpeedCurve.Evaluate(speed);

        var targetAV = Vector3.Scale(new Vector3(controlInputs.ElevatorInput, controlInputs.RudderInput, controlInputs.AileronInput), new Vector3(currentValues.BaseValues.TurnSpeed, currentValues.BaseValues.TurnSpeed, currentValues.BaseValues.TurnSpeed) * steeringPower);
        var av = currentValues.ValuesHolder.CurrentAngularVelocityLocal * Mathf.Rad2Deg;
        var correction = new Vector3(
            CalculateSteering(Time.deltaTime, av.x, targetAV.x, currentValues.BaseValues.AccelerationOfTurn.x * steeringPower),
            CalculateSteering(Time.deltaTime, av.y, targetAV.y, currentValues.BaseValues.AccelerationOfTurn.y * steeringPower),
            CalculateSteering(Time.deltaTime, av.z, targetAV.z, currentValues.BaseValues.AccelerationOfTurn.z * steeringPower)
        );

        return correction * Mathf.Deg2Rad;    //ignore rigidbody mass
    }
}