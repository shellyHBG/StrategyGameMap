using UnityEngine;

/// <summary>
/// Tile No. 1
/// Support checking whether movable or not (if a obstacle exists in a range => not movable).
/// </summary>
public class Tile_1 : Rectangle2DTile
{
    /// <summary>
    /// This tile can be move to or not.
    /// </summary>
    public bool Movable { get; private set; }
    public int Cost { get; private set; } // temp cost for role to reach here

    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlightColor;
    private SpriteRenderer _spRenderer;

    protected override void onAwake()
    {
        MapPosition.x = (int)transform.position.x;
        MapPosition.y = (int)transform.position.y;
        Cost = int.MaxValue;
        _spRenderer = GetComponent<SpriteRenderer>();
        CheckMovable();
    }

    #region MonoBehaviour
    private void OnMouseEnter()
    {
        if (!Movable)
            return;

        transform.localScale += Vector3.one * 0.35f;
        _spRenderer.sortingOrder = (int)eBagroundSortOder.TileHighLight;
    }

    private void OnMouseExit()
    {
        if (!Movable)
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

    public void HighlightTile(int cost)
    {
        _spRenderer.color = _highlightColor;
        Cost = cost;
    }

    public void ResetNomalTile()
    {
        _spRenderer.color = _normalColor;
        Cost = int.MaxValue;
    }

    private void CheckMovable()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, _spRenderer.bounds.extents.x, _obstacleLayer);
        Movable = collider == null ? true : false;
    }
}
