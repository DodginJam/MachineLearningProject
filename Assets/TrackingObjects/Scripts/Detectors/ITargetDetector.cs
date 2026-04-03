using System.Collections.Generic;
using UnityEngine;

public interface ITargetDetector
{
    public float DetectionDistance
    { get; } 

    public Dictionary<int, TargetData> DetectedTargets
    { get; }
}
