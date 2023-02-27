using UnityEngine;

/// <summary>
/// Tile No. 1
/// Support checking whether walkable or not (if a obstacle exists in a range => not walkable)
/// </summary>
public class Tile_1 : Rectangle2DTile
{
    public bool CanWalk;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlightColor;
    private SpriteRenderer _spRenderer;

    protected override void onAwake()
    {
        MapPosition.x = (int)transform.position.x;
        MapPosition.y = (int)transform.position.y;
        _spRenderer = GetComponent<SpriteRenderer>();
        checkCanWalk();
    }

    #region MonoBehaviour
    private void OnMouseEnter()
    {
        if (!CanWalk)
            return;

        transform.localScale += Vector3.one * 0.35f;
        _spRenderer.sortingOrder = (int)eBagroundSortOder.TileHighLight;
    }

    private void OnMouseExit()
    {
        if (!CanWalk)
            return;

        transform.localScale -= Vector3.one * 0.35f;
        _spRenderer.sortingOrder = (int)eBagroundSortOder.Tile;
    }

    private void OnMouseDown()
    {
        // Move to here
        GameMapManager.Instance.MoveToHere(MapPosition);
    }
    #endregion

    public bool HighlightTile()
    {
        _spRenderer.color = CanWalk ? _highlightColor : _normalColor;
        return CanWalk;
    }

    public void ResetNomalTile()
    {
        _spRenderer.color = _normalColor;
    }

    private void checkCanWalk()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _spRenderer.bounds.extents.x, _obstacleLayer);
        CanWalk = collider == null ? true : false;
    }
}
