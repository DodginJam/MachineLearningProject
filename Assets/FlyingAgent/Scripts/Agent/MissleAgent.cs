using UnityEngine;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MissleAgent : FlyingAgent
{
    [field: SerializeField]
    public Target LockedOnTarget
    { get; set; }

    [field: SerializeField]
    public float StartingForwardVelocity
    { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        base.Initialize();

        Controller.PlaneRigidBody.linearVelocity = new Vector3(0, 0, StartingForwardVelocity);
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

    /// <summary>
    /// Called every time the FlyingAgent receives an action to take. Receives the action chosen by the FlyingAgent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        base.OnActionReceived(actionBuffers);
    }
}
