using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /* ======================
     * Gameplay / Run Settings
     * ====================== */
    [Header("Gameplay Settings")]
    [SerializeField] private int quotaTarget = 3000;
    [SerializeField] private int maxRuns = 3;
    [SerializeField] private int currentRun = 1;

    [Header("Scene Settings")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string lossSceneName = "LossScene";

    public int QuotaTarget => quotaTarget;
    public int MaxRuns => maxRuns;
    public int CurrentRun => currentRun;
    public int RemainingRuns => maxRuns - currentRun + 1;
    
    public bool HasWon { get; private set; }
    public bool HasLost { get; private set; }
    
    private bool isRunComplete = false;

    /* ======================
     * Gem Tracking (Legacy)
     * ====================== */
    [Header("Gem Tracking (Legacy)")]
    public int gemsMinedRed;
    public int gemsMinedGreen;
    public int gemsMinedBlue;

    /* ======================
     * Upgrades
     * ====================== */
    [Header("Upgrades")]
    public int shotgunLevel = 0;
    public int pickaxeLevel = 0;
    public int playerHealthLevel = 0;
    public int playerSpeedLevel = 0;
    public int playerDefenceLevel = 0;
    public int torchLimitLevel = 0;

    /* ======================
     * Money
     * ====================== */
    [Header("Money")]
    public int money = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* ======================
     * Run Flow
     * ====================== */

    public void StartNewRun()
    {
        isRunComplete = false;
        
        Debug.Log(
            $"Starting run {currentRun} of {maxRuns}. " +
            $"Total accumulated: {ResourceManager.Instance?.TotalPoints ?? 0}"
        );
    }

    public void CompleteRunSuccess()
    {
        if (isRunComplete)
        {
            Debug.LogWarning("Run already completed - ignoring duplicate call to CompleteRunSuccess");
            return;
        }
        
        isRunComplete = true;
        
        if (ResourceManager.Instance == null) return;

        ResourceManager.Instance.SaveRunProgress();
        int totalPoints = ResourceManager.Instance.TotalPoints;

        if (totalPoints >= quotaTarget)
        {
            Debug.Log(
                $"<color=green>🎉 YOU WIN! 🎉</color> Reached quota with {totalPoints} points!"
            );

            HasWon = true;
        }
        else
        {
            currentRun++;

            if (currentRun > maxRuns)
            {
                Debug.Log(
                    $"<color=red>Game Over!</color> Failed quota: {totalPoints}/{quotaTarget}"
                );

                HasLost = true;
            }
            else
            {
                Debug.Log(
                    $"Run complete. Total: {totalPoints}/{quotaTarget}. " +
                    $"Next run: {currentRun}/{maxRuns}"
                );
            }
        }
    }

    public void CompleteRunFailure()
    {
        if (isRunComplete)
        {
            Debug.LogWarning("Run already completed - ignoring duplicate call to CompleteRunFailure");
            return;
        }
        
        isRunComplete = true;
        
        if (ResourceManager.Instance == null) return;

        int lostPoints = ResourceManager.Instance.CurrentRunPoints;

        Debug.Log(
            $"<color=red>⏰ Time's Up!</color> Lost {lostPoints} points this run."
        );

        ResourceManager.Instance.ResetSession();
        currentRun++;

        if (currentRun > maxRuns)
        {
            int totalPoints = ResourceManager.Instance.TotalPoints;

            Debug.Log(
                $"<color=red>Game Over!</color> Final total: {totalPoints}/{quotaTarget}"
            );

            HasLost = true;
        }
        else
        {
            Debug.Log($"Run failed. Runs remaining: {RemainingRuns}");
        }
    }

    public void LoadWinScene()
    {
        money += ResourceManager.Instance.TotalPoints;
        ResetGameState();
        
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeToScene(winSceneName, true);
        }
        else
        {
            RestoreCursor();
            SceneManager.LoadScene(winSceneName);
        }
    }

    public void LoadLossScene()
    {
        ResetGameState();
        
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeToScene(lossSceneName, true);
        }
        else
        {
            RestoreCursor();
            SceneManager.LoadScene(lossSceneName);
        }
    }

    private void ResetGameState()
    {
        currentRun = 1;
        HasWon = false;
        HasLost = false;
        isRunComplete = false;
        
        shotgunLevel = 0;
        pickaxeLevel = 0;
        playerHealthLevel = 0;
        playerSpeedLevel = 0;
        playerDefenceLevel = 0;
        torchLimitLevel = 0;
        
        gemsMinedRed = 0;
        gemsMinedGreen = 0;
        gemsMinedBlue = 0;
        
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ResetAllProgress();
        }
    }

    public void CompleteReset()
    {
        currentRun = 1;
        HasWon = false;
        HasLost = false;
        isRunComplete = false;
        
        money = 0;
        
        shotgunLevel = 0;
        pickaxeLevel = 0;
        playerHealthLevel = 0;
        playerSpeedLevel = 0;
        playerDefenceLevel = 0;
        torchLimitLevel = 0;
        
        gemsMinedRed = 0;
        gemsMinedGreen = 0;
        gemsMinedBlue = 0;
        
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.ResetAllProgress();
        }

        Debug.Log("Complete game reset - all stats and progress cleared");
    }

    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
