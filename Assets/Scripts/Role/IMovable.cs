using System;
using UnityEngine;

public interface IMovable
{
    Vector2 MapPosition { get; }
    int MoveRange { get; }
    void Move(Transform trTarget, Action onMoveEnd);
}
