using DG.Tweening;
using TMPro;
using UnityEngine;

public class WorkShopUI : MonoBehaviour
{
    [Header("Target UI Object")]
    public GameObject targetObject;     // UI element (Image, Panel, etc.)

    [Header("Floating Settings")]
    public float floatDistance = 20f;   // pixels to move up/down
    public float floatDuration = 1f;    // seconds for one cycle

    [SerializeField] private TMP_Text UItext;
    private RectTransform rect;
    private int requiredAmount = 1;
    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("QuestMarkFloatUI: No target object assigned!");
            enabled = false;
            return;
        }

        rect = targetObject.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogWarning("QuestMarkFloatUI: Target is not a UI element!");
            enabled = false;
            return;
        }

        Vector2 startPos = rect.anchoredPosition;

        // DOTween bobbing animation for UI
        rect.DOAnchorPosY(startPos.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void UpdateUI(int deliveredAmount)
    {
        UItext.text = $"{deliveredAmount}/{requiredAmount}";
    }
}
