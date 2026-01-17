using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public int shotgunShopLevel;
    public int torchLimitShopLevel;
    public int playerHealthShopLevel;
    public int playerSpeedShopLevel;
    public int playerDefenceShopLevel;
    public int pickaxeShopLevel;

    private void Update()
    {
        
    }
    private void Awake()
    {
        refreshValues();
    }
    private void Start()
    {
        
    }
    public void refreshValues()
    {
        balanceText.text = "Balance: " + GameManager.Instance.money;
        shotgunShopLevel = GameManager.Instance.shotgunLevel;
        torchLimitShopLevel = GameManager.Instance.torchLimitLevel;
        playerHealthShopLevel = GameManager.Instance.playerHealthLevel;
        playerDefenceShopLevel = GameManager.Instance.playerDefencelevel;
        playerSpeedShopLevel = GameManager.Instance.playerSpeedlevel;
        pickaxeShopLevel = GameManager.Instance.pickaxeLevel;
    }
    public void updateValues()
    {
        GameManager.Instance.shotgunLevel = shotgunShopLevel; 
        GameManager.Instance.torchLimitLevel = torchLimitShopLevel;
        GameManager.Instance.playerHealthLevel = playerHealthShopLevel;
        GameManager.Instance.playerSpeedlevel = playerSpeedShopLevel;
        GameManager.Instance.playerDefencelevel = playerDefenceShopLevel;
        GameManager.Instance.pickaxeLevel = pickaxeShopLevel;
    }
    public void OnPlayButtonPressed()
    {
        updateValues();
        SceneManager.LoadScene("MainScene");
    }
}
