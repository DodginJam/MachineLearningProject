using UnityEngine;

/// <summary>
/// Single access point for reading envionment parameters from the academy, as defined in the YAML file. Provides values for default / inference running of agent.
/// </summary>
public abstract class EnvironmentParameters<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Singleton reference to the current instance.
    /// </summary>
    public static T Instance
    { get; private set; }

    private void Awake()
    {
        Initialise();
    }

    public void Initialise()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
