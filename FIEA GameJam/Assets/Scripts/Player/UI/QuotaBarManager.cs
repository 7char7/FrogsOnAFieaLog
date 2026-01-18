using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuotaBarManager : MonoBehaviour
{
    [SerializeField] private Image quotaBar;
    [SerializeField] private TMPro.TextMeshProUGUI pointsText;
    [SerializeField] private float currentPoints;
    private const float QUOTA_TARGET = 5000f;
    private Color emptyColor = Color.red;
    private Color midColor = Color.yellow;
    private Color fullColor = Color.green;
    private Coroutine quotaBarCoroutine;

    public void UpdatePointsValue(float points)
    {
        pointsText.text = $"{Mathf.CeilToInt(points)}";
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
        float targetFillAmount = Mathf.Clamp01(currentPoints / QUOTA_TARGET);
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
        quotaBar.fillAmount = Mathf.Clamp01(currentPoints / QUOTA_TARGET);
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
