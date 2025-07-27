using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewTower", menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    public string Name;
    public float Damage;
    public float fireRate;
    public Sprite icon;
    public GameObject towerPref;
    [SerializeField] private TowerType towerType;
    [SerializeField] private TileType[] allowedTileTypes;
    public TileType[] AllowedTileTypes => allowedTileTypes;


    public enum TowerType
    {
        Attack,
        Wall,
        Bomb,
    }

}
