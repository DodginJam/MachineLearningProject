using System;
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
    {  get; private set; }

    [field:SerializeField]
    public float CurrentAngle
    { get; private set; }

    public enum RotationAxis
    {
        X, Y, Z
    }

    public void SetAngle()
    {
        if (!LimitRotation)
        {
            if (CurrentAngle < 0)
            {
                CurrentAngle = CurrentAngle % 360;
                CurrentAngle += 360;
            }
            else if (CurrentAngle >= 360)
            {
                CurrentAngle = CurrentAngle % 360;
            }
        }

        float newAngle = LimitRotation ? Mathf.Clamp(CurrentAngle, AngleLimitLower, AngleLimitUpper) : CurrentAngle;

        Vector3 newLocalRotation = GetAdjustedVector3(newAngle);

        ObjectToAngle.localRotation = Quaternion.Euler(newLocalRotation);
    }

    Vector3 GetAdjustedVector3(float newAngle)
    {
        if (RotateAround == RotationAxis.X)
        {
            return new Vector3(newAngle, 0, 0);
        }
        else if (RotateAround == RotationAxis.Y)
        {
            return new Vector3(0, newAngle , 0);
        }
        else
        {
            return new Vector3(0, 0, newAngle);
        }
    }
}
