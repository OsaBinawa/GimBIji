using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class UImanagers : MonoBehaviour
{
    [System.Serializable]
    public class UIEntry
    {
        public RectTransform uiElement;
        public SlideDirection direction;
    }

    public enum SlideDirection { Left, Right, Top, Bottom }

    [SerializeField] private List<UIEntry> uiElements = new();
    [SerializeField] private float offset = 300f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutBack;

    void Start()
    {
        foreach (UIEntry entry in uiElements)
        {
            if (entry.uiElement == null) continue;

            Vector3 original = entry.uiElement.localPosition;
            Vector3 startPos = GetStartPosition(original, entry.uiElement, entry.direction);

            entry.uiElement.localPosition = startPos;
            entry.uiElement.DOLocalMove(original, duration).SetEase(ease);
        }
    }

    Vector3 GetStartPosition(Vector3 original, RectTransform ui, SlideDirection direction)
    {
        Vector3 start = original;

        switch (direction)
        {
            case SlideDirection.Left:
                start.x -= offset + ui.rect.width;
                break;
            case SlideDirection.Right:
                start.x += offset + ui.rect.width;
                break;
            case SlideDirection.Top:
                start.y += offset + ui.rect.height;
                break;
            case SlideDirection.Bottom:
                start.y -= offset + ui.rect.height;
                break;
        }

        return start;
    }
}

