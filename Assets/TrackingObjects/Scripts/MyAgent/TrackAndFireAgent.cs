using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TrackAndFireAgent : Agent
{
    /// <summary>
    /// The horizontal rotation point from the base.
    /// </summary>
    [field: SerializeField, Header("Control Points")]
    public RotationPoint Rotator
    { get; private set; }

    /// <summary>
    /// The vertical rotation point of the head.
    /// </summary>
    [field: SerializeField]
    public RotationPoint Pitcher
    { get; private set; }

    /// <summary>
    /// The controller for the weapon fire and fire detection.
    /// </summary>
    [field: SerializeField, Header("Firing Assistance Scripts")]
    public WeaponControl WeaponController
    { get; private set; }

    /// <summary>
    /// The script for global target detection, no radar required (performance friendly).
    /// </summary>
    [field: SerializeField]
    public GlobalDetector GlobalDetector 
    { get; private set; }

    /// <summary>
    /// The script for local target detection, radar required (non-performance friendly).
    /// </summary>
    [field: SerializeField]
    public RadarDetector RadarDetector
    { get; private set; }

    /// <summary>
    /// Current detection type being used.
    /// </summary>
    public Detector TargetDetector
    { get; private set; }

    [field: SerializeField, Header("Managers Targets in the Environment in Episodes")]
    public TargetManager TargetManager
    { get; private set; }

    private BufferSensorComponent BufferSensorComp
    { get; set; }

    [field: SerializeField, Header("Agent Control Values")]
    public float DamagePerTick
    { get; private set; } = 0.5f;

    [field: SerializeField]
    public DetectionMode DetectionMode
    { get; set; }

    protected override void Awake()
    {
        base.Awake();
        GlobalDetector.enabled = false;
        GlobalDetector.gameObject.SetActive(false);
        RadarDetector.enabled = false;
        RadarDetector.gameObject.SetActive(false);

        SetTargetDetectorType();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rotator.SetAngle(Random.Range(0f, 359.9999f));
        BufferSensorComp = GetComponent<BufferSensorComponent>();
    }

    /// <summary>
    /// Called at the beginning of an TrackingAgent's episode, including at the beginning of the simulation
    /// </summary>
    public override void OnEpisodeBegin()
    {
        TargetDetector.DetectedTargets.Clear();
        TargetManager.SetTargetsToNewSpot();
        TargetManager.ActivateTargets(UnityEngine.Random.Range(1, TargetManager.AllTargets.Length));
        TargetManager.SetTargetsSpeed();
    }

    /// <summary>
    /// Sets the type of target detector to be used via activating the selected the script / game object and deactivating the others.
    /// </summary>
    public void SetTargetDetectorType()
    {
        if (TargetDetector != null)
        {
            TargetDetector.enabled = false;
            TargetDetector.gameObject.SetActive(false);
            TargetDetector.DetectedTargets.Clear();
        }

        if (DetectionMode == DetectionMode.Global)
        {
            DetectionMode = DetectionMode.Global;
            TargetDetector = GlobalDetector;
        }
        else if (DetectionMode == DetectionMode.Radar)
        {
            DetectionMode = DetectionMode.Radar;
            TargetDetector = RadarDetector;
        }

        TargetDetector.enabled = true;
        TargetDetector.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called every step that the TrackingAgent requests a decision. This is one possible way for collecting the TrackingAgent's observations of the environment.
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Vector observations - continious.
        sensor.AddObservation(Rotator.GetNormalisedRotationValue()); // Index 0
        sensor.AddObservation(Pitcher.GetNormalisedRotationValue()); // Index 1

        int index = 0;
        // Adding observations into the buffer sensor.
        foreach (var target in TargetDetector.DetectedTargets)
        {
            if (index >= BufferSensorComp.MaxNumObservables)
            {
                Debug.LogWarning($"Index at {index} - Number of visable targets exceeded the max number of observables allowed by the buffer sesnsor. Stopping additional visable target observations.");
                break;
            }
            index++;
            float[] observationArray = new float[BufferSensorComp.ObservableSize];

            Vector3 localSpace = WeaponController.transform.InverseTransformPoint(target.Value.CurrentTargetPosition);

            // First 3 values as the position of the target relative to the face of the agent.
            Vector3 relativeDir = localSpace.normalized;
            observationArray[0] = relativeDir.x;
            observationArray[1] = relativeDir.y;
            observationArray[2] = relativeDir.z;

            // Magnitude / length set to a normalised value based on the max detection range.
            observationArray[3] = localSpace.magnitude / TargetDetector.DetectionDistance;

            // Dot product to represent how the agent is facing the target.
            float dot = Vector3.Dot(WeaponController.transform.forward, (target.Value.CurrentTargetPosition - WeaponController.transform.position).normalized);
            observationArray[4] = dot;

            BufferSensorComp.AppendObservation(observationArray);
        }
    }

    /// <summary>
    /// Called every time the TrackingAgent receives an action to take. Receives the action chosen by the TrackingAgent. It is also common to assign a reward in this method.
    /// </summary>
    /// <param name="actionBuffers"></param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Applying the input from continious actions.
        float rotationOutput = actionBuffers.ContinuousActions[0];
        float pitchOutput = actionBuffers.ContinuousActions[1];
        Rotator.RotateAngle(rotationOutput, Rotator.RotationSpeed, Time.fixedDeltaTime);
        Pitcher.RotateAngle(pitchOutput, Pitcher.RotationSpeed, Time.fixedDeltaTime);

        // Applying the input for discrete actions.
        int fireAction = actionBuffers.DiscreteActions[0];

        WeaponController.SetFiringMaterial(fireAction == 0 ? false : true);

        // If a target has been detected by the weapon controller...
        if (WeaponController.IsTargetDetected(out Target detectedTarget))
        {
            // And if the agent is current intending to fire, fire at the detected target.
            if (fireAction == 1)
            {
                WeaponController.FireAtTarget(detectedTarget, DamagePerTick);
            }

            // Bigger reward if the action resulted in the target death.
            if (detectedTarget.IsDead)
            {
                TargetDetector.RemoveTargetFromDictionary(detectedTarget.GetGameObjectsInstanceID());

                AddReward(1.0f);
                if (TargetManager.AreAllTargetsInactive())
                {
                    EndEpisode();
                    return;
                }
            }
        }
        else
        {
            // Punish blind firing for when a target in sight of the detector.
            if (fireAction == 1)
            {
                AddReward(-0.05f);
            }
        }

        // Reward based on the best dot product calculated - rewarding facing towards the targets closest to weapon face.
        if (TargetDetector.DetectedTargets.Count > 0)
        {
            float bestDotProduct = -1;
            Target targetBest = null;

            FindTargetWithHighestDotproduct(out bestDotProduct, out targetBest);

            if (targetBest != null && bestDotProduct > 0)
            {
                AddReward(0.01f * bestDotProduct * Time.fixedDeltaTime);
            }
            else if (bestDotProduct <= 0)
            {
                AddReward(-0.01f * Time.fixedDeltaTime);
            }

            // Reward shaping to face towards the target in the correct direction.
            Vector3 toTarget = (targetBest.transform.position - WeaponController.transform.position).normalized;
            Vector3 localDir = WeaponController.transform.InverseTransformDirection(toTarget);

            AddReward(0.002f * Mathf.Sign(localDir.x) * rotationOutput * Time.fixedDeltaTime);
        }

        // Time penelty.
        AddReward(-0.001f * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Provides the value dot product and the target object that the agents face is best facing.
    /// </summary>
    /// <param name="bestDotProductOut"></param>
    /// <param name="targetBestOut"></param>
    void FindTargetWithHighestDotproduct(out float bestDotProductOut, out Target targetBestOut)
    {
        float bestDotProduct = -1;
        Target targetBest = null;

        foreach (var target in TargetDetector.DetectedTargets)
        {
            float dotProductNew = Vector3.Dot(WeaponController.transform.forward, (target.Value.CurrentTargetPosition - WeaponController.transform.position).normalized);

            if (dotProductNew > bestDotProduct)
            {
                bestDotProduct = dotProductNew;
                targetBest = target.Value.TargetObject;
            }
        }

        bestDotProductOut = bestDotProduct;
        targetBestOut = targetBest;
    }

    /// <summary>
    /// When the Behavior Type is set to Heuristic Only in the Behavior Parameters of the TrackingAgent, the TrackingAgent will use the Heuristic() method to generate the actions of the TrackingAgent. As such, the Heuristic() method writes to the array of floats provided to the Heuristic method as argument. Note: Do not create a new float array of action in the Heuristic() method, as this will prevent writing floats to the original action array.
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
