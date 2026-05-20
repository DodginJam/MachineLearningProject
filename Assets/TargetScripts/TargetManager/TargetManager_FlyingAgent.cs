using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using ProjectEnums;

public class TargetManager_FlyingAgent : TargetManager<Target_FlyingAgent>
{
    [field: SerializeField]
    public BoxCollider TargetArea
    { get; private set; }

    private void Start()
    {
        SetTargetsToNewSpot();

        ActivateTargets(20);
    }

    public override void SetTargetsToNewSpot()
    {
        foreach (Target_FlyingAgent target in AllTargets)
        {
            target.transform.position = GetPositionWithinArea();
        }   
    }

    /// <summary>
    /// First, deactivate all targets, then revive them to re-set them
    /// </summary>
    /// <param name="amountToActivate"></param>
    public override void ActivateTargets(int amountToActivate)
    {
        amountToActivate = Mathf.Clamp(amountToActivate, 1, AllTargets.Length);

        foreach (Target_FlyingAgent target in AllTargets)
        {
            target.Die();
        }

        for (int i = 0; i < amountToActivate; i++)
        {
            AllTargets[i].Revive();
            AllTargets[i].SetTargetTyping(TargetType.Enemy);
            AllTargets[i].transform.position = GetPositionWithinArea();
            AllTargets[i].SetPositionToMoveTo(GetPositionWithinArea());
        }
    }

    public Vector3 GetPositionWithinArea()
    {
        Vector3 position = Vector3.zero;

        float xExtents = TargetArea.size.z / 2;
        float yExtents = TargetArea.size.y / 2;
        float zExtents = TargetArea.size.z / 2;

        Vector3 localPosition = TargetArea.center + new Vector3 (Random.Range(-xExtents, xExtents), Random.Range(-yExtents, yExtents), Random.Range(-zExtents, zExtents));
        position = TargetArea.transform.TransformPoint(localPosition);

        Debug.DrawLine(position, Vector3.zero, Color.magenta);

        return position;
    }

    float GetScale()
    {
        return EnvironmentParametersFlyingAgent.Instance.GetTargetsScale();
    }

    float GetMovementSpeed()
    {
        return EnvironmentParametersFlyingAgent.Instance.GetMovementSpeed();
    }
}
