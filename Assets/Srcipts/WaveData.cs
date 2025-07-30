using System.Collections.Generic;

[System.Serializable]
public class WaveData
{
    public int enemiesInWave;
    public float timeBetweenEnemies;
    public List<int> enemyTypeIndices; // Each int corresponds to an index in enemyPrefabs list
}
