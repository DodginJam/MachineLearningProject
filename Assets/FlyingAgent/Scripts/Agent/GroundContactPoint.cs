using UnityEngine;

public class GroundContactPoint : MonoBehaviour
{
    public FlyingAgent AgentOwner
    { get; private set; }

    public bool InContactWithSurface
    { get; set; }

    public CapsuleCollider Collider
    { get; private set; }

    [field: SerializeField]
    public float CastDistance
    { get; private set; } = 0.3f;

    private void Awake()
    {
        Collider = GetComponent<CapsuleCollider>();
        AgentOwner = GetComponentInParent<FlyingAgent>();
    }

    private void FixedUpdate()
    {
        Vector3 centerWorld = transform.TransformPoint(Collider.center);
        Vector3 up = transform.up;

        bool foundHit = Physics.Raycast(centerWorld, -up, out RaycastHit hit, CastDistance);

        if (foundHit)
        {
            // Don't allow raycast hit with with own self.
            if (hit.rigidbody != null)
            {
                if (hit.rigidbody.gameObject != AgentOwner.gameObject)
                {
                    InContactWithSurface = true;
                }
            }
            else
            {
                InContactWithSurface = true;

            }
        }
        else
        {
            InContactWithSurface = false;
        }

        Debug.DrawRay(centerWorld, -up * CastDistance, InContactWithSurface == true ? Color.blue : Color.red);
    }
}
