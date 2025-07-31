using UnityEngine;

public class TowerButton : MonoBehaviour
{
    [SerializeField] public TowerData towerToSelect;

    public void OnClick()
    {
        TowerManager.Instance.SelectTower(towerToSelect);
        //TowerSelector.Instance.UpdateTowerUI(towerToSelect); // notify UI
    }
}
