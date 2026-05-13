using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using ProjectEnums;

public class TargetManager_FlyingAgent : TargetManager<Target_FlyingAgent>
{
    public override float GetMaxTargetHeight()
    {
        return default;
    }

    public override void SetTargetsToNewSpot()
    {
        foreach (Target_FlyingAgent target in AllTargets)
        {
            target.transform.position = GetPositionInArea();
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
            AllTargets[i].SetSize(GetScale());

            AllTargets[i].SetPositionToMoveTo(GetPositionInArea());
        }
    }

    Vector3 GetPositionInArea()
    {
        return default(Vector3);
    }

    float GetScale()
    {
        return default(float);
    }

    float GetMovementSpeed()
    {
        return default(float);
    }

    /// <summary>
    /// Apply the targets speed via environment parameter from the configuration file of the agent.
    /// </summary>
    public void SetTargetsSpeed()
    {
        foreach (Target_FlyingAgent target in AllTargets)
        {
            target.SetMovementSpeed(GetMovementSpeed());
        }
    }
}
