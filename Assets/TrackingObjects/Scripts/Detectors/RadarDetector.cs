using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RadarDetector : Detector
{
    /// <summary>
    /// The max height at which raycasts are to be sent from, starting from the transform base.
    /// </summary>
    [field: SerializeField]
    public float RadarHeight
    { get; private set; } = 50.0f;

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

    [field: SerializeField, Header("Rays Display")]
    public bool ShowRays
    { get; private set; }

    [field: SerializeField, Min(0.01f)]
    public float RayDuration
    { get; private set; }

    private void FixedUpdate()
    {
        ApplyNewRotation();

        // Grab the data from the targets detected this frame.
        Dictionary<int, TargetData> aqquiredTargets = DetectAndReturnTargets();
        AddOrUpdateTargetsToDictionary(aqquiredTargets);

        // Find the items that have timed out from last detection limit.
        var itemsToRemove = GetTargetsToRemoveFromDictionary();
        RemoveTargetsFromDictionary(itemsToRemove);

        ProgressTimeoutOnDetectedTargets();

        // Debug.Log($"Number of Targets Detected this step: {aqquiredTargets.Count} AND Number of Targets Detected Overall: {DetectedTargets.Count}");
    }

    protected override void UpdateTargetData(TargetData targetData)
    {
        targetData.UpdateTargetData(targetData.CurrentTargetPosition);
        targetData.ResetTimeSinceLastDetection();
    }

    protected override List<KeyValuePair<int, TargetData>> GetTargetsToRemoveFromDictionary()
    {
        return DetectedTargets.Where((item) => item.Value.TimeSinceLastDetection > TimeOutTarget).ToList();
    }

    void ProgressTimeoutOnDetectedTargets()
    {
        // Progress the timeout timer on the remaining detected targets.
        foreach (var target in DetectedTargets)
        {
            target.Value.IncrementTimeSinceLastDetection(Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Returns a dictionary of targets the projected raycasts have found.
    /// </summary>
    /// <returns></returns>
    protected override Dictionary<int, TargetData> DetectAndReturnTargets()
    {
        // Generarting the directions for the raycast projection.
        List<Vector3> directions = GenerateDirections(NumberOfRaysPerPoint, RadarAngleOfRays);

        // Return the targets found via the raycasts.
        return ProjectRaysForTargets(directions);
    }

    /// <summary>
    /// Project the rays from a given heights in multiple directions.
    /// </summary>
    /// <param name="rayDirections"></param>
    /// <returns></returns>
    Dictionary<int, TargetData> ProjectRaysForTargets(List<Vector3> rayDirections)
    {
        Dictionary<int, TargetData> toBeAqquiredTargets = new Dictionary<int, TargetData>();

        // Looping over the number of points the rays should be projected from.
        for (int i = 0; i < NumberOfStackRayOriginPoints; i++)
        {
            float rayHeight = Mathf.Lerp(0, RadarHeight, i / (float)(NumberOfStackRayOriginPoints - 1));

            foreach (Vector3 direction in rayDirections)
            {
                Ray ray = GenerateRay(rayHeight, direction);

                if (ProjectRay(ray, out RaycastHit hitInfo))
                {
                    ValidateTarget(hitInfo, toBeAqquiredTargets);
                }

                DrawHitDebugRay(hitInfo, rayHeight, direction);
            }
        }

        return toBeAqquiredTargets;
    }

    /// <summary>
    /// Generate a ray in a given direction from a set height.
    /// </summary>
    /// <param name="rayHeight"></param>
    /// <param name="rayDirection"></param>
    /// <returns></returns>
    Ray GenerateRay(float rayHeight, Vector3 rayDirection)
    {
        return new Ray(
                    transform.position + (Vector3.up * rayHeight),
                    Quaternion.Euler(XRotation, RadarRotationCurrent, ZRotation) * rayDirection
                );
    }

    /// <summary>
    /// Project the raycast and return the hit info.
    /// </summary>
    /// <param name="rayToProject"></param>
    /// <param name="hitInfo"></param>
    /// <returns></returns>
    bool ProjectRay(Ray rayToProject, out RaycastHit hitInfo)
    {
        return Physics.Raycast(rayToProject, out hitInfo, DetectionDistance, MasksToDetect);
    }

    /// <summary>
    /// Check a hit object for it is a target to be added to dictionary of aqquired targets.
    /// </summary>
    /// <param name="hitInfo"></param>
    /// <param name="toBeAqquiredTargets"></param>
    void ValidateTarget(RaycastHit hitInfo, Dictionary<int, TargetData> toBeAqquiredTargets)
    {
        GameObject hitGameObject = hitInfo.transform.gameObject;
        if (hitGameObject.TryGetComponent<Target>(out Target target))
        {
            if (!toBeAqquiredTargets.ContainsKey(hitGameObject.GetInstanceID()))
            {
                TargetData newData = new TargetData(target, target.TargetTyping, target.transform.position);

                toBeAqquiredTargets.Add(target.gameObject.GetInstanceID(), newData);
            }
        }
    }

    /// <summary>
    /// For debugging, draw the colour of the ray to help visualise what was hit.
    /// </summary>
    /// <param name="hitInfo"></param>
    /// <param name="rayHeight"></param>
    /// <param name="direction"></param>
    void DrawHitDebugRay(RaycastHit hitInfo, float rayHeight, Vector3 direction)
    {
        Color color = new Color(1, 1, 1, 0.05f);

        if (hitInfo.transform != null)
        {
            if (hitInfo.transform.TryGetComponent<Target>(out Target target))
            {
                if (target.TargetTyping == TargetType.Enemy)
                {
                    color = Color.red;
                }
                else //(target.TargetTyping == TargetType.Friendly)
                {
                    color = Color.blue;
                }
            }
            else
            {
                color = new Color(1, 0.92f, 0.016f, 0.05f);
            }
        }

        if (ShowRays)
        {
            Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Quaternion.Euler(XRotation, RadarRotationCurrent, ZRotation) * (direction * DetectionDistance), color, RayDuration);
        }
    }

    /// <summary>
    /// Apply rotation to the radar, keeping the values within a 360 degrees of radius.
    /// </summary>
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
                Gizmos.DrawLine(transform.position, targetData.Value.CurrentTargetPosition);
                Gizmos.DrawSphere(targetData.Value.CurrentTargetPosition, targetData.Value.TargetObject.transform.localScale.x * 0.66f);
            }
        }
    }

    /// <summary>
    /// Generate the directions within a given range of angle.
    /// </summary>
    /// <param name="numberOfDirections"></param>
    /// <param name="angleRangeOfDirections"></param>
    /// <returns></returns>
    List<Vector3> GenerateDirections(int numberOfDirections, float angleRangeOfDirections)
    {
        List<Vector3> directions = new List<Vector3>();

        for (int j = 0; j < numberOfDirections; j++)
        {
            float angle = (j / (float)numberOfDirections) * angleRangeOfDirections;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            directions.Add(dir);
        }

        return directions;
    }
}
