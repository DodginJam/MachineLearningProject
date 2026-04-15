using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    /// <summary>
    /// Global list of targets in the scene for management, particualry around start / end episodes - NOT to be used by the agent for observations.
    /// </summary>
    [field: SerializeField, Header("Target Data")]
    public Target[] AllTargets
    { get; private set; }

    /// <summary>
    /// The max height at which the targets can be placed during runtime.
    /// </summary>
    [field: SerializeField]
    public float MaxTargetHeight
    { get; private set; }

    /// <summary>
    /// The training area mesh renderer can be used to access the bounds to ensure the target is spawned at a random point on the surface.
    /// </summary>
    [field: SerializeField, Header("Surface Ref")]
    public MeshRenderer TrainingArea
    { get; private set; }

    const float MinTargetHeight = 0.5f;

    public void SetTargetsToNewSpot()
    {
        Bounds areaBounds = TrainingArea.bounds;
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

    public void ActivateTargets(int amountToActivate)
    {
        amountToActivate = Mathf.Clamp(amountToActivate, 1, AllTargets.Length);

        foreach(Target target in AllTargets)
        {
            target.Die();
        }

        for (int i = 0; i < amountToActivate; i++)
        {
            AllTargets[i].Revive();
        }
    }

    public bool AreAllTargetsInactive()
    {
        return AllTargets.All((target) => target.transform.gameObject.activeSelf == false);
    }
}
