using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Radar : MonoBehaviour
{
    public Dictionary<int, TargetData> DetectedTargets
    { get; private set; } = new Dictionary<int, TargetData>();

    [field: SerializeField]
    public float RadarHeight
    { get; private set; } = 50.0f;

    [field: SerializeField]
    public float DetectionDistance
    { get; private set; } = 100.0f;

    public float RadarRotationCurrent
    { get; private set; }

    [field: SerializeField]
    public float RotationSpeed
    { get; private set; } = 1.0f;

    [field: SerializeField]
    public int NumberOfRaycasts
    { get; private set; }

    [field: SerializeField]
    public LayerMask MasksToDetect
    { get; private set; }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        CastRotatingRaycasts();
    }

    void CastRotatingRaycasts()
    {
        Dictionary<int, TargetData> aqquiredTargets = new Dictionary<int, TargetData>();

        for (int i = 0; i < NumberOfRaycasts; i++)
        {
            float rayHeight = Mathf.Lerp(0, RadarHeight, i / (float)(NumberOfRaycasts - 1));

            Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left};

            foreach(Vector3 direction in directions)
            {
                Ray ray = new Ray(
                    transform.position + (Vector3.up * rayHeight),
                    Quaternion.Euler(0, RadarRotationCurrent, 0) * direction
                );

                Color rayColour = Color.white;

                if (Physics.Raycast(ray, out RaycastHit hitInfo, DetectionDistance, MasksToDetect))
                {
                    GameObject hitGameObject = hitInfo.transform.gameObject;
                    if (hitGameObject.TryGetComponent<Target>(out Target target))
                    {
                        // Update a targets positional data and time since last detected if it has been previosuly detected.
                        if (aqquiredTargets.ContainsKey(hitGameObject.GetInstanceID()))
                        {
                            aqquiredTargets[hitGameObject.GetInstanceID()].TargetPosition = hitGameObject.transform.position;
                            aqquiredTargets[hitGameObject.GetInstanceID()].TimeSinceLastDetection = 0;
                        }
                        else // Add a target to the detected targets dictionary if it is not already added.

                        {
                            TargetData newData = new TargetData(target, target.TargetTyping, target.transform.position);

                            aqquiredTargets.Add(target.gameObject.GetInstanceID(), newData);
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

                Debug.DrawRay(transform.position + (Vector3.up * rayHeight), Quaternion.Euler(0, RadarRotationCurrent, 0) * (direction * DetectionDistance), rayColour);
            }


        }

        foreach (var target in aqquiredTargets)
        {
            if (DetectedTargets.ContainsKey(target.Key))
            {
                DetectedTargets[target.Key].TargetPosition = target.Value.TargetPosition;
            }
            else
            {
                DetectedTargets.Add(target.Key, target.Value);
            }
        }

        ApplyNewRotation();

        Debug.Log($"Number of Targets Detected this step: {aqquiredTargets.Count} AND Number of Targets Detected Overall: {DetectedTargets.Count}");
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
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetData.Value.TargetPosition, 5.0f);
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
