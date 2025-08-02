using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    public TowerData SelectedTowerData { get; private set; }

    [Header("Towers Available To Player")]
    public List<TowerData> availableTowers = new(); // Assign in Inspector or dynamically

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectTower(TowerData data)
    {
        SelectedTowerData = data;
        Debug.Log("Selected tower: " + data.name);
    }

    private void OnValidate()
    {
        HashSet<TowerData> uniqueSet = new HashSet<TowerData>(availableTowers);
        if (uniqueSet.Count != availableTowers.Count)
        {
            availableTowers = new List<TowerData>(uniqueSet);
            Debug.LogWarning("Duplicate towers removed from availableTowers list.");
        }
    }

    public bool TryAddTower(TowerData tower)
    {
        // Check limit
        if (availableTowers.Count >= GameManager.Instance.maxTowerSelected)
        {
            Debug.Log("❌ Cannot add more towers — reached limit of " + GameManager.Instance.maxTowerSelected);
            return false;
        }

        // Check duplicates
        if (availableTowers.Contains(tower))
        {
            Debug.Log("⚠ Tower already in the list: " + tower.name);
            return false;
        }

        // Add tower
        availableTowers.Add(tower);
        Debug.Log("✅ Added tower: " + tower.name);
        return true;
    }
}
