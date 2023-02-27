using System;
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
    /// Selected role to do something
    /// </summary>
    private Role _selectedRole;
    /// <summary>
    /// Walkable tiles of selected role
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
    /// Show/Hide higlight tiles where is walkable for a role
    /// </summary>
    public void ToggleWalkableTiles(Role selectedRole, Transform trToTarget, int range)
    {
        if (_setHighlightedTiles == null || _setHighlightedTiles.Count == 0)
        {
            _selectedRole = selectedRole;
            showWalkableTiles(trToTarget, range);
        }
        else
        {
            _selectedRole = null;
            hideWalkableTiles();
        }
    }

    /// <summary>
    /// Move selected role to new map position
    /// </summary>
    public void MoveToHere(Vector2 v2Target)
    {
        if (_selectedRole == null)
            return;

        if (!_dictVec2Tiles.TryGetValue(v2Target, out Tile_1 tileTarget) || tileTarget == null)
            return;

        // TODO: calculate the path

        _selectedRole.Move(tileTarget.transform, onMoveToEnd);
    }

    private void showWalkableTiles(Transform trTaget, int range)
    {
        Vector2 v2Target = new Vector2(trTaget.position.x, trTaget.position.y);
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
        Vector2[] adjPos = searchAdjacentPosition(tileTarget, range);
        for (int i = 0; i < adjPos.Length; i++)
        {
            if (!_dictVec2Tiles.TryGetValue(adjPos[i], out Tile_1 highlightTile) || highlightTile == null)
                continue;
            if (highlightTile.HighlightTile())
            {
                _setHighlightedTiles.Add(highlightTile);
            }
        }
    }

    private void hideWalkableTiles()
    {
        foreach (Tile_1 tile in _setHighlightedTiles)
        {
            tile.ResetNomalTile();
        }
        _setHighlightedTiles.Clear();
    }

    private void onMoveToEnd()
    {
        _selectedRole = null;
        hideWalkableTiles();
    }

    /// <summary>
    /// Search all position of tiles in range
    /// Recursively function call
    /// </summary>
    private Vector2[] searchAdjacentPosition(Tile_1 tile, int depth)
    {
        if (depth <= 0)
            return null;

        List<Vector2> listAllPos = new List<Vector2>();
        Vector2[] adjacentPos = tile.GetAdjacentPosition();
        for (int i = 0; i < adjacentPos.Length; i++)
        {
            // check position in map
            if (_dictVec2Tiles.TryGetValue(adjacentPos[i], out Tile_1 adTile) && adTile != null)
            {
                listAllPos.Add(adjacentPos[i]);
                // call recursively to get next depth position
                Vector2[] nextAdjPos = searchAdjacentPosition(adTile, depth - 1);
                if (nextAdjPos != null)
                    listAllPos.AddRange(nextAdjPos);
            }
        }

        return listAllPos.ToArray();
    }
}
