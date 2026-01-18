using UnityEngine;

public class CrystalFragment : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private float magnetSpeed = 8f;
    [SerializeField] private float accelerationMultiplier = 2f;
    [SerializeField] private float collectionDistance = 0.5f;
    [SerializeField] private float attractionDelay = 0.3f;
    
    [Header("Fragment Info")]
    [SerializeField] private CrystalType crystalType;
    [SerializeField] private int fragmentValue = 1;
    
    private Transform playerTransform;
    private bool isBeingAttracted = false;
    private float currentSpeed = 0f;
    private float spawnTime;
    
    private void Start()
    {
        spawnTime = Time.time;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! CrystalFragment cannot be attracted.");
        }
    }
    
    public void Initialize(CrystalType type, int value)
    {
        crystalType = type;
        fragmentValue = value;
    }
    
    private void Update()
    {
        if (playerTransform == null)
            return;
        
        if (Time.time - spawnTime < attractionDelay)
            return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= magnetRange)
        {
            isBeingAttracted = true;
        }
        
        if (isBeingAttracted)
        {
            MoveTowardsPlayer(distanceToPlayer);
            
            if (distanceToPlayer <= collectionDistance)
            {
                CollectFragment();
            }
        }
    }
    
    private void MoveTowardsPlayer(float distance)
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        
        currentSpeed += magnetSpeed * accelerationMultiplier * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, magnetSpeed * 3f);
        
        transform.position += direction * currentSpeed * Time.deltaTime;
        
        transform.Rotate(Vector3.up, 360f * Time.deltaTime);
    }
    
    private void CollectFragment()
    {
        ResourceManager resourceManager = FindFirstObjectByType<ResourceManager>();
        if (resourceManager != null)
        {
            resourceManager.AddCrystalFragments(crystalType, fragmentValue);
        }
        else
        {
            Debug.LogWarning("ResourceManager not found! Fragment collected but not tracked.");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCrystalCollectSound();
        }
        
        Destroy(gameObject);
    }
}
