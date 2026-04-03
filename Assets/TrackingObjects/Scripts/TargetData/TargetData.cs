using UnityEngine;

public class TargetData
{
    public TargetData(Target target, TargetType targetType, Vector3 targetPosition)
    {
        this.TargetObject = target;
        this.TargetType = targetType;
        this.TargetPosition = targetPosition;

        this.TimeSinceLastDetection = 0;
    }

    public Target TargetObject
    { get; set; }

    public TargetType TargetType
    { get; set; }

    public Vector3 TargetPosition
    { get; private set; }

    public float TimeSinceLastDetection
    { get; private set; }

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        this.TargetPosition = newTargetPosition;
    }

    public void IncrementTimeSinceLastDetection(float timeToIncrease)
    {
        TimeSinceLastDetection += timeToIncrease;
    }

    public void ResetTimeSinceLastDetection()
    {
        TimeSinceLastDetection = 0;
    }
}
