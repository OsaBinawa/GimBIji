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
    [SerializeField] private List<GameObject> shopContent = new();
    [SerializeField] private GameObject vigShop;
    [SerializeField] private GameObject Shop;

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

    public void OpenShop()
    {
        if (Shop == null) return;
        vigShop.SetActive(true);
        // Activate shop so DOTween can run
        Shop.SetActive(true);

        // Get the RectTransform to animate
        RectTransform shopRect = Shop.GetComponent<RectTransform>();
        foreach (var obj in shopContent)
        {
            if (obj != null)
            {
                obj.SetActive(true); // Must be active for DOTween to run
                obj.transform.localScale = Vector3.zero; // Start hidden
            }
        }
        // Optional: disable shop content initially
       /* foreach (Transform child in shopRect)
        {
            child.gameObject.SetActive(false);
        }*/

        // Start scale at zero for animation
        shopRect.localScale = Vector3.zero;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            shopRect.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Enable content after 0.1 sec
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    foreach (var obj in shopContent)
                    {
                        if (obj != null)
                        {
                            obj.transform.DOScale(.5f, 0.25f)
                                .SetEase(Ease.OutBack)
                                .SetDelay(.5f);
                           
                        }
                    }
                });
            });
        });
    }

}

