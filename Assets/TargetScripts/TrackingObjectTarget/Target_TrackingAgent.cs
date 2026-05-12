using UnityEngine;
using ProjectEnums;

public class Target_TrackingAgent : Target, IKillable, IMoveable, IMaterialChanger
{
    public float StartingHealth 
    { get; set; } = 1.0f;

    public float CurrentHealth
    { get; set; }

    public bool IsDead
    { get; set; }

    [field: SerializeField]
    public float MovementSpeed
    { get; set; } = 1.0f;

    public Vector3 EndPoint
    { get; set; }

    [field: SerializeField]
    public MeshRenderer MeshRendererRef
    { get; set; }

    [field: SerializeField]
    public Material FriendlyMaterial
    { get; set; }

    [field: SerializeField]
    public Material EnemyMaterial
    { get; set; }

    public override void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Movement();
    }

    public void SetMaterial()
    {
        MeshRendererRef.material = this.TargetTyping == TargetType.Friendly ? FriendlyMaterial : EnemyMaterial;
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        IsDead = true;
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth = Mathf.Clamp01(CurrentHealth - damageToTake);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void Revive()
    {
        Initialise();
    }

    public void Movement()
    {
        float distanceToMove = Time.fixedDeltaTime * MovementSpeed;

        Vector3 newPosition = Vector3.MoveTowards(transform.position, EndPoint, distanceToMove);
        transform.position = newPosition;

        if (Vector3.Distance(transform.position, EndPoint) <= 0.01f)
        {
            TargetManager.GetBoundsLimits(TargetManager.TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);
            SetEndPoint(TargetManager.GetRandomPointInArea(minX, maxX, minZ, maxZ));
        }
    }

    public void SetMovementMultiplier(float newMovementMultiplier)
    {
        MovementSpeed = newMovementMultiplier;
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        EndPoint = endPoint;
    }
}
