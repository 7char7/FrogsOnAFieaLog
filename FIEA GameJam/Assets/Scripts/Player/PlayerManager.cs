using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public Stats playerStatsScriptableObject;

    [SerializeField] private Image healthBar;
    [SerializeField] private float currentHealth;
    [SerializeField] private HealthBarManager healthBarManager;
    private Color fullHealthColor = Color.green;
    private Color midHealthColor = Color.yellow;
    private Color zeroHealthColor = Color.red;
    private Coroutine healthBarCoroutine;

    void Awake()
    {
        playerStatsScriptableObject = Instantiate(playerStatsScriptableObject);
        healthBarManager = GetComponentInChildren<HealthBarManager>();
        healthBarManager.SetPlayerStatsScriptableObject(playerStatsScriptableObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = playerStatsScriptableObject.GetStat(Stat.maxHealth);
        healthBarManager.UpdateHealthValue(currentHealth);
    }

    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        float startingHealth = currentHealth;
        float effectiveDamage = damage * playerStatsScriptableObject.GetStat(Stat.defense);
        effectiveDamage = Mathf.Max(effectiveDamage, 0); // Ensure damage is not negative
        currentHealth = Mathf.Clamp(currentHealth - effectiveDamage, 0, playerStatsScriptableObject.GetStat(Stat.maxHealth));
        healthBarManager.UpdateHealthValue(currentHealth);
        healthBarManager.StartHealthBarAnimation(startingHealth, currentHealth);
        if (currentHealth <= 0)
        {
            //Die();
        }
    }
}
