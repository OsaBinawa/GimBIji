using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardsUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private TMP_Text TowerName;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text bulletText;
    [SerializeField] private Image iconImg;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool isOverGrid = false;
    [SerializeField] private GameObject previewInstance;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private GridManager gridManager;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.position;

        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Start()
    {
        TowerName.text = towerData.Name;
        damageText.text = towerData.Damage.ToString();
        bulletText.text = towerData.fireRate.ToString();
        iconImg.sprite = towerData.icon;
        previewInstance = towerData.towerPref;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        Vector3 screenPosition = eventData.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        if (gridManager != null)
        {
            Vector2 origin = gridManager.autoCenter
                ? new Vector2(-gridManager.width * gridManager.tileSize / 2f + gridManager.tileSize / 2f,
                              -gridManager.height * gridManager.tileSize / 2f + gridManager.tileSize / 2f)
                : gridManager.gridOrigin;

            int x = Mathf.FloorToInt((worldPosition.x - origin.x) / gridManager.tileSize);
            int y = Mathf.FloorToInt((worldPosition.y - origin.y) / gridManager.tileSize);

            if (x >= 0 && x < gridManager.width && y >= 0 && y < gridManager.height)
            {
                Vector3 snappedPosition = origin + new Vector2(x * gridManager.tileSize, y * gridManager.tileSize);
                isOverGrid = true;

                if (previewInstance == null && towerData.towerPref != null)
                {
                    previewInstance = Instantiate(towerData.towerPref, snappedPosition, Quaternion.identity);
                    SetPreviewMaterial(previewInstance, 0.5f); // optional transparency
                }
                else if (previewInstance != null)
                {
                    previewInstance.transform.position = snappedPosition;
                }

                canvasGroup.alpha = 0f; // Hide the card
            }
            else
            {
                DestroyPreview();
                canvasGroup.alpha = 0.6f; // Keep showing card when not over grid
                isOverGrid = false;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Vector3 screenPosition = eventData.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        if (gridManager != null)
        {
            Vector2 origin = gridManager.autoCenter
                ? new Vector2(-gridManager.width * gridManager.tileSize / 2f + gridManager.tileSize / 2f,
                              -gridManager.height * gridManager.tileSize / 2f + gridManager.tileSize / 2f)
                : gridManager.gridOrigin;

            Vector2 local = new Vector2(worldPosition.x, worldPosition.y) - origin;
            int x = Mathf.FloorToInt(local.x / gridManager.tileSize);
            int y = Mathf.FloorToInt(local.y / gridManager.tileSize);
            Vector2Int gridPos = new Vector2Int(x, y);

            if (x >= 0 && x < gridManager.width && y >= 0 && y < gridManager.height)
            {
                Vector2 spawnPos = origin + new Vector2(x * gridManager.tileSize, y * gridManager.tileSize);

                GridTile tile = gridManager.GetTileAtPosition(gridPos);

                if (tile == null)
                {
                    Debug.Log("Invalid tile.");
                }
                else if (tile.IsOccupied)
                {
                    Debug.Log("Tile is already occupied.");
                }
                else if (System.Array.IndexOf(towerData.AllowedTileTypes, tile.tileType) == -1)
                {
                    Debug.Log($"Tower type {towerData.name} not allowed on tile type {tile.tileType}");
                }
                else if (towerData.towerPref != null)
                {
                    Instantiate(towerData.towerPref, spawnPos, Quaternion.identity);
                    tile.SetOccupied(true);
                    Debug.Log("Tower placed at grid: " + gridPos);
                }
                else
                {
                    Debug.LogWarning("Tower prefab is missing in TowerData!");
                }
            }
            else
            {
                Debug.Log("Tried to place tower outside the grid.");
            }
        }
        else
        {
            Debug.LogWarning("GridManager not found.");
        }

        DestroyPreview();
        rectTransform.position = originalPosition;
    }


    private void DestroyPreview()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    private void SetPreviewMaterial(GameObject preview, float alpha)
    {
        var sr = preview.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }


}
