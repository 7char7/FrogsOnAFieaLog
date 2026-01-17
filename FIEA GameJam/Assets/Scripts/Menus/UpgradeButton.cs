using UnityEngine;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        Shotgun,
        PlayerHealth,
        TorchLimit
    }

    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private GameObject myUpgradeDisplay;
    [SerializeField] private ShopButtonManager shopButtonManager;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private ShopMenu shopMenu;

    public void OnButtonClicked()
    {
        shopButtonManager.ShowClicked(myUpgradeDisplay);
        UpdateCostText();
    }

    private void UpdateCostText()
    {
        int level = 1;

        switch (upgradeType)
        {
            case UpgradeType.Shotgun:
                level = shopMenu.shotgunShopLevel;
                break;
            case UpgradeType.PlayerHealth:
                level = shopMenu.playerHealthShopLevel;
                break;
            case UpgradeType.TorchLimit:
                level = shopMenu.torchLimitShopLevel;
                break;
        }

        int cost = 100 + 25 * level;
        costText.text = "Cost: $" + cost;
    }
}
