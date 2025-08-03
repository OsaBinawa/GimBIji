using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public WaveManager WaveManager;
    EnemySpawner enemySpawner; 
    PlayerController playerController;
    public Slider WaveSlider;
    public float animationDuration = 0.5f;
    [SerializeField] Slider TowerBar;
    [SerializeField] TMP_Text TowerCountText;
    [SerializeField] private int remainingEnemies = 0;
    [SerializeField] TextMeshProUGUI enemyCountText;
    [SerializeField] GameObject LosePanel;
    [SerializeField] GameObject WinPanel;
    [SerializeField] TMP_Text AllTowerCountText;
    [SerializeField] Text waveText;
    public int maxTowerSelected;
    public int maxTowersAllowed = 5; 
    public int currentTowerCount = 0;
    public CanvasGroup fadeCanvas;
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

    private void Update()
    {
        enemyCountText.text = remainingEnemies.ToString();
        AllTowerCountText.text = $"{TowerManager.Instance.availableTowers.Count}/{maxTowerSelected}";
        TowerBar.value = currentTowerCount;
        TowerBar.maxValue = maxTowersAllowed;
    }

    public void UpdateWaveUI()
    {
        WaveSlider.DOValue(enemySpawner.currentWaveIndex + 1, 1);
    }

    public bool CanPlaceTower()
    {
        return currentTowerCount < maxTowersAllowed;
    }

    
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
            DestroyAllTowers();
            enemySpawner.OnAllEnemiesDefeated();
            if (enemySpawner.currentWaveIndex >= enemySpawner.waveManager.waves.Count)
            {
                WinPanel.SetActive(true);
                WinPanel.transform.localScale = Vector3.zero;
                WinPanel.transform.DOScale(Vector3.one, .2f)
                    .SetEase(Ease.OutSine)
                    .SetUpdate(true);
                Debug.Log("Win game"); 
            }
        }
    }

    public void DestroyAllTowers()
    {
        TowerStats[] allTowers = FindObjectsByType<TowerStats>(FindObjectsSortMode.None);
        foreach (var tower in allTowers)
        {
            GridTile tile = tower.GetComponentInParent<GridTile>();
            if (tile != null && (tile.tileType == TileType.Start || tile.tileType == TileType.Finish))
                continue;
            tower.OnDestroyButton(); 
        }

        
        currentTowerCount = 0;
        updateUI();
        if (enemySpawner != null)
        {
            enemySpawner.playerEnd(false);
            Debug.Log("Reset playerReachEnd to false because all towers were destroyed.");
        }
        TowerBar.DOValue(currentTowerCount, 0.5f).SetEase(Ease.OutSine);
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0;

        DOTween.Sequence()
            .Append(fadeCanvas.DOFade(1, 0.2f).SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                if (waveText != null && enemySpawner != null)
                {
                    waveText.gameObject.SetActive(true); // Show text
                    int waveNumber = enemySpawner.currentWaveIndex + 1;
                    waveText.text = ""; // start empty
                    waveText.DOText($"Wave {waveNumber}", 1f, true, ScrambleMode.None);
                }
            })
            .AppendInterval(2f)
            .Append(fadeCanvas.DOFade(0, 0.2f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                if (waveText != null)
                    fadeCanvas.gameObject.SetActive(false);
                    waveText.gameObject.SetActive(false);
                
            });
    }


    public void updateUI()
    {
        TowerCountText.text = $"{currentTowerCount} / {maxTowersAllowed}";
        TowerBar.DOValue(currentTowerCount, 1)
                .SetEase(Ease.OutSine);
    }
    public void GameOver()
    {
        if (playerController.CurrentHealth <= 0)
        {
            LosePanel.SetActive(true);
            LosePanel.transform.localScale = Vector3.zero;
            LosePanel.transform.DOScale(Vector3.one, .2f)
                .SetEase(Ease.OutSine)
                .SetUpdate(true);
            Debug.Log("GameOver");
            Time.timeScale = 0f;
        }
    }
    public void RetryGame()
    {
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0;

        fadeCanvas.DOFade(1, .2f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

}

