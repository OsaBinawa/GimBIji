using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public EnemySpawner enemySpawner; 

    [SerializeField] private int remainingEnemies = 0;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartWave(int totalEnemies)
    {
        remainingEnemies = totalEnemies;
        Debug.Log("Wave started with enemies: " + totalEnemies);
    }

    public void NotifyEnemyResolved(Enemy enemy)
    {
        remainingEnemies--;
        Debug.Log("Enemy resolved. Remaining: " + remainingEnemies);

        if (remainingEnemies <= 0)
        {
            enemySpawner.OnAllEnemiesDefeated();
        }
    }


}

