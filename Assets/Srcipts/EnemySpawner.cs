using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<GridTile> pathToFollow;

    public int enemiesPerWave = 4;
    public float timeBetweenWaves = 5f;
    public float timeBetweenEnemy;
    public int totalEnemies = 20;

    private int spawnedEnemies = 0;
    private bool isSpawning = false;public PathDrawer pathDrawer;

    public void StartSpawning(List<GridTile> path)
    {
        if (isSpawning || path == null || path.Count == 0)
        {
            Debug.LogWarning("Cannot start spawning - path invalid or already spawning.");
            return;
        }

        pathToFollow = new List<GridTile>(path);
        spawnedEnemies = 0;
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        isSpawning = true;

        while (spawnedEnemies < totalEnemies)
        {
            int batchCount = Mathf.Min(enemiesPerWave, totalEnemies - spawnedEnemies);

            for (int i = 0; i < batchCount; i++)
            {
                GameObject enemyObj = Instantiate(enemyPrefab, pathToFollow[0].transform.position, Quaternion.identity);
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                enemyScript.SetPath(pathToFollow);
                spawnedEnemies++;
                yield return new WaitForSeconds(timeBetweenEnemy); // small delay between each enemy in a wave
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        isSpawning = false;
    }
    public void StartSpawningFromPathDrawer()
    {
        StartSpawning(pathDrawer.pathTiles);
    }
}
