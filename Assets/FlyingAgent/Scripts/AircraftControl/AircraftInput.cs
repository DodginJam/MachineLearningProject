using System;
using UnityEngine;
using UnityEngine.InputSystem;
using AircraftData;

public class AircraftInput : MonoBehaviour
{
    public InputActions.AircraftActions AircraftActionMap
    { get; private set; }

    public float ThrottleInput
    { get; private set; }

    public float ElevatorInput
    { get; private set; }

    public float AileronInput
    { get; private set; }

    public float RudderInput
    { get; private set; }

    public int CurrentControlSchemeID
    { get; private set; }



    private static int JoystickID
    { get; set; }

    private static int GamepadID
    { get; set; }

    private static int MouseKeyboardID
    { get; set; }



    public bool IsJoytickControl
    { get; private set; } = false;

    public ControlInputType CurrentInputType
    { get; private set; }

    public bool CameraTogglePressed
    { get; set; }

    public bool CameraFreeLookTogglePressed
    { get; set; }

    public Vector2 CameraInput
    { get; private set; }

    public bool IsFiring
    { get; private set; }

    public bool FireSafetyDisabled
    { get; private set; }

    [field: SerializeField]
    public bool EnablePlayerControl
    { get; private set; } = true;

    /// <summary>
    /// Methods to be assigned related to break inputs.
    /// </summary>
    public event Action<bool> OnBreakInput;

    private void Awake()
    {
        if (InputManager.Instance.InputActions != null)
        {
            AircraftActionMap = InputManager.Instance.InputActions.Aircraft;
        }
        else
        {
            Debug.LogError("Unable to assign class instance to InputActions_action");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        JoystickID = Animator.StringToHash("Joystick");
        GamepadID = Animator.StringToHash("Gamepad");
        MouseKeyboardID = Animator.StringToHash("Keyboard&Mouse");
    }

    void OnEnable()
    {
        ResetInputs();
        AircraftActionMap.Enable();
        SetUpInputListeners(AircraftActionMap);
    }

    void OnDisable()
    {
        ResetInputs();
        AircraftActionMap.Disable();
        DisableListeners(AircraftActionMap);
    }

    private void ResetInputs()
    {
        ThrottleInput = 0;
        ElevatorInput = 0;
        AileronInput = 0;
        RudderInput = 0;
        CameraInput = Vector2.zero;
        IsFiring = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputControllerScheme();

        UpdateIsJoystickBeingUsed();

        if (EnablePlayerControl)
        {
            PollingInput newInput = ReturnPollingInput();
            ApplyPollingInput(newInput);
        }
    }

    public void OverridePlayerInput(PollingInput inputData)
    {
        ApplyPollingInput(inputData);
    }

    public PollingInput ReturnPollingInput()
    {
        PollingInput newInput = new PollingInput();
        newInput.ThrottleInput = ReadThrottleInput();
        newInput.ElevatorInput = AircraftActionMap.PitchAndRoll.ReadValue<Vector2>().y;
        newInput.AileronInput = -AircraftActionMap.PitchAndRoll.ReadValue<Vector2>().x;
        newInput.RudderInput = AircraftActionMap.Yaw.ReadValue<float>();
        newInput.CameraInput = AircraftActionMap.Look.ReadValue<Vector2>();

        return newInput;
    }

    private void ApplyPollingInput(PollingInput currentInput)
    {
        ThrottleInput = currentInput.ThrottleInput;
        ElevatorInput = currentInput.ElevatorInput;
        AileronInput = currentInput.AileronInput;
        RudderInput = currentInput.RudderInput;
        CameraInput = currentInput.CameraInput;
    }

    private void UpdateIsJoystickBeingUsed()
    {
        // Check for joystick being used as control so that throttle input can be swapped to a different binding setup.
        if (CurrentControlSchemeID != JoystickID)
        {
            if (IsJoytickControl == true)
            {
                IsJoytickControl = false;
            }
        }
        else
        {
            if (IsJoytickControl == false)
            {
                IsJoytickControl = true;
            }
        }
    }

    public float ReadThrottleInput()
    {
        float input = 0;

        if (IsJoytickControl == false)
        {
            input = AircraftActionMap.ThrottleComposite.ReadValue<float>();
        }
        else
        {
            input = AircraftActionMap.ThrottleSlider.ReadValue<float>();
        }
        
        return input;
    }

    /// <summary>
    /// Check to see if the input scheme has been changed and capture the string name of the new input type.
    /// </summary>
    private void UpdateInputControllerScheme()
    {
        if (CurrentControlSchemeID != Animator.StringToHash(InputManager.Instance.PlayerInputComponent.currentControlScheme))
        {
            CurrentControlSchemeID = Animator.StringToHash(InputManager.Instance.PlayerInputComponent.currentControlScheme);

            // Update the control type enum to reflect the current input when it has switched.
            switch (CurrentControlSchemeID)
            {
                case var value when value == MouseKeyboardID:
                    CurrentInputType = ControlInputType.MouseKeyboard;
                    break;
                case var value when value == JoystickID:
                    CurrentInputType = ControlInputType.Joystick;
                    break;
                case var value when value == GamepadID:
                    CurrentInputType = ControlInputType.Gamepad;
                    break;
                default:
                    CurrentInputType = ControlInputType.None;
                    break;
            }
        }
    }

    void SetUpInputListeners(InputActions.AircraftActions aircraftActions)
    {
        aircraftActions.CameraToggle.started += OnCameraToggle;

        aircraftActions.Fire.started += OnFire;

        aircraftActions.Fire.canceled += OnFire;

        aircraftActions.FireSafety.started += OnFireSafety;

        aircraftActions.CameraFreeLookToggle.started += OnCameraFreeLookToggle;

        aircraftActions.Brake.started += OnBrake;

        aircraftActions.Brake.canceled += OnBrake;
    }

    void DisableListeners(InputActions.AircraftActions aircraftActions)
    {
        aircraftActions.CameraToggle.started -= OnCameraToggle;

        aircraftActions.Fire.started -= OnFire;

        aircraftActions.Fire.canceled -= OnFire;

        aircraftActions.FireSafety.started -= OnFireSafety;

        aircraftActions.CameraFreeLookToggle.started -= OnCameraFreeLookToggle;

        aircraftActions.Brake.started -= OnBrake;

        aircraftActions.Brake.canceled -= OnBrake;
    }

    public void OnCameraToggle(InputAction.CallbackContext context)
    {
        CameraTogglePressed = context.ReadValueAsButton();
    }

    public void OnCameraFreeLookToggle(InputAction.CallbackContext context)
    {
        CameraFreeLookTogglePressed = context.ReadValueAsButton();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsFiring = true;
        }
        else if (context.canceled)
        {
            IsFiring = false;
        }
    }

    public void OnFireSafety(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            FireSafetyDisabled = !FireSafetyDisabled;
        }
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnBreakInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnBreakInput?.Invoke(false);
        }
    }

    public enum ControlInputType
    {
        None,
        Gamepad,
        Joystick,
        MouseKeyboard
    }
}

namespace AircraftData
{
    public struct PollingInput
    {
        public float ThrottleInput
        { get; set; }
        public float ElevatorInput
        { get; set; }
        public float AileronInput
        { get; set; }
        public float RudderInput
        { get; set; }
        public Vector2 CameraInput
        { get; set; }
    }
}