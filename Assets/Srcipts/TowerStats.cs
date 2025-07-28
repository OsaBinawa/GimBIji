using UnityEngine;

public class TowerStats : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private float fireRate => towerData.fireRate;
    [SerializeField] private float maxHP => towerData.HP;
    [SerializeField] private float dmg => towerData.Damage;
    [SerializeField] private float curHP;
    private Vector2Int facingDirection = Vector2Int.up;

    public GridManager gridManager;
    public Vector2Int currentGridPosition;


    private int RangeWidth => towerData.RangeWidth;
    private int RangeHeight => towerData.RangeHeight;
    Vector2Int RotateOffset(Vector2Int offset, Vector2Int forward)
    {
        if (forward == Vector2Int.up) return offset;
        if (forward == Vector2Int.right) return new Vector2Int(offset.y, -offset.x);
        if (forward == Vector2Int.down) return new Vector2Int(-offset.x, -offset.y);
        if (forward == Vector2Int.left) return new Vector2Int(-offset.y, offset.x);
        return offset;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        curHP = maxHP;
        if (gridManager != null)
        {
            currentGridPosition = GetGridPositionFromWorld(transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RotateDirectionClockwise();
            //Sementara aja nanti ganti ke button UI aja ini ampas soal e
        }
    }
    private void RotateDirectionClockwise()
    {
        if (facingDirection == Vector2Int.up)
            facingDirection = Vector2Int.right;
        else if (facingDirection == Vector2Int.right)
            facingDirection = Vector2Int.down;
        else if (facingDirection == Vector2Int.down)
            facingDirection = Vector2Int.left;
        else if (facingDirection == Vector2Int.left)
            facingDirection = Vector2Int.up;

        Debug.Log($"Tower facing now: {facingDirection}");
    }


    void Update()
    {
        DetectTilesInRange();
    }

    void DetectTilesInRange()
    {
        Vector2Int forward = Vector2Int.up;

        for (int dx = -RangeWidth / 2; dx <= RangeWidth / 2; dx++)
        {
            for (int dy = 1; dy <= RangeHeight; dy++)
            {
                Vector2Int offset = new Vector2Int(dx, dy);
                Vector2Int targetPos = currentGridPosition + RotateOffset(offset, forward);
                var tile = gridManager.GetTileAtPosition(targetPos);

                if (tile != null)
                {
                    //Debug.DrawLine(transform.position, tile.transform.position, Color.red);
                    // Detection logic here
                }
            }
        }
    }


    private Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        Vector2 origin = gridManager.autoCenter
            ? new Vector2(-gridManager.width * gridManager.tileSize / 2f + gridManager.tileSize / 2f,
                          -gridManager.height * gridManager.tileSize / 2f + gridManager.tileSize / 2f)
            : gridManager.gridOrigin;

        Vector2 local = new Vector2(worldPosition.x, worldPosition.y) - origin;

        int x = Mathf.FloorToInt(local.x / gridManager.tileSize);
        int y = Mathf.FloorToInt(local.y / gridManager.tileSize);

        return new Vector2Int(x, y);
    }

    void OnDrawGizmosSelected()
    {
        if (gridManager == null) return;

        Gizmos.color = Color.yellow;
        Vector2Int forward = facingDirection; 

        for (int dx = -RangeWidth / 2; dx <= RangeWidth / 2; dx++)
        {
            for (int dy = 1; dy <= RangeHeight; dy++)
            {
                Vector2Int offset = new Vector2Int(dx, dy);
                Vector2Int targetPos = currentGridPosition + RotateOffset(offset, forward);
                var tile = gridManager.GetTileAtPosition(targetPos);

                if (tile != null)
                {
                    Gizmos.DrawWireCube(tile.transform.position, Vector3.one * gridManager.tileSize * 0.9f);
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }


}
