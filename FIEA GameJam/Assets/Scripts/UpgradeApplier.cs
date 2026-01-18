using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{
    void Start()
    {
        UpgradeHandler handler = Object.FindFirstObjectByType<UpgradeHandler>();
        handler.ApplyUpgrades();
    }
}
