using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHandler : MonoBehaviour
{
    [Header("Shotgun Upgrades")]
    [SerializeField] private List<StatsUpgrade> shotgunUpgrades = new List<StatsUpgrade>();
    [SerializeField] private int currentShotgunLevel = 0;
    [SerializeField] private int maxShotgunLevel = 10;

    [Header("Pickaxe Upgrades")]
    [SerializeField] private List<StatsUpgrade> pickaxeUpgrades = new List<StatsUpgrade>();
    [SerializeField] private int currentPickaxeLevel = 0;
    [SerializeField] private int maxPickaxeLevel = 10;

    [Header("Player Upgrades")]
    // Max Health Upgrade
    [SerializeField] private StatsUpgrade playerMaxHealthUpgrade;
    [SerializeField] private int currentPlayerMaxHealthLevel = 0;
    [SerializeField] private int maxPlayerMaxHealthLevel = 10;

    // Movement Speed Upgrade
    [SerializeField] private StatsUpgrade playerMovementSpeedUpgrade;
    [SerializeField] private int currentPlayerMovementSpeedLevel = 0;
    [SerializeField] private int maxPlayerMovementSpeedLevel = 10;

    // Defense Upgrade
    [SerializeField] private StatsUpgrade playerDefenseUpgrade;
    [SerializeField] private int currentPlayerDefenseLevel = 0;
    [SerializeField] private int maxPlayerDefenseLevel = 10;

    //apply all upgrades
    public void ApplyUpgrades()
    {
        currentShotgunLevel = GameManager.Instance.shotgunLevel;
        currentPickaxeLevel = GameManager.Instance.pickaxeLevel;
        currentPlayerMaxHealthLevel = GameManager.Instance.playerHealthLevel;
        currentPlayerMovementSpeedLevel = GameManager.Instance.playerSpeedLevel;
        currentPlayerDefenseLevel = GameManager.Instance.playerDefenceLevel;
        ApplyShotgunUpgrade();
        ApplyPickaxeUpgrade();
        ApplyPlayerHealthUpgrade();
        ApplyPlayerMovementSpeedUpgrade();
        ApplyPlayerDefenseUpgrade();
    }
    public void ApplyShotgunUpgrade()
    {
        if (currentShotgunLevel >= maxShotgunLevel)
        {
            Debug.Log("Shotgun is already at max level");
            return;
        }
        //currentShotgunLevel++;
        Debug.Log("Applying Shotgun Upgrades");

        Shotgun shotgun = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shotgun>();

        foreach (var upgrade in shotgunUpgrades)
        {
            shotgun.gunStatsScriptableObject.UnlockUpgrade(upgrade);
        }
    }

    public void ApplyPickaxeUpgrade()
    {
        if (currentPickaxeLevel >= maxPickaxeLevel)
        {
            Debug.Log("Pickaxe is already at max level");
            return;
        }
        //currentPickaxeLevel++;
        Debug.Log("Applying Pickaxe Upgrades");

        Pickaxe pickaxe = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Pickaxe>();

        foreach (var upgrade in pickaxeUpgrades)
        {
            pickaxe.pickaxeStatsScriptableObject.UnlockUpgrade(upgrade);
        }
    }

    public void ApplyPlayerHealthUpgrade()
    {
        if (currentPlayerMaxHealthLevel >= maxPlayerMaxHealthLevel)
        {
            Debug.Log("Player Max Health is already at max level");
            return;
        }

        //currentPlayerMaxHealthLevel++;
        Debug.Log("Applying Player Max Health Upgrade");

        PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerManager.playerStatsScriptableObject.UnlockUpgrade(playerMaxHealthUpgrade);
        playerManager.SetCurrentHealth(playerManager.playerStatsScriptableObject.GetStat(Stat.maxHealth));
    }

    public void ApplyPlayerMovementSpeedUpgrade()
    {
        if (currentPlayerMovementSpeedLevel >= maxPlayerMovementSpeedLevel)
        {
            Debug.Log("Player Movement Speed is already at max level");
            return;
        }

        //currentPlayerMovementSpeedLevel++;
        Debug.Log("Applying Player Movement Speed Upgrade");

        PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerManager.playerStatsScriptableObject.UnlockUpgrade(playerMovementSpeedUpgrade);
    }

    public void ApplyPlayerDefenseUpgrade()
    {
        if (currentPlayerDefenseLevel >= maxPlayerDefenseLevel)
        {
            Debug.Log("Player Defense is already at max level");
            return;
        }

        //currentPlayerDefenseLevel++;
        Debug.Log("Applying Player Defense Upgrade");

         PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerManager.playerStatsScriptableObject.UnlockUpgrade(playerDefenseUpgrade);
    }
}
