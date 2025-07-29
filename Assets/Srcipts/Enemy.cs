using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHealth
{
    private List<GridTile> path = new();
    private int currentIndex = 1; // Start from second tile
    public float moveSpeed = 2f;

    private bool isStopped = false;
    public float towerCheckRadius = 0.5f;
    public float attackDelay = 1f;
    public LayerMask towerLayer;

    private bool attacking = false;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public void SetPath(List<GridTile> path)
    {
        this.path = new List<GridTile>(path); // clone the path
        if (path.Count == 0) return;

        transform.position = path[0].transform.position;
        currentIndex = 1;
        StartCoroutine(FollowPath());
    }

    public void Stop() => isStopped = true;
    public void Resume() => isStopped = false;

    IEnumerator FollowPath()
    {
        while (currentIndex < path.Count)
        {
            Vector3 targetPos = path[currentIndex].transform.position;

            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                if (!isStopped && !attacking)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                    // Check for nearby tower while moving
                    Collider2D hit = Physics2D.OverlapCircle(transform.position, towerCheckRadius, towerLayer);
                    if (hit != null)
                    {
                        isStopped = true;
                        attacking = true;
                        StartCoroutine(AttackTower(hit.gameObject));
                    }
                }
                yield return null;
            }

            if (!attacking)
                currentIndex++;
        }

        if (!attacking)
            OnReachFinish();
    }

    IEnumerator AttackTower(GameObject tower)
    {
        Debug.Log("Enemy found a tower! Attacking...");

        yield return new WaitForSeconds(attackDelay); // simulate attack delay

        if (tower != null)
        {
            // Optional: Call a method on the tower, like TakeDamage()
            Destroy(tower);
            Debug.Log("Tower destroyed!");
        }

        attacking = false;
        isStopped = false;
        currentIndex++; // continue walking
    }
    public void TakeDamage(float damage)
    {
        Debug.Log($"Enemy took {damage} damage!");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Tower died.");
        Destroy(gameObject);
    }

    void OnReachFinish()
    {
        Debug.Log("Enemy reached the end!");
        Destroy(gameObject); // Or trigger event
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerCheckRadius);
    }
}
