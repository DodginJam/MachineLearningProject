using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance
    { get; private set; }

    public InputActions InputActions
    { get; private set; }

    public PlayerInput PlayerInputComponent
    { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            Instance = this;
            InputActions = new InputActions();
            InputActions.Enable();
        }

        if (TryGetComponent<PlayerInput>(out PlayerInput playerInputComponent))
        {
            PlayerInputComponent = playerInputComponent;
        }
        else
        {
            Debug.LogError("Unable to locate a player input component");
        }
    }
}