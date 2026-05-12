using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using ProjectEnums;

public class TargetManager_TrackingAgent : TargetManager<Target_TrackingAgent>
{
    [field: SerializeField]
    public float AreanaSizeToHeightRation
    { get; set; } = 5f;

    public float MinTargetHeight
    { get; private set; } = 0.5f;

    /// <summary>
    /// The training area mesh renderer can be used to access the bounds to ensure the target is spawned at a random point on the surface.
    /// </summary>
    [field: SerializeField, Header("Surface Ref")]
    public MeshRenderer TrainingArea
    { get; private set; }

    public override float GetMaxTargetHeight()
    {
        float returneHeight = YAMLCommunicatorTrackingObject.Instance.GetArenaSize() * AreanaSizeToHeightRation;
        return returneHeight;
    }

    public override void SetTargetsToNewSpot()
    {
        GetBoundsLimits(TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);

        foreach (Target_TrackingAgent target in AllTargets)
        {
            target.transform.position = GetRandomPointInArea(minX, maxX, minZ, maxZ);
        }   
    }

    /// <summary>
    /// First, deactivate all targets, then revive them to re-set them
    /// </summary>
    /// <param name="amountToActivate"></param>
    public override void ActivateTargets(int amountToActivate)
    {
        amountToActivate = Mathf.Clamp(amountToActivate, 1, AllTargets.Length);

        foreach (Target_TrackingAgent target in AllTargets)
        {
            target.Die();
        }

        float friendlyRatio = YAMLCommunicatorTrackingObject.Instance.GetFriendlyRatio();
        int numberOfFriendlies = Mathf.FloorToInt(amountToActivate * friendlyRatio);

        for (int i = 0; i < amountToActivate; i++)
        {
            AllTargets[i].Revive();
            AllTargets[i].SetTargetTyping(i < numberOfFriendlies ? TargetType.Friendly : TargetType.Enemy);
            AllTargets[i].SetMaterial();
            AllTargets[i].SetSize(YAMLCommunicatorTrackingObject.Instance.GetSizeOfTargets());

            GetBoundsLimits(TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);
            AllTargets[i].SetPositionToMoveTo(GetRandomPointInArea(minX, maxX, minZ, maxZ));
        }
    }


    public Vector3 GetRandomPointInArea(float minX, float maxX, float minZ, float maxZ)
    {
        Vector3 newPosition = Vector3.zero;

        do
        {
            float xPosition = UnityEngine.Random.Range(minX, maxX);
            float zPosition = UnityEngine.Random.Range(minZ, maxZ);

            float yPosition = transform.position.y + UnityEngine.Random.Range(MinTargetHeight, GetMaxTargetHeight());

            newPosition = new Vector3(xPosition, yPosition, zPosition);
        }
        while (Vector3.Distance(newPosition, this.transform.position) < YAMLCommunicatorTrackingObject.Instance.GetSizeOfTargets());

        return newPosition;

    }

    public void GetBoundsLimits(MeshRenderer areaMesh, out float minX, out float maxX, out float minZ, out float maxZ)
    {
        Bounds areaBounds = areaMesh.bounds;
        minX = areaBounds.center.x - areaBounds.extents.x;
        maxX = areaBounds.center.x + areaBounds.extents.x;

        minZ = areaBounds.center.z - areaBounds.extents.z;
        maxZ = areaBounds.center.z + areaBounds.extents.z;
    }

    /// <summary>
    /// Apply the targets speed via environment parameter from the configuration file of the agent.
    /// </summary>
    public void SetTargetsSpeed()
    {
        foreach (Target_TrackingAgent target in AllTargets)
        {
            target.SetMovementSpeed(YAMLCommunicatorTrackingObject.Instance.GetMovementSpeed());
        }
    }
}
