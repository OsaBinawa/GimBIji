using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<GridTile> pathToFollow;
    public PathDrawer pathDrawer;
    public PlayerController playerController;
    public WaveManager waveManager;
    public Button startWaveButton;
    public bool nextWave;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int aliveEnemies = 0;

    public void StartSpawning(List<GridTile> path)
    {
        Debug.Log("Trying to start spawning with path count: " + (path != null ? path.Count : 0));

        if (isSpawning || path == null || path.Count < 2 || waveManager == null || waveManager.waves.Count == 0)
        {
            Debug.LogWarning("Cannot start spawning - path is null or too short, or waveManager is missing.");
            return;
        }

        pathToFollow = new List<GridTile>(path);
        currentWaveIndex = 0;
        isSpawning = true;
        ShowStartWaveButton();
    }

    public void StartSpawningFromPathDrawer()
    {
        Debug.Log("StartSpawningFromPathDrawer called.");
        StartSpawning(pathDrawer.pathTiles);
    }

    public void TriggerNextWave()
    {
        if (!isSpawning || currentWaveIndex >= waveManager.waves.Count) return;

        startWaveButton.interactable = false;
        StartCoroutine(SpawnWaveCoroutine(waveManager.waves[currentWaveIndex]));
    }

    IEnumerator SpawnWaveCoroutine(WaveData waveData)
    {
        aliveEnemies = 0;

        if (pathToFollow == null || pathToFollow.Count < 2)
        {
            Debug.LogWarning("Cannot spawn wave - pathToFollow is invalid.");
            yield break;
        }

        for (int i = 0; i < waveData.enemiesInWave; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, pathToFollow[0].transform.position, Quaternion.identity);
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            enemyScript.SetPath(pathToFollow);

            enemyScript.OnEnemyDied += HandleEnemyDeath;

            aliveEnemies++;
            yield return new WaitForSeconds(waveData.timeBetweenEnemies);
        }

        currentWaveIndex++;
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        deadEnemy.OnEnemyDied -= HandleEnemyDeath;
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            if (currentWaveIndex < waveManager.waves.Count)
            {
                StartCoroutine(WaitAndResetPath());
            }
            else
            {
                Debug.Log("All waves completed.");
                isSpawning = false;
            }
        }
    }

    IEnumerator WaitAndResetPath()
    {
        yield return new WaitForSeconds(2f);

        nextWave = true;

        if (pathDrawer != null)
        {
            pathDrawer.ClearPath();           // Clear visuals
            pathDrawer.EnableDrawing();       // Allow player to draw again
        }

        if (playerController != null)
        {
            playerController.ResetToStartPosition();
        }

        Debug.Log("Path reset. Waiting for player to draw a new path...");

        yield return StartCoroutine(WaitForNewPath());         // Wait until at least 2 tiles
        yield return StartCoroutine(WaitForPathToFinish());    // Wait until player finishes drawing

        nextWave = false;

        if (pathDrawer.pathTiles != null && pathDrawer.pathTiles.Count >= 2)
        {
            pathToFollow = new List<GridTile>(pathDrawer.pathTiles);
            Debug.Log("New path ready with count: " + pathToFollow.Count);
            //ShowStartWaveButton();
        }
        else
        {
            Debug.LogWarning("New path is still invalid. Waiting again...");
            yield return StartCoroutine(WaitAndResetPath()); // Retry
        }
    }

    IEnumerator WaitForNewPath()
    {
        while (pathDrawer.pathTiles == null || pathDrawer.pathTiles.Count < 2)
        {
            yield return null;
        }
    }

    IEnumerator WaitForPathToFinish()
    {
        while (pathDrawer.IsDrawing)
        {
            yield return null;
        }
    }

    void ShowStartWaveButton()
    {
        if (startWaveButton != null)
        {
            startWaveButton.interactable = true;
        }
    }
}
