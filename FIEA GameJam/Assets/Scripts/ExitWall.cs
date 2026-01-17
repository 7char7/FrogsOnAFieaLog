using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitWall : MonoBehaviour
{
    [Header("Exit Settings")]
    [SerializeField] private bool requireAllCrystals = false;
    [SerializeField] private string exitSceneName = "MainMenu";

    private MineTimer mineTimer;

    private void Start()
    {
        mineTimer = FindFirstObjectByType<MineTimer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerReachedExit();
        }
    }

    private void OnPlayerReachedExit()
    {
        if (mineTimer != null)
        {
            mineTimer.PauseTimer();
        }

        RestoreCursor();

        Debug.Log("Player reached the exit! Returning to shop...");
        SceneManager.LoadScene(exitSceneName);
    }

    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
