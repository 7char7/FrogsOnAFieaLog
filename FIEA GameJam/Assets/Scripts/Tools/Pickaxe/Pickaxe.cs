using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Header("General Settings")]
    [SerializeField] private float reachDistance;
    [SerializeField] private float miningSpeed;
    [SerializeField] private float miningDamage;
    private Coroutine miningCoroutine;
    WaitForSeconds miningWait;

    void Awake()
    {
        miningWait = new WaitForSeconds(1f / miningSpeed);
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
                    // Add code to mine object
                }
            }
            yield return miningWait;
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

