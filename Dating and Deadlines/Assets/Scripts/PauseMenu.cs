using UnityEngine;
using UnityEngine.SceneManagement;
 
public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
 
    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenuScene";
 
    private bool isPaused = false;
 
    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }
 
    public void OnPauseClicked()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }
 
    public void OnResumeClicked()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
 
    public void OnSaveAndExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
 
    public void OnQuitWithoutSavingClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
 
    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                OnResumeClicked();
            else
                OnPauseClicked();
        }
    }
}