using UnityEngine;
using ProjectEnums;
using System;

public class Target_FlyingAgent : Target, IKillable, IMoveable
{
    public TargetManager_FlyingAgent TargetManager
    { get; private set; }

    public float StartingHealth 
    { get; set; } = 1.0f;

    public float CurrentHealth
    { get; set; }

    public bool IsDead
    { get; set; }

    [field: SerializeField]
    public float MovementSpeed
    { get; set; } = 1.0f;

    public Vector3 MoveToPosition
    { get; set; }

    protected override void Awake()
    {
        base.Awake();

        TargetManager = transform.parent.GetComponentInChildren<TargetManager_FlyingAgent>();
    }

    public override void Initialise()
    {
        CurrentHealth = StartingHealth;
        gameObject.SetActive(true);
        IsDead = false;
    }

    protected virtual void FixedUpdate()
    {
        Movement();
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

        Vector3 newPosition = Vector3.MoveTowards(transform.position, MoveToPosition, distanceToMove);
        transform.position = newPosition;

        if (Vector3.Distance(transform.position, MoveToPosition) <= 0.01f)
        {
            Vector3 positionToMoveTo = GetPositionInArea();
            SetPositionToMoveTo(positionToMoveTo);
        }
    }

    public Vector3 GetPositionInArea()
    {
        return default(Vector3);
    }

    public void SetMovementSpeed(float newMovementMultiplier)
    {
        MovementSpeed = newMovementMultiplier;
    }

    public void SetPositionToMoveTo(Vector3 endPoint)
    {
        MoveToPosition = endPoint;
    }
}
