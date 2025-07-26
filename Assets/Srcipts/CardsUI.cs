using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardsUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TowerData towerData;
    [SerializeField] private TMP_Text TowerName;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text bulletText;
    [SerializeField] private Image iconImg;
    [SerializeField] private Canvas canvas; 

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = transform.position;
    }

    void Start()
    {
        TowerName.text = towerData.Name;
        damageText.text = towerData.Damage.ToString();
        bulletText.text = towerData.fireRate.ToString();
        iconImg.sprite = towerData.icon;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Vector3 screenPosition = eventData.position;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f; 

        if (towerData.towerPref != null)
        {
            Instantiate(towerData.towerPref, worldPosition, Quaternion.identity);
            Debug.Log("Spawned tower at: " + worldPosition);
        }
        else
        {
            Debug.LogWarning("Tower prefab is missing in TowerData!");
        }
        rectTransform.position = originalPosition;
    }

}
