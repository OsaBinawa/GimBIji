using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    EnemySpawner enemySpawner; 
    PlayerController playerController;

    [SerializeField] private int remainingEnemies = 0;
    
    
    void Awake()
    {
        Time.timeScale = 1f;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        playerController = FindAnyObjectByType<PlayerController>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();
    }
    /*private void Update()
    {
        if (enemySpawner != null && enemySpawner.waveManager != null)
        {
            int index = enemySpawner.currentWaveIndex;
            int total = enemySpawner.waveManager.waves.Count;
            Debug.Log($"[DEBUG] Current Wave: {index + 1} / {total}");
        }
    }*/

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
            if (enemySpawner.currentWaveIndex >= enemySpawner.waveManager.waves.Count)
            {
                Debug.Log("Win game"); //INI Win Game ya Nanti bikin method aja gpp nunggu UI dulu aku debug dulu no problem lah ya
            }
        }
    }

    public void GameOver()
    {
        if (playerController.CurrentHealth <= 0)
        {
            Debug.Log("GameOver");
            Time.timeScale = 0f;
        }
    }
}

