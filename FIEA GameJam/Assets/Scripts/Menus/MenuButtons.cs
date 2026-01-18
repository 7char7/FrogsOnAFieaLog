using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
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

   public void LoadSummary()
   {
       if (SceneFadeManager.Instance != null)
       {
           SceneFadeManager.Instance.FadeToScene("Summary", true);
       }
       else
       {
           SceneManager.LoadScene("Summary");
       }
   }

   public void LoadFakePlaying()
   {
       if (SceneFadeManager.Instance != null)
       {
           SceneFadeManager.Instance.FadeToScene("FakePlaying", false);
       }
       else
       {
           SceneManager.LoadScene("FakePlaying");
       }
   }

   public void Quit()
   {
       Application.Quit();
   }

}
