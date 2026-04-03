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

    public void ActivateTargets()
    {
        foreach (Target target in AllTargets)
        {
            target.Revive();
        }
    }
}
