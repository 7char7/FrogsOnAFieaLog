using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public abstract class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;

    public abstract void ApplyUpgrade();
}
