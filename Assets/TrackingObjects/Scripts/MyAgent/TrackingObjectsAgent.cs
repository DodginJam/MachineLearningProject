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
        sensor.AddObservation(Rotator.GetNormalisedRotationValue()); // Index 0
        sensor.AddObservation(Pitcher.GetNormalisedRotationValue()); // Index 1

        // Where the target is relative to its current facing direction
        Vector3 relativeDir = TargetDetector.transform.InverseTransformPoint(Target.position).normalized;
        sensor.AddObservation(relativeDir); // Index 2, 3, 4
    }

    /// <summary>
    /// Called every time the Agent receives an action to take. Receives the action chosen by the Agent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float rotationOutput = actionBuffers.ContinuousActions[0];
        float pitchOutput = actionBuffers.ContinuousActions[1];

        Rotator.SetAngle(Rotator.GetLocalAngleRotation() + (rotationOutput * Rotator.RotationSpeed * Time.fixedDeltaTime));
        Pitcher.SetAngle(Pitcher.GetLocalAngleRotation() + (pitchOutput * Pitcher.RotationSpeed * Time.fixedDeltaTime));

        /*
        // Small incentive to look at the target.
        float dot = Vector3.Dot(TargetDetector.transform.forward, (Target.position - transform.position).normalized);
        if (dot > 0) AddReward(dot * 0.001f);
        */

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

        Debug.Log(Vector3.Dot(TargetDetector.transform.forward, (Target.position - transform.position).normalized));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Away from centre allows for deadspace where the agent is to not allow the target to be placed there.
    /// </summary>
    /// <param name="awayFromCentre"></param>
    void SetTargetToNewSpot()
    {
        Bounds areaBounds = TrainingArea.bounds;
        float minX = areaBounds.center.x - areaBounds.extents.x;
        float maxX = areaBounds.center.x + areaBounds.extents.x;

        float minZ = areaBounds.center.z - areaBounds.extents.z;
        float maxZ = areaBounds.center.z + areaBounds.extents.z;

        Vector3 newPosition = Vector3.zero;

        do
        {
            float xPosition = Random.Range(minX, maxX);
            float zPosition = Random.Range(minZ, maxZ);

            float yPosition = TrainingArea.transform.position.y + Random.Range(0.5f, 2.5f);

            newPosition = new Vector3(xPosition, yPosition, zPosition);
        }
        while (Vector3.Distance(newPosition, transform.position) < 1f);

        Target.transform.position = newPosition;
    }
}
