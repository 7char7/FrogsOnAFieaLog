using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Visual")]
    [SerializeField] private Color debugColor = Color.red;

    public EnemyType Type => enemyType;
    public int Health => health;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;

    private void OnValidate()
    {
        switch (enemyType)
        {
            case EnemyType.Shallow:
                health = 50;
                damage = 5;
                moveSpeed = 2f;
                debugColor = new Color(0.8f, 0.8f, 0.8f);
                break;
            case EnemyType.Medium:
                health = 100;
                damage = 10;
                moveSpeed = 3f;
                debugColor = new Color(0.3f, 0.6f, 1f);
                break;
            case EnemyType.Deep:
                health = 150;
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
