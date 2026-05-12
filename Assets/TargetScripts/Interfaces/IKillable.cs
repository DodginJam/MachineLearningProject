using UnityEngine;

public interface IKillable
{
    public float StartingHealth 
    { set; get; }

    public float CurrentHealth
    { get; set; }

    public bool IsDead
    { get; set; }

    public void Die();

    public void TakeDamage(float damageToTake);

    public void Revive();
}
