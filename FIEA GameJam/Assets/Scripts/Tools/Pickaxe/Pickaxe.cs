using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    public Stats pickaxeStatsScriptableObject; 

    [Header("General Settings")]
    [SerializeField] private float reachDistance;
    [SerializeField] private float fortune;
    [SerializeField] private float miningSpeed;
    [SerializeField] private float miningDamage;
    private Coroutine miningCoroutine;

    void Awake()
    {
        pickaxeStatsScriptableObject = Instantiate(pickaxeStatsScriptableObject);
    }

    public IEnumerator Mine()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, reachDistance))
            {
                if (hit.transform.CompareTag("Mineable"))
                {
                    Debug.Log("Mining " + hit.transform.name);
                    hit.transform.GetComponent<Crystal>().MineCrystal(pickaxeStatsScriptableObject.GetStat(Stat.miningDamage));
                }
            }
            yield return new WaitForSeconds(1f / pickaxeStatsScriptableObject.GetStat(Stat.miningSpeed));
        }
    }

    public void StartMining(InputAction.CallbackContext context)
    {
        if (this.gameObject.activeSelf == false)
            return;

        if (context.started)
        {
            miningCoroutine = StartCoroutine(Mine());
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
}

