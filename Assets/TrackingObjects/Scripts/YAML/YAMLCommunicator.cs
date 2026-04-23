using System;
using Unity.MLAgents;
using UnityEngine;

public class YAMLCommunicator : MonoBehaviour
{
    [field: SerializeField, Header("Envionment Parameters: Inference Values")]
    public float MovementSpeedMultiplier
    { get; private set; } = 0.1f;

    [field: SerializeField]
    public int NumberOfTargets
    { get; private set; } = 2;

    [field: SerializeField]
    public float FriendlyRatio
    { get; private set; } = 0.5f;

    [field: SerializeField]
    public float SizeOfTargets
    { get; private set; } = 10.51f;

    [field: SerializeField]
    public float BlindFirePenalty
    { get; private set; } = 0f;

    [field: SerializeField]
    public float ArenaSize
    { get; private set; }

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

    public float GetMovementMultiplier()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("movement_multiplier", MovementSpeedMultiplier);
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

    public float GerArenaSize()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("arena_size", ArenaSize);
    }
}
