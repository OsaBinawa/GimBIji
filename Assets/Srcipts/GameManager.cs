using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    EnemySpawner enemySpawner; 
    PlayerController playerController;
    public Slider WaveSlider;
    public float animationDuration = 0.5f;
    [SerializeField] Slider TowerBar;
    [SerializeField] TMP_Text TowerCountText;
    [SerializeField] private int remainingEnemies = 0;

    public int maxTowersAllowed = 5; 
    private int currentTowerCount = 0;
    void Awake()
    {
        Time.timeScale = 1f;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        playerController = FindAnyObjectByType<PlayerController>();
        enemySpawner = FindAnyObjectByType<EnemySpawner>();

        TowerBar.value = currentTowerCount;
        TowerBar.maxValue = maxTowersAllowed;
        WaveSlider.DOValue(enemySpawner.currentWaveIndex + 1,1);
        TowerCountText.text = $"{currentTowerCount} / {maxTowersAllowed}";
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

    public void UpdateWaveUI()
    {
        WaveSlider.DOValue(enemySpawner.currentWaveIndex + 1, 1);
    }

    public bool CanPlaceTower()
    {
        return currentTowerCount < maxTowersAllowed;
    }

    // ✅ Register a tower after it is successfully placed
    public void RegisterPlacedTower()
    {
        currentTowerCount++;
        TowerBar.DOValue(currentTowerCount, 1)
                .SetEase(Ease.OutSine);
        updateUI();
        Debug.Log("Tower placed. Total: " + currentTowerCount);
    }
    public void RemoveTower()
    {
        currentTowerCount = Mathf.Max(0, currentTowerCount - 1);
        updateUI();
        TowerBar.DOValue(currentTowerCount, 1)
                .SetEase(Ease.OutSine);
        Debug.Log("Tower removed. Total: " + currentTowerCount);
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
            if (enemySpawner.currentWaveIndex >= enemySpawner.waveManager.waves.Count)
            {
                Debug.Log("Win game"); //INI Win Game ya Nanti bikin method aja gpp nunggu UI dulu aku debug dulu no problem lah ya
            }
        }
    }

    void updateUI()
    {
        TowerCountText.text = $"{currentTowerCount} / {maxTowersAllowed}";
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

