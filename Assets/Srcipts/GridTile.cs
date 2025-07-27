using UnityEngine;

public enum TileType { Normal, Start, Finish }

[RequireComponent(typeof(SpriteRenderer))]
public class GridTile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType tileType;

    private SpriteRenderer sr;
    public Sprite[] Sprite;
    public bool IsOccupied {  get; private set; }
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateBaseColor();
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (isHighlighted)
            sr.sprite = Sprite[1];
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
                sr.sprite = Sprite[0];
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
