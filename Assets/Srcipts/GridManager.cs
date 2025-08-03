using System.Collections.Generic;
using UnityEngine;
using DG.Tweening.Core.Easing;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum objectiveType { pickUp, dropOff }

[System.Serializable]
public class ResoucePosition
{
    public Vector2Int position;
    public objectiveType type;
}

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    public bool autoCenter = true;
    public Vector2 gridOrigin;

    public Vector2Int startTilePos;
    public Vector2Int finishTilePos;

    public GameObject pickupPrefab;
    public GameObject dropoffPrefab;
    public List<ResoucePosition> Resources = new();

    public WaveManager waveManager;
    [SerializeField] private EnemySpawner spawner;
    public GameObject startTilePrefabObject;
    public GameObject finishTilePrefabObject;

    private GridTile[,] grid;

    private void Awake()
    {
        instance = this;
        waveManager.SetCurrentWave(spawner.currentWaveIndex);
    }

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        RefreshResourcesFromWave();
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

        if (startTilePrefabObject != null && IsInBounds(startTilePos))
        {
            GridTile startTile = grid[startTilePos.x, startTilePos.y];
            Instantiate(startTilePrefabObject, startTile.transform.position, Quaternion.identity, startTile.transform);
            startTile.SetOccupied(true); // so it can't place towers there
        }

        
        if (finishTilePrefabObject != null && IsInBounds(finishTilePos))
        {
            GridTile finishTile = grid[finishTilePos.x, finishTilePos.y];
            Instantiate(finishTilePrefabObject, finishTile.transform.position, Quaternion.identity, finishTile.transform);
            finishTile.SetOccupied(true); // so it can't place towers there
        }

        SpawnAllResources();
    }

    void SpawnAllResources()
    {
        foreach (var point in Resources)
        {
            SpawnResource(point);
        }
    }

    void SpawnResource(ResoucePosition point)
    {
        if (IsInBounds(point.position))
        {
            GridTile tile = grid[point.position.x, point.position.y];
            tile.SetOccupied(true);

            Vector3 spawnPos = tile.transform.position;
            GameObject obj = null;

            switch (point.type)
            {
                case objectiveType.pickUp:
                    if (pickupPrefab != null)
                        obj = Instantiate(pickupPrefab, spawnPos, Quaternion.identity, tile.transform);
                    break;

                case objectiveType.dropOff:
                    if (dropoffPrefab != null)
                        obj = Instantiate(dropoffPrefab, spawnPos, Quaternion.identity, tile.transform);
                    break;
            }

            if (obj != null)
                tile.SetOccupied(obj);
        }
    }

    public void RemoveResourceAt(Vector2Int pos)
    {
        // Remove from list
        Resources.RemoveAll(r => r.position == pos);

        // Remove from scene
        if (IsInBounds(pos))
        {
            GridTile tile = grid[pos.x, pos.y];
            for (int i = tile.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(tile.transform.GetChild(i).gameObject);
            }
            tile.SetOccupied(false);
        }
    }

    public void UpdatePickupsAndDropoffs()
    {
        // Remove anything not in Resources
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                bool stillExists = Resources.Exists(r => r.position == pos);

                if (!stillExists && IsInBounds(pos))
                {
                    GridTile tile = grid[pos.x, pos.y];
                    for (int i = tile.transform.childCount - 1; i >= 0; i--)
                    {
                        Destroy(tile.transform.GetChild(i).gameObject);
                    }
                    tile.SetOccupied(false);
                }
            }
        }

        // Spawn new resources from list
        foreach (var point in Resources)
        {
            SpawnResource(point);
        }
    }

    public void RefreshResourcesFromWave()
    {
        if (waveManager != null && waveManager.CurrentWaveResources != null)
        {
            Resources = new List<ResoucePosition>(waveManager.CurrentWaveResources);
        }
        else
        {
            Resources.Clear();
        }
    }

    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        Vector2 origin = autoCenter
            ? new Vector2(-width * tileSize / 2f + tileSize / 2f,
                          -height * tileSize / 2f + tileSize / 2f)
            : gridOrigin;

        Vector2 local = new Vector2(worldPosition.x, worldPosition.y) - origin;

        int x = Mathf.RoundToInt(local.x / tileSize);
        int y = Mathf.RoundToInt(local.y / tileSize);

        return new Vector2Int(x, y);
    }

    void SetTileType(Vector2Int pos, TileType type)
    {
        if (IsInBounds(pos))
        {
            grid[pos.x, pos.y].tileType = type;
            grid[pos.x, pos.y].SendMessage("Awake");
        }
    }

    public GridTile GetTileAtPosition(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
            return grid[pos.x, pos.y];
        return null;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void OnDrawGizmos()
    {
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
