using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [field: SerializeField]
    public Button TrackingAgentSceneLoad
    {  get; private set; }

    [field: SerializeField]
    public Button FlyingAgentSceneLoad
    { get; private set; }

    private void OnEnable()
    {
        if (TrackingAgentSceneLoad != null)
        {
            TrackingAgentSceneLoad.onClick.AddListener(LoadTrackingAgentScene);
        }

        if (FlyingAgentSceneLoad != null)
        {
            FlyingAgentSceneLoad.onClick.AddListener(LoadFlyingAgentScene);
        }
    }

    private void OnDisable()
    {
        if (TrackingAgentSceneLoad != null)
        {
            TrackingAgentSceneLoad.onClick.RemoveListener(LoadTrackingAgentScene);
        }

        if (FlyingAgentSceneLoad != null)
        {
            FlyingAgentSceneLoad.onClick.RemoveListener(LoadFlyingAgentScene);
        }
    }

    void LoadTrackingAgentScene()
    {
        SceneManager.LoadScene(1);
    }

    void LoadFlyingAgentScene()
    {
        SceneManager.LoadScene(2);
    }
}
