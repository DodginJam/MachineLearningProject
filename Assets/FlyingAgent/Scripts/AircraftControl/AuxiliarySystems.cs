using System;
using System.Collections.Generic;
using UnityEngine;

public class AuxiliarySystems : MonoBehaviour
{
    [field: SerializeField]
    public AircraftInput Input
    { get; private set; }

    [field: SerializeField]
    public BrakeSystem BrakingSystem
    { get; private set; }

    private void Awake()
    {
        if (Input == null)
        {
            if (TryGetComponent<AircraftInput>(out AircraftInput input))
            {
                Input = input;
            }
            else
            {
                Debug.LogWarning("Unable to locate the aircraft input on this transform.");
            }
        }
    }

    private void OnEnable()
    {
        if (Input != null && BrakingSystem != null)
        {
            Input.OnBreakInput += BrakingSystem.SetBreaksActiveStatus;
        }
    }

    private void OnDisable()
    {
        if (Input != null && BrakingSystem != null)
        {
            Input.OnBreakInput -= BrakingSystem.SetBreaksActiveStatus;
        }
    }
}

[Serializable]
public class BrakeSystem
{
    public bool AreBrakesActive
    { get; private set; }

    [field: SerializeField]
    public List<BreakData> Brakes
    { get; private set; }

    public void SetBreaksActiveStatus(bool newStatus)
    {
        AreBrakesActive = newStatus;

        foreach (BreakData breakData in Brakes)
        {
            breakData.SetBreakStatus(newStatus);
        }

        // Debug.Log($"Break set to active: {newStatus}");
    }
}

[Serializable]
public class BreakData
{
    [field: SerializeField]
    public string Name
    { get; private set; }

    [field: SerializeField]
    public Collider BreakSurface
    { get; private set; }

    [field: SerializeField]
    public PhysicsMaterial DefaultMat
    { get; private set; }

    [field: SerializeField]
    public PhysicsMaterial BrakingMat
    { get; private set; }
    public bool IsBreakActive
    { get; private set; }


    public void SetBreakStatus(bool activeStatus)
    {
        IsBreakActive = activeStatus;

        if (IsBreakActive && BrakingMat != null)
        {
            BreakSurface.material = BrakingMat;
        }
        else if (!IsBreakActive && DefaultMat != null)
        {
            BreakSurface.material = DefaultMat;
        }
    }
}
