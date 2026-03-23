using UnityEngine;

public class Target : MonoBehaviour
{
    private const float StartingHealth = 1.0f;

    public float CurrentHealth
    { get; private set; }

    public bool IsDead
    { get; private set; }

    private void Awake()
    {
        
    }

    public void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;
    }

    public void Die()
    {
        this.gameObject.SetActive(false);
        IsDead = true;
    }

    public void TakeDamage(float damageToTake)
    {
        CurrentHealth = Mathf.Clamp01(CurrentHealth - damageToTake);
        Debug.Log($"Current Health: {CurrentHealth}");

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
}
