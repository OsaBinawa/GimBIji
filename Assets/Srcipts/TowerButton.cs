using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButton : MonoBehaviour
{
    [SerializeField] public TowerData towerToSelect;
    [SerializeField] private GridManager gridManager;

    private GameObject previewObject;
    private bool isPlacing = false;

    void Update()
    {
        if (!isPlacing) return;

        // Prevent placement if over UI
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }
        else if (Input.GetMouseButtonDown(1)) // Right-click to cancel
        {
            CancelPlacement();
        }
    }

    public void OnClick()
    {
        if (towerToSelect == null)
        {
            Debug.LogWarning("No tower assigned to button.");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogWarning("GridManager not assigned on TowerButton.");
            return;
        }

        isPlacing = true;
        Debug.Log("Started placing tower: " + towerToSelect.name);
    }

    private void UpdatePreview()
    {
        if (towerToSelect == null || towerToSelect.towerPref == null)
        {
            DestroyPreview();
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        Vector2Int gridPos = gridManager.GetGridPositionFromWorld(worldPos);
        GridTile tile = gridManager.GetTileAtPosition(gridPos);

        if (tile == null || tile.IsOccupied ||
            (towerToSelect.AllowedTileTypes.Length > 0 &&
             System.Array.IndexOf(towerToSelect.AllowedTileTypes, tile.tileType) == -1))
        {
            DestroyPreview();
            return;
        }

        Vector3 targetPos = tile.transform.position;

        if (previewObject == null)
        {
            previewObject = Instantiate(towerToSelect.towerPref, targetPos, Quaternion.identity);
        }
        else
        {
            previewObject.transform.position = targetPos;
        }
    }

    private void TryPlaceTower()
    {
        if (!GameManager.Instance.TryPlaceTower()) 
        {
            Debug.Log("Tower limit reached, cannot place.");
            return;
        }

       
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldClick.z = 0;
        Vector2Int gridPos = gridManager.GetGridPositionFromWorld(worldClick);
        GridTile tile = gridManager.GetTileAtPosition(gridPos);

        if (gridPos.x < 0 || gridPos.x >= gridManager.width || gridPos.y < 0 || gridPos.y >= gridManager.height)
        {
            Debug.Log("Clicked outside grid.");
            return;
        }

        if (tile == null || tile.IsOccupied)
        {
            Debug.Log("Invalid or occupied tile.");
            return;
        }

        if (towerToSelect.AllowedTileTypes != null &&
            System.Array.IndexOf(towerToSelect.AllowedTileTypes, tile.tileType) == -1)
        {
            Debug.Log("Tower not allowed on this tile type.");
            return;
        }

        if (towerToSelect.towerPref == null)
        {
            Debug.LogWarning("Tower prefab is missing!");
            return;
        }

        GameObject tower = Instantiate(towerToSelect.towerPref, tile.transform.position, Quaternion.identity);
        TowerStats stats = tower.GetComponent<TowerStats>();
        stats.gridManager = gridManager;
        stats.currentGridPosition = gridPos;

        tile.SetOccupied(tower);

        Debug.Log("Placed tower: " + towerToSelect.name + " at " + gridPos);
        isPlacing = false;
        DestroyPreview();
    }

    private void CancelPlacement()
    {
        Debug.Log("Placement canceled.");
        isPlacing = false;
        DestroyPreview();
    }

    private void DestroyPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }
}
