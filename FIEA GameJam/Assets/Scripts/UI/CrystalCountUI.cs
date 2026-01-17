using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrystalCountUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private TextMeshProUGUI runsText;
    [SerializeField] private TextMeshProUGUI commonText;
    [SerializeField] private TextMeshProUGUI uncommonText;
    [SerializeField] private TextMeshProUGUI rareText;
    [SerializeField] private TextMeshProUGUI legendaryText;
    
    private ResourceManager resourceManager;
    private GameManager gameManager;
    
    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        gameManager = GameManager.Instance;
        
        if (resourceManager != null)
        {
            resourceManager.OnCrystalCollected += UpdateDisplay;
            resourceManager.OnPointsChanged += UpdatePointsDisplay;
        }
        
        UpdateDisplay(CrystalType.Common, 0);
    }
    
    private void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.OnCrystalCollected -= UpdateDisplay;
            resourceManager.OnPointsChanged -= UpdatePointsDisplay;
        }
    }
    
    private void UpdatePointsDisplay(int totalPoints, int pointsEarned)
    {
        UpdateDisplay(CrystalType.Common, 0);
    }
    
    private void UpdateDisplay(CrystalType type, int amount)
    {
        if (resourceManager == null)
            return;
        
        if (pointsText != null && gameManager != null)
        {
            int runPoints = resourceManager.CurrentRunPoints;
            int totalPoints = resourceManager.TotalPoints;
            int combinedPoints = totalPoints + runPoints;
            int quota = gameManager.QuotaTarget;
            
            pointsText.text = $"Run: {runPoints} | Total: {totalPoints} | Goal: {quota}";
            
            if (combinedPoints >= quota)
            {
                pointsText.color = Color.green;
            }
            else if (combinedPoints >= quota * 0.75f)
            {
                pointsText.color = Color.yellow;
            }
            else
            {
                pointsText.color = Color.white;
            }
        }
        
        if (quotaText != null && gameManager != null)
        {
            quotaText.text = $"Quota: {gameManager.QuotaTarget}";
        }
        
        if (runsText != null && gameManager != null)
        {
            runsText.text = $"Run: {gameManager.CurrentRun}/{gameManager.MaxRuns}";
        }
        
        if (commonText != null)
            commonText.text = $"Common: {resourceManager.GetCrystalCount(CrystalType.Common)} ({resourceManager.GetPointValue(CrystalType.Common)}pts)";
        
        if (uncommonText != null)
            uncommonText.text = $"Uncommon: {resourceManager.GetCrystalCount(CrystalType.Uncommon)} ({resourceManager.GetPointValue(CrystalType.Uncommon)}pts)";
        
        if (rareText != null)
            rareText.text = $"Rare: {resourceManager.GetCrystalCount(CrystalType.Rare)} ({resourceManager.GetPointValue(CrystalType.Rare)}pts)";
        
        if (legendaryText != null)
            legendaryText.text = $"Legendary: {resourceManager.GetCrystalCount(CrystalType.Legendary)} ({resourceManager.GetPointValue(CrystalType.Legendary)}pts)";
    }
}
