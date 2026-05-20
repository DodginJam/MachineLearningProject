using System;
using Unity.MLAgents;
using UnityEngine;

public class EnvironmentParametersFlyingAgent : EnvironmentParameters<EnvironmentParametersFlyingAgent>
{
    [field: SerializeField, Header("Envionment Parameters: Inference Values")]
    public float StartingVelocity
    { get; private set; } = 50;

    [field: SerializeField]
    public float StartingThrottleValue
    { get; private set; } = 1;

    [field: SerializeField]
    public float TargetsScale
    { get; private set; } = 50;

    [field: SerializeField]
    public float TargetsMovement
    { get; private set; } = 20;

    public float GetStartingVelocity()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("starting_velocity", StartingVelocity);
    }

    public float GetThrottleValue()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("starting_throttle_value", StartingThrottleValue);
    }

    public float GetTargetsScale()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("targets_scale", TargetsScale);
    }

    public float GetMovementSpeed()
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("targets_movement", TargetsMovement);
    }
}
