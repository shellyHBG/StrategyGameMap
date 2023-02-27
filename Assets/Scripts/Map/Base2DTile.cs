using UnityEngine;

public abstract class Base2DTile<T> : MonoBehaviour where T : Base2DTile<T>
{
    public Vector2 MapPosition;

    public abstract Vector2[] GetAdjacentPosition();
}
