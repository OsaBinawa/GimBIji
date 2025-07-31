using UnityEngine;
using UnityEngine.UI;

public class TowerButtonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;   // Prefab with TowerButton + Image + Text
    [SerializeField] private Transform buttonParent;    // UI parent (e.g., horizontal layout)

    private void Start()
    {
        SpawnTowerButtons();
    }

    public void SpawnTowerButtons()
    {
        foreach (TowerData towerData in TowerManager.Instance.availableTowers)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            TowerButton towerButton = btn.GetComponent<TowerButton>();
            towerButton.towerToSelect = towerData;

            // Optional UI setup
            Image icon = btn.GetComponentInChildren<Image>();
            if (icon != null) icon.sprite = towerData.icon;

            Text label = btn.GetComponentInChildren<Text>();
            if (label != null) label.text = towerData.Name;
        }
    }
}
