using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public abstract class Detector : MonoBehaviour, ITargetDetector
{
    /// <summary>
    /// The master set of detected active targets by the radar.
    /// </summary>
    public Dictionary<int, TargetData> DetectedTargets
    { get; private set; } = new Dictionary<int, TargetData>();

    /// <summary>
    /// The max distance at which the rays are projected.
    /// </summary>
    [field: SerializeField]
    public float DetectionDistance
    { get; set; } = 100.0f;

    /// <summary>
    /// Returns a dictionary of targets the projected raycasts have found.
    /// </summary>
    /// <returns></returns>
    protected abstract Dictionary<int, TargetData> RaycastForAndReturnTargets();
}
