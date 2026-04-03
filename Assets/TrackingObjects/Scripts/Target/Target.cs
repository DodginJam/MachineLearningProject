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

    private void Awake()
    {
        Initialise();
    }

    public void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;

        TargetTyping = UnityEngine.Random.Range(0, 2) == 0 ? TargetType.Enemy : TargetType.Friendly;
        SetMaterial = TargetTyping == TargetType.Friendly ? FriendlyMaterial : EnemyMaterial;
        MeshRendererRef.material = SetMaterial;
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        IsDead = true;
    }

    public void Revive()
    {
        this.gameObject.SetActive(true);
        IsDead = false;
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth = Mathf.Clamp01(CurrentHealth - damageToTake);

        if (CurrentHealth <= 0)
        {
            Die(); 
        }
    }
}
