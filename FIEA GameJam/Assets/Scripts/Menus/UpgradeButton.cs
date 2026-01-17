using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private GameObject myUpgradeDisplay;
    [SerializeField] private ShopButtonManager shopButtonManager;

    public void OnButtonClicked()
    {
        shopButtonManager.ShowClicked(myUpgradeDisplay);
    }
}
