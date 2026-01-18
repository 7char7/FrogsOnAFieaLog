using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class HealthBarManager : MonoBehaviour
{
    public Stats playerStatsScriptableObject;
    [SerializeField] private Image healthBar;
    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private float currentHealth;
    private Color fullHealthColor = Color.green;
    private Color midHealthColor = Color.yellow;
    private Color zeroHealthColor = Color.red;
    private Coroutine healthBarCoroutine;

    public void SetPlayerStatsScriptableObject(Stats stats)
    {
        playerStatsScriptableObject = stats;
    }

    public void UpdateHealthValue(float health)
    {
        healthText.text = $"{Mathf.CeilToInt(health)}";
    }

    public void StartHealthBarAnimation(float startingHealth, float currentHealth)
    {
        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }
        healthBarCoroutine = StartCoroutine(UpdateHealthBar(startingHealth, currentHealth));
    }

    private IEnumerator UpdateHealthBar(float startingHealth, float currentHealth)
    {
        float elapsedTime = 0f;
        float duration = 0.5f; // Duration of the animation        
        float startingFill = healthBar.fillAmount;
        float maxHealth = playerStatsScriptableObject.GetStat(Stat.maxHealth);
        float targetFillAmount = currentHealth / maxHealth;
        Color startingColor = healthBar.color;
        Color targetColor = GetHealthColor(targetFillAmount);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            healthBar.fillAmount = Mathf.Lerp(startingFill, targetFillAmount, t);
            healthBar.color = Color.Lerp(startingColor, targetColor, t);
            yield return null;
        }

        EnsureProperHealthBar(currentHealth);
        healthBarCoroutine = null;
    }

    private void EnsureProperHealthBar(float currentHealth)
    {
        float maxHealth = playerStatsScriptableObject.GetStat(Stat.maxHealth);
        healthBar.fillAmount = currentHealth / maxHealth;
        healthBar.color = GetHealthColor(healthBar.fillAmount);
    }

    private Color GetHealthColor(float healthPercent)
    {
        if (healthPercent >= 0.5f)
        {
            float t = (healthPercent - 0.5f) / (1f - 0.5f);
            return Color.Lerp(midHealthColor, fullHealthColor, t);
        }
        else if (healthPercent >= .25f)
        {
            float t = (healthPercent - 0.25f) / (0.5f - 0.25f);
            return Color.Lerp(zeroHealthColor, midHealthColor, t);
        }
        else
        {
            return zeroHealthColor;
        }
    }
}
