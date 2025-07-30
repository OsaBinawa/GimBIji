using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    public Camera cam;
    public PlayerController player;

    public List<GridTile> pathTiles = new();
    private bool isDragging = false;
    private bool pathFinished = false;
    private bool isDrawing = false;
    public bool IsDrawing => isDrawing;

    void Update()
    {
        // Start drawing
        if (Input.GetMouseButtonDown(0) && !pathFinished)
        {
            isDragging = true;
            isDrawing = true;
        }

        // Stop drawing
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isDrawing = false;
        }

        if (isDragging && !pathFinished)
        {
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null && hit.TryGetComponent(out GridTile tile))
            {
                // Cannot draw on resource
                Collider2D resourceCheck = Physics2D.OverlapPoint(tile.transform.position, LayerMask.GetMask("Resource"));
                if (resourceCheck != null && resourceCheck.CompareTag("Resource"))
                {
                    return;
                }

                // Drawing logic
                if (pathTiles.Count == 0)
                {
                    if (tile.tileType != TileType.Start)
                        return;

                    AddTileToPath(tile);
                }
                else
                {
                    GridTile lastTile = pathTiles[^1];

                    if (pathTiles.Count > 1 && tile == pathTiles[^2])
                    {
                        RemoveLastTileFromPath();
                    }
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
            isDrawing = false;
            Debug.Log("Finish tile reached. Path completed!");

            if (player != null)
                player.SetPath(pathTiles);
            else
                Debug.LogWarning("Player reference not set in PathDrawer.");
        }
    }

    void RemoveLastTileFromPath()
    {
        if (pathTiles.Count == 0)
            return;

        GridTile tile = pathTiles[^1];
        pathTiles.RemoveAt(pathTiles.Count - 1);
        tile.SetHighlight(false);
    }

    public void ResetAllTiles()
    {
        foreach (var tile in pathTiles)
        {
            tile.SetHighlight(false);
        }

        pathTiles.Clear();
        pathFinished = false;
        isDrawing = false;
    }

    public void ClearPath()
    {
        ResetAllTiles();
    }

    public void ResetButton()
    {
        if (!pathFinished)
        {
            ResetAllTiles();
        }
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);  
        return (dx + dy == 1); // Only allow cardinal directions
    }

    public void EnableDrawing()
    {
        isDragging = false;
        isDrawing = false;
        pathFinished = false;
        pathTiles.Clear();
    }
}
