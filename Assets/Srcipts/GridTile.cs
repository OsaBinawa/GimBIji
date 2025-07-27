using UnityEngine;

public enum TileType { Normal, Start, Finish }

[RequireComponent(typeof(SpriteRenderer))]
public class GridTile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType tileType;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateBaseColor();
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (isHighlighted)
            sr.color = Color.green;
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
                sr.color = Color.white;
                break;
        }
    }

    public void UpdateTileVisual()
    {
        UpdateBaseColor();
    }
}
