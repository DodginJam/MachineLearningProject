using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(SphereCollider))]
public class RollerAgent : Agent
{
    /// <summary>
    /// The physics component of the roller agent.
    /// </summary>
    public Rigidbody Rigidbody
    { get; set; }

    public SphereCollider SphereCollider
    { get; set; }

    /// <summary>
    /// The starting position of the roller agent upon start - stores default starting position agent to be reset to on episode start if fell.
    /// </summary>
    public Vector3 StartingPosition
    { get; private set; }

    /// <summary>
    /// The target that the roller agent is to locate and find.
    /// </summary>
    [field: SerializeField, Header("Roller Agent Properties")]
    public Transform Target
    { get; set; }

    /// <summary>
    /// The training area mesh renderer can be used to access the bounds to ensure the target is spawned at a random point on the surface.
    /// </summary>
    [field: SerializeField]
    public MeshRenderer TrainingArea
    { get; private set; }

    /// <summary>
    /// Get a random range from between 1 and Force Multiplier Max Range
    /// </summary>
    public float ForceMultiplier
    { get; private set; }

    [field: SerializeField, Range(0, 100)]
    public float ForceMultiplierMaxRange
    { get; private set; } = 100f;

    public float DistanceToTarget
    { 
        get { return Vector3.Distance(this.transform.localPosition, Target.localPosition); }
    }

    public float LastStepDistanceToTarget
    { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Target == null)
        {
            Debug.LogError("The Target has not been assigned");
        }

        Rigidbody = GetComponent<Rigidbody>();
        SphereCollider = GetComponent<SphereCollider>();
        StartingPosition = transform.position;
    }

    /// <summary>
    /// Called at the beginning of an Agent's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (HasAgentFallen())
        {
            ResetAgent();
        }

        ForceMultiplier = Academy.Instance.EnvironmentParameters.GetWithDefault("force_multiplier", Random.Range(10, 50));
        Debug.Log($"ForceMode: {ForceMultiplier}");

        SetTargetToNewSpot();
    }

    /// <summary>
    /// Called every step that the Agent requests a decision. This is one possible way for collecting the Agent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // 3 values of data
        sensor.AddObservation(Target.localPosition); // Index of 0, 1, 2 correspond to x, y, z
        // 3 values of data
        sensor.AddObservation(this.transform.localPosition); // Index of 3, 4, 5 correspond to x, y, z
        // 1 value of data
        sensor.AddObservation(Rigidbody.linearVelocity.x); // Index of 6
        // 1 value of data
        sensor.AddObservation(Rigidbody.linearVelocity.z); // Index of 7

        // 8 values in total
    }

    /// <summary>
    /// Called every time the Agent receives an action to take. Receives the action chosen by the Agent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        Rigidbody.AddForce(controlSignal * ForceMultiplier);

        if (DistanceToTarget < SphereCollider.radius + 0.1f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        else if (HasAgentFallen())
        {
            AddReward(-1.0f);
            EndEpisode();
        }
        else if (DistanceToTarget < LastStepDistanceToTarget)
        {
            AddReward(0.0005f);
        }
        else if (DistanceToTarget > LastStepDistanceToTarget)
        {
            AddReward(-0.001f);
        }

        LastStepDistanceToTarget = DistanceToTarget;
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

    private void Update()
    {
        
    }

    bool HasAgentFallen()
    {
        return this.transform.localPosition.y < TrainingArea.transform.localPosition.y;
    }

    void ResetAgent()
    {
        transform.position = StartingPosition;
        Rigidbody.linearVelocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
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

        float yPosition = Target.transform.position.y;

        Ray upRay = new Ray(new Vector3(xPosition, areaBounds.min.y - 5f, zPosition), Vector3.up);
        Ray downRay = new Ray(new Vector3(xPosition, areaBounds.max.y + 5f, zPosition), Vector3.down);

        if (Physics.Raycast(upRay, out RaycastHit upHitInfo))
        {
            yPosition = upHitInfo.point.y;
        }
        else if (Physics.Raycast(downRay, out RaycastHit downHitInfo))
        {
            yPosition = downHitInfo.point.y;
        }

        Vector3 newPosition = new Vector3(xPosition, yPosition + 0.5f, zPosition);

        Target.transform.position = newPosition;
    }
}
