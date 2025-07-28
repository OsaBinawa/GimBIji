using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public Camera cam;
    public PlayerController player;

    public List<GridTile> pathTiles = new();
    private bool isDragging = false;
    private bool pathFinished = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pathFinished)
        {
            isDragging = true;
            //ResetAllTiles();
            //pathTiles.Clear();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && !pathFinished)
        {
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null && hit.TryGetComponent(out GridTile tile))
            {
                Collider2D resourceCheck = Physics2D.OverlapPoint(tile.transform.position, LayerMask.GetMask("Resource"));
                if (resourceCheck != null && resourceCheck.CompareTag("Resource"))
                {
                    // Block drawing on top of resource
                    return;
                }

                if (!pathTiles.Contains(tile))
                {
                    // Only allow drawing from Start tile
                    if (pathTiles.Count == 0 && tile.tileType != TileType.Start)
                        return;

                    if (pathTiles.Count > 0)
                    {
                        GridTile lastTile = pathTiles[^1];
                        if (!IsAdjacent(lastTile.gridPosition, tile.gridPosition))
                            return;
                    }

                    pathTiles.Add(tile);
                    tile.SetHighlight(true);

                    if (tile.tileType == TileType.Finish)
                    {
                        pathFinished = true;
                        isDragging = false;
                        Debug.Log("Finish tile reached. Path completed!");

                        if (player != null)
                            player.SetPath(pathTiles);
                    }
                }

                if (pathTiles.Count == 0)
                {
                    if (tile.tileType != TileType.Start)
                        return;

                    AddTileToPath(tile);
                }
                else
                {
                    GridTile lastTile = pathTiles[^1];

                    // If the current tile is already in the path and it's the one before the last, backtrack
                    if (pathTiles.Count > 1 && tile == pathTiles[^2])
                    {
                        RemoveLastTileFromPath();
                    }
                    // If it's a new, adjacent tile, add it
                    else if (!pathTiles.Contains(tile) && IsAdjacent(lastTile.gridPosition, tile.gridPosition))
                    {
                        AddTileToPath(tile);
                    }
                }
            }
        }
    }

    void AddTileToPath(GridTile tile)
    {
        pathTiles.Add(tile);
        tile.SetHighlight(true);

        if (tile.tileType == TileType.Finish)
        {
            pathFinished = true;
            isDragging = false;
            Debug.Log("Finish tile reached. Path completed!");

            if (player != null)
                player.SetPath(pathTiles);
            else
                Debug.LogWarning("Player reference not set in PathDrawer.");
        }
    }

    void RemoveLastTileFromPath()
    {
        GridTile tile = pathTiles[^1];
        pathTiles.RemoveAt(pathTiles.Count - 1);
        tile.SetHighlight(false);
    }

    public void ResetAllTiles()
    {
        if (!pathFinished)
        {
            foreach (var tile in pathTiles)
            {
                tile.SetHighlight(false);
            }
            pathTiles.Clear();
            pathFinished = false;
        }
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy == 1); // Manhattan distance
    }
}
