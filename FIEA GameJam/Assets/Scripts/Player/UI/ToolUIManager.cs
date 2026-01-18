using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class ToolValueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image circleIndicator;
    [SerializeField] private Image torchIndicator;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI torchText;
    [SerializeField] private GameObject ammoContainer;
    [SerializeField] private GameObject torchContainer;

    [Header("Tool References")]
    [SerializeField] private Gun gun;
    [SerializeField] private Torch torch;
    [SerializeField] private ToolManager toolManager;

    private void OnEnable()
    {
        if (gun != null)
            gun.OnAmmoChanged += UpdateAmmoDisplay;

        if (torch != null)
            torch.OnTorchCountChanged += UpdateTorchDisplay;

        if (toolManager != null)
            toolManager.OnToolSwitched += OnToolSwitched;
    }

    private void OnDisable()
    {
        if (gun != null)
            gun.OnAmmoChanged -= UpdateAmmoDisplay;

        if (torch != null)
            torch.OnTorchCountChanged -= UpdateTorchDisplay;

        if (toolManager != null)
            toolManager.OnToolSwitched -= OnToolSwitched;
    }

    private void OnToolSwitched(GameObject activeTool)
    {
        ResetIndicator();

        if (activeTool == gun.gameObject)
        {
            ShowAmmoUI();
            UpdateAmmoDisplay();
        }
        else
        {
            HideAllUI();
        }
    }

    private void ResetIndicator()
    {
        circleIndicator.gameObject.SetActive(false);
        circleIndicator.fillAmount = 1f;
    }

    private void UpdateAmmoDisplay()
    {
        if (gun != null && ammoText != null)
        {
            int currentAmmo = Mathf.FloorToInt(gun.currentAmmo);
            int maxAmmo = Mathf.FloorToInt(gun.gunStatsScriptableObject.GetStat(Stat.maxAmmo));
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }

    private void UpdateTorchDisplay()
    {
        if (torch != null && torchText != null)
        {
            int torchCount = torch.GetTorchCount();
            torchText.text = torchCount.ToString();
        }
    }

    private void ShowAmmoUI()
    {
        if (ammoContainer != null)
            ammoContainer.SetActive(true);
    }

    private void ShowTorchUI()
    {
        if (torchContainer != null)
            torchContainer.SetActive(true);
    }

    private void HideAllUI()
    {
        if (ammoContainer != null)
            ammoContainer.SetActive(false);
    }
}