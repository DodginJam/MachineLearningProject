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
        if ((collision.impulse * LastFixedDeltaTimeAmount).sqrMagnitude >= DestroyForce)
        {
            Impact();
        }
        else if ((LayerMaskForFailure.value & (1 << collision.gameObject.layer)) != 0)
        {
            Impact();
        }
    }

    public void Impact()
    {
        Agent.OnAgentCrash();
    }
}
