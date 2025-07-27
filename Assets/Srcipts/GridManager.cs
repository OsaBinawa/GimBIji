using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    public bool autoCenter = true;
    public Vector2 gridOrigin;

    public Vector2Int startTilePos;
    public Vector2Int finishTilePos;

    private GridTile[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile Prefab is not assigned!");
            return;
        }

        grid = new GridTile[width, height];

        Vector2 origin = autoCenter
            ? new Vector2(-width * tileSize / 2f + tileSize / 2f, -height * tileSize / 2f + tileSize / 2f)
            : gridOrigin;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 spawnPos = origin + new Vector2(x * tileSize, y * tileSize);
                GameObject obj = Instantiate(tilePrefab, spawnPos, Quaternion.identity, transform);

                GridTile tile = obj.GetComponent<GridTile>();
                tile.gridPosition = new Vector2Int(x, y);
                tile.tileType = TileType.Normal;

                grid[x, y] = tile;
            }
        }

        SetTileType(startTilePos, TileType.Start);
        SetTileType(finishTilePos, TileType.Finish);
    }

    void SetTileType(Vector2Int pos, TileType type)
    {
        if (IsInBounds(pos))
        {
            grid[pos.x, pos.y].tileType = type;
            grid[pos.x, pos.y].SendMessage("Awake"); // refresh color
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void OnDrawGizmos()
    {
        // Draw tile bounds
        Vector2 origin = autoCenter
            ? new Vector2(-width * tileSize / 2f + tileSize / 2f, -height * tileSize / 2f + tileSize / 2f)
            : gridOrigin;

        Gizmos.color = Color.gray;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 center = origin + new Vector2(x * tileSize, y * tileSize);
                Gizmos.DrawWireCube(center, Vector3.one * tileSize * 0.95f);

#if UNITY_EDITOR
                // Label tile coordinates
                GUIStyle style = new GUIStyle
                {
                    normal = { textColor = Color.yellow },
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };

                Vector3 worldPos = center + Vector2.up * 0.6f;
                Vector2 labelPos = HandleUtility.WorldToGUIPoint(worldPos);
                Handles.BeginGUI();
                GUI.Label(new Rect(labelPos.x - 20, labelPos.y - 10, 40, 20), $"({x},{y})", style);
                Handles.EndGUI();
#endif
            }
        }
    }
}
