using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public float attackRange;
    public GameObject projectilePrefab;
    public Transform firePoint; 
    public List<int> priorityIDs = new(); // Editable in Inspector

    protected override IEnumerator AttackTower(GameObject _)
    {
        Debug.Log("RangedEnemy scanning for towers by priority...");

        List<GameObject> towers = new();
        GameObject[] allTowers = GameObject.FindGameObjectsWithTag("Tower");

        // Filter towers within range
        foreach (GameObject tower in allTowers)
        {
            if (Vector3.Distance(transform.position, tower.transform.position) <= attackRange)
            {
                towers.Add(tower);
            }
        }

        if (towers.Count == 0)
        {
            Debug.Log("No towers in range.");
            attacking = false;
            isStopped = false;
            yield break;
        }

        // Sort towers based on priority list
        towers.Sort((a, b) =>
        {
            TowerStats ta = a.GetComponent<TowerStats>();
            TowerStats tb = b.GetComponent<TowerStats>();

            if (ta == null || tb == null) return 0;

            int ida = priorityIDs.IndexOf(ta.towerID);
            int idb = priorityIDs.IndexOf(tb.towerID);

            // If ID not found, send it to the bottom (i.e., treat as low priority)
            ida = ida == -1 ? int.MaxValue : ida;
            idb = idb == -1 ? int.MaxValue : idb;

            return ida.CompareTo(idb);
        });

        // Target the highest-priority tower
        GameObject target = towers[0];

        while (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            IHealth towerHealth = target.GetComponent<IHealth>();

            if (target == null || towerHealth == null || (towerHealth is MonoBehaviour mb && mb == null))
                break;

            ShootProjectile(target.transform.position);
            yield return new WaitForSeconds(attackDelay);
        }

        // Wait for destruction
        while (target != null)
        {
            yield return null;

            if (target.GetComponent<IHealth>() == null)
                break;
        }

        attacking = false;
        isStopped = false;
        currentIndex++;
    }

    private void ShootProjectile(Vector3 target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab not assigned.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (target - firePoint.position).normalized;

        projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 5f; // set projectile speed
    }
}
