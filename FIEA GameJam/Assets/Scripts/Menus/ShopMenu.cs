using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    public TextMeshProUGUI balanceText;
    public int shotgunShopLevel;
    public int torchLimitShopLevel;
    public int playerHealthShopLevel;

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
    }
    public void updateValues()
    {
        GameManager.Instance.shotgunLevel = shotgunShopLevel; 
        GameManager.Instance.torchLimitLevel = torchLimitShopLevel;
        GameManager.Instance.playerHealthLevel = playerHealthShopLevel;
    }
    public void OnPlayButtonPressed()
    {
        updateValues();
        SceneManager.LoadScene("MainScene");
    }
}
