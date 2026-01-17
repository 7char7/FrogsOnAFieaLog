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

    public void OnBuy()
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

        if (GameManager.Instance.money >= cost)
        {
            GameManager.Instance.money -= cost;

            switch (upgradeType)
            {
                case UpgradeType.Shotgun:
                    shopMenu.shotgunShopLevel += 1;
                    break;
                case UpgradeType.PlayerHealth:
                    shopMenu.playerHealthShopLevel += 1;
                    break;
                case UpgradeType.TorchLimit:
                    shopMenu.torchLimitShopLevel += 1;
                    break;
            }

            shopMenu.updateValues(); // Write new level to GameManager
            UpdateCostText();        // Update cost for next level
            shopMenu.balanceText.text = "Balance: " + GameManager.Instance.money;
        }
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
