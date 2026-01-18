using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel;
    [SerializeField] private GameObject controlsPanel;
    public void LoadMainMenu()
    {
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeToScene("MainMenu", true);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void ToggleHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            if (howToPlayPanel.activeSelf)
            {
                howToPlayPanel.SetActive(false);
                return;
            }
            else
            {
                howToPlayPanel.SetActive(true);
                return;
            }
        }
    }

    public void ToggleControls()
    {
        if (controlsPanel != null)
        {
            if (controlsPanel.activeSelf)
            {
                controlsPanel.SetActive(false);
                return;
            }
            else
            {
                controlsPanel.SetActive(true);
                return;
            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
