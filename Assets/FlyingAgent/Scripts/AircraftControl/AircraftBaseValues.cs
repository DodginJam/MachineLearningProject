using System;
using UnityEngine;

/// <summary>
/// Class representing the intrinsic values of an aircraft that affect its flight peformance.
/// </summary>
[CreateAssetMenu(fileName = "AircraftBaseValues", menuName = "AircraftBaseValues/Aircraft")]
public class AircraftBaseValues : ScriptableObject
{
    /// <summary>
    /// The maximum amount of thrust that the engine is able to produce.
    /// </summary>
    [field: SerializeField, Min(0)]
    public float ThrustMax
    { get; private set; } = 50.0f;

    /// <summary>
    /// The rate at which the the throttle value changes in response to player input.
    /// </summary>
    [field: SerializeField, Range(0.1f, 10.0f), Min(0.1f)]
    public float ThrottleSpeedOfChange
    { get; private set; } = 1.0f;

    /// <summary>
    /// The maximum speed of rotation allowed by the propeller.
    /// </summary>
    [field: SerializeField]
    public float PropellerRotationPerSecondAtMaxThrottle
    { get; private set; }

    /// <summary>
    /// The weight of the plane.
    /// </summary>
    [field: SerializeField, Min(0)]
    public float Weight
    { get; private set; }

    /// <summary>
    /// The life power of the plane.
    /// </summary>
    [field: SerializeField, Min(0.1f)]
    public float LiftPower
    { get; private set; } = 20.0f;

    /// <summary>
    /// The life power of the plane.
    /// </summary>
    [field: SerializeField, Min(0.1f)]
    public float LiftPowerVertical
    { get; private set; } = 20.0f;

    /// <summary>
    /// The max degree of rotation the elevator can be turned (absolute value).
    /// </summary>
    [field: SerializeField]
    public float ElevatorDegreeLimit
    { get; private set; }

    /// <summary>
    /// The rate at which the the Elevator value changes in response to player input.
    /// </summary>
    [field: SerializeField, Range(0.1f, 200.0f), Min(0.1f)]
    public float ElevatorSpeedOfRotation
    { get; private set; } = 60.0f;

    /// <summary>
    /// The max degree of rotation the aileron can be turned (absolute value).
    /// </summary>
    [field: SerializeField]
    public float AileronDegreeLimit
    { get; private set; }

    /// <summary>
    /// The rate at which the the aileron value changes in response to player input.
    /// </summary>
    [field: SerializeField, Range(0.1f, 200.0f), Min(0.1f)]
    public float AileronSpeedOfRotation
    { get; private set; } = 60.0f;

    /// <summary>
    /// The max degree of rotation the rudder can be turned (absolute value).
    /// </summary>
    [field: SerializeField]
    public float RudderDegreeLimit
    { get; private set; }

    /// <summary>
    /// The rate at which the the Rudder value changes in response to player input.
    /// </summary>
    [field: SerializeField, Range(0.1f, 200.0f), Min(0.1f)]
    public float RudderSpeedOfRotation
    { get; private set; } = 60.0f;

    /// <summary>
    /// The drag coefficent values for the aircraft axis - ideally forward (z positive) should provide the least amount of drag, with the other values providing increased drag.
    /// </summary>
    [field: SerializeField]
    public DragCoefficient DragCoefficientValues
    { get; private set; }

    /// <summary>
    /// Animation Curve used to represent the Lift coefficent relationship with the Angle of attack of the plane.
    /// </summary>
    [field: SerializeField]
    public AnimationCurve LiftCurve
    { get; private set; }

    /// <summary>
    /// Animation Curve used to represent the Lift coefficent relationship with the Angle of attack of the plane.
    /// </summary>
    [field: SerializeField]
    public AnimationCurve LiftCurveVertical
    { get; private set; }

    /// <summary>
    /// Animation Curve used to represent the turning force applied on the Elevator axis tied to the current velocity / Angle of attack.
    /// </summary>
    [field: SerializeField]
    public AnimationCurve ElevatorTurnSpeedCurve
    { get; private set; }

    /// <summary>
    /// Animation Curve used to represent the turning force applied on the Aileron axis tied to the current velocity / Angle of attack.
    /// </summary>
    [field: SerializeField]
    public AnimationCurve AileronTurnSpeedCurve
    { get; private set; }

    /// <summary>
    /// Animation Curve used to represent the turning force applied on the Rudder axis tied to the current velocity / Angle of attack.
    /// </summary>
    [field: SerializeField]
    public AnimationCurve RudderTurnSpeedCurve
    { get; private set; }


    [field: SerializeField, Min(0.1f)]
    public float TurnSpeed
    { get; private set; } = 60f;

    [field: SerializeField]
    public Vector3 AccelerationOfTurn
    { get; private set; } = new Vector3(80f, 10f, 80f);

    [field: SerializeField, Min(0.1f)]
    public float SteeringPower
    { get; private set; } = 80.0f;

    /// <summary>
    /// Abstract figure representing the level of induced drag this plane has on a turn.
    /// </summary>
    [field: SerializeField, Min(0.1f)]
    public float InducedDrag
    { get; private set; } = 20.0f;
}

/// <summary>
/// Class containing the coefficent values for the drag of an aircraft based upon the axis the aircraft is facing upon travelling.
/// </summary>
[Serializable]
public class DragCoefficient
{
    [field: SerializeField]
    public AnimationCurve AxisX_Pos
    { get; private set; }

    [field: SerializeField]
    public AnimationCurve AxisX_Neg
    { get; private set; }


    [field: SerializeField]
    public AnimationCurve AxisY_Pos
    { get; private set; }

    [field: SerializeField]
    public AnimationCurve AxisY_Neg
    { get; private set; }


    [field: SerializeField]
    public AnimationCurve AxisZ_Pos
    { get; private set; }

    [field: SerializeField]
    public AnimationCurve AxisZ_Neg
    { get; private set; }
}