using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private const float StartingHealth = 1.0f;

    public float CurrentHealth
    { get; private set; }

    public bool IsDead
    { get; private set; }

    [field: SerializeField]
    public MeshRenderer MeshRendererRef
    { get; private set; }

    [field: SerializeField]
    public Material DefaultMaterial
    { get; private set; }

    [field: SerializeField]
    public Material ShotAtMaterial
    { get; private set; }

    private Coroutine MaterialChange
    { get; set; }

    private void Awake()
    {
        
    }

    public void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;
        MeshRendererRef.material = DefaultMaterial;
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        IsDead = true;
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth = Mathf.Clamp01(CurrentHealth - damageToTake);
        
        if (MaterialChange != null)
        {
            StopCoroutine(MaterialChange);
            MaterialChange = null;
        }

        MaterialChange = StartCoroutine(SetMaterialForFrame());

        if (CurrentHealth <= 0)
        {
            Die(); 
        }
    }

    IEnumerator SetMaterialForFrame()
    {
        MeshRendererRef.material = ShotAtMaterial;

        yield return new WaitForSeconds(Time.fixedDeltaTime);

        MeshRendererRef.material = DefaultMaterial;
    }
}
