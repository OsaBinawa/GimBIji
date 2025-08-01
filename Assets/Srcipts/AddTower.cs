using UnityEngine;
using UnityEngine.UI;

public class AddTower : MonoBehaviour
{
    [SerializeField] private TowerData towerToAdd;
    [SerializeField] private TowerButtonSpawner buttonSpawner;
    [SerializeField] private Button addButton;

    public void OnClick()
    {
        if (TowerManager.Instance == null || towerToAdd == null)
        {
            Debug.LogWarning("TowerManager or TowerData missing.");
            return;
        }

        if (!TowerManager.Instance.availableTowers.Contains(towerToAdd))
        {
            TowerManager.Instance.availableTowers.Add(towerToAdd);
            Debug.Log("Tower added to available list: " + towerToAdd.name);
        }
        else
        {
            Debug.Log("Tower already in the list: " + towerToAdd.name);
        }
        addButton.interactable = false;
        buttonSpawner.SpawnTowerButtons();
        
    }

    public void RemoveTowerFromList()
    {
        if (TowerManager.Instance == null || towerToAdd == null)
        {
            Debug.LogWarning("TowerManager or TowerData missing.");
            return;
        }

        if (TowerManager.Instance.availableTowers.Contains(towerToAdd))
        {
            TowerManager.Instance.availableTowers.Remove(towerToAdd);
            Debug.Log("Tower removed: " + towerToAdd.name);

            // Optionally re-enable the add button so you can add again
            if (addButton != null)
                addButton.interactable = true;

            buttonSpawner.SpawnTowerButtons();
        }
        else
        {
            Debug.Log("Tower not found in the list: " + towerToAdd.name);
        }
    }
}
