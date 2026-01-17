using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "StatsUpgrade", menuName = "Scriptable Objects/StatsUpgrade")]
public class StatsUpgrade : Upgrade
{
    public List<Stats> unitsToUpgrade = new List<Stats>();
    public List<StatInfo> upgradeToApply = new List<StatInfo>();
    public bool isPercentageUpgrade = false;

    public override void ApplyUpgrade()
    {
        foreach (var unitToUpgrade in unitsToUpgrade)
        {
            unitToUpgrade.UnlockUpgrade(this);
        }
    }

}
