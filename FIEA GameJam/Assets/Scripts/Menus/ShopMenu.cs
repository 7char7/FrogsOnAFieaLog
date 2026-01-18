using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class ShopMenu : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public int shotgunShopLevel;
    public int torchLimitShopLevel;
    public int playerHealthShopLevel;
    public int playerSpeedShopLevel;
    public int playerDefenceShopLevel;
    public int pickaxeShopLevel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject enterShopButton;

    public event Action OnRefreshValues;

    private void Awake()
    {
        refreshValues();
        upgradePanel.SetActive(false);
    }
    public void refreshValues()
    {
        balanceText.text = "Balance: " + GameManager.Instance.money;
        shotgunShopLevel = GameManager.Instance.shotgunLevel;
        torchLimitShopLevel = GameManager.Instance.torchLimitLevel;
        playerHealthShopLevel = GameManager.Instance.playerHealthLevel;
        playerDefenceShopLevel = GameManager.Instance.playerDefenceLevel;
        playerSpeedShopLevel = GameManager.Instance.playerSpeedLevel;
        pickaxeShopLevel = GameManager.Instance.pickaxeLevel;
        OnRefreshValues?.Invoke();
    }
    public void updateValues()
    {
        GameManager.Instance.shotgunLevel = shotgunShopLevel;
        GameManager.Instance.torchLimitLevel = torchLimitShopLevel;
        GameManager.Instance.playerHealthLevel = playerHealthShopLevel;
        GameManager.Instance.playerSpeedLevel = playerSpeedShopLevel;
        GameManager.Instance.playerDefenceLevel = playerDefenceShopLevel;
        GameManager.Instance.pickaxeLevel = pickaxeShopLevel;
    }
    public void OnPlayButtonPressed()
    {
        updateValues();
        SceneManager.LoadScene("MainScene");
    }

    public void OnShopOpened()
    {
        if (!upgradePanel.activeSelf)
        {
            upgradePanel.SetActive(true);
            playButton.SetActive(false);
            enterShopButton.SetActive(false);
            refreshValues();
        }
    }

    public void OnShopClosed()
    {
        if (upgradePanel.activeSelf)
        {
            upgradePanel.SetActive(false);
            playButton.SetActive(true);
            enterShopButton.SetActive(true);
        }
    }
}
