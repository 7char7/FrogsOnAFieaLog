using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    [Header("Crystal Resources")]
    [SerializeField] private int commonCrystals = 0;
    [SerializeField] private int uncommonCrystals = 0;
    [SerializeField] private int rareCrystals = 0;
    [SerializeField] private int legendaryCrystals = 0;
    
    [Header("Point Values")]
    [SerializeField] private int commonPointValue = 2;
    [SerializeField] private int uncommonPointValue = 5;
    [SerializeField] private int rarePointValue = 10;
    [SerializeField] private int legendaryPointValue = 20;
    
    [Header("Current Session")]
    [SerializeField] private int currentRunPoints = 0;
    [SerializeField] private int totalPoints = 0;
    
    public static ResourceManager Instance { get; private set; }
    
    public event Action<CrystalType, int> OnCrystalCollected;
    public event Action<int, int> OnPointsChanged;
    
    public int CurrentRunPoints => currentRunPoints;
    public int TotalPoints => totalPoints;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddCrystalFragments(CrystalType type, int amount)
    {
        int pointsEarned = 0;
        
        switch (type)
        {
            case CrystalType.Common:
                commonCrystals += amount;
                pointsEarned = commonPointValue * amount;
                break;
            case CrystalType.Uncommon:
                uncommonCrystals += amount;
                pointsEarned = uncommonPointValue * amount;
                break;
            case CrystalType.Rare:
                rareCrystals += amount;
                pointsEarned = rarePointValue * amount;
                break;
            case CrystalType.Legendary:
                legendaryCrystals += amount;
                pointsEarned = legendaryPointValue * amount;
                break;
        }
        
        currentRunPoints += pointsEarned;
        
        OnCrystalCollected?.Invoke(type, amount);
        OnPointsChanged?.Invoke(currentRunPoints, pointsEarned);
        
        Debug.Log($"Collected {amount} {type} crystal(s)! Earned {pointsEarned} points. Run: {currentRunPoints} | Total: {totalPoints}");
    }
    
    public int GetCrystalCount(CrystalType type)
    {
        return type switch
        {
            CrystalType.Common => commonCrystals,
            CrystalType.Uncommon => uncommonCrystals,
            CrystalType.Rare => rareCrystals,
            CrystalType.Legendary => legendaryCrystals,
            _ => 0
        };
    }
    
    public bool SpendCrystals(CrystalType type, int amount)
    {
        int currentAmount = GetCrystalCount(type);
        if (currentAmount >= amount)
        {
            switch (type)
            {
                case CrystalType.Common:
                    commonCrystals -= amount;
                    break;
                case CrystalType.Uncommon:
                    uncommonCrystals -= amount;
                    break;
                case CrystalType.Rare:
                    rareCrystals -= amount;
                    break;
                case CrystalType.Legendary:
                    legendaryCrystals -= amount;
                    break;
            }
            return true;
        }
        return false;
    }
    
    public void ResetSession()
    {
        commonCrystals = 0;
        uncommonCrystals = 0;
        rareCrystals = 0;
        legendaryCrystals = 0;
        currentRunPoints = 0;
        
        OnPointsChanged?.Invoke(currentRunPoints, 0);
    }
    
    public void SaveRunProgress()
    {
        totalPoints += currentRunPoints;
        Debug.Log($"Run complete! Added {currentRunPoints} to total. Total points: {totalPoints}");
        currentRunPoints = 0;
        commonCrystals = 0;
        uncommonCrystals = 0;
        rareCrystals = 0;
        legendaryCrystals = 0;
    }
    
    public void ResetAllProgress()
    {
        totalPoints = 0;
        ResetSession();
    }
    
    public int GetPointValue(CrystalType type)
    {
        return type switch
        {
            CrystalType.Common => commonPointValue,
            CrystalType.Uncommon => uncommonPointValue,
            CrystalType.Rare => rarePointValue,
            CrystalType.Legendary => legendaryPointValue,
            _ => 0
        };
    }
}
