using System;
using UnityEngine;

/// <summary>
/// The mprimary flight controls of a fixed wing aircraft which influence the forward flight Angle and rotation of an aircraft.
/// </summary>
public class PrimaryFlightControls
{
    /// <summary>
    /// The Angle at which the elevator is set to.
    /// </summary>
    public float ElevatorDegree
    { get; set; } = 0;

    /// <summary>
    /// The Angle at which the elevator is set to.
    /// </summary>
    public float AileronDegree_Left
    { get; set; } = 0;

    /// <summary>
    /// The Angle at which the elevator is set to.
    /// </summary>
    public float AileronDegree_Right
    { get; set; } = 0;

    /// <summary>
    /// The Angle at which the elevator is set to.
    /// </summary>
    public float RudderDegree
    { get; set; } = 0;

    /// <summary>
    /// The value of the throttle which directs the power sent to the engine.
    /// </summary>
    public float ThrottleValue
    { get; set; } = 0;

    public event Action<float> OnThrottleValue;

    /// <summary>
    /// The rate of which the propeller should rotate per second.
    /// </summary>
    public float PropellerRotationsPerSecond
    { get; set; } = 0;

    /// <summary>
    /// Calculates the rotation a control surface should be at when provied with a normalised value representing how much the rotation should be towards maximum rotation value.
    /// </summary>
    /// <param name="normalisedInput"></param>
    /// <param name="currentElevatorDegrees"></param>
    /// <param name="degreeLimitOfRotation"></param>
    /// <param name="speedOfRotation"></param>
    /// <returns></returns>
    public static float CalculateCurrentRotation(float normalisedInput, float currentElevatorDegrees, float degreeLimitOfRotation, float speedOfRotation)
    {
        // Calculating the elevator normalised angles of degrees.
        float newDegreeTarget = normalisedInput * degreeLimitOfRotation;

        float newCurrentDegree = Mathf.MoveTowardsAngle(currentElevatorDegrees, newDegreeTarget, speedOfRotation * Time.fixedDeltaTime);

        return newCurrentDegree;
    }

    public void AdjustThrottleValue(float newValue)
    {
        ThrottleValue = newValue;
        OnThrottleValue?.Invoke(newValue);
    }
}