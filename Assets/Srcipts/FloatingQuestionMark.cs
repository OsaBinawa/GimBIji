using DG.Tweening;
using UnityEngine;

public class FloatingQuestionMark : MonoBehaviour
{
    public GameObject targetObject;     // The object you want to float
    public float floatDistance = 0.25f; // how far it moves up/down
    public float floatDuration = 1f;    // time for one up/down cycle

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("QuestMarkFloat: No target object assigned!");
            enabled = false;
            return;
        }

        Vector3 startPos = targetObject.transform.localPosition;

        // DOTween bobbing animation
        targetObject.transform.DOLocalMoveY(startPos.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
