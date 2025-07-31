using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    [SerializeField] private CardsUI cardsUI;          // Assign in Inspector
    [SerializeField] private GridManager gridManager;  // Assign in Inspector

    private TowerData selectedTowerData; // Tower selected from UI
    private TowerStats selectedTower;    // Tower selected in the scene

    private GameObject previewObject;    // Ghost/preview tower instance

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (selectedTowerData != null)
        {
            UpdatePreview();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceTower();
            }
            else if (Input.GetMouseButtonDown(1)) // Right click to cancel
            {
                CancelPlacement();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TrySelectExistingTower();
        }
    }

    /// <summary>
    /// Called by UI button (hook this in the Inspector).
    /// </summary>
    public void OnTowerButtonClick(TowerData data)
    {
        selectedTowerData = data;
        selectedTower = null;
        //cardsUI.ShowTowerInfo(data);
    }

    private void TryPlaceTower()
    {
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldClick.z = 0;
        Vector2Int gridPos = gridManager.GetGridPositionFromWorld(worldClick);
        GridTile tile = gridManager.GetTileAtPosition(gridPos);

        if (gridPos.x < 0 || gridPos.x >= gridManager.width || gridPos.y < 0 || gridPos.y >= gridManager.height)
        {
            Debug.Log("Tried to place tower outside the grid.");
            return;
        }

        if (tile == null)
        {
            Debug.Log("Invalid tile.");
            return;
        }

        if (tile.IsOccupied)
        {
            Debug.Log("Tile is already occupied.");
            return;
        }

        if (selectedTowerData.AllowedTileTypes != null &&
            System.Array.IndexOf(selectedTowerData.AllowedTileTypes, tile.tileType) == -1)
        {
            Debug.Log($"Tower {selectedTowerData.name} not allowed on tile type {tile.tileType}");
            return;
        }

        if (selectedTowerData.towerPref == null)
        {
            Debug.LogWarning("Tower prefab is missing in TowerData!");
            return;
        }

        // Place the tower
        GameObject tower = Instantiate(selectedTowerData.towerPref, tile.transform.position, Quaternion.identity);
        TowerStats stats = tower.GetComponent<TowerStats>();
        stats.gridManager = gridManager;
        stats.currentGridPosition = gridPos;

        tile.SetOccupied(tower);

        Debug.Log("Tower placed at grid: " + gridPos);

        selectedTowerData = null;
        //cardsUI.Hide();
        DestroyPreview();
    }

    private void TrySelectExistingTower()
    {
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clickPos = new Vector2(worldClick.x, worldClick.y);

        RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);
        if (hit.collider != null)
        {
            TowerStats tower = hit.collider.GetComponent<TowerStats>();
            if (tower != null)
            {
                SelectTower(tower);
                return;
            }
        }

        DeselectTower();
    }

    private void SelectTower(TowerStats tower)
    {
        if (selectedTower != null && selectedTower != tower)
            selectedTower.HideUI();

        selectedTower = tower;
        selectedTower.ShowUI();
    }

    private void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.HideUI();
            selectedTower = null;
        }
    }

    private void CancelPlacement()
    {
        Debug.Log("Placement canceled.");
        selectedTowerData = null;
        //cardsUI.Hide();
        DestroyPreview();
    }

    private void UpdatePreview()
    {
        if (selectedTowerData == null || selectedTowerData.towerPref == null)
        {
            DestroyPreview();
            return;
        }

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        Vector2Int gridPos = gridManager.GetGridPositionFromWorld(worldPos);

        GridTile tile = gridManager.GetTileAtPosition(gridPos);
        if (tile == null || tile.IsOccupied ||
            (selectedTowerData.AllowedTileTypes.Length > 0 &&
             System.Array.IndexOf(selectedTowerData.AllowedTileTypes, tile.tileType) == -1))
        {
            DestroyPreview();
            return;
        }

        Vector3 targetPos = tile.transform.position;

        if (previewObject == null)
        {
            previewObject = Instantiate(selectedTowerData.towerPref, targetPos, Quaternion.identity);
        }
        else
        {
            previewObject.transform.position = targetPos;
        }
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
