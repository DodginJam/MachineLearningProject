using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [field: SerializeField]
    public Button TrackingAgentSceneLoad
    {  get; private set; }

    [field: SerializeField]
    public Button TrackingAgentWitnessSceneLoad
    { get; private set; }

    [field: SerializeField]
    public Button FlyingAgentSceneLoad
    { get; private set; }

    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnEnable()
    {
        if (TrackingAgentSceneLoad != null)
        {
            TrackingAgentSceneLoad.onClick.AddListener(LoadTrackingAgentScene);
        }

        if (TrackingAgentWitnessSceneLoad != null)
        {
            TrackingAgentWitnessSceneLoad.onClick.AddListener(LoadTrackingAgentWitnessScene);
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

        if (TrackingAgentWitnessSceneLoad != null)
        {
            TrackingAgentWitnessSceneLoad.onClick.RemoveListener(LoadTrackingAgentWitnessScene);
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

    void LoadTrackingAgentWitnessScene()
    {
        SceneManager.LoadScene(3);
    }

    void LoadFlyingAgentScene()
    {
        SceneManager.LoadScene(2);
    }
}
