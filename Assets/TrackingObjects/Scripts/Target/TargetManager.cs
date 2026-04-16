using System.Linq;
using Unity.MLAgents;
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

    /// <summary>
    /// Sets the targets to random spots within the provided area.
    /// </summary>
    public void SetTargetsToNewSpot()
    {
        GetBoundsLimits(TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);

        foreach (Target target in AllTargets)
        {
            target.transform.position = GetRandomPointInArea(minX, maxX, minZ, maxZ);
        }
    }

    public void GetBoundsLimits(MeshRenderer areaMesh, out float minX, out float maxX, out float minZ, out float maxZ)
    {
        Bounds areaBounds = areaMesh.bounds;
        minX = areaBounds.center.x - areaBounds.extents.x;
        maxX = areaBounds.center.x + areaBounds.extents.x;

        minZ = areaBounds.center.z - areaBounds.extents.z;
        maxZ = areaBounds.center.z + areaBounds.extents.z;
    }

    public Vector3 GetRandomPointInArea(float minX, float maxX, float minZ, float maxZ)
    {
        Vector3 newPosition = Vector3.zero;

        do
        {
            float xPosition = UnityEngine.Random.Range(minX, maxX);
            float zPosition = UnityEngine.Random.Range(minZ, maxZ);

            float yPosition = transform.position.y + UnityEngine.Random.Range(MinTargetHeight, MaxTargetHeight);

            newPosition = new Vector3(xPosition, yPosition, zPosition);
        }
        while (Vector3.Distance(newPosition, this.transform.position) < 6f);

        return newPosition;

    }

    /// <summary>
    /// First, deactivate all targets, then revive them to re-set them.
    /// </summary>
    /// <param name="amountToActivate"></param>
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

            GetBoundsLimits(TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);
            AllTargets[i].SetEndPoint(GetRandomPointInArea(minX, maxX, minZ, maxZ));
        }
    }

    /// <summary>
    /// Returns whether all target game objects are inactive.
    /// </summary>
    /// <returns></returns>
    public bool AreAllTargetsInactive()
    {
        return AllTargets.All((target) => target.transform.gameObject.activeSelf == false);
    }

    public bool AreAllEnemyTargetsInactive()
    {
        return AllTargets.Where(target => target.TargetTyping == TargetType.Enemy).All((target) => target.transform.gameObject.activeSelf == false);
    }

    /// <summary>
    /// Apply the targets speed via environment parameter from the configuration file of the agent.
    /// </summary>
    public void SetTargetsSpeed()
    {
        foreach (Target target in AllTargets)
        {
            target.SetMovementMultiplier(Academy.Instance.EnvironmentParameters.GetWithDefault("movement_multiplier", 1.0f));
        }
    }
}
