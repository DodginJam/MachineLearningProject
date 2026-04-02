using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    [field: SerializeField, Header("Target Data")]
    public Target[] AllTargets
    { get; private set; }

    public List<Target> VisableTargets
    { get; private set; } = new List<Target>();

    [field: SerializeField]
    public float MaxTargetHeight
    { get; private set; }

    const float MinTargetHeight = 0.5f;

    public void SetTargetsToNewSpot(MeshRenderer meshBounds)
    {
        Bounds areaBounds = meshBounds.bounds;
        float minX = areaBounds.center.x - areaBounds.extents.x;
        float maxX = areaBounds.center.x + areaBounds.extents.x;

        float minZ = areaBounds.center.z - areaBounds.extents.z;
        float maxZ = areaBounds.center.z + areaBounds.extents.z;

        foreach (Target target in AllTargets)
        {
            Vector3 newPosition = Vector3.zero;

            do
            {
                float xPosition = Random.Range(minX, maxX);
                float zPosition = Random.Range(minZ, maxZ);

                float yPosition = transform.position.y + Random.Range(MinTargetHeight, MaxTargetHeight);

                newPosition = new Vector3(xPosition, yPosition, zPosition);
            }
            while (Vector3.Distance(newPosition, this.transform.position) < 6f);

            target.transform.position = newPosition;
        }
    }

    public void RemoveAndClearTargets()
    {
        // Remove all targets from the scene and clear visable targets..
        foreach (Target target in AllTargets)
        {
            target.gameObject.SetActive(false);
        }
        VisableTargets.Clear();
    }

    public void SetRandomVisableTargets()
    {
        // Set a random amount of targets to be visable.
        int visableTargetsForEpisode = Random.Range(1, AllTargets.Length);
        for (int i = 0; i < visableTargetsForEpisode; i++)
        {
            VisableTargets.Add(AllTargets[i]);
            VisableTargets[i].Initialise();
        }
    }

    public void RemoveVisableTarget(Target detectedTransfom, FireSolution fireSolution)
    {
        // Removing detected targets from the visable list.
        bool wasVisableTargetRemoved = VisableTargets.Remove(detectedTransfom);
        fireSolution.RemoveDetectedInfo();

        // Debug check.
        if (wasVisableTargetRemoved == false) Debug.LogError("The detected target did not exist on the visable targets list.");
    }
}
