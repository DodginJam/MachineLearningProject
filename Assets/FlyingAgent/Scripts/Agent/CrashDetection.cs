using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CrashDetection : MonoBehaviour
{
    /// <summary>
    /// Holds the last time taken in the last FixedUpdate frame.
    /// </summary>
    public static float LastFixedDeltaTimeAmount
    { get; private set; }

    [field: SerializeField]
    public float DestroyForce
    { get; private set; }

    [field: SerializeField]
    public FlyingAgent Agent
    { get; private set; }

    [field: SerializeField]
    public LayerMask LayerMaskForFailure
    { get; private set; }

    private void FixedUpdate()
    {
        if (LastFixedDeltaTimeAmount != Time.fixedDeltaTime)
        {
            LastFixedDeltaTimeAmount = Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 force = collision.impulse / LastFixedDeltaTimeAmount;
        Debug.Log($"Force Of Collision: {force.sqrMagnitude} needs to be greater then: {DestroyForce * DestroyForce}");

        if (force.sqrMagnitude >= DestroyForce * DestroyForce)
        {
            Debug.Log("Impact Should be called");
            Impact();
        }
        else if ((LayerMaskForFailure.value & (1 << collision.gameObject.layer)) != 0)
        {
            Impact();
        }
    }

    public void Impact()
    {
        Debug.Log("Impact Called! Agent Should crash.");
        Agent.OnAgentCrash();
    }
}
