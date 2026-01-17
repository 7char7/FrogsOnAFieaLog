using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pickaxe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Image crystalHealthBar;
    public Stats pickaxeStatsScriptableObject;

    [Header("General Settings")]
    [SerializeField] private float reachDistance;
    [SerializeField] private float fortune;
    [SerializeField] private float miningSpeed;
    [SerializeField] private float miningDamage;
    private Coroutine miningCoroutine;
    private float nextMineTime;

    void Awake()
    {
        crystalHealthBar.gameObject.SetActive(false);
        pickaxeStatsScriptableObject = Instantiate(pickaxeStatsScriptableObject);
        nextMineTime = 0f;
    }

    public void Mine()
    {
        if (Time.time < nextMineTime)
            return;

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, reachDistance))
        {
            if (hit.transform.CompareTag("Mineable"))
            {
                Debug.Log("Mining " + hit.transform.name);
                Crystal crystal = hit.transform.GetComponent<Crystal>();
                crystal.MineCrystal(pickaxeStatsScriptableObject.GetStat(Stat.miningDamage));
                UpdateCrystalHealthBar(crystal);
            }
        }

        nextMineTime = Time.time + (1f / pickaxeStatsScriptableObject.GetStat(Stat.miningSpeed));
    }

    public IEnumerator HandleMine()
    {
        while (true)
        {
            Mine();
            yield return new WaitForSeconds(1f / pickaxeStatsScriptableObject.GetStat(Stat.miningSpeed));
        }
    }

    public void StartMining(InputAction.CallbackContext context)
    {
        if (this.gameObject.activeSelf == false)
            return;

        if (context.started)
        {
            miningCoroutine = StartCoroutine(HandleMine());
        }
    }

    public void StopMining(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (miningCoroutine != null)
            {
                StopCoroutine(miningCoroutine);
            }
        }
    }

    private void UpdateCrystalHealthBar(Crystal crystal)
    {
        float currentHealth = crystal.GetCurrentCrystalHealth();
        float maxHealth = crystal.GetMaxCrystalHealth();
        crystalHealthBar.fillAmount = 1 - currentHealth / maxHealth;
        crystalHealthBar.gameObject.SetActive(true);

        if (currentHealth <= 0)
            crystalHealthBar.gameObject.SetActive(false);
    }
}

