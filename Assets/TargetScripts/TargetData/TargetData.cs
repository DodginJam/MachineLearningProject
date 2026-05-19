using System;
using UnityEngine;
using ProjectEnums;

public class TargetData
{
    public TargetData(Target target, TargetType targetType, Vector3 targetPosition)
    {
        this.TargetObject = target;
        this.TargetType = targetType;
        this.CurrentTargetPosition = targetPosition;
        this.PriorTargetPosition = targetPosition;

        this.TimeSinceLastDetection = 0;
    }

    public Target TargetObject
    { get; set; }

    public TargetType TargetType
    { get; set; }

    public Vector3 CurrentTargetPosition
    { get; private set; }

    public Vector3 PriorTargetPosition
    { get; private set; }

    public float TimeSinceLastDetection
    { get; private set; }

    public void UpdateTargetData(Vector3 newTargetPosition)
    {
        this.PriorTargetPosition = this.CurrentTargetPosition;
        this.CurrentTargetPosition = newTargetPosition;
    }

    public void IncrementTimeSinceLastDetection(float timeToIncrease)
    {
        TimeSinceLastDetection += timeToIncrease;
    }

    public void ResetTimeSinceLastDetection()
    {
        TimeSinceLastDetection = 0;
    }

    public void CalculatePredictedLocation()
    {
        throw new NotImplementedException("Not implemented the calculation of the predicted location - should use prior target position and current target position.");
    }
}
