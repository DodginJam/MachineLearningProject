using AircraftData;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class FlyingAgent : Agent
{
    [field: SerializeField]
    public int MaxStepsDuringTraining
    { get; private set; } = 10000;

    [field: SerializeField]
    public AircraftInput AircraftInputScript
    { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initialize()
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            this.MaxStep = 0;
            Debug.Log($"Max Steps set to {this.MaxStep}");
        }
        else
        {
            this.MaxStep = MaxStepsDuringTraining;
            Debug.Log($"Max Steps set to {this.MaxStep}");
        }
    }

    /// <summary>
    /// Called at the beginning of an Agents's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {
        AircraftInputScript.ResetInputs();
    }

    /// <summary>
    /// Called every step that the FlyingAgent requests a decision. This is one possible way for collecting the FlyingAgent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    /// <summary>
    /// Called every time the FlyingAgent receives an action to take. Receives the action chosen by the FlyingAgent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Applying the input from continious actions.
        PollingInput inputData = new PollingInput();
        inputData.ThrottleInput = actionBuffers.ContinuousActions[0];
        inputData.ElevatorInput = actionBuffers.ContinuousActions[1];
        inputData.AileronInput = actionBuffers.ContinuousActions[2];
        inputData.RudderInput = actionBuffers.ContinuousActions[3];

        // Apply the inputs to the aircraft scripts.
        AircraftInputScript.OverridePlayerInput(inputData);
    }

    /// <summary>
    /// When the Behavior Type is set to Heuristic Only in the Behavior Parameters of the Agent, the Agent will use the Heuristic() method to generate the actions of the Agent. As such, the Heuristic() method writes to the array of floats provided to the Heuristic method as argument. Note: Do not create a new float array of action in the Heuristic() method, as this will prevent writing floats to the original action array.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        PollingInput playerInput = AircraftInputScript.ReturnPollingInput();

        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = playerInput.ThrottleInput;
        continuousActionsOut[1] = playerInput.ElevatorInput;
        continuousActionsOut[2] = playerInput.AileronInput;
        continuousActionsOut[3] = playerInput.RudderInput;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
    }
}
