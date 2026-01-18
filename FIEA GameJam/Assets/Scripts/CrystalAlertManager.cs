using UnityEngine;
using System.Collections.Generic;

public class CrystalAlertManager : MonoBehaviour
{
    [Header("Alert Settings")]
    [SerializeField] private float commonUncommonAlertRadius = 15f;
    [SerializeField] private float rareAlertRadius = 25f;
    [SerializeField] private float legendaryAlertRadius = 35f;

    private static CrystalAlertManager instance;

    public static CrystalAlertManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CrystalAlertManager>();
                
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("CrystalAlertManager");
                    instance = managerObject.AddComponent<CrystalAlertManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    public void AlertEnemiesOfCrystalMined(CrystalType crystalType, Vector3 miningPosition)
    {
        float alertRadius = GetAlertRadiusForCrystalType(crystalType);
        List<EnemyType> targetEnemyTypes = GetTargetEnemyTypesForCrystalType(crystalType);
        
        AlertNearbyEnemies(miningPosition, alertRadius, targetEnemyTypes);
    }

    private float GetAlertRadiusForCrystalType(CrystalType crystalType)
    {
        switch (crystalType)
        {
            case CrystalType.Common:
            case CrystalType.Uncommon:
                return commonUncommonAlertRadius;
            case CrystalType.Rare:
                return rareAlertRadius;
            case CrystalType.Legendary:
                return legendaryAlertRadius;
            default:
                return commonUncommonAlertRadius;
        }
    }

    private List<EnemyType> GetTargetEnemyTypesForCrystalType(CrystalType crystalType)
    {
        List<EnemyType> targetTypes = new List<EnemyType>();
        
        switch (crystalType)
        {
            case CrystalType.Common:
            case CrystalType.Uncommon:
                targetTypes.Add(EnemyType.Shallow);
                break;
            case CrystalType.Rare:
                targetTypes.Add(EnemyType.Medium);
                break;
            case CrystalType.Legendary:
                targetTypes.Add(EnemyType.Deep);
                break;
        }
        
        return targetTypes;
    }

    private void AlertNearbyEnemies(Vector3 position, float radius, List<EnemyType> targetEnemyTypes)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int alertedCount = 0;
        
        foreach (GameObject enemyObj in enemies)
        {
            float distance = Vector3.Distance(position, enemyObj.transform.position);
            
            if (distance <= radius)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
                
                if (enemy != null && enemyAI != null && targetEnemyTypes.Contains(enemy.Type))
                {
                    enemyAI.AlertToPlayer(position);
                    alertedCount++;
                }
            }
        }
    }
}
