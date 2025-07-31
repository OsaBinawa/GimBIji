using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;

    public TowerData SelectedTowerData { get; private set; }

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
}
