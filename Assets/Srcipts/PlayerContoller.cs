using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour,IHealth
{
    public float moveSpeed = 3f;
    public int maxResourceHeld = 1;
    [SerializeField]private int heldResourceCount = 0;
    public float CurrentHealth { get; private set; } = 100f;
    public float MaxHealth { get; private set; } = 100f;
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;
    public float detectionRange = 0.1f;
    bool reachEnd;
    [SerializeField] Button startButton;
    UImanagers UI;
    public LayerMask resourceLayer;
    public LayerMask dropOffLayer;
    //public bool openShopAfterMove = false;
    // Optional: event when path is completed
    public UnityEvent onPathCompleted;

    // Optional: track last position (Finish)
    private Vector3 finishTilePosition;

    private Vector3 startPosition;

    private void Awake()
    {
        UI = FindFirstObjectByType<UImanagers>();
    }

    public void Start()
    {
        startPosition = transform.position;
    }

    public void SetPath(List<GridTile> path)
    {
        pathQueue.Clear();
        foreach (var tile in path)
        {
            pathQueue.Enqueue(tile.transform.position);
        }

        if (path.Count > 0)
            finishTilePosition = path[^1].transform.position;
    }

    public void Move()
    {
        if (!isMoving && !reachEnd)
            StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            startButton.interactable = false;
            Vector3 target = pathQueue.Dequeue();

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;

            // Try collecting resources if not full
            if (heldResourceCount < maxResourceHeld)
            {
                bool collected = TryCollectAdjacentResources();
                if (collected)
                    yield return new WaitForSeconds(0.3f);
            }

            if (heldResourceCount == maxResourceHeld)
            {
                bool drop = TryDropResource();

                if (drop)
                    yield return new WaitForSeconds(0.3f);
            }

            yield return null;
        }

        isMoving = false;
        

        // Call event when finished path
        onPathCompleted?.Invoke();
    }

    private bool TryCollectAdjacentResources()
    {
        bool foundAny = false;
        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (var dir in directions)
        {
            if (heldResourceCount >= maxResourceHeld) break;

            Vector3 checkPos = transform.position + dir;
            Collider2D hit = Physics2D.OverlapCircle(checkPos, detectionRange, resourceLayer);

            if (hit != null)
            {
                Debug.Log("Collected: " + hit.name);
                Destroy(hit.gameObject);
                heldResourceCount++;
                foundAny = true;
            }
        }

        return foundAny;
    }

    private bool TryDropResource()
    {
        bool foundAny = false;
        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (var dir in directions)
        {
            if (heldResourceCount <= 0) break;

            Vector3 checkPos = transform.position + dir;
            Collider2D hit = Physics2D.OverlapCircle(checkPos, detectionRange, dropOffLayer);

            if (hit != null)
            {
                Debug.Log("Collected: " + hit.name);
                Destroy(hit.gameObject);
                heldResourceCount--;
                GameManager.Instance.maxTowerSelected++;
                foundAny = true;
            }
        }

        return foundAny;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Player {damage} damage!");
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die(); 
        }
    }
    public void Die()
    {
        Debug.Log("Tower died.");
        GameManager.Instance.GameOver();
        //Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (var dir in directions)
        {
            Gizmos.DrawWireSphere(transform.position + dir, detectionRange);
        }
    }


    public int GetHeldResourceCount()
    {
        return heldResourceCount;
    }

    public void ResetResources()
    {
        heldResourceCount = 0;
    }

    public bool IsAtEndOfPath => !isMoving && pathQueue.Count == 0;

    public bool IsOnFinishTile()
    {
        return Vector3.Distance(transform.position, finishTilePosition) < 0.1f;
    }
    public void ResetToStartPosition()
    {
        StopAllCoroutines(); // Hentikan gerakan saat ini
        pathQueue.Clear();   // Bersihkan path
        isMoving = false;
        reachEnd = false;
        transform.position = startPosition;
        startButton.interactable = true;
        ResetResources();    // Opsional: reset resource juga
    }
}
