using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitWall : MonoBehaviour
{
    [Header("Exit Settings")]
    [SerializeField] private string shopSceneName = "Shop";

    private MineTimer mineTimer;
    private ResourceManager resourceManager;
    private GameManager gameManager;

    private void Start()
    {
        mineTimer = FindFirstObjectByType<MineTimer>();
        resourceManager = ResourceManager.Instance;
        gameManager = GameManager.Instance;
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

        if (resourceManager == null || gameManager == null)
        {
            Debug.LogWarning("ResourceManager or GameManager not found!");
            ReturnToShop();
            return;
        }

        int runPoints = resourceManager.CurrentRunPoints;
        int totalPoints = resourceManager.TotalPoints + runPoints;
        int quotaTarget = gameManager.QuotaTarget;

        Debug.Log($"Exiting mine. This run: {runPoints} points. Total will be: {totalPoints}/{quotaTarget}");
        
        gameManager.CompleteRunSuccess();
        ReturnToShop();
    }

    private void ReturnToShop()
    {
        RestoreCursor();
        SceneManager.LoadScene(shopSceneName);
    }

    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
