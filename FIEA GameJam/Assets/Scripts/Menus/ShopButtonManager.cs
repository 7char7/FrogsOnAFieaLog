using UnityEngine;

public class ShopButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject[] allUpgradeDisplays;

    private void Start()
    {
        HideAll();
    }

    public void ShowClicked(GameObject displayToShow)
    {
        foreach (GameObject display in allUpgradeDisplays)
        {
            display.SetActive(display == displayToShow);
        }
    }
    public void HideAll()
    {
        foreach (GameObject display in allUpgradeDisplays)
        {
            display.SetActive(false);
        }
    }


}
