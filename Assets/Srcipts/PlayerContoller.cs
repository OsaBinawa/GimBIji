using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int maxResourceHeld = 1;
    [SerializeField]private int heldResourceCount = 0;

    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;
    public float detectionRange = 0.1f;

    public LayerMask resourceLayer;
    public LayerMask dropOffLayer;

    public void SetPath(List<GridTile> path)
    {
        pathQueue.Clear();
        foreach (var tile in path)
        {
            pathQueue.Enqueue(tile.transform.position);
        }

        if (!isMoving)
            StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            Vector3 target = pathQueue.Dequeue();

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;

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
                foundAny = true;
            }
        }

        return foundAny;
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
}
