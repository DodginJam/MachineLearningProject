using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Radar : MonoBehaviour
{
    /// <summary>
    /// The master set of detected active targets by the radar.
    /// </summary>
    public Dictionary<int, TargetData> DetectedTargets
    { get; private set; } = new Dictionary<int, TargetData>();

    /// <summary>
    /// The max height at which raycasts are to be sent from, starting from the transform base.
    /// </summary>
    [field: SerializeField]
    public float RadarHeight
    { get; private set; } = 50.0f;

    /// <summary>
    /// The max distance at which the rays are projected.
    /// </summary>
    [field: SerializeField]
    public float DetectionDistance
    { get; private set; } = 100.0f;

    /// <summary>
    /// The current rotation that rays are sent from, considered the "facing" direction of the radar.
    /// </summary>
    public float RadarRotationCurrent
    { get; private set; }

    /// <summary>
    /// The speed at which the direction the rays that are generated rotate around the Y axis.
    /// </summary>
    [field: SerializeField]
    public float RotationSpeed
    { get; private set; } = 1.0f;

    /// <summary>
    /// The number of raycasts origin points from the base to height.
    /// </summary>
    [field: SerializeField, Min(1)]
    public int NumberOfStackRayOriginPoints
    { get; private set; }

    /// <summary>
    /// The amount of rays projecting from each raycast origin point.
    /// </summary>
    [field: SerializeField, Min(1)]
    public int NumberOfRaysPerPoint
    { get; private set; }

    /// <summary>
    /// The angle range the rays will project from their origin point.
    /// </summary>
    [field: SerializeField, Range(0, 360)]
    public float RadarAngleOfRays
    { get; private set; } = 360f;

    /// <summary>
    /// The detected collision the raycasts detect.
    /// </summary>
    [field: SerializeField]
    public LayerMask MasksToDetect
    { get; private set; }

    [field: SerializeField]
    public float XRotation
    { get; private set; }

    [field: SerializeField]
    public float ZRotation
    { get; private set; }

    /// <summary>
    /// The time until a previosuly detected target is dropped from the tracking dictionary.
    /// </summary>
    [field: SerializeField]
    public float TimeOutTarget
    { get; private set; } = 0.5f;

    void Start()
    {

    }

    private void FixedUpdate()
    {
        // Grab the data from the targets detected this frame.
        Dictionary<int, TargetData> aqquiredTargets = DetectTargetsViaRayCasts();

        // Loop over the aqquired targets data.
        foreach (var targetItem in aqquiredTargets)
        {
            // If targetItem is already contained in the master targetItem dict, simply update position and reset the last detected timer.
            if (DetectedTargets.ContainsKey(targetItem.Key))
            {
                DetectedTargets[targetItem.Key].TargetPosition = targetItem.Value.TargetPosition;
                DetectedTargets[targetItem.Key].TimeSinceLastDetection = 0;
            }
            else
            {
                DetectedTargets.Add(targetItem.Key, targetItem.Value);
            }
        }
        
        // Find the items that have timed out from last detection limit.
        var itemsToRemove = DetectedTargets.Where((item) => item.Value.TimeSinceLastDetection > TimeOutTarget).ToList();

        // Remove the items.
        foreach(var item in itemsToRemove)
        {
            DetectedTargets.Remove(item.Key);
        }

        // Progress the timeout timer on the remaining detected targets.
        foreach (var target in DetectedTargets)
        {
            target.Value.TimeSinceLastDetection += Time.fixedDeltaTime;
        }

        Debug.Log($"Number of Targets Detected this step: {aqquiredTargets.Count} AND Number of Targets Detected Overall: {DetectedTargets.Count}");

        ApplyNewRotation();
    }

    /// <summary>
    /// Returns a dictionary of targets the projected raycasts have found.
    /// </summary>
    /// <returns></returns>
    Dictionary<int, TargetData> DetectTargetsViaRayCasts()
    {
        Dictionary<int, TargetData> toBeAqquiredTargets = new Dictionary<int, TargetData>();

        // Generarting the directions
        List<Vector3> directions = new List<Vector3>();
        for (int j = 0; j < NumberOfRaysPerPoint; j++)
        {
            float angle = (j / (float)NumberOfRaysPerPoint) * RadarAngleOfRays;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            directions.Add(dir);
        }

        // Looping over the number of points the rays should be projected from.
        for (int i = 0; i < NumberOfStackRayOriginPoints; i++)
        {
            float rayHeight = Mathf.Lerp(0, RadarHeight, i / (float)(NumberOfStackRayOriginPoints - 1));

            foreach (Vector3 direction in directions)
            {
                Ray ray = new Ray(
                    transform.position + (Vector3.up * rayHeight),
                    Quaternion.Euler(XRotation, RadarRotationCurrent, ZRotation) * direction
                );

                Color rayColour = Color.white;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, DetectionDistance, MasksToDetect))
                {
                    GameObject hitGameObject = hitInfo.transform.gameObject;
                    if (hitGameObject.TryGetComponent<Target>(out Target target))
                    {
                        if (!toBeAqquiredTargets.ContainsKey(hitGameObject.GetInstanceID()))
                        {
                            TargetData newData = new TargetData(target, target.TargetTyping, target.transform.position);

                            toBeAqquiredTargets.Add(target.gameObject.GetInstanceID(), newData);
                        }

                        // Ray colour based on whether friendly or enemy detected.
                        if (target.TargetTyping == TargetType.Enemy)
                        {
                            rayColour = Color.red;
                        }
                        else if (target.TargetTyping == TargetType.Friendly)
                        {
                            rayColour = Color.blue;
                        }
                    }
                    else
                    {
                        rayColour = Color.yellow;
                    }
                }

                Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Quaternion.Euler(XRotation, RadarRotationCurrent, ZRotation) * (direction * DetectionDistance), rayColour);
            }
        }

        return toBeAqquiredTargets;
    }

    void ApplyNewRotation()
    {
        // Rotation of the radar calulation.
        float rotationAmount = RotationSpeed * Time.fixedDeltaTime;
        RadarRotationCurrent += rotationAmount;

        if (RadarRotationCurrent >= 360)
        {
            RadarRotationCurrent -= 360;
        }
    }

    private void OnDrawGizmos()
    {
        if (DetectedTargets != null && DetectedTargets.Count > 0)
        {
            foreach (KeyValuePair<int, TargetData> targetData in DetectedTargets)
            {
                Gizmos.color = (targetData.Value.TargetType == TargetType.Friendly) ? Color.blue : Color.red;
                Gizmos.DrawSphere(targetData.Value.TargetPosition, targetData.Value.TargetObject.transform.localScale.x * 0.66f);
            }
        }
    }
}

public enum TargetType
{
    NonTarget,
    Friendly,
    Enemy
}

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
    { get; set; }

    public float TimeSinceLastDetection
    { get; set; }
}
