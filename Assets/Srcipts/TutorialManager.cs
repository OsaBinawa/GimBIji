using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public List<TutorialStep> steps;
    public GameObject dimScreen;
    public GameObject dialogPanel;
    public Vector2 newPanelPost = new Vector2(0, 0); // adjust to your setup
    public Vector2 defaultPanelPosition = new Vector2(0, 0); // adjust to your setup
    public TextMeshProUGUI dialogText;
    public UIOverlayController overlay;
    public float waitingTime;

    private int currentStep = 0;
    private bool waitingForClick = false; 
    public RectTransform characterImage; // Assign the portrait RectTransform
    public Vector3 defaultScale = Vector3.one; // Used to flip
    public Vector2 defaultCharacterPosition = new Vector2(-300, 0); // adjust to your setup

    public GameObject nextButton;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RunStep());
    }
    IEnumerator RunStep()
    {
        RectTransform dialogRect = dialogPanel.GetComponent<RectTransform>();
        while (currentStep < steps.Count)
        {
            TutorialStep step = steps[currentStep];

            dimScreen.SetActive(true);

            step.onStepStart?.Invoke();

            dialogPanel.SetActive(true);
            dialogText.text = step.dialogText;

            if (step.flipDialogPosition)
            {
                characterImage.anchoredPosition = new Vector2(-defaultCharacterPosition.x, defaultCharacterPosition.y);
            }
            else
            {
                characterImage.anchoredPosition = defaultCharacterPosition;
            }

            if (step.showPanel)
            {
                dialogPanel.SetActive(true);
            }
            else
            {
                dialogPanel.SetActive(false);
            }

            if (step.nextButton)
            {
                nextButton.SetActive(true);
            }
            else
            {
                nextButton.SetActive(false);
            }

            if (step.changePanelPosition)
            {
                dialogRect.anchoredPosition = new Vector2(defaultPanelPosition.x, newPanelPost.y);
            }
            else
            {
                dialogRect.anchoredPosition = defaultPanelPosition;
            }

            characterImage.localScale = new Vector3(
                step.flipCharacter ? -defaultScale.x : defaultScale.x,
                defaultScale.y,
                defaultScale.z
            );

            overlay.SetOverlay(true);
            overlay.Highlight(step.highlightTarget);

            if (step.waitForClick)
            {
                waitingForClick = true;
                yield return new WaitUntil(() => !waitingForClick);
            }
            else
            {
                float waitTime = step.waitTime;
                for (float t = waitTime; t > 0; t -= 1f)
                {
                    Debug.Log($"[Tutorial] Continuing in {t} second(s)...");
                    yield return new WaitForSeconds(1f);
                }
            }


            overlay.ClearHighlight();
            dialogPanel.SetActive(false);

            currentStep++;
        }

        overlay.SetOverlay(false);
        dimScreen.SetActive(false); // optional: hide at the very end
    }
    public void NotifyClicked()
    {
        Debug.Log("Clicked");
        waitingForClick = false;
    }

    public void Show(bool show)
    {
        overlay.SetOverlay(show);
    }
}
