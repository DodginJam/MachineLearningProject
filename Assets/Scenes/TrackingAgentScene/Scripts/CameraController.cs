using UnityEngine;

[DefaultExecutionOrder(50)]
public class CameraController : MonoBehaviour
{
    public Camera AttachedCamera
    { get; private set; }

    [field: SerializeField]
    public Transform TransformToFollow
    { get; private set; }

    /// <summary>
    /// Keeps tracks of the current pitch angle of the gameobject the camera is to match the rotation of.
    /// </summary>
    public float CameraPitch
    { get; private set; }

    /// <summary>
    /// Whether the camera is a first person or third person camera.
    /// </summary>
    public CameraPositionState CameraPosition
    { get; private set; }

    /// <summary>
    /// Reference to the player controller and the player input system contained within.
    /// </summary>
    [field: SerializeField]

    public PlayerController PlayerControllerOwner
    { get; set; }

    private void Awake()
    {
        if (TryGetComponent<Camera>(out Camera camera))
        {
            AttachedCamera = camera;
        }

        InitialiseCameraController(TransformToFollow, PlayerControllerOwner);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (CameraPosition == CameraPositionState.FirstPerson && TransformToFollow != null)
        {
            // Update the pitch of the camera holder object before...
            UpdateCameraHolderPitch(TransformToFollow, CameraPosition);
            /// ... setting the cameras position and rotation to mirror the camera holder.
            transform.SetPositionAndRotation(TransformToFollow.position, TransformToFollow.rotation);
        }
    }

    public void InitialiseCameraController(Transform transformForCameraToFollow, PlayerController playerController)
    {
        PlayerControllerOwner = playerController;
        TransformToFollow = transformForCameraToFollow;

        Debug.Log("Player Camera initialised");
    }

    public void UpdateCameraHolderPitch(Transform cameraHolder, CameraPositionState cameraPosition)
    {
        if (cameraPosition == CameraPositionState.FirstPerson)
        {
            if (PlayerControllerOwner != null && PlayerControllerOwner.InputHandler != null)
            {
                CameraPitch -= PlayerControllerOwner.InputHandler.RotationInput.y * Time.deltaTime * PlayerControllerOwner.RotationSpeed;
                CameraPitch = Mathf.Clamp(CameraPitch, -85, 85);

                cameraHolder.transform.localRotation = Quaternion.Euler(CameraPitch, cameraHolder.transform.localRotation.y, cameraHolder.transform.localRotation.z);
            }
            else
            {
                Debug.LogError("CharacterControllerComp is null or player input handler is null");
            }
        }
    }

    /// <summary>
    /// The camera state affecting it's position and how it is rotated and controlled.
    /// </summary>
    public enum CameraPositionState
    {
        FirstPerson,
        ThirdPerson
    }
}