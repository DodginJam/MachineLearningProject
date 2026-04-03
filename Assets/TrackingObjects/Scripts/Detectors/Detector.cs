using UnityEngine;
using System.Collections.Generic;

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
    protected abstract Dictionary<int, TargetData> DetectAndReturnTargets();

    /// <summary>
    /// Return a list of key value pairs of the target datas that should be removed from the detected targets dictionary.
    /// </summary>
    /// <returns></returns>
    protected abstract List<KeyValuePair<int, TargetData>> GetTargetsToRemoveFromDictionary();

    /// <summary>
    /// Loop over the dictionary and check if a target data item is already contained - either update existing or add new target data to detected targets.
    /// </summary>
    /// <param name="aqquiredTargets"></param>
    protected void AddOrUpdateTargetsToDictionary(Dictionary<int, TargetData> aqquiredTargets)
    {
        // Loop over the aqquired targets data.
        foreach (var targetItem in aqquiredTargets)
        {
            // If targetItem is already contained in the master targetItem dict, simply update position and reset the last detected timer.
            if (DetectedTargets.ContainsKey(targetItem.Key))
            {
                UpdateTargetData(targetItem.Value);
            }
            else
            {
                DetectedTargets.Add(targetItem.Key, targetItem.Value);
            }
        }
    }

    /// <summary>
    /// Remove the provided taget data key values pairs from the detected targets dictionary.
    /// </summary>
    /// <param name="itemsToRemove"></param>
    protected void RemoveTargetsFromDictionary(List<KeyValuePair<int, TargetData>> itemsToRemove)
    {
        // Remove the items.
        foreach (var item in itemsToRemove)
        {
            DetectedTargets.Remove(item.Key);
        }
    }

    /// <summary>
    /// Update the target data as required for the implementation.
    /// </summary>
    /// <param name="targetData"></param>
    protected abstract void UpdateTargetData(TargetData targetData);
}
