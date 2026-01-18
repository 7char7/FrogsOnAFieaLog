using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    [Header("Death Settings")]
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private float deathDelay = 1.5f;
    
    [Header("Events")]
    public UnityEvent<int, int> OnHealthChanged;
    public UnityEvent<int> OnDamageTaken;
    public UnityEvent OnPlayerDeath;
    
    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnDamageTaken?.Invoke(damage);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPlayerDamagedSound();
        }
        
        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        OnPlayerDeath?.Invoke();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteRunFailure();
        }
        
        StartCoroutine(HandleDeath());
    }
    
    private System.Collections.IEnumerator HandleDeath()
    {
        yield return new UnityEngine.WaitForSeconds(deathDelay);
        
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
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0;
}
