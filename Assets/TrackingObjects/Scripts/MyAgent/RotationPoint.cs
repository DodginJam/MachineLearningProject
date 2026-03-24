using System;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

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

    private float CurrentAngle
    { get; set; }

    public enum RotationAxis
    {
        X, Y, Z
    }

    /// <summary>
    /// Setting an angle of the rotation point with respect to rotation limitations and preventing angles higher then 360 / lower then zero for no-limits.
    /// </summary>
    /// <param name="newAngle"></param>
    public void SetAngle(float newAngle)
    {
        if (!LimitRotation)
        {
            newAngle %= 360f;

            if (newAngle < 0) 
            {
                newAngle += 360f;
            }
        }
        else
        {
            newAngle = Mathf.Clamp(newAngle, AngleLimitLower, AngleLimitUpper);
        }

        CurrentAngle = newAngle;

        Vector3 newLocalRotation = SetAdjustedVector3(CurrentAngle);
        ObjectToAngle.localRotation = Quaternion.Euler(newLocalRotation);
    }

    /// <summary>
    /// Return Vector3 with new angle set to the current axis of rotation.
    /// </summary>
    /// <param name="newAngle"></param>
    /// <returns></returns>
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
        return CurrentAngle;
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

    public void RotateAngle(float normalisedRotationInput, float rotationSpeed, float timeStepMultiplyer)
    {
        SetAngle(GetLocalAngleRotation() + (normalisedRotationInput * rotationSpeed * timeStepMultiplyer));
    }
}
