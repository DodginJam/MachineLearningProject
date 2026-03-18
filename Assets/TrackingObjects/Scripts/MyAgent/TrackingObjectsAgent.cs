using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TrackingObjectsAgent : Agent
{
    [field: SerializeField]
    public RotationPoint Rotator
    { get; private set; }

    [field: SerializeField]
    public RotationPoint Pitcher
    { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    /// <summary>
    /// Called at the beginning of an Agent's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {

    }

    /// <summary>
    /// Called every step that the Agent requests a decision. This is one possible way for collecting the Agent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    /// <summary>
    /// Called every time the Agent receives an action to take. Receives the action chosen by the Agent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {

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
        Rotator.SetAngle();
        Pitcher.SetAngle();
    }
}
