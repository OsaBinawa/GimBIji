using DG.Tweening;
using TMPro;
using UnityEngine;

public class WorkShopUI : MonoBehaviour
{
    [Header("Target UI Object")]
    public GameObject targetObject;
    [Header("Floating Settings")]
    public float floatDistance = 20f;
    public float floatDuration = 1f;
    [SerializeField] private TMP_Text UItext;
    [SerializeField] private int requiredAmount = 1;

    private RectTransform rect;
    [SerializeField] private int deliveredAmount = 0;
    private bool isCompleted = false;

    void Start()
    {
        if (targetObject == null)
        {
            enabled = false;
            return;
        }

        rect = targetObject.GetComponent<RectTransform>();
        if (rect == null)
        {
            enabled = false;
            return;
        }

        Vector2 startPos = rect.anchoredPosition;
        rect.DOAnchorPosY(startPos.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        UpdateUI();
    }
    private void Update()
    {
        //UpdateUI();
    }
    public void AddDelivered(int amount)
    {
        if (isCompleted) return;

        deliveredAmount += amount;
        if (deliveredAmount >= requiredAmount)
        {
            deliveredAmount = requiredAmount;
            isCompleted = true;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        UItext.text = $"{deliveredAmount}/{requiredAmount}";
    }
}

