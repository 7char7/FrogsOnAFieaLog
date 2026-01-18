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

    [Header("Torch Upgrades")]
    [SerializeField] private List<StatsUpgrade> torchUpgrades = new List<StatsUpgrade>();
    [SerializeField] private int currentTorchLevel = 0;
    [SerializeField] private int maxTorchLevel = 10;

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

    public void ApplyUpgrades()
    {
        ResetAllUpgrades();

        currentShotgunLevel = GameManager.Instance.shotgunLevel;
        currentPickaxeLevel = GameManager.Instance.pickaxeLevel;
        currentTorchLevel = GameManager.Instance.torchLimitLevel;
        currentPlayerMaxHealthLevel = GameManager.Instance.playerHealthLevel;
        currentPlayerMovementSpeedLevel = GameManager.Instance.playerSpeedLevel;
        currentPlayerDefenseLevel = GameManager.Instance.playerDefenceLevel;

        for (int i = 0; i < currentShotgunLevel; i++)
        {
            ApplyShotgunUpgrade(i);
        }
        for (int i = 0; i < currentPickaxeLevel; i++)
        {
            ApplyPickaxeUpgrade(i);
        }
        for (int i = 0; i < currentTorchLevel; i++)
        {
            ApplyTorchUpgrade(i);
        }
        for (int i = 0; i < currentPlayerMaxHealthLevel; i++)
        {
            ApplyPlayerHealthUpgrade();
        }
        for (int i = 0; i < currentPlayerMovementSpeedLevel; i++)
        {
            ApplyPlayerMovementSpeedUpgrade();
        }
        for (int i = 0; i < currentPlayerDefenseLevel; i++)
        {
            ApplyPlayerDefenseUpgrade();
        }
    }

    private void ResetAllUpgrades()
    {
        Shotgun shotgun = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Shotgun>();
        if (shotgun != null)
        {
            shotgun.gunStatsScriptableObject.ResetUpgrades();
        }

        Pickaxe pickaxe = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Pickaxe>();
        if (pickaxe != null)
        {
            pickaxe.pickaxeStatsScriptableObject.ResetUpgrades();
        }

        Torch torch = GameObject.FindGameObjectWithTag("Player")?.GetComponentInChildren<Torch>();
        if (torch != null)
        {
            torch.torchStatsScriptableObject.ResetUpgrades();
        }

        PlayerManager playerManager = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.playerStatsScriptableObject.ResetUpgrades();
        }
    }


    public void ApplyShotgunUpgrade(int level)
    {
        if (level >= maxShotgunLevel || level >= shotgunUpgrades.Count)
        {
            Debug.Log("Shotgun upgrade level out of range");
            return;
        }

        Debug.Log($"Applying Shotgun Upgrade Level {level}");

        Shotgun shotgun = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shotgun>();
        shotgun.gunStatsScriptableObject.UnlockUpgrade(shotgunUpgrades[level]);
    }

    public void ApplyPickaxeUpgrade(int level)
    {
        if (level >= maxPickaxeLevel || level >= pickaxeUpgrades.Count)
        {
            Debug.Log("Pickaxe upgrade level out of range");
            return;
        }

        Debug.Log($"Applying Pickaxe Upgrade Level {level}");

        Pickaxe pickaxe = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Pickaxe>();
        pickaxe.pickaxeStatsScriptableObject.UnlockUpgrade(pickaxeUpgrades[level]);
    }

    public void ApplyTorchUpgrade(int level)
    {
        if (level >= maxTorchLevel || level >= torchUpgrades.Count)
        {
            Debug.Log("Torch upgrade level out of range");
            return;
        }

        Debug.Log($"Applying Torch Upgrade Level {level}");

        Torch torch = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Torch>();
        torch.torchStatsScriptableObject.UnlockUpgrade(torchUpgrades[level]);
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
