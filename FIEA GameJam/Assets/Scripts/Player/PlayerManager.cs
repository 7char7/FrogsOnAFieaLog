using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Stats playerStatsScriptableObject;

    [SerializeField] private float currentHealth;

    void Awake()
    {
        playerStatsScriptableObject = Instantiate(playerStatsScriptableObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = playerStatsScriptableObject.GetStat(Stat.maxHealth);
    }

    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = damage * playerStatsScriptableObject.GetStat(Stat.defense);
        effectiveDamage = Mathf.Max(effectiveDamage, 0); // Ensure damage is not negative
        currentHealth -= effectiveDamage;
        if (currentHealth <= 0)
        {
            //Die();
        }
    }
}
