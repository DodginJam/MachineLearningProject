using System.Collections.Generic;
using UnityEngine;

public class GroundDetection_Aircraft : GroundDetection
{
    [field: SerializeField]
    public List<GroundContactPoint> ContactPointsForGround
    { get; private set; }

    public override bool IsGrounded()
    {
        foreach (GroundContactPoint contactPoint in ContactPointsForGround)
        {
            if (contactPoint.InContactWithSurface == true) return true;
        }

        return false;
    }
}
