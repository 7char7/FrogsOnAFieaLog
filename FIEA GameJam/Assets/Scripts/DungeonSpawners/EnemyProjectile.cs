using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private GameObject hitEffectPrefab;
    
    private Rigidbody rb;
    private int damage;
    private bool initialized;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
    
    public void Initialize(Vector3 direction, float speed, int projectileDamage)
    {
        damage = projectileDamage;
        rb.linearVelocity = direction * speed;
        initialized = true;
        
        Destroy(gameObject, lifetime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!initialized) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            SpawnHitEffect(other.ClosestPoint(transform.position));
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            SpawnHitEffect(transform.position);
            Destroy(gameObject);
        }
    }
    
    private void SpawnHitEffect(Vector3 position)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }
}
