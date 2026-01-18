using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using JetBrains.Annotations;

public class PlayerManager : MonoBehaviour
{
    public Stats playerStatsScriptableObject;
    public GameObject shotgunObject;
    public GameObject pickaxeObject;
    public GameObject torchObject;
    [SerializeField] private Image healthBar;
    [SerializeField] private float currentHealth;
    [SerializeField] private HealthBarManager healthBarManager;
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private float deathDelay = 1.5f;
    
    private Color fullHealthColor = Color.green;
    private Color midHealthColor = Color.yellow;
    private Color zeroHealthColor = Color.red;
    private Coroutine healthBarCoroutine;
    private bool isDead = false;

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
        effectiveDamage = Mathf.Max(effectiveDamage, 0);
        currentHealth = Mathf.Clamp(currentHealth - effectiveDamage, 0, playerStatsScriptableObject.GetStat(Stat.maxHealth));
        healthBarManager.UpdateHealthValue(currentHealth);
        healthBarManager.StartHealthBarAnimation(startingHealth, currentHealth);
        
        TriggerDamageFeedback();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDamagedSound();
        }
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    private void TriggerDamageFeedback()
    {
        if (DamageFlashEffect.Instance != null)
        {
            DamageFlashEffect.Instance.Flash();
        }
        
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.2f, 0.3f, 2f);
        }
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log("Player died!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteRunFailure();
        }
        
        StartCoroutine(HandleDeath());
    }
    
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(deathDelay);
        
        if (SceneFadeManager.Instance != null)
        {
            SceneFadeManager.Instance.FadeToScene(shopSceneName, true);
        }
        else
        {
            RestoreCursor();
            SceneManager.LoadScene(shopSceneName);
        }
    }
    
    private void RestoreCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
