using System;
using Unity.MLAgents;
using UnityEngine;

/// <summary>
/// Single access point for reading envionment parameters from the academy, as defined in the YAML file. Provides values for default / inference running of agent.
/// </summary>
public class YAMLCommunicator : MonoBehaviour
{
    /// <summary>
    /// The value for the speed of the targets when moving from point A to point B.
    /// </summary>
    [field: SerializeField, Header("Envionment Parameters: Inference Values")]
    public float MovementSpeed
    { get; private set; } = 10f;

    /// <summary>
    /// Number of targets to enable in an episode - this cannot increase the number of the targets in the scene, nor add targets beyond the buffer observables limit.
    /// </summary>
    [field: SerializeField]
    public int NumberOfTargets
    { get; private set; } = 2;

    /// <summary>
    /// The percentage of the targets that are to be assigned as friendly.
    /// </summary>
    [field: SerializeField]
    public float FriendlyRatio
    { get; private set; } = 0.5f;

    /// <summary>
    /// The local scale of the targets.
    /// </summary>
    [field: SerializeField]
    public float SizeOfTargets
    { get; private set; } = 10.51f;

    /// <summary>
    /// The penalty applied to the agent when firing the weapon without aiming at a valid target.
    /// </summary>
    [field: SerializeField]
    public float BlindFirePenalty
    { get; private set; } = 0f;

    /// <summary>
    /// The local scale of the area - this affects the spawning and movement space of the targets, thus also the range the agent has to fire at them from.
    /// </summary>
    [field: SerializeField]
    public float ArenaSize
    { get; private set; }

    /// <summary>
    /// Singleton reference to the current instance.
    /// </summary>
    public static YAMLCommunicator Instance
    { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetMovementSpeed()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("movement_speed", MovementSpeed);
    }

    public int GetNumberOfTargets()
    {
        return (int)Academy.Instance.EnvironmentParameters.GetWithDefault("number_of_targets", NumberOfTargets);
    }

    public float GetFriendlyRatio()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("friendly_ratio", FriendlyRatio);
    }

    public float GetSizeOfTargets()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("size_of_target", SizeOfTargets);
    }

    public float GetBlindFirePenalty()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("blind_fire_penalty", BlindFirePenalty);
    }

    public float GetArenaSize()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("arena_size", ArenaSize);
    }
}
