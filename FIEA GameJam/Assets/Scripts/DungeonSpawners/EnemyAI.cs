using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private LayerMask obstacleLayers;
    
    [Header("Combat Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float aimAccuracy = 0.85f;
    [SerializeField] private float stoppingDistance = 8f;
    
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waypointWaitTime = 2f;
    [SerializeField] private float waypointReachDistance = 1f;
    
    private NavMeshAgent agent;
    private Enemy enemy;
    private Transform player;
    private EnemyState currentState = EnemyState.Patrol;
    
    private Vector3 patrolTarget;
    private float lastFireTime;
    private float waypointWaitTimer;
    private bool hasLineOfSight;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        Debug.Log($"[EnemyAI] {gameObject.name} Awake: agent={agent != null}, enemy={enemy != null}");
    }
    
    private void Start()
    {
        agent.speed = enemy.MoveSpeed;
        agent.stoppingDistance = stoppingDistance;
        
        Debug.Log($"[EnemyAI] {gameObject.name} Start: pos={transform.position}, agent.speed={agent.speed}, isOnNavMesh={agent.isOnNavMesh}, enabled={agent.enabled}");
        
        FindPlayer();
        
        if (agent.isOnNavMesh)
        {
            SetNewPatrolTarget();
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} is NOT on NavMesh! Position: {transform.position}");
        }
    }
    
    private void Update()
    {
        Debug.Log($"[EnemyAI] {gameObject.name} Update() CALLED - enabled={enabled}, gameObject.activeInHierarchy={gameObject.activeInHierarchy}");
        
        if (!agent.isOnNavMesh || !agent.enabled)
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} Update: EARLY RETURN - isOnNavMesh={agent.isOnNavMesh}, enabled={agent.enabled}");
            return;
        }
        
        if (player == null)
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} Update: player is NULL, finding player...");
            FindPlayer();
            if (player == null)
            {
                Debug.LogWarning($"[EnemyAI] {gameObject.name} Update: player STILL NULL after FindPlayer!");
                return;
            }
        }
        
        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Chase:
                UpdateChase();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
        
        CheckPlayerDetection();
    }
    
    private void UpdatePatrol()
    {
        if (agent.pathPending)
        {
            Debug.Log($"[EnemyAI] {gameObject.name} UpdatePatrol: pathPending=true, waiting...");
            return;
        }
        
        if (agent.hasPath)
        {
            Debug.Log($"[EnemyAI] {gameObject.name} UpdatePatrol: hasPath=true, remainingDistance={agent.remainingDistance}, velocity={agent.velocity.magnitude}");
            
            if (agent.remainingDistance <= waypointReachDistance)
            {
                waypointWaitTimer += Time.deltaTime;
                
                if (waypointWaitTimer >= waypointWaitTime)
                {
                    SetNewPatrolTarget();
                    waypointWaitTimer = 0f;
                }
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} UpdatePatrol: hasPath=false, pathPending=false, requesting new target");
            SetNewPatrolTarget();
        }
    }
    
    private void UpdateChase()
    {
        if (player == null) return;
        
        agent.SetDestination(player.position);
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= stoppingDistance && hasLineOfSight)
        {
            currentState = EnemyState.Attack;
            agent.isStopped = true;
        }
    }
    
    private void UpdateAttack()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            agent.isStopped = false;
            SetNewPatrolTarget();
            return;
        }
        
        LookAtPlayer();
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > stoppingDistance * 1.5f || !hasLineOfSight)
        {
            currentState = EnemyState.Chase;
            agent.isStopped = false;
        }
        else
        {
            if (Time.time >= lastFireTime + (1f / fireRate))
            {
                ShootAtPlayer();
                lastFireTime = Time.time;
            }
        }
    }
    
    private void CheckPlayerDetection()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            
            if (angleToPlayer <= fieldOfView / 2f)
            {
                hasLineOfSight = HasLineOfSightToPlayer();
                
                if (hasLineOfSight && currentState == EnemyState.Patrol)
                {
                    currentState = EnemyState.Chase;
                    agent.isStopped = false;
                }
            }
            else
            {
                hasLineOfSight = false;
            }
        }
        else
        {
            hasLineOfSight = false;
            
            if (currentState != EnemyState.Patrol)
            {
                currentState = EnemyState.Patrol;
                agent.isStopped = false;
                SetNewPatrolTarget();
            }
        }
    }
    
    private bool HasLineOfSightToPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        
        if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer.normalized, 
            out RaycastHit hit, detectionRadius, obstacleLayers))
        {
            return hit.transform == player;
        }
        
        return false;
    }
    
    private void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    
    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        Vector3 targetPosition = player.position + Vector3.up;
        Vector3 direction = (targetPosition - firePoint.position).normalized;
        
        float inaccuracy = 1f - aimAccuracy;
        direction.x += Random.Range(-inaccuracy, inaccuracy);
        direction.y += Random.Range(-inaccuracy * 0.5f, inaccuracy * 0.5f);
        direction.z += Random.Range(-inaccuracy, inaccuracy);
        direction.Normalize();
        
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, projectileSpeed, enemy.Damage);
        }
    }
    
    private void SetNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0;
        randomDirection += transform.position;
        
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            bool pathSet = agent.SetDestination(patrolTarget);
            Debug.Log($"[EnemyAI] {gameObject.name} SetPatrolTarget: target={patrolTarget}, pathSet={pathSet}, hasPath={agent.hasPath}, pathPending={agent.pathPending}");
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name} SetPatrolTarget: Failed to sample NavMesh position!");
        }
    }
    
    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }
    
    public void AlertToPlayer(Vector3 playerPosition)
    {
        if (currentState == EnemyState.Patrol)
        {
            currentState = EnemyState.Chase;
            agent.isStopped = false;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2f, transform.up) * transform.forward * detectionRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2f, transform.up) * transform.forward * detectionRadius;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);
        
        if (currentState == EnemyState.Patrol)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(patrolTarget, 0.5f);
        }
    }
}

public enum EnemyState
{
    Patrol,
    Chase,
    Attack
}
