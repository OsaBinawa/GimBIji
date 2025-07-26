using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public Camera cam;

    private List<GridTile> pathTiles = new();
    private bool isDragging = false;
    private bool pathFinished = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pathFinished)
        {
            isDragging = true;
            pathTiles.Clear();
            ResetAllTiles(); // clear highlights from previous path
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
                if (!pathTiles.Contains(tile))
                {
                    // Only allow drawing from Start tile
                    if (pathTiles.Count == 0 && tile.tileType != TileType.Start)
                        return;

                    // Optional: restrict to adjacent tiles
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
                    }
                }
            }
        }
    }

    void ResetAllTiles()
    {
        GridTile[] allTiles = FindObjectsOfType<GridTile>();
        foreach (var tile in allTiles)
        {
            tile.SetHighlight(false);
        }
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy == 1); // Manhattan distance = 1
    }
}
