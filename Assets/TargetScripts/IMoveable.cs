using UnityEngine;

public interface IMoveable
{
    public float MovementSpeed
    { get; set; }

    public Vector3 EndPoint
    { get; set; }

    public void SetMovementMultiplier(float newMovementMultiplier);

    public void Movement();

    public void SetEndPoint(Vector3 endPoint);
}
