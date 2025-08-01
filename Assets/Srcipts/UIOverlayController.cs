using UnityEngine;
using UnityEngine.UI;

public class UIOverlayController : MonoBehaviour
{
    public Canvas canvas;
    public Image dimScreen;
    public Image highlightCircle;
    public void SetOverlay(bool enabled)
    {
        dimScreen.gameObject.SetActive(enabled);
    }
    public void Highlight(GameObject target)
    {
        if (target == null)
        {
            highlightCircle.gameObject.SetActive(false);
            return;
        }

        highlightCircle.gameObject.SetActive(true);
        RectTransform highlightRT = highlightCircle.GetComponent<RectTransform>();
        Vector3 screenPos;

        RectTransform targetRT = target.GetComponent<RectTransform>();
        if (targetRT != null)
        {
            // Use the canvas camera (or null if in Screen Space - Overlay mode)
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

            screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetRT.position);
        }
        else
        {
            screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        }

        highlightRT.position = screenPos;
    }

    public void ClearHighlight()
    {
        highlightCircle.gameObject.SetActive(false);
    }
}
