using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /* ======================
     * Gameplay / Run Settings
     * ====================== */
    [Header("Gameplay Settings")]
    [SerializeField] private int quotaTarget = 5000;
    [SerializeField] private int maxRuns = 3;
    [SerializeField] private int currentRun = 1;

    public int QuotaTarget => quotaTarget;
    public int MaxRuns => maxRuns;
    public int CurrentRun => currentRun;
    public int RemainingRuns => maxRuns - currentRun + 1;

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
        Debug.Log(
            $"Starting run {currentRun} of {maxRuns}. " +
            $"Total accumulated: {ResourceManager.Instance?.TotalPoints ?? 0}"
        );
    }

    public void CompleteRunSuccess()
    {
        if (ResourceManager.Instance == null) return;

        ResourceManager.Instance.SaveRunProgress();
        int totalPoints = ResourceManager.Instance.TotalPoints;

        if (totalPoints >= quotaTarget)
        {
            Debug.Log(
                $"<color=green>🎉 YOU WIN! 🎉</color> Reached quota with {totalPoints} points!"
            );

            money += totalPoints;
            ResetRuns();
            ResourceManager.Instance.ResetAllProgress();
        }
        else
        {
            currentRun++;

            if (currentRun > maxRuns)
            {
                Debug.Log(
                    $"<color=red>Game Over!</color> Failed quota: {totalPoints}/{quotaTarget}"
                );

                ResetRuns();
                ResourceManager.Instance.ResetAllProgress();
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

            ResetRuns();
            ResourceManager.Instance.ResetAllProgress();
        }
        else
        {
            Debug.Log($"Run failed. Runs remaining: {RemainingRuns}");
        }
    }

    private void ResetRuns()
    {
        currentRun = 1;
    }
}
