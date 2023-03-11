using UnityEngine;

public abstract class Rectangle2DTile : Base2DTile<Rectangle2DTile>
{
    [SerializeField] private Vector2[] _neighborVec2;
    private bool _initAdjPosition;

    private void Awake()
    {
        onAwake();
    }

    protected abstract void onAwake();

    public override Vector2[] GetAdjacentPosition()
    {
        if (!_initAdjPosition)
        {
            _neighborVec2 = new Vector2[4];
            _neighborVec2[0] = new Vector2(MapPosition.x, MapPosition.y + 1);   // up
            _neighborVec2[1] = new Vector2(MapPosition.x + 1, MapPosition.y);   // right
            _neighborVec2[2] = new Vector2(MapPosition.x, MapPosition.y - 1);   // down
            _neighborVec2[3] = new Vector2(MapPosition.x - 1, MapPosition.y);   // left
            _initAdjPosition = true;
        }
        return _neighborVec2;
    }
}
