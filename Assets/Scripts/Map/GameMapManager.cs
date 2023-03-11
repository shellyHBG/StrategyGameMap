using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapManager : MonoSingleton<GameMapManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }

    /// <summary>
    /// All tiles in this game map
    /// </summary>
    private Dictionary<Vector2, Tile_1> _dictVec2Tiles;

    /// <summary>
    /// Selected role to do something (An object implements IMovable)
    /// </summary>
    private IMovable _selectedRole;
    private bool _bMoving;
    /// <summary>
    /// Movable tiles of selected role
    /// </summary>
    private HashSet<Tile_1> _setHighlightedTiles;

    /// <summary>
    /// Initialize game map
    /// </summary>
    /// <param name="mapName"></param>
    public void RelinkGameMap(string mapName)
    {
        if (_dictVec2Tiles == null)
            _dictVec2Tiles = new Dictionary<Vector2, Tile_1>();
        _dictVec2Tiles.Clear();

        Transform tileParent_ = GameObject.Find(mapName).transform;
        if (tileParent_ == null)
            throw new NullReferenceException($"Map: {mapName} not found.");

        // Assign tiles in the map
        Tile_1[] allTiles = tileParent_.GetComponentsInChildren<Tile_1>();
        for (int i = 0; i < allTiles.Length; i++)
        {
            _dictVec2Tiles[allTiles[i].MapPosition] = allTiles[i];
        }

        if (_setHighlightedTiles == null)
        {
            _setHighlightedTiles = new HashSet<Tile_1>();
        }
        else
        {
            _setHighlightedTiles.Clear();
        }
    }

    /// <summary>
    /// Show/Hide higlight tiles where is movable for a role
    /// </summary>
    public void ToggleMovableTiles(IMovable movableRole, Vector2 v2ToTarget, int range)
    {
        if (_setHighlightedTiles == null || _setHighlightedTiles.Count == 0)
        {
            _selectedRole = movableRole;
            ShowMovableTiles(v2ToTarget, range);
        }
        else
        {
            _selectedRole = null;
            HideMovableTiles();
        }
    }

    /// <summary>
    /// Move selected role to new map position
    /// </summary>
    public void MoveToHere(Vector2 v2Target)
    {
        if (_selectedRole == null || _bMoving)
            return;

        if (!_dictVec2Tiles.TryGetValue(v2Target, out Tile_1 tileTarget) || tileTarget == null)
            return;

        _bMoving = true;

        // calculate the path
        Vector2 v2Source = _selectedRole.MapPosition;
        if (!CalculatePathTo(v2Source, v2Target, _selectedRole.MoveRange, out Stack<Vector2> inversedPath))
        {
            OnMoveToEnd();
            throw new Exception($"Path from {v2Source} to {v2Target} cannot be calculated.");
        }

        // move selected role to the destination
        StartCoroutine(MoveToByStep(inversedPath));
    }

    private void ShowMovableTiles(Vector2 v2Target, int range)
    {
        //Vector2 v2Target = new Vector2(trTaget.position.x, trTaget.position.y);
        if (!_dictVec2Tiles.TryGetValue(v2Target, out Tile_1 tileTarget) || tileTarget == null)
        {
            throw new NullReferenceException($"Tile position [{v2Target}] not in Map.");
        }

        // Method 1: iterate all tiles
        //foreach(Tile_1 tile in _dictIndexTiles.Values)
        //{
        //    float distX = Mathf.Abs(v2Target.x - tile.MapPosition.x);
        //    float distY = Mathf.Abs(v2Target.y - tile.MapPosition.y);
        //    if (distX + distY <= range)
        //    {
        //        if (tile.HighlightTile())
        //        {
        //            _setHighlightedTiles.Add(tile);
        //        }
        //    }
        //}

        // Method 2: search with depth = range
        Dictionary<Vector2, int> adjPos = CalculateMovablePosition(tileTarget, range, 0);
        foreach (var pos in adjPos)
        {
            if (!_dictVec2Tiles.TryGetValue(pos.Key, out Tile_1 highlightTile) || highlightTile == null)
                continue;
            if (pos.Key == v2Target)    // the cost of start position is zero
                highlightTile.HighlightTile(0);
            else
                highlightTile.HighlightTile(pos.Value);
            _setHighlightedTiles.Add(highlightTile);
        }
    }

    private void HideMovableTiles()
    {
        foreach (Tile_1 tile in _setHighlightedTiles)
        {
            tile.ResetNomalTile();
        }
        _setHighlightedTiles.Clear();
    }

    private void OnMoveToEnd()
    {
        _selectedRole = null;
        _bMoving = false;
        HideMovableTiles();
    }

    /// <summary>
    /// Search all movable position of tiles and their cost in range.
    /// Recursively function call.
    /// </summary>
    private Dictionary<Vector2, int> CalculateMovablePosition(Tile_1 tile, int depth, int cost)
    {
        if (depth <= 0)
            return null;

        cost++;
        Dictionary<Vector2, int> listAllPos = new Dictionary<Vector2, int>();

        Vector2[] adjacentPos = tile.GetAdjacentPosition();
        for (int i = 0; i < adjacentPos.Length; i++)
        {
            // check position in map
            if (_dictVec2Tiles.TryGetValue(adjacentPos[i], out Tile_1 adTile) && adTile != null)
            {
                if (!adTile.Movable)
                    continue;
                if (!listAllPos.TryAdd(adjacentPos[i], cost) && cost < listAllPos[adjacentPos[i]])
                    listAllPos[adjacentPos[i]] = cost;
                // call recursively to get next depth position
                Dictionary<Vector2, int> nextAdjPos = CalculateMovablePosition(adTile, depth - 1, cost);
                if (nextAdjPos != null)
                {
                    foreach (var newPos in nextAdjPos)
                    {
                        // override cost if duplicated position cost less
                        if (!listAllPos.TryAdd(newPos.Key, newPos.Value) && newPos.Value < listAllPos[newPos.Key])
                            listAllPos[newPos.Key] = newPos.Value;
                    }
                }
            }
        }

        return listAllPos;
    }

    /// <summary>
    /// Calculate the inversed path from source to destination. (= the path from destination tp source)
    /// Then call method `MoveToByStep` to move selected role.
    /// </summary>
    private bool CalculatePathTo(Vector2 source, Vector2 destination, int maxStep, out Stack<Vector2> inversedPath)
    {
        inversedPath = new Stack<Vector2>();
        Vector2 curPos = destination;
        inversedPath.Push(curPos);
        int curStep = 0;
        while(source != curPos && ++curStep <= maxStep)
        {
            _dictVec2Tiles.TryGetValue(curPos, out Tile_1 curTile);
            if (curTile == null)
                break;

            Vector2[] adjPos = curTile.GetAdjacentPosition();
            Tile_1 nextTile = null;
            for (int i = 0; i < adjPos.Length; i++)
            {
                _dictVec2Tiles.TryGetValue(adjPos[i], out Tile_1 adjTile);
                if (adjTile == null)
                    continue;
                if (nextTile == null || adjTile.Cost < nextTile.Cost)
                    nextTile = adjTile;
            }

            curPos = nextTile.MapPosition;
            if (source != curPos)
                inversedPath.Push(curPos);
        }
        return source == curPos;
    }

    /// <summary>
    /// Move by step
    /// </summary>
    private IEnumerator MoveToByStep(Stack<Vector2> inversedPath)
    {
        bool stepFinished = false;
        while(inversedPath.Count > 0)
        {
            Vector2 nextPos = inversedPath.Pop();
            _dictVec2Tiles.TryGetValue(nextPos, out Tile_1 nextTile);
            _selectedRole.Move(nextTile.transform, () => { stepFinished = true; });
            yield return new WaitUntil(()=>stepFinished);
            stepFinished = false;
        }
        OnMoveToEnd();
    }
}
