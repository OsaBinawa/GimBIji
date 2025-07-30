using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHealth
{
    [SerializeField] protected List<GridTile> path = new();
    [SerializeField] protected int currentIndex = 1; // Start from second tile
    [SerializeField] protected float moveSpeed = 2f;

    [SerializeField] protected bool isStopped = false;
    [SerializeField] protected float towerCheckRadius = 0.5f;
    [SerializeField] protected float attackDelay = 1f;
    [SerializeField] protected LayerMask towerLayer;

    [SerializeField] protected bool attacking = false;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth = 100f;
    [SerializeField] protected float Damage;

    public delegate void EnemyDeath(Enemy enemy);
    public event EnemyDeath OnEnemyDied;

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
    protected virtual IEnumerator AttackTower(GameObject tower)
    {
        Debug.Log("Enemy found a tower! Attacking...");

        IHealth towerHealth = tower.GetComponent<IHealth>();

        while (tower != null && towerHealth != null)
        {
            yield return new WaitForSeconds(attackDelay);

            // Double-check if tower still exists and is alive
            if (tower == null || towerHealth == null)
                break;

            towerHealth.TakeDamage(Damage);
            Debug.Log("Enemy damaged the tower!");

            // Optional: check if tower was destroyed inside TakeDamage()
            if (towerHealth is MonoBehaviour mb && mb == null)
            {
                // tower's GameObject was destroyed inside TakeDamage()
                break;
            }
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
            OnEnemyDied?.Invoke(this);
            Destroy(gameObject);
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
        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, towerCheckRadius);
    }
}
