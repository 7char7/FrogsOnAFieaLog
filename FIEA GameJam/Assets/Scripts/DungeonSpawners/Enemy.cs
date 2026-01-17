using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Visual")]
    [SerializeField] private Color debugColor = Color.red;
    
    [Header("Events")]
    public UnityEvent<int> OnDamageTaken;
    public UnityEvent OnDeath;
    
    private int currentHealth;

    public EnemyType Type => enemyType;
    public int Health => currentHealth;
    public int MaxHealth => maxHealth;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnDamageTaken?.Invoke(damageAmount);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        switch (enemyType)
        {
            case EnemyType.Shallow:
                maxHealth = 50;
                damage = 5;
                moveSpeed = 2f;
                debugColor = new Color(0.8f, 0.8f, 0.8f);
                break;
            case EnemyType.Medium:
                maxHealth = 100;
                damage = 10;
                moveSpeed = 3f;
                debugColor = new Color(0.3f, 0.6f, 1f);
                break;
            case EnemyType.Deep:
                maxHealth = 150;
                damage = 20;
                moveSpeed = 4f;
                debugColor = new Color(1f, 0.2f, 0.2f);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

public enum EnemyType
{
    Shallow,
    Medium,
    Deep
}
