using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using ProjectEnums;

public abstract class TargetManager<T> : MonoBehaviour where T : Target
{
    /// <summary>
    /// Global list of targets in the scene for management, particualry around start / end episodes - NOT to be used by the agent for observations.
    /// </summary>
    [field: SerializeField, Header("Target Data")]
    public T[] AllTargets
    { get; private set; }

    /// <summary>
    /// Sets the targets to random spots within the provided area.
    /// </summary>
    public abstract void SetTargetsToNewSpot();

    public abstract float GetMaxTargetHeight();

    /// <summary>
    /// Activate targets.
    /// </summary>
    /// <param name="amountToActivate"></param>
    public abstract void ActivateTargets(int amountToActivate);

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
}
