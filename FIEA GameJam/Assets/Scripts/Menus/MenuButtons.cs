using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
   //loads main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //loads the shop/upgrades
    public void LoadShop()
    {
        SceneManager.LoadScene("Shop");
    }

    //loads summary page after dying or extracting
    public void LoadSummary()
    {
        SceneManager.LoadScene("Summary");
    }

    //loads testing gameplay scene
    public void LoadFakePlaying()
    {
        SceneManager.LoadScene("FakePlaying");
    }

    // Update is called once per frame
    public void Quit()
    {
        Application.Quit();
    }

}
