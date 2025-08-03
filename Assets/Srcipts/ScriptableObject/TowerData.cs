using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    public string Name;
    public float HP;
    public float Damage;
    public float fireRate;
    public Sprite icon;
    public GameObject towerPref;
    public GameObject towerPreview;
    [SerializeField] private TowerType towerType;
    [SerializeField] private TileType[] allowedTileTypes;
    [SerializeField] private int rangeWidth = 2;
    [SerializeField] private int rangeHeight = 2;
    

    public int RangeWidth => rangeWidth;
    public int RangeHeight => rangeHeight;
    public TileType[] AllowedTileTypes => allowedTileTypes;


    public enum TowerType
    {
        Attack,
        Wall,
        Bomb,
    }

}
