using UnityEngine;

public interface IMoveable
{
    public float MovementSpeed
    { get; set; }

    public Vector3 MoveToPosition
    { get; set; }

    public void SetMovementSpeed(float movementSpeed);

    public void Movement();

    public void SetPositionToMoveTo(Vector3 position);
}
