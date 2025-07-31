using System.Collections;
using System.Linq;
using UnityEngine;

public class EnemyThief : Enemy
{
    [Tooltip("IDs of towers this enemy can walk through without attacking")]
    public int[] walkThroughTowerIDs;
    protected override IEnumerator AttackTower(GameObject tower)
    {
        TowerStats towerComp = tower.GetComponent<TowerStats>();

        if (towerComp != null && CanWalkThroughTower(towerComp.towerID))
        {
            Debug.Log($"GhostEnemy ignored tower ID {towerComp.towerID} and continued walking.");
            shouldSkipAttack = true; // tell FollowPath it's okay to continue
            attacking = false;
            isStopped = false;
            yield break;
        }

        // Normal attack
        Debug.Log("GhostEnemy attacking tower...");

        yield return new WaitForSeconds(attackDelay);

        if (tower != null)
        {
            IHealth towerHealth = tower.GetComponent<IHealth>();
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(Damage);
                Debug.Log("Tower took damage!");
            }
        }

        // Wait until tower is destroyed
        while (tower != null)
        {
            yield return null;

            if (tower.GetComponent<IHealth>() == null)
                break;
        }

        attacking = false;
        isStopped = false;
    }
    private bool CanWalkThroughTower(int towerID)
    {
        return walkThroughTowerIDs.Contains(towerID);
    }

}
