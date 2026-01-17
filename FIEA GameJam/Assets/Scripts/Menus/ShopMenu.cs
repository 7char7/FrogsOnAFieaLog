using UnityEngine;
using TMPro;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI balanceText;

    private void Update()
    {
        
    }
    private void Awake()
    {
        balanceText.text = "Balance: " + GameManager.Instance.money;
    }
    private void Start()
    {
        
    }
    public void updateMoney()
    {
        balanceText.text = "Balance: " + GameManager.Instance.money;
    }

}
