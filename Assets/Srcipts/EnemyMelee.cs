using System.Collections;
using UnityEngine;

public class EnemyMelee : Enemy
{
    protected override IEnumerator AttackTower(GameObject tower)
    {
        Debug.Log("MeleeEnemy attempting directional attack...");

        // Check if tower is aligned horizontally or vertically
        Vector3 direction = (tower.transform.position - transform.position).normalized;

        bool isHorizontal = Mathf.Abs(direction.x) > 0.9f && Mathf.Abs(direction.y) < 0.1f;
        bool isVertical = Mathf.Abs(direction.y) > 0.9f && Mathf.Abs(direction.x) < 0.1f;

        if (!isHorizontal && !isVertical)
        {
            Debug.Log("Tower is not aligned in straight direction. Skipping attack.");
            attacking = false;
            isStopped = false;
            yield break; // skip attacking, continue walking
        }

        Debug.Log("MeleeEnemy attacking aligned tower...");

        yield return new WaitForSeconds(attackDelay);

        if (tower != null)
        {
            IHealth towerHealth = tower.GetComponent<IHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(Damage);
                Debug.Log("MeleeEnemy damaged the tower!");
            }
        }

        // Wait for destruction before continuing
        while (tower != null)
        {
            yield return null;

            if (tower.GetComponent<IHealth>() == null)
                break;
        }

        attacking = false;
        isStopped = false;
        currentIndex++;
    }
}
