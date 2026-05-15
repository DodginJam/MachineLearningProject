using System.Collections.Generic;

public interface ITargetDetector
{
    public float DetectionDistance
    { get; } 

    public Dictionary<int, TargetData> DetectedTargets
    { get; }

    void ApplyDetectionLoop();
}
