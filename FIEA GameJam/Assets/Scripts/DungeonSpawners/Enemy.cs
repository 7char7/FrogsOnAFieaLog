using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damage = 10;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Visual")]
    [SerializeField] private Color debugColor = Color.red;
    
    [Header("Events")]
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;
    

    public EnemyType Type => enemyType;
    public float Health => currentHealth;
    public float MaxHealth => maxHealth;
    public float Damage => damage;
    public float MoveSpeed => moveSpeed;
    
    private void Start()
    {
        currentHealth = maxHealth;
        EnsureHitboxComponents();
    }
    
    private void EnsureHitboxComponents()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag("Enemy") && col.GetComponent<EnemyHitbox>() == null)
            {
                col.gameObject.AddComponent<EnemyHitbox>();
            }
        }
    }
    
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(0, currentHealth);
        
        OnDamageTaken?.Invoke(damageAmount);
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyHitSound();
        }
        
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
