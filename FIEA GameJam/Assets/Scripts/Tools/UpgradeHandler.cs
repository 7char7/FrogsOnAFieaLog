using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeHandler : MonoBehaviour
{
    [SerializeField] private List<StatsUpgrade> shotgunUpgrades = new List<StatsUpgrade>();
    [SerializeField] private List<StatsUpgrade> pickaxeUpgrades = new List<StatsUpgrade>();

    public void ApplyShotgunUpgrade()
    {
        Debug.Log("Applying Shotgun Upgrades");

        Shotgun shotgun = GameObject.FindGameObjectWithTag("Player").GetComponent<Shotgun>();

        foreach (var upgrade in shotgunUpgrades)
        {
            upgrade.ApplyUpgrade();
        }
    }
}
