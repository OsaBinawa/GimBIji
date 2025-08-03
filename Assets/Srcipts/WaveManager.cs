using System.Collections.Generic;
//using UnityEditor.Overlays;
using UnityEngine;

[CreateAssetMenu(menuName = "Wave Manager")]
public class WaveManager : ScriptableObject
{
    public List<WaveData> waves = new();

    public List<ResoucePosition> CurrentWaveResources { get; private set; } = new();

    public void SetCurrentWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < waves.Count)
        {
            // Copy resources from selected wave
            CurrentWaveResources = new List<ResoucePosition>(waves[waveIndex].Resources);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.maxTowersAllowed = waves[waveIndex].maxTowersAllowed;
                GameManager.Instance.updateUI();
            }

            // Update GridManager too, if needed
            if (GridManager.instance != null)
            {
                GridManager.instance.Resources = new List<ResoucePosition>(CurrentWaveResources);
            }
        }
    }
}

