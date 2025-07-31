using System.Collections.Generic;
using UnityEngine;

public class TowerMortar : TowerStats
{
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private Vector2Int selectedTargetTile;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private bool hasSelectedTarget = false;
    private Vector2Int? previousTargetTile = null;
    private SpriteRenderer previousTileRenderer = null;
    [SerializeField] private int targetRangeX = 3; // customizable horizontal range
    [SerializeField] private int targetRangeY = 2; // customizable vertical range
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    private List<SpriteRenderer> highlightedTiles = new List<SpriteRenderer>();

    [SerializeField] private bool isChoosingTarget = false;
    [SerializeField] private LayerMask tileLayer;


    private void Update()
    {
        HandleMouseTargetSelection();
        if (hasSelectedTarget && Time.time >= cooldown)
        {
            ShootAtSelectedTile();
        }
    }

    void ShootAtSelectedTile()
    {
        Vector3 targetWorldPos = new Vector3(
            selectedTargetTile.x * tileSize + tileSize / 2f,
            selectedTargetTile.y * tileSize + tileSize / 2f,
            0f
        );

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        ProjectileMortar mortar = proj.GetComponent<ProjectileMortar>();

        Vector2 velocity = CalculateParabolicVelocity(firePoint.position, targetWorldPos, arcHeight);
        mortar.SetTargetCenter(targetWorldPos); // New method
        mortar.Launch(velocity);

        cooldown = Time.time + 1f / fireRate;
    }
    Vector2 CalculateParabolicVelocity(Vector2 start, Vector2 target, float height)
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float displacementY = target.y - start.y;

        float timeToApex = Mathf.Sqrt(2 * height / gravity);
        float timeFromApex;

        float fallHeight = height - displacementY;

        // Prevent sqrt of negative number
        if (fallHeight < 0)
        {
            fallHeight = 0.1f;
        }

        timeFromApex = Mathf.Sqrt(2 * fallHeight / gravity);

        float totalTime = timeToApex + timeFromApex;

        Vector2 velocityY = Vector2.up * Mathf.Sqrt(2 * gravity * height);
        Vector2 velocityX = new Vector2(target.x - start.x, 0f) / totalTime;

        return velocityX + velocityY;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw target area
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            selectedTargetTile.x * tileSize + tileSize / 2f,
            selectedTargetTile.y * tileSize + tileSize / 2f,
            0f
        );
        Gizmos.DrawWireCube(center, Vector3.one * tileSize);

        // Draw arc
        if (firePoint == null) return;
        Vector2 velocity = CalculateParabolicVelocity(firePoint.position, center, arcHeight);
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float time = (2 * velocity.y) / gravity;

        Vector3 previous = firePoint.position;
        Gizmos.color = Color.cyan;
        for (int i = 1; i <= 30; i++)
        {
            float t = i / 30f * time;
            Vector3 point = firePoint.position + (Vector3)(velocity * t) + Vector3.down * 0.5f * gravity * t * t;
            Gizmos.DrawLine(previous, point);
            previous = point;
        }
    }
    void HandleMouseTargetSelection()
    {
        if (Input.GetMouseButtonDown(0) && isChoosingTarget)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorld2D = new Vector2(mouseWorld.x, mouseWorld.y);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorld2D, Vector2.zero, Mathf.Infinity, tileLayer);

            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                GameObject tileGO = hit.collider.gameObject;
                SpriteRenderer renderer = tileGO.GetComponent<SpriteRenderer>();

                if (renderer != null && highlightedTiles.Contains(renderer))
                {
                    // Restore all highlighted tiles to normal color first
                    foreach (var tile in highlightedTiles)
                    {
                        if (tile != null)
                            tile.color = normalColor;
                    }

                    // Set selected tile
                    renderer.color = selectedColor;
                    previousTileRenderer = renderer;

                    Vector3 tilePos = tileGO.transform.position;
                    selectedTargetTile = new Vector2Int(
                        Mathf.FloorToInt(tilePos.x / tileSize),
                        Mathf.FloorToInt(tilePos.y / tileSize)
                    );

                    hasSelectedTarget = true;
                    isChoosingTarget = false;
                    TowerSelector.Instance.isSelectingTile = false;

                    highlightedTiles.Clear(); // Clear because selection is done
                }
            }
        }
    }

    public void EnableTargetSelection()
    {
        if (isChoosingTarget) return;

        isChoosingTarget = true;
        TowerSelector.Instance.isSelectingTile = true;

        HighlightTilesInRange();
    }
    void HighlightTilesInRange()
    {
        ClearHighlightedTiles();
        Vector2Int towerCoord = new Vector2Int(
    
            Mathf.RoundToInt(transform.position.x / tileSize),
    
            Mathf.RoundToInt(transform.position.y / tileSize));


        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject tile in allTiles)
        {
            Vector3 tilePos = tile.transform.position;
            Vector2Int tileCoord = new Vector2Int(
            Mathf.RoundToInt(tilePos.x / tileSize),
            Mathf.RoundToInt(tilePos.y / tileSize));
            ;

            bool inRange =
                Mathf.Abs(tileCoord.x - towerCoord.x) <= targetRangeX && Mathf.Abs(tileCoord.y - towerCoord.y) <= targetRangeY;


            if (inRange)
            {
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = highlightColor;
                    highlightedTiles.Add(renderer);
                }
            }
        }
    }
    void ClearHighlightedTiles()
    {
        foreach (SpriteRenderer r in highlightedTiles)
        {
            if (r != null && r != previousTileRenderer) // Don't reset selected tile's color
            {
                r.color = normalColor;
            }
        }
        highlightedTiles.Clear();
    }

    private void OnDestroy()
    {
        ClearHighlightedTiles(); 
        if (previousTileRenderer != null)
        {
            previousTileRenderer.color = normalColor;
        }
    }

}
