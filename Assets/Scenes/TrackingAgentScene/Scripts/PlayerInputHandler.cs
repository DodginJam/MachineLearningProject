using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[DefaultExecutionOrder(-1)]
public class PlayerInputHandler : MonoBehaviour
{
    public InputManager InputManager
    { get; private set; }

    public InputActions.PlayerActions PlayerActionMap
    { get; private set; }

    public Vector2 MovementInput
    { get; set; }

    public Vector2 RotationInput
    { get; set; }

    public event Action Pause;

    private void Awake()
    {
        if (InputManager == null)
        {
            InputManager = FindAnyObjectByType<InputManager>();

            if (InputManager == null)
            {
                Debug.LogError("Unable to find the global input manager.");
            }
            else
            {
                PlayerActionMap = InputManager.InputActions.Player;
            }
        }
    }

    private void OnEnable()
    {
        EnableInputListeners();
    }

    private void OnDisable()
    {
        DisableInputListeners();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MovementInput = HandleMovement();

        RotationInput = HandleRotation();
    }

    public Vector2 HandleMovement()
    {
        Vector2 playerMovement = PlayerActionMap.Movement.ReadValue<Vector2>();

        return playerMovement;
    }

    public Vector2 HandleRotation()
    {
        Vector2 rotationMovement = PlayerActionMap.Look.ReadValue<Vector2>();

        return rotationMovement;
    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Pause?.Invoke();
        }
    }

    public void EnableInputListeners()
    {
        PlayerActionMap.Pause.performed += OnPause;
    }

    public void DisableInputListeners()
    {
        PlayerActionMap.Pause.performed -= OnPause;
    }
}