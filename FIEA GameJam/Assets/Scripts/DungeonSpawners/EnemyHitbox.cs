using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHitbox : MonoBehaviour
{
    private Enemy enemyReference;
    
    public Enemy Enemy
    {
        get
        {
            if (enemyReference == null)
            {
                enemyReference = GetComponentInParent<Enemy>();
            }
            return enemyReference;
        }
    }
    
    private void Awake()
    {
        enemyReference = GetComponentInParent<Enemy>();
        
        if (enemyReference == null)
        {
            Debug.LogWarning($"EnemyHitbox on {gameObject.name} couldn't find Enemy component in parent!");
        }
    }
}
