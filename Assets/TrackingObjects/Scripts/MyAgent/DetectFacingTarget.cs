using System;
using Unity.Mathematics;
using Unity.MLAgents;
using UnityEngine;

public class DetectFacingTarget : MonoBehaviour
{
    [field:SerializeField]
    public LayerMask LayersToHit
    { get; private set; }

    public bool TargetDetected
    { get; set; }

    public Transform DetectedTarget
    {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, Mathf.Infinity, LayersToHit))
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                TargetDetected = true;
                DetectedTarget = hitInfo.transform;
            }
        }
    }

    public bool IsTargetDetected(out Transform detectedTarget)
    {
        detectedTarget = DetectedTarget;
        return TargetDetected;
    }

    public void RemoveDetectedInfo()
    {
        TargetDetected = false;
        DetectedTarget = null;
    }
}
