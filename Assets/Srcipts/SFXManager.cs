using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    [Header("Sounds")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public AudioSource audioSource;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        List<Button> allButtons = new List<Button>();
        foreach (Button btn in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (btn.gameObject.hideFlags == HideFlags.None)
            {
                allButtons.Add(btn);
            }
        }

        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() => PlaySFX(clickSound));

            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) => { PlaySFX(hoverSound); });
            trigger.triggers.Add(entry);
        }
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
