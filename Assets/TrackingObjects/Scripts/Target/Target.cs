using UnityEngine;

public class Target : MonoBehaviour, ITarget
{
    private const float StartingHealth = 1.0f;

    public float CurrentHealth
    { get; private set; }

    public bool IsDead
    { get; private set; }

    [field: SerializeField]
    public MeshRenderer MeshRendererRef
    { get; private set; }

    public Material SetMaterial
    { get; private set; }

    [field: SerializeField]
    public Material FriendlyMaterial
    { get; private set; }

    [field: SerializeField]
    public Material EnemyMaterial
    { get; private set; }

    public TargetType TargetTyping
    { get; private set; }



    [field: SerializeField]
    public float MovementMultiplier
    { get; private set; } = 1.0f;

    public float MovementTime
    { get; private set; } = 0;

    public Vector3 StartPoint
    { get; private set; }

    public Vector3 EndPoint
    { get; private set; }

    public TargetManager TargetManager
    { get; private set; }

    private void Awake()
    {
        TargetManager = transform.parent.GetComponentInChildren<TargetManager>();

        Initialise();
    }

    public void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;

        StartPoint = this.transform.position;
    }

    public void SetSize(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetTargetTyping(TargetType type)
    {
        TargetTyping = type;
        SetMaterial = TargetTyping == TargetType.Friendly ? FriendlyMaterial : EnemyMaterial;
        MeshRendererRef.material = SetMaterial;

        // TargetTyping = UnityEngine.Random.Range(0, 2) == 0 ? TargetType.Enemy : TargetType.Friendly;
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        IsDead = true;
    }

    public void Revive()
    {
        Initialise();
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth = Mathf.Clamp01(CurrentHealth - damageToTake);

        if (CurrentHealth <= 0)
        {
            Die(); 
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public int GetGameObjectsInstanceID()
    {
        return this.gameObject.GetInstanceID();
    }

    public void SetMovementMultiplier(float newMovementMultiplier)
    {
        MovementMultiplier = newMovementMultiplier;
    }

    public void Movement()
    {
        MovementTime += Time.fixedDeltaTime * MovementMultiplier;

        Vector3 newPosition = Vector3.Lerp(StartPoint, EndPoint, MovementTime);
        transform.position = newPosition;

        if (MovementTime >= 1)
        {
            StartPoint = this.transform.position;

            TargetManager.GetBoundsLimits(TargetManager.TrainingArea, out float minX, out float maxX, out float minZ, out float maxZ);
            SetEndPoint(TargetManager.GetRandomPointInArea(minX, maxX, minZ, maxZ));

            MovementTime = 0;
        }
    }

    public void SetEndPoint(Vector3 endPoint)
    {
        EndPoint = endPoint;
    }
}
