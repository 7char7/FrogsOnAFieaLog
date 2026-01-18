using UnityEngine;

public class BloodParticleEffect : MonoBehaviour
{
    private static BloodParticleEffect instance;
    
    [Header("Blood Particle Settings")]
    [SerializeField] private GameObject commonBloodPrefab;
    [SerializeField] private GameObject uncommonBloodPrefab;
    [SerializeField] private GameObject rareBloodPrefab;
    [SerializeField] private GameObject legendaryBloodPrefab;
    [SerializeField] private int poolSizePerType = 10;
    
    private ParticleSystem[] commonPool;
    private ParticleSystem[] uncommonPool;
    private ParticleSystem[] rarePool;
    private ParticleSystem[] legendaryPool;
    private int commonIndex = 0;
    private int uncommonIndex = 0;
    private int rareIndex = 0;
    private int legendaryIndex = 0;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePools()
    {
        commonPool = InitializePool(commonBloodPrefab);
        uncommonPool = InitializePool(uncommonBloodPrefab);
        rarePool = InitializePool(rareBloodPrefab);
        legendaryPool = InitializePool(legendaryBloodPrefab);
    }
    
    private ParticleSystem[] InitializePool(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"Blood particle prefab not assigned!");
            return null;
        }
        
        ParticleSystem[] pool = new ParticleSystem[poolSizePerType];
        
        for (int i = 0; i < poolSizePerType; i++)
        {
            GameObject particleObj = Instantiate(prefab, transform);
            pool[i] = particleObj.GetComponent<ParticleSystem>();
            particleObj.SetActive(false);
        }
        
        return pool;
    }
    
    public static void PlayBloodEffect(Vector3 position, Vector3 normal, EnemyType enemyType)
    {
        if (instance == null)
            return;
        
        ParticleSystem ps = instance.GetNextParticle(enemyType);
        if (ps == null)
            return;
        
        ps.transform.position = position;
        ps.transform.rotation = Quaternion.LookRotation(normal);
        ps.gameObject.SetActive(true);
        ps.Play();
        
        instance.StartCoroutine(instance.DeactivateAfterPlay(ps));
    }

    public static void PlayCrystalBloodEffect(Vector3 position, Vector3 normal, CrystalType crystalType)
    {
        if (instance == null)
            return;
        
        ParticleSystem ps = instance.GetNextParticleForCrystal(crystalType);
        if (ps == null)
            return;
        
        ps.transform.position = position;
        ps.transform.rotation = Quaternion.LookRotation(normal);
        ps.gameObject.SetActive(true);
        ps.Play();
        
        instance.StartCoroutine(instance.DeactivateAfterPlay(ps));
    }
    
    private ParticleSystem GetNextParticle(EnemyType enemyType)
    {
        ParticleSystem[] pool;
        ref int index = ref commonIndex;
        
        switch (enemyType)
        {
            case EnemyType.Shallow:
                pool = commonPool;
                index = ref commonIndex;
                break;
            case EnemyType.Medium:
                pool = uncommonPool;
                index = ref uncommonIndex;
                break;
            case EnemyType.Deep:
                pool = legendaryPool;
                index = ref legendaryIndex;
                break;
            default:
                pool = commonPool;
                index = ref commonIndex;
                break;
        }
        
        if (pool == null)
            return null;
        
        for (int i = 0; i < poolSizePerType; i++)
        {
            index = (index + 1) % poolSizePerType;
            if (!pool[index].isPlaying)
            {
                return pool[index];
            }
        }
        
        return pool[index];
    }

    private ParticleSystem GetNextParticleForCrystal(CrystalType crystalType)
    {
        ParticleSystem[] pool;
        ref int index = ref commonIndex;
        
        switch (crystalType)
        {
            case CrystalType.Common:
                pool = commonPool;
                index = ref commonIndex;
                break;
            case CrystalType.Uncommon:
                pool = uncommonPool;
                index = ref uncommonIndex;
                break;
            case CrystalType.Rare:
                pool = rarePool;
                index = ref rareIndex;
                break;
            case CrystalType.Legendary:
                pool = legendaryPool;
                index = ref legendaryIndex;
                break;
            default:
                pool = commonPool;
                index = ref commonIndex;
                break;
        }
        
        if (pool == null)
            return null;
        
        for (int i = 0; i < poolSizePerType; i++)
        {
            index = (index + 1) % poolSizePerType;
            if (!pool[index].isPlaying)
            {
                return pool[index];
            }
        }
        
        return pool[index];
    }
    
    private System.Collections.IEnumerator DeactivateAfterPlay(ParticleSystem ps)
    {
        yield return new WaitWhile(() => ps.isPlaying);
        ps.gameObject.SetActive(false);
    }
}

