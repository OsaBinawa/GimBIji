using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TowerButton : MonoBehaviour
{
    [SerializeField] public TowerData towerToSelect;
    [SerializeField] private GridManager gridManager;

    [Header("Cooldown Settings")]
    [SerializeField] private float towerCooldown = 2f; // different per tower
    [SerializeField] private Image cooldownOverlay; // UI Image (filled type)
    public Image towerIcon;

    private GameObject previewObject;
    private bool isPlacing = false;
    private float nextPlaceTime = 0f;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        gridManager = FindFirstObjectByType<GridManager>();
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f; // start with no overlay
        }
    }

    void Update()
    {
        if (!isPlacing) return;

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        UpdatePreview();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    public void OnClick()
    {
        // Check cooldown first
        if (Time.time < nextPlaceTime)
        {
            Debug.Log($"Tower {towerToSelect.name} is on cooldown.");
            return;
        }

        if (towerToSelect == null || gridManager == null)
        {
            Debug.LogWarning("Tower data or grid manager missing.");
            return;
        }

        isPlacing = true;
        
        Debug.Log("Started placing tower: " + towerToSelect.name);
    }

    private void TryPlaceTower()
    {
        // 1️⃣ Check if under tower limit
        if (!GameManager.Instance.CanPlaceTower())
        {
            Debug.Log("Tower limit reached, cannot place.");
            return;
        }

        // 2️⃣ Get mouse position in world and find grid tile
        Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldClick.z = 0;
        Vector2Int gridPos = gridManager.GetGridPositionFromWorld(worldClick);
        GridTile tile = gridManager.GetTileAtPosition(gridPos);

        // 3️⃣ Validate grid position
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

        // 4️⃣ Place tower
        GameObject tower = Instantiate(towerToSelect.towerPref, tile.transform.position, Quaternion.identity);
        TowerStats stats = tower.GetComponent<TowerStats>();
        stats.gridManager = gridManager;
        stats.currentGridPosition = gridPos;

        tile.SetOccupied(tower);

        // 5️⃣ Only now count it towards tower limit
        GameManager.Instance.RegisterPlacedTower();

        Debug.Log("Placed tower: " + towerToSelect.name + " at " + gridPos);

        // 6️⃣ Reset placement state
        isPlacing = false;
        StartCooldown();
        DestroyPreview();
    }


    private void StartCooldown()
    {
        nextPlaceTime = Time.time + towerCooldown;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f; // Start full
            cooldownOverlay
                .DOFillAmount(0f, towerCooldown) // Drain to empty
                .SetEase(Ease.Linear);
        }

        if (button != null)
        {
            button.interactable = false;
            DOVirtual.DelayedCall(towerCooldown, () => button.interactable = true);
        }
    }

    private void CancelPlacement()
    {
        Debug.Log("Placement canceled.");
        isPlacing = false;
        DestroyPreview();
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

    private void DestroyPreview()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }
}
