using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AircraftAgent : FlyingAgent
{
    [field: SerializeField]
    public GroundDetection_Aircraft GroundDetection
    { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Called at the beginning of an Agents's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Called every step that the FlyingAgent requests a decision. This is one possible way for collecting the FlyingAgent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void CollectSensorObservations(VectorSensor sensor)
    {
        base.CollectSensorObservations(sensor);

        // Are wheels on ground.
        sensor.AddObservation(IsGrounded()); // 11
    }

    /// <summary>
    /// Called every time the FlyingAgent receives an action to take. Receives the action chosen by the FlyingAgent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        base.OnActionReceived(actionBuffers);
    }

    bool IsGrounded()
    {
        if (GroundDetection == null)
        {
            Debug.Log("Error: Ground Detection is not assigned.");
            return false;
        }

        return GroundDetection.IsGrounded();
    }

    protected override void PrintDebugStatements()
    {
        base.PrintDebugStatements();
    }
}
