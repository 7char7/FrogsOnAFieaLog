using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameButtons : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void BackToMenu()
    {
        ResetAllGameSystems();
        LoadMainMenu();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ResetAllGameSystems()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteReset();
        }

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ResetAllProgress();
        }

        if (NarratorManager.Instance != null)
        {
            NarratorManager.Instance.CompleteReset();
        }
    }

    private void LoadMainMenu()
    {
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeToScene(mainMenuSceneName, true);
        }
        else
        {
            RestoreCursor();
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
