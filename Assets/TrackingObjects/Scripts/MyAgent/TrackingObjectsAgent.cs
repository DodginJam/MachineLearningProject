using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrackingObjectsAgent : Agent
{
    [field: SerializeField]
    public RotationPoint Rotator
    { get; private set; }

    [field: SerializeField]
    public RotationPoint Pitcher
    { get; private set; }

    [field: SerializeField]
    public Transform Target
    { get; private set; }

    [field: SerializeField]
    public DetectFacingTarget TargetDetector
    { get; private set; }

    /// <summary>
    /// The training area mesh renderer can be used to access the bounds to ensure the target is spawned at a random point on the surface.
    /// </summary>
    [field: SerializeField]
    public MeshRenderer TrainingArea
    { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rotator.SetAngle(Random.Range(0f, 359.9999f));
        // Pitcher.SetAngle(Random.Range(Pitcher.AngleLimitLower, Pitcher.AngleLimitUpper));
    }

    /// <summary>
    /// Called at the beginning of an Agent's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {
        SetTargetToNewSpot();
    }

    /// <summary>
    /// Called every step that the Agent requests a decision. This is one possible way for collecting the Agent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Rotator.GetLocalAngleRotation()); // Index 0
        sensor.AddObservation(Pitcher.GetLocalAngleRotation()); // Index 1
        sensor.AddObservation(Target.transform.localPosition - this.transform.localPosition); // Index 2, 3, 4
    }

    /// <summary>
    /// Called every time the Agent receives an action to take. Receives the action chosen by the Agent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float rotationalAmountToMove = actionBuffers.ContinuousActions[0];
        float pitchAmountToMove = actionBuffers.ContinuousActions[1];

        Rotator.SetAngle(Rotator.GetLocalAngleRotation() + (rotationalAmountToMove * Rotator.RotationSpeed));
        Pitcher.SetAngle(Pitcher.GetLocalAngleRotation() + (pitchAmountToMove * Pitcher.RotationSpeed));

        if (TargetDetector.TargetDetected)
        {
            TargetDetector.TargetDetected = false;
            AddReward(1.0f);
            EndEpisode();
        }
    }

    /// <summary>
    /// When the Behavior Type is set to Heuristic Only in the Behavior Parameters of the Agent, the Agent will use the Heuristic() method to generate the actions of the Agent. As such, the Heuristic() method writes to the array of floats provided to the Heuristic method as argument. Note: Do not create a new float array of action in the Heuristic() method, as this will prevent writing floats to the original action array.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetTargetToNewSpot()
    {
        Bounds areaBounds = TrainingArea.bounds;
        float minX = areaBounds.center.x - areaBounds.extents.x;
        float maxX = areaBounds.center.x + areaBounds.extents.x;

        float minZ = areaBounds.center.z - areaBounds.extents.z;
        float maxZ = areaBounds.center.z + areaBounds.extents.z;

        float xPosition = Random.Range(minX, maxX);
        float zPosition = Random.Range(minZ, maxZ);

        float yPosition = Target.transform.position.y + Random.Range(0, 2.5f);

        Vector3 newPosition = new Vector3(xPosition, yPosition, zPosition);

        Target.transform.position = newPosition;
    }
}
