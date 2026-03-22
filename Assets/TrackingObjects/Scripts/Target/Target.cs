using UnityEngine;

public class Target : MonoBehaviour
{
    const float StartingHealth = 1.0f;

    public float CurrentHealth
    { get; private set; }

    void Initialise()
    {
        CurrentHealth = StartingHealth;
    }
}
