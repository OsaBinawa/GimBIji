using UnityEngine;
using UnityEngine.UI;

public class TowerButtonSpawner : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Start()
    {
        SpawnTowerButtons();
    }

    public void SpawnTowerButtons()
    {
        // Remove old UI buttons
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // Spawn buttons for exactly what's in availableTowers
        foreach (TowerData towerData in TowerManager.Instance.availableTowers)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            TowerButton towerButton = btn.GetComponent<TowerButton>();
            towerButton.towerToSelect = towerData;

            if (towerButton.towerIcon != null)
            {
                towerButton.towerIcon.sprite = towerData.icon;
            }

            Text label = btn.GetComponentInChildren<Text>();
            if (label != null) label.text = towerData.Name;
        }
    }
}
