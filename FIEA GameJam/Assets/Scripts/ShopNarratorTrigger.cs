using UnityEngine;

public class ShopNarratorTrigger : MonoBehaviour
{
    [SerializeField] private float delayBeforeWelcome = 0.5f;

    private void Start()
    {
        Invoke(nameof(PlayShopWelcome), delayBeforeWelcome);
    }

    private void PlayShopWelcome()
    {
        if (NarratorManager.Instance != null)
        {
            NarratorManager.Instance.PlayShopWelcome();
        }
        else
        {
            Debug.LogWarning("NarratorManager instance not found in scene.");
        }
    }
}
