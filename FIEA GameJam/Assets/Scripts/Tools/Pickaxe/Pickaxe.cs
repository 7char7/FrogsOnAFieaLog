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
    
    [Header("Animation Settings")]
    [SerializeField] private float swingAnimationDuration = 0.3f;
    
    private Coroutine miningCoroutine;
    private float nextMineTime;
    
    //bool to make animation not play while player by char
    private bool isSwinging;

    void Awake()
    {
        if (crystalHealthBar != null)
        {
            crystalHealthBar.gameObject.SetActive(false);
        }
        
        if (pickaxeStatsScriptableObject != null)
        {
            pickaxeStatsScriptableObject = Instantiate(pickaxeStatsScriptableObject);
        }
        else
        {
            Debug.LogError("[PICKAXE] pickaxeStatsScriptableObject is not assigned in the Inspector!");
        }
        
        nextMineTime = 0f;
    }

    public void Mine()
    {
        if (Time.time < nextMineTime)
            return;

        Debug.Log($"[PICKAXE] Mine called - Camera Position: {cameraTransform.position}, Forward: {cameraTransform.forward}, Reach Distance: {reachDistance}");

        RaycastHit hit;
        bool didHit = Physics.SphereCast(cameraTransform.position, 0.3f, cameraTransform.forward, out hit, reachDistance, ~0, QueryTriggerInteraction.Collide);
        
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * reachDistance, didHit ? Color.green : Color.red, 1f);
        
        if (didHit)
        {
            Debug.Log($"[PICKAXE] HIT something! Name: {hit.transform.name}, Tag: {hit.transform.tag}, Distance: {hit.distance}, Layer: {LayerMask.LayerToName(hit.transform.gameObject.layer)}");
            
            Transform targetTransform = hit.transform;
            
            if (!targetTransform.CompareTag("Mineable") && targetTransform.parent != null && targetTransform.parent.CompareTag("Mineable"))
            {
                Debug.Log($"[PICKAXE] Child hit, checking parent: {targetTransform.parent.name}");
                targetTransform = targetTransform.parent;
            }
            
            if (targetTransform.CompareTag("Mineable"))
            {
                Debug.Log($"[PICKAXE] Trying to mine {targetTransform.name}, looking for Crystal component...");
                Crystal crystal = targetTransform.GetComponentInParent<Crystal>();
                
                if (crystal != null)
                {
                    Debug.Log($"[PICKAXE] Mining {crystal.name}");
                    crystal.MineCrystal(pickaxeStatsScriptableObject.GetStat(Stat.miningDamage), hit.point, hit.normal);
                    UpdateCrystalHealthBar(crystal);

                    //added by char
                    if (!isSwinging)
                        StartCoroutine(Swing());
                }
                else
                {
                    Debug.LogError($"[PICKAXE] Hit object '{targetTransform.name}' tagged 'Mineable' but has no Crystal component in parent hierarchy!");
                }
            }
            else
            {
                Debug.LogWarning($"[PICKAXE] Hit object '{hit.transform.name}' but tag is '{hit.transform.tag}' not 'Mineable'");
            }
        }
        else
        {
            Debug.Log($"[PICKAXE] No hit detected within reach distance {reachDistance}");
        }

        nextMineTime = Time.time + (pickaxeStatsScriptableObject.GetStat(Stat.miningSpeed));
    }

    public IEnumerator HandleMine()
    {
        while (true)
        {
            Mine();
            yield return new WaitForSeconds(pickaxeStatsScriptableObject.GetStat(Stat.miningSpeed));
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

    //plays animation by char
    private IEnumerator Swing()
    {
        isSwinging = true;

        float halfDuration = swingAnimationDuration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(0f, 50f, t / halfDuration);
            transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            yield return null;
        }

        t = 0f;

        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float angle = Mathf.Lerp(50f, 0f, t / halfDuration);
            transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            yield return null;
        }

        transform.localRotation = Quaternion.identity;
        isSwinging = false;
    }

}

