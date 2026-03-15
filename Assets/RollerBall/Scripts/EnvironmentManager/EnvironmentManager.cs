using Unity.MLAgents;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [field: SerializeField]
    public float TimeScale
    { get; set; } = 20.0f;

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;

        Time.timeScale = TimeScale;
    }

    void EnvironmentReset()
    {
        // Reset the scene here
    }
}
