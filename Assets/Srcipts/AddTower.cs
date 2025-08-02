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

        // Limit based on GameManager.maxTowerSelected
        if (TowerManager.Instance.availableTowers.Count >= GameManager.Instance.maxTowerSelected)
        {
            Debug.Log(" Cannot add more towers — limit reached (" + GameManager.Instance.maxTowerSelected + ")");
            return;
        }

        // Prevent duplicates
        if (TowerManager.Instance.availableTowers.Contains(towerToAdd))
        {
            Debug.Log("Tower already in selection: " + towerToAdd.name);
            return;
        }

        // Add tower to the list
        TowerManager.Instance.availableTowers.Add(towerToAdd);
        Debug.Log(" Added tower: " + towerToAdd.name);
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
