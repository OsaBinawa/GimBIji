using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;

    public List<GridTile> pathToFollow;
    public PathDrawer pathDrawer;
    public PlayerController playerController;
    public WaveManager waveManager;
    public Button startWaveButton;
    public bool nextWave;

    [SerializeField]public int currentWaveIndex = 0;
    private bool isSpawning = false;
    public bool playerReachEnd;
    //private int aliveEnemies = 0;

    public void StartSpawning(List<GridTile> path)
    {
        Debug.Log("Trying to start spawning with path count: " + (path != null ? path.Count : 0));

        if (isSpawning || path == null || path.Count < 2 || waveManager == null || waveManager.waves.Count == 0)
        {
            Debug.LogWarning("Cannot start spawning - path is null or too short, or waveManager is missing.");
            return;
        }

        pathToFollow = new List<GridTile>(path);
        //currentWaveIndex = 0;
        isSpawning = true;
        ShowStartWaveButton();
    }
    public void playerEnd(bool End)
    {
        playerReachEnd = End;
    }

    public void StartSpawningFromPathDrawer()
    {
        if (playerReachEnd)
        {
            Debug.Log("StartSpawningFromPathDrawer called.");
            StartSpawning(pathDrawer.pathTiles);
        }
    }

    public void TriggerNextWave()
    {
        if (playerReachEnd)
        {
            if (!isSpawning || currentWaveIndex >= waveManager.waves.Count) return;

            startWaveButton.interactable = false;
            StartCoroutine(SpawnWaveCoroutine(waveManager.waves[currentWaveIndex]));
        }
    }
    IEnumerator SpawnWaveCoroutine(WaveData waveData)
    {
        GameManager.Instance.StartWave(waveData.totalEnemies);
        //aliveEnemies = 0;

        // Check enemyPrefabs list
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("Enemy prefabs list is null or empty.");
            yield break;
        }

        // Check for null elements in enemyPrefabs
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i] == null)
            {
                Debug.LogError($"Enemy prefab at index {i} is null.");
                yield break;
            }
        }

        // Check pathToFollow
        if (pathToFollow == null || pathToFollow.Count < 2 || pathToFollow[0] == null)
        {
            Debug.LogError("Path to follow is invalid or contains null.");
            yield break;
        }

        int enemiesSpawned = 0;

        while (enemiesSpawned < waveData.totalEnemies)
        {
            int spawnCount = Mathf.Min(waveData.batchSize, waveData.totalEnemies - enemiesSpawned);

            for (int i = 0; i < spawnCount; i++)
            {
                int enemyIndex = enemiesSpawned;
                int typeIndex = 0;

                // Validate enemyTypeIndices
                if (waveData.enemyTypeIndices != null)
                {
                    if (enemyIndex < waveData.enemyTypeIndices.Count)
                    {
                        typeIndex = Mathf.Clamp(waveData.enemyTypeIndices[enemyIndex], 0, enemyPrefabs.Count - 1);
                    }
                    else
                    {
                        Debug.LogWarning($"enemyTypeIndices missing entry for index {enemyIndex}. Defaulting to 0.");
                    }
                }

                // Check typeIndex bounds
                if (typeIndex < 0 || typeIndex >= enemyPrefabs.Count)
                {
                    Debug.LogError($"Invalid typeIndex: {typeIndex}. enemyPrefabs count: {enemyPrefabs.Count}");
                    yield break;
                }

                GameObject prefabToSpawn = enemyPrefabs[typeIndex];

                if (prefabToSpawn == null)
                {
                    Debug.LogError($"Prefab at index {typeIndex} is null.");
                    yield break;
                }

                Vector3 spawnPosition = pathToFollow[0].transform.position;
                Debug.Log($"[SPAWN] Spawning enemy #{enemiesSpawned} | typeIndex: {typeIndex} | prefab: {prefabToSpawn.name}");

                GameObject enemyObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetPath(pathToFollow);
                    enemyScript.OnEnemyDied += HandleEnemyDeath;
                }
                else
                {
                    Debug.LogWarning("Spawned enemy does not have an Enemy script attached.");
                }

                //aliveEnemies++;
                enemiesSpawned++;

                if (i < spawnCount - 1)
                    yield return new WaitForSeconds(waveData.timeBetweenEnemies);
            }

            if (enemiesSpawned < waveData.totalEnemies)
                yield return new WaitForSeconds(waveData.delayBetweenBatches);
        }
        
        currentWaveIndex++;
        GameManager.Instance.UpdateWaveUI();
        isSpawning = false;
    }


    public void OnAllEnemiesDefeated()
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


    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        deadEnemy.OnEnemyDied -= HandleEnemyDeath;
        //aliveEnemies--;

        // 🔥 FIX: Notify the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyEnemyResolved(deadEnemy);
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null in HandleEnemyDeath");
        }

        /*if (aliveEnemies <= 0)
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
        }*/
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
