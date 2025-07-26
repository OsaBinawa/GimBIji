using UnityEngine;

public enum TileType { Normal, Start, Finish }
public class GridTile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public TileType tileType;
    public SpriteRenderer sr;
    void Awake()
    {
        Vector3 pos = transform.position;
        gridPosition = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetHighlight(bool isHighlighted)
    {
        sr.color = isHighlighted ? Color.green : Color.white;
    }
}

