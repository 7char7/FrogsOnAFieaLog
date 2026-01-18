using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public enum UpgradeType
    {
        Shotgun,
        PlayerHealth,
        TorchLimit,
        Speed,
        Defence,
        Pickaxe
    }

    [SerializeField] private UpgradeType upgradeType;
    [SerializeField] private GameObject myUpgradeDisplay;
    [SerializeField] private ShopButtonManager shopButtonManager;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private ShopMenu shopMenu;
    [SerializeField] private Image[] indicatorImage;

    private void OnEnable()
    {
        shopMenu.OnRefreshValues += RefreshIndicators;
    }

    private void OnDisable()
    {
        shopMenu.OnRefreshValues -= RefreshIndicators;
    }

    public void OnButtonClicked()
    {
        shopButtonManager.ShowClicked(myUpgradeDisplay);
        UpdateCostText();
    }

    public void OnBuy()
    {
        int level = 0;

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
            case UpgradeType.Pickaxe:
                level = shopMenu.pickaxeShopLevel;
                break;
            case UpgradeType.Defence:
                level = shopMenu.playerDefenceShopLevel;
                break;
            case UpgradeType.Speed:
                level = shopMenu.playerSpeedShopLevel;
                break;
        }

        int cost = 100 + 25 * level;

        if (ResourceManager.Instance != null && ResourceManager.Instance.TotalPoints >= cost)
        {
            if (ResourceManager.Instance.SpendPoints(cost))
            {
                switch (upgradeType)
                {
                    case UpgradeType.Shotgun:
                        shopMenu.shotgunShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                    case UpgradeType.PlayerHealth:
                        shopMenu.playerHealthShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                    case UpgradeType.TorchLimit:
                        shopMenu.torchLimitShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                    case UpgradeType.Pickaxe:
                        shopMenu.pickaxeShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                    case UpgradeType.Defence:
                        shopMenu.playerDefenceShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                    case UpgradeType.Speed:
                        shopMenu.playerSpeedShopLevel += 1;
                        if (indicatorImage[level].color != Color.green)
                        {
                            indicatorImage[level].color = Color.green;
                        }
                        break;
                }

                shopMenu.updateValues();
                UpdateCostText();
                shopMenu.refreshValues();
            }
        }
    }

    private void RefreshIndicators()
    {
        int level = 0;

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
            case UpgradeType.Pickaxe:
                level = shopMenu.pickaxeShopLevel;
                break;
            case UpgradeType.Defence:
                level = shopMenu.playerDefenceShopLevel;
                break;
            case UpgradeType.Speed:
                level = shopMenu.playerSpeedShopLevel;
                break;
        }

        for (int i = 0; i < level; i++)
        {
            if (indicatorImage[i].color != Color.green)
            {
                indicatorImage[i].color = Color.green;
            }
        }

        int cost = 100 + 25 * level;
        costText.text = "Cost: $" + cost;
    }

    private void UpdateCostText()
    {
        int level = 0;

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
            case UpgradeType.Pickaxe:
                level = shopMenu.pickaxeShopLevel;
                break;
            case UpgradeType.Defence:
                level = shopMenu.playerDefenceShopLevel;
                break;
            case UpgradeType.Speed:
                level = shopMenu.playerSpeedShopLevel;
                break;
        }

        int cost = 100 + 25 * level;
        costText.text = "Cost: $" + cost;
    }
}
