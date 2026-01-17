using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Stat
{
    // Shotgun
    fireRate,
    damage,
    bulletSpread,
    reloadSpeed,
    range,
    maxAmmo,
    bulletsPerShot,

    // Pickaxe
    miningSpeed,
    miningDamage,
    reachDistance,

    // Player
    maxHealth,
    movementSpeed,
    defense,

    // Torch
    numberOfTorches
}


[CreateAssetMenu(fileName = "New Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    public List<StatInfo> instanceStats = new List<StatInfo>();
    public List<StatInfo> stats = new List<StatInfo>();
    public List<StatsUpgrade> appliedUpgrades = new List<StatsUpgrade>();

    public float GetStat(Stat stat)
    {
        if (instanceStats != null)
        {
            foreach (var s in instanceStats)
            {
                if (s.statType == stat)
                {
                    return GetUpgradedValue(stat, s.statValue);
                }
            }
        }

        if (stats != null)
        {
            foreach (var s in stats)
            {
                if (s.statType == stat)
                {
                    return GetUpgradedValue(stat, s.statValue);
                }
            }
        }

        return 0;
    }

    public void UnlockUpgrade(StatsUpgrade upgrade)
    {
        appliedUpgrades.Add(upgrade);
    }

    private float GetUpgradedValue(Stat stat, float baseValue)
    {
        foreach (var upgrade in appliedUpgrades)
        {
            if (!upgrade.upgradeToApply.Exists(x => x.statType == stat))
                continue;

            if (upgrade.isPercentageUpgrade)
            {
                float percentageIncrease = upgrade.upgradeToApply.Find(x => x.statType == stat).statValue;
                baseValue += baseValue * (percentageIncrease / 100f);
            }
            else
            {
                float flatIncrease = upgrade.upgradeToApply.Find(x => x.statType == stat).statValue;
                baseValue += flatIncrease;
            }
        }

        return baseValue;
    }

    public void ResetUpgrades()
    {
        appliedUpgrades.Clear();
    }
}
