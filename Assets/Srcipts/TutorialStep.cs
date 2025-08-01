using System;
using UnityEngine;

[Serializable]
public class TutorialStep
{
    [TextArea]
    public string dialogText;
    public GameObject highlightTarget;
    public bool waitForClick = true;
    public float waitTime = 2f;

    public bool showPanel = true;
    public bool nextButton;
    public bool changePanelPosition = false;
    public bool flipDialogPosition = false;
    public bool flipCharacter = false;

    public UnityEngine.Events.UnityEvent onStepStart; // <-- Add this line
}
