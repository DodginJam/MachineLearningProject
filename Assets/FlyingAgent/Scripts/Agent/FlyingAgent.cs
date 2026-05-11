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

    public Vector3 StartingPosition
    { get; private set; }

    [field: SerializeField]
    public AircraftController Controller
    { get; private set; }

    [field: SerializeField]
    public GroundDetection_Aircraft GroundDetection
    { get; private set; }

    private float TimeAlive
    { get; set; }

    protected override void Awake()
    {
        base.Awake();
        StartingPosition = transform.position;
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

        Controller.PlaneRigidBody.position = StartingPosition;
        Controller.PlaneRigidBody.rotation = Quaternion.identity;
        Controller.ResetPlane(new Vector3(0, 0, EnvironmentParametersFlyingAgent.Instance.GetStartingVelocity()), EnvironmentParametersFlyingAgent.Instance.GetThrottleValue());

        TimeAlive = 0;
    }

    public void Update()
    {
        TimeAlive += Time.deltaTime;
    }

    /// <summary>
    /// Called every step that the FlyingAgent requests a decision. This is one possible way for collecting the FlyingAgent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        float maxExpectedVelocity = 100;

        // LinearVelocity
        sensor.AddObservation(Controller.CurrentValues.ValuesHolder.CurrentVelocityLocal / maxExpectedVelocity); // 1, 2, and 3

        float maxExpectedAngularVelocity = 5;

        // AngularVelocity
        sensor.AddObservation(Controller.CurrentValues.ValuesHolder.CurrentAngularVelocityLocal / maxExpectedAngularVelocity); // 4, 5 and 6

        // AngleOfAttack
        sensor.AddObservation(Mathf.Clamp(Controller.CurrentValues.ValuesHolder.AngleOfAttack / 45f, -1f, 1f)); // 7

        // AngleOfAttackYaw
        sensor.AddObservation(Mathf.Clamp(Controller.CurrentValues.ValuesHolder.AngleOfAttackYaw / 45f, -1f, 1f)); // 8

        // ThrottleValue - normalised.
        sensor.AddObservation(Controller.CurrentValues.FlightControls.ThrottleValue); // 9

        // Level Flight.
        sensor.AddObservation(Controller.CurrentValues.ValuesHolder.LevelOfFlight); // 10

        // Are wheels on ground.
        sensor.AddObservation(IsGrounded()); // 11
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

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Debug.Log($"ThrottleValue: {Controller.CurrentValues.FlightControls.ThrottleValue}", this.gameObject);
            Debug.Log($"LevelOfFlight: {Controller.CurrentValues.ValuesHolder.LevelOfFlight}", this.gameObject);
            Debug.Log($"Is Grounded: {GroundDetection.IsGrounded()}");
        }
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

        /*
        Debug.Log($"ThrottleInput {inputData.ThrottleInput}");
        Debug.Log($"ElevatorInput {inputData.ElevatorInput}");
        Debug.Log($"AileronInput {inputData.AileronInput}");
        Debug.Log($"RudderInput {inputData.RudderInput}");

        Debug.Log($"ThrottleValue {Controller.CurrentValues.FlightControls.ThrottleValue}");
        */

        float baseReward = 0.0001f;

        // Award for flying straight and level.
        if (Controller.CurrentValues.ValuesHolder.LevelOfFlight > 0)
        {
            float levelOfFlightReward = baseReward * Controller.CurrentValues.ValuesHolder.LevelOfFlight;
            AddReward(levelOfFlightReward);
        }

        // Award for time alive.
        float timeAliveForMaxReward = 60;
        float timeAliveBonus = Mathf.Clamp(TimeAlive / timeAliveForMaxReward, 0, 1);
        float timeAliveReward = baseReward * timeAliveBonus;
        AddReward(timeAliveReward);

        // Award for maintaining high airspeed.
        float maxSpeedForNormalisation = 50f;
        float speedReward = baseReward * (Controller.CurrentValues.ValuesHolder.AirSpeed / maxSpeedForNormalisation);
        AddReward(speedReward);

        // Award for having low angle of attack.
        float angleOfAttack = Mathf.Abs(Controller.CurrentValues.ValuesHolder.AngleOfAttack);
        if (angleOfAttack > 20f) AddReward(-baseReward);

        if (CheckEndEpisodeAfterStepCount())
        {
            return;
        }
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

    public void OnAgentCrash()
    {
        AddReward(-1.0f);
        EndEpisode();
    }

    bool CheckEndEpisodeAfterStepCount()
    {
        bool shouldEpisodeEnd = false;

        if (Academy.Instance.IsCommunicatorOn && StepCount >= MaxStepsDuringTraining)
        {
            Debug.Log($"Max Steps hit, ending episode");
            EndEpisode();
            shouldEpisodeEnd = true;
        }

        return shouldEpisodeEnd;
    }
}
