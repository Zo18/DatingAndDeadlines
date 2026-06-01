using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;

    void Start()
    {
        mainMenuPanel.SetActive(true);
    }

    public void OnNewGamePressed()
    {
        if (StatsManager.Instance != null)
            StatsManager.Instance.ResetStats();
        
        SceneManager.LoadScene(1);
    }

    public void OnSettingsPressed()
    {
        SceneManager.LoadScene(2);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}