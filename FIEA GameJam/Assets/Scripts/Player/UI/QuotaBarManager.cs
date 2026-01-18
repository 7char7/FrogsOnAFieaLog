using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuotaBarManager : MonoBehaviour
{
    [SerializeField] private Image quotaBar;
    [SerializeField] private TMPro.TextMeshProUGUI pointsText;
    [SerializeField] private float currentPoints;
    private Color emptyColor = Color.red;
    private Color midColor = Color.yellow;
    private Color fullColor = Color.green;
    private Coroutine quotaBarCoroutine;
    
    private ResourceManager resourceManager;
    private GameManager gameManager;
    private float quotaTarget;

    private void Start()
    {
        resourceManager = ResourceManager.Instance;
        gameManager = GameManager.Instance;
        
        if (gameManager != null)
        {
            quotaTarget = gameManager.QuotaTarget;
        }
        else
        {
            quotaTarget = 5000f;
            Debug.LogWarning("GameManager not found, using default quota target of 5000");
        }
        
        if (resourceManager != null)
        {
            resourceManager.OnPointsChanged += OnPointsChanged;
            InitializeQuotaBar();
        }
        else
        {
            Debug.LogError("ResourceManager not found! QuotaBar will not update.");
        }
    }
    
    private void OnDestroy()
    {
        if (resourceManager != null)
        {
            resourceManager.OnPointsChanged -= OnPointsChanged;
        }
    }
    
    private void InitializeQuotaBar()
    {
        int totalPoints = resourceManager.TotalPoints + resourceManager.CurrentRunPoints;
        UpdatePointsValue(totalPoints);
        EnsureProperQuotaBar(totalPoints);
    }
    
    private void OnPointsChanged(int currentRunPoints, int pointsEarned)
    {
        int totalPoints = resourceManager.TotalPoints + resourceManager.CurrentRunPoints;
        currentPoints = totalPoints;
        UpdatePointsValue(totalPoints);
        StartQuotaBarAnimation(totalPoints - pointsEarned, totalPoints);
    }
    
    public void UpdatePointsValue(float points)
    {
        if (pointsText != null)
        {
            pointsText.text = $"{Mathf.CeilToInt(points)}/{Mathf.CeilToInt(quotaTarget)}";
        }
    }

    public void StartQuotaBarAnimation(float startingPoints, float currentPoints)
    {
        if (quotaBarCoroutine != null)
        {
            StopCoroutine(quotaBarCoroutine);
        }
        quotaBarCoroutine = StartCoroutine(UpdateQuotaBar(startingPoints, currentPoints));
    }

    private IEnumerator UpdateQuotaBar(float startingPoints, float currentPoints)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startingFill = quotaBar.fillAmount;
        float targetFillAmount = Mathf.Clamp01(currentPoints / quotaTarget);
        Color startingColor = quotaBar.color;
        Color targetColor = GetQuotaColor(targetFillAmount);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            quotaBar.fillAmount = Mathf.Lerp(startingFill, targetFillAmount, t);
            quotaBar.color = Color.Lerp(startingColor, targetColor, t);
            yield return null;
        }

        EnsureProperQuotaBar(currentPoints);
        quotaBarCoroutine = null;
    }

    private void EnsureProperQuotaBar(float currentPoints)
    {
        quotaBar.fillAmount = Mathf.Clamp01(currentPoints / quotaTarget);
        quotaBar.color = GetQuotaColor(quotaBar.fillAmount);
    }

    private Color GetQuotaColor(float quotaPercent)
    {
        if (quotaPercent >= 0.5f)
        {
            float t = (quotaPercent - 0.5f) / (1f - 0.5f);
            return Color.Lerp(midColor, fullColor, t);
        }
        else if (quotaPercent >= 0.25f)
        {
            float t = (quotaPercent - 0.25f) / (0.5f - 0.25f);
            return Color.Lerp(emptyColor, midColor, t);
        }
        else
        {
            return emptyColor;
        }
    }
}
