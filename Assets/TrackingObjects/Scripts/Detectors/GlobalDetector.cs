using System.Collections.Generic;
using UnityEngine;

public class GlobalDetector : Detector, ITargetDetector
{
    [field: SerializeField]
    public TargetManager TargetManager
    { get; private set; }

    private void OnDisable()
    {
        DetectedTargets.Clear();
    }


    protected override Dictionary<int, TargetData> DetectAndReturnTargets()
    {
        var allTargetsToAdd = new Dictionary<int, TargetData>();

        foreach (Target target in TargetManager.AllTargets)
        {
            if (target.gameObject.activeSelf)
            {
                int id = target.GetGameObjectsInstanceID();
                TargetData targetData = new TargetData(target, target.TargetTyping, target.transform.position);
                allTargetsToAdd.Add(id, targetData);
            }
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
}
