using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [Header("All Panels")]
    public List<GameObject> panels = new();

    public CanvasGroup fadeCanvas;

    public void ShowPanel(GameObject panelToShow)
    {
        foreach (var panel in panels)
            panel.SetActive(panel == panelToShow);
    }

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void GoToScene(string Scenes)
    {
        fadeCanvas.gameObject.SetActive(true);
        fadeCanvas.alpha = 0;
        fadeCanvas.DOFade(1, .2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                SceneManager.LoadScene(Scenes);
            });
    
    
    }
}
