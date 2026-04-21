using Unity.MLAgents;
using UnityEngine;

public static class YAMLCommunicator
{
    public static float GetMovementMultiplier(float value = 0.1f)
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("movement_multiplier", value);
    }

    public static int GetNumberOfTargets(float value = 2)
    {
        return (int)Academy.Instance.EnvironmentParameters.GetWithDefault("number_of_targets", value);
    }

    public static float GetFriendlyRatio(float value = 0.5f)
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("friendly_ratio", value);
    }

    public static float GetSizeOfTargets(float value = 10.5f)
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("size_of_target", value);
    }

    public static float GetBlindFirePenalty(float value = 0f)
    {
        return Academy.Instance.EnvironmentParameters.GetWithDefault("blind_fire_penalty", value);
    }
}
