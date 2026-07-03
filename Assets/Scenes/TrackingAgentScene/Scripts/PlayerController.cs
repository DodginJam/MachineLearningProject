using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Reference to the input manager for the player.
    /// </summary>
    public PlayerInputHandler InputHandler
    { get; private set; }

    public CharacterController CharacterControllerComp
    { get; private set; }

    [field: SerializeField]
    public float MovementSpeed
    { get; private set; } = 1.0f;

    [field: SerializeField]
    public float RotationSpeed
    { get; private set; } = 1.0f;


    /// <summary>
    /// Contains the velocity calculated from the movement inputs, which is applied to the character controllers overall movement per frame.
    /// </summary>
    public Vector3 CharacterMovementVelocity
    { get; private set; }

    /// <summary>
    /// Contains the velocity calculated from the forces applied to the character, which is applied to the character controllers overall movement per frame.
    /// </summary>
    public Vector3 CharacterForcesVelocity
    { get; private set; }

    private bool isGrounded;
    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }

        set
        {
            isGrounded = value;
            GroundedStateChange = true;
        }
    }

    /// <summary>
    /// Flag that is set for when the IsGrounded setter is changed.
    /// </summary>
    private bool GroundedStateChange
    { get; set; }

    /// <summary>
    /// The layers then count as walkable on for the IsGrounded status of the character.
    /// </summary>
    [field: SerializeField, Tooltip("The layers then count as walkable on for the IsGrounded status of the character.")]
    public LayerMask WalkableLayers
    { get; private set; }

    [field: SerializeField, Header("Camera Prefab")]
    public GameObject CameraPrefab
    { get; private set; }

    public GameObject AssignedPlayerCamera
    { get; set; }

    [field: SerializeField]
    public Transform PlayerCameraLead
    { get; private set; }

    private void Awake()
    {
        InitialiseVariables();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    private void Update()
    {
        HandleMovement();

        HandleRotation();

        UpdateGroundedStatus();

        SimulateGravity();

        CharacterControllerComp.Move(CharacterMovementVelocity + (CharacterForcesVelocity * Time.deltaTime));
    }

    public void InitialiseVariables()
    {
        if (TryGetComponent<CharacterController>(out CharacterController characterController))
        {
            CharacterControllerComp = characterController;
        }
        else
        {
            Debug.LogError("Unable to locate a character controller component on this transform.");
        }

        if (TryGetComponent<PlayerInputHandler>(out PlayerInputHandler inputHandler))
        {
            InputHandler = inputHandler;
        }
        else
        {
            Debug.LogError("Unable to locate a player input handler component on this transform.");
        }
    }

    /// <summary>
    /// Applies the movement inputs to the desired movement directions of the character controller and store this movement in the Values Movement Velocity.
    /// </summary>
    public void HandleMovement()
    {
        // Reset the movement velocity so the new inputs override the last inputs.
        CharacterMovementVelocity = Vector3.zero;

        if (CharacterControllerComp != null & InputHandler != null)
        {
            if (InputHandler.MovementInput.x == 0 && InputHandler.MovementInput.y == 0)
            {
                return;
            }

            // Only output the X and Z axis of movement, taken from the Vector2 of the input movement.
            Vector3 globalMovement = new Vector3(InputHandler.MovementInput.x, 0, InputHandler.MovementInput.y);

            // Translate the direction of movement locally to the characters current orientation.
            CharacterMovementVelocity = MovementSpeed * Time.deltaTime * transform.TransformDirection(globalMovement);
        }
        else
        {
            Debug.LogError("CharacterControllerComp is null or player input handler is null");
        }
    }

    /// <summary>
    /// Applies rotation to the character controller via the inputs recevied from the player mouse input.
    /// </summary>
    public void HandleRotation()
    {
        if (CharacterControllerComp != null & InputHandler != null)
        {
            if (InputHandler.RotationInput.x == 0)
            {
                return;
            }

            // The player object should rotate on only the Y axis to allow change in X and Z movement direction.
            Vector3 globalRotation = new Vector3(0, InputHandler.RotationInput.x, 0);

            transform.Rotate(RotationSpeed * Time.deltaTime * globalRotation);
        }
        else
        {
            Debug.LogError("CharacterControllerComp is null or player input handler is null");
        }
    }

    /// <summary>
    /// Simulates the downward force to applt the character controller.
    /// </summary>
    public void SimulateGravity()
    {
        // The bool is to flag if the player should have their gravity reset.
        bool resetGravityToZero = false;

        // If the grounded state has changed, flag to reset gravity to zero, as it means the player has just change their grounded state.
        if (GroundedStateChange)
        {
            resetGravityToZero = true;

            // Reset the grounded state change flag to prevent further checks.
            GroundedStateChange = false;
        }


        // If not grounded, apply gravity.
        if (IsGrounded)
        {
            // Setting the gravity value when grounded as it should remain a constant to allow natural falls and to stop playing from bumping down shallow slopes
            CharacterForcesVelocity = new Vector3(CharacterForcesVelocity.x, -2, CharacterForcesVelocity.z);
        }
        else
        {
            // If the flag to reset gravity has occured, hard set the gravity to zero so that the player falls like a rigidbody without the already constant "IsGrounded" downward force already starting with.
            if (resetGravityToZero)
            {
                CharacterForcesVelocity = new Vector3(CharacterForcesVelocity.x, 0, CharacterForcesVelocity.z);
            }
            else
            {
                CharacterForcesVelocity += new Vector3(0, Physics.gravity.y * Time.deltaTime, 0);
            }
        }
    }

    /// <summary>
    /// Using SphereCasts, check if the character controller is in contact with the ground.
    /// </summary>
    public void UpdateGroundedStatus()
    {
        float sphereCastRadius = CharacterControllerComp.radius;
        Vector3 sphereCastDirection = Vector3.down;

        // The origin is at the base of the character controller minus the radius of the sphere cast, ensuring the spherecast and feet of controller capusle line up correctly.
        Vector3 sphereCastOrigin = transform.position - new Vector3(0, (CharacterControllerComp.height / 2) - (sphereCastRadius), 0);

        float sphereCastMaxDistance = 0.1f;

        bool isGrounded = Physics.SphereCast(sphereCastOrigin, sphereCastRadius, sphereCastDirection, out _, sphereCastMaxDistance, WalkableLayers);

        if (isGrounded != IsGrounded)
        {
            IsGrounded = isGrounded;
        }

        // Debugging
        /*      
        Debug.DrawLine(transform.position, sphereCastOrigin, Color.red); // The origin point of the sphere cast from the transform centre.
        Debug.DrawLine(sphereCastOrigin, sphereCastOrigin + Vector3.down * sphereCastMaxDistance, Color.blue); // The sphere origin to sphere max DistanceMax.
        Debug.Log($"Is Grounded: {isGrounded}");
        */
    }
}