using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardsUI : MonoBehaviour
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private TMP_Text TowerName;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text bulletText;
    [SerializeField] private Image iconImg;
  

    void Start()
    {
        TowerName.text = towerData.Name;
        damageText.text = towerData.Damage.ToString();
        bulletText.text = towerData.fireRate.ToString();
        iconImg.sprite = towerData.icon;
        
    }
}
