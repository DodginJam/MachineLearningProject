using System;
using Unity.AppUI.UI;
using UnityEngine;

[Serializable]
public class RotationPoint
{
    [field: SerializeField]
    public Transform ObjectToAngle
    { get; private set; }

    [field: SerializeField]
    public bool LimitRotation
    { get; private set; }

    [field: SerializeField]
    public float AngleLimitLower
    { get; private set; } = -10f;

    [field: SerializeField]
    public float AngleLimitUpper
    { get; private set; } = 10f;

    [field: SerializeField]
    public RotationAxis RotateAround
    { get; private set; }

    [field: SerializeField]
    public float RotationSpeed
    { get; private set; }

    private float currentAngle;

    public enum RotationAxis
    {
        X, Y, Z
    }

    public void SetAngle(float newAngle)
    {
        if (!LimitRotation)
        {
            newAngle %= 360f;
            if (newAngle < 0) newAngle += 360f;
        }
        else
        {
            newAngle = Mathf.Clamp(newAngle, AngleLimitLower, AngleLimitUpper);
        }

        currentAngle = newAngle;

        Vector3 newLocalRotation = SetAdjustedVector3(currentAngle);
        ObjectToAngle.localRotation = Quaternion.Euler(newLocalRotation);
    }

    Vector3 SetAdjustedVector3(float newAngle)
    {
        if (RotateAround == RotationAxis.X)
        {
            return new Vector3(newAngle, 0, 0);
        }
        else if (RotateAround == RotationAxis.Y)
        {
            return new Vector3(0, newAngle, 0);
        }
        else
        {
            return new Vector3(0, 0, newAngle);
        }
    }

    public float GetLocalAngleRotation()
    {
        return currentAngle;
    }

    public float GetNormalisedRotationValue()
    {
        if (LimitRotation)
        {
            return (GetLocalAngleRotation() - AngleLimitLower) / (AngleLimitUpper - AngleLimitLower);
        }
        else
        {
            return GetLocalAngleRotation() / 360f;
        }
    }
}
