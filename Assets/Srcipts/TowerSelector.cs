using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelector : MonoBehaviour
{
    private TowerStats selectedTower;
    public static TowerSelector Instance;

    public bool isSelectingTile;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Avoid clicks through UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(worldClick.x, worldClick.y);

            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            if (hit.collider != null)
            {
                TowerStats tower = hit.collider.GetComponent<TowerStats>();
                if (tower != null)
                {
                    SelectTower(tower);
                    return;
                }
            }

            if (!isSelectingTile)
            {

                DeselectTower();
            }
        }
    }

    void SelectTower(TowerStats tower)
    {
        if (selectedTower != null && selectedTower != tower)
            selectedTower.HideUI();

        selectedTower = tower;
        selectedTower.ShowUI();
    }

    void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.HideUI();
            selectedTower = null;
        }
    }
}
