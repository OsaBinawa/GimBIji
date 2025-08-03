using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    [Header("All Panels")]
    public List<GameObject> panels = new(); // Drag all your panels here in the Inspector

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
        SceneManager.LoadScene(Scenes);
    }
}
