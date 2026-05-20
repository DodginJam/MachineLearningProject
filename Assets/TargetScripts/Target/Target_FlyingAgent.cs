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

    [field: SerializeField]
    public LayerMask TriggerMasks
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

        SetMovementSpeed(EnvironmentParametersFlyingAgent.Instance.GetMovementSpeed());
        SetSize(EnvironmentParametersFlyingAgent.Instance.GetTargetsScale());
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
        return TargetManager.GetPositionWithinArea();
    }

    public void SetMovementSpeed(float newMovementMultiplier)
    {
        MovementSpeed = newMovementMultiplier;
    }

    public void SetPositionToMoveTo(Vector3 endPoint)
    {
        MoveToPosition = endPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((TriggerMasks.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.attachedRigidbody != null && other.attachedRigidbody.transform.TryGetComponent<FlyingAgent>(out FlyingAgent flyingAgent))
            {
                Debug.Log("Success trigger plane to target.");
                flyingAgent.AddReward(1.0f);
                transform.position = GetPositionInArea();
                SetPositionToMoveTo(GetPositionInArea());

                Revive();
            }
        }
    }
}
