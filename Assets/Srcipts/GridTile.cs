using UnityEngine;

public enum TileType { Normal, Start, Finish }

[RequireComponent(typeof(SpriteRenderer))]
public class GridTile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType tileType;

    private SpriteRenderer sr;
    public Sprite[] basedSprites;
    [SerializeField] private int randomBasedSprite;
    public Sprite[] pathSprites;

    public bool IsOccupied {  get; private set; }
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateBaseColor();
    }

    private void Start()
    {
        randomBasedSprite = Random.Range(0, basedSprites.Length);
        sr.sprite = basedSprites[randomBasedSprite];
    }

    public void SetHighlight(bool isHighlighted)
    {
        int randomSprite = Random.Range(0, pathSprites.Length);

        if (isHighlighted)
            sr.sprite = pathSprites[randomSprite];
        else
            UpdateBaseColor();
    }

    private void UpdateBaseColor()
    {
        switch (tileType)
        {
            case TileType.Start:
                sr.color = Color.blue;
                break;
            case TileType.Finish:
                sr.color = Color.red;
                break;
            default:
                sr.sprite = basedSprites[randomBasedSprite];
                break;
        }
    }

    public void UpdateTileVisual()
    {
        UpdateBaseColor();
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
}
