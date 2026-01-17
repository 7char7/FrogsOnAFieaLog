using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHandler : MonoBehaviour
{
    [SerializeField] private List<StatsUpgrade> shotgunUpgrades = new List<StatsUpgrade>();
    [SerializeField] private int currentShotgunLevel = 0;
    [SerializeField] private int maxShotgunLevel = 10;
    [SerializeField] private List<StatsUpgrade> pickaxeUpgrades = new List<StatsUpgrade>();
    [SerializeField] private int currentPickaxeLevel = 0;
    [SerializeField] private int maxPickaxeLevel = 10;

    public void ApplyShotgunUpgrade()
    {
        if (currentShotgunLevel >= maxShotgunLevel)
        {
            Debug.Log("Shotgun is already at max level");
            return;
        }
        currentShotgunLevel++;
        Debug.Log("Applying Shotgun Upgrades");

        Shotgun shotgun = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Shotgun>();

        foreach (var upgrade in shotgunUpgrades)
        {
            shotgun.gunStatsScriptableObject.UnlockUpgrade(upgrade);
        }
    }
}
