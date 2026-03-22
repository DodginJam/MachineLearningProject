using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.PackageManager;
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
    public Transform[] AllTargets
    { get; private set; }

    public List<Transform> VisableTargets
    { get; private set; } = new List<Transform>();

    public int InitialVisableTargets
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

    private BufferSensorComponent BufferSensorComp
    { get; set; }

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
        SetTargetsToNewSpot();

        // Remove all targets from the scene and clear visable targets..
        foreach(Transform target in AllTargets)
        {
            target.gameObject.SetActive(false);
        }
        VisableTargets.Clear();

        // Set a random amount of targets to be visable.
        int visableTargetsForEpisode = Random.Range(1, AllTargets.Length);
        for (int i = 0; i <= visableTargetsForEpisode; i++)
        {
            VisableTargets.Add(AllTargets[i]);
            VisableTargets[i].gameObject.SetActive(true);
        }

        InitialVisableTargets = VisableTargets.Count;
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

        // Debug.Log($"Max Number of Allowed Observations: {BufferSensorComp.MaxNumObservables}");
        // Debug.Log($"Visable Targers Count: {VisableTargets.Count}");

        // Adding observations into the buffer sensor.
        for (int i = 0; i < VisableTargets.Count; i++)
        {
            if (i > BufferSensorComp.MaxNumObservables)
            {
                Debug.LogWarning("Number of visable targets exceeded the max number of observables allowed by the buffer sesnsor. Stopping additional visable target observations.");
                break;
            }

            float[] observationArray = new float[BufferSensorComp.ObservableSize];

            // First 3 values as the normalised position of the target relative to the face of the agent.
            Vector3 relativeDir = TargetDetector.transform.InverseTransformPoint(VisableTargets[i].position).normalized;
            observationArray[0] = relativeDir.x;
            observationArray[1] = relativeDir.y;
            observationArray[2] = relativeDir.z;

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
        Rotator.SetAngle(Rotator.GetLocalAngleRotation() + (rotationOutput * Rotator.RotationSpeed * Time.fixedDeltaTime));
        Pitcher.SetAngle(Pitcher.GetLocalAngleRotation() + (pitchOutput * Pitcher.RotationSpeed * Time.fixedDeltaTime));

        // To detect the nearest target to the agent.
        Transform nearestTarget = null;
        float distanceToNearestTarget = float.MaxValue;
        foreach(Transform target in VisableTargets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < distanceToNearestTarget)
            {
                distanceToNearestTarget = distanceToTarget;
                nearestTarget = target;
            }
        }
        nearestTarget.GetComponent<MeshRenderer>().material.color = Color.red;

        // Small incentive to look at the nearest target.
        //float dot = Vector3.Dot(TargetDetector.transform.forward, (nearestTarget.position - transform.position).normalized);
        //if (dot > 0) AddReward(dot * 0.001f);
        
        // Rewarding if the nearest target has been detected.
        if (TargetDetector.IsTargetDetected(out Transform dectectedTransfom) && dectectedTransfom == nearestTarget)
        {
            AddReward(1.0f / InitialVisableTargets);

            // Removing detected targets from being active and removing them from the visable list.
            dectectedTransfom.gameObject.SetActive(false);
            bool wasVisableTargetRemoved = VisableTargets.Remove(dectectedTransfom);
            if (wasVisableTargetRemoved == false) Debug.LogError("The detected target did not exist on the visable targets list.");
            TargetDetector.RemoveDetectedInfo();
        }

        if (VisableTargets.Count <= 0)
        {
            EndEpisode();
        }
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetTargetsToNewSpot()
    {
        Bounds areaBounds = TrainingArea.bounds;
        float minX = areaBounds.center.x - areaBounds.extents.x;
        float maxX = areaBounds.center.x + areaBounds.extents.x;

        float minZ = areaBounds.center.z - areaBounds.extents.z;
        float maxZ = areaBounds.center.z + areaBounds.extents.z;

        foreach (Transform target in AllTargets)
        {
            Vector3 newPosition = Vector3.zero;

            target.GetComponent<MeshRenderer>().material.color = Color.yellow;

            do
            {
                float xPosition = Random.Range(minX, maxX);
                float zPosition = Random.Range(minZ, maxZ);

                float yPosition = TrainingArea.transform.position.y + Random.Range(0.5f, 2.5f);

                newPosition = new Vector3(xPosition, yPosition, zPosition);
            }
            while (Vector3.Distance(newPosition, this.transform.position) < 1f);

            target.position = newPosition;
        }
    }
}
