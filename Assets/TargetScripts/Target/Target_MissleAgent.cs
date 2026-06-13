using UnityEngine;
using ProjectEnums;
using System;

public class Target_MissleAgent : Target_FlyingAgent
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        
    }

    protected override void OnTriggerEnter(Collider other)
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
