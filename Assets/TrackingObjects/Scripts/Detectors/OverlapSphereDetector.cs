using System.Collections.Generic;
using UnityEngine;
using ProjectEnums;

public class OverlapSphereDetector : Detector, ITargetDetector
{
    /// <summary>
    /// The detected collision the raycasts detect.
    /// </summary>
    [field: SerializeField]
    public LayerMask MasksToDetect
    { get; private set; }


    private void OnDisable()
    {
        DetectedTargets.Clear();
    }

    public override void GetTargets()
    {
        DetectedTargets.Clear();

        Dictionary<int, TargetData> aqquiredTargets = DetectAndReturnTargets();
        AddOrUpdateTargetsToDictionary(aqquiredTargets);
    }

    public override void RemoveTargets()
    {
        
    }

    protected override Dictionary<int, TargetData> DetectAndReturnTargets()
    {
        var allTargetsToAdd = new Dictionary<int, TargetData>();

        Collider[] foundColliders = Physics.OverlapSphere(transform.position, DetectionDistance, MasksToDetect);
        List<Target> foundTargets = new List<Target>();

        foreach (Collider collider in foundColliders)
        {
            if (collider.TryGetComponent<Target>(out Target target))
            {
                foundTargets.Add(target);
            }
        }

        foreach (Target target in foundTargets)
        {
            int id = target.GetGameObjectsInstanceID();
            TargetData targetData = new TargetData(target, target.TargetTyping, target.transform.position);
            allTargetsToAdd.Add(id, targetData);
        }

        return allTargetsToAdd;
    }

    protected override List<KeyValuePair<int, TargetData>> GetTargetsToRemoveFromDictionary()
    {
        List<KeyValuePair<int, TargetData>> listOfTargetsToRemove = new List<KeyValuePair<int, TargetData>>();

        foreach (var target in DetectedTargets)
        {
            if (target.Value.TargetObject.gameObject.activeSelf == false)
            {
                listOfTargetsToRemove.Add(target);
            }
        }

        return listOfTargetsToRemove;
    }

    protected override void UpdateTargetData(TargetData targetData)
    {
        targetData.UpdateTargetData(targetData.TargetObject.transform.position);
    }

    private void OnDrawGizmos()
    {
        if (this.isActiveAndEnabled)
        {
            if (DetectedTargets != null && DetectedTargets.Count > 0)
            {
                foreach (KeyValuePair<int, TargetData> targetData in DetectedTargets)
                {
                    if (targetData.Value.TargetObject.gameObject.activeSelf)
                    {
                        Gizmos.color = (targetData.Value.TargetType == TargetType.Friendly) ? Color.blue : Color.red;
                        Gizmos.DrawLine(transform.position, targetData.Value.CurrentTargetPosition);
                        Gizmos.DrawSphere(targetData.Value.CurrentTargetPosition, targetData.Value.TargetObject.transform.localScale.x * 0.66f);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (this.isActiveAndEnabled)
        {
            Gizmos.DrawWireSphere(transform.position, DetectionDistance);
        }
    }
}

