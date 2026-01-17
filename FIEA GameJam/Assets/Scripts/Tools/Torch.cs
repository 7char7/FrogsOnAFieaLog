using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class Torch : MonoBehaviour
{
    public Stats torchStatsScriptableObject;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private GameObject torch;
    [SerializeField] private float currentNumberOfTorches;
    [SerializeField] private float placeDistance = 1.5f;

    void Awake()
    {
        torchStatsScriptableObject = Instantiate(torchStatsScriptableObject);
        currentNumberOfTorches = torchStatsScriptableObject.GetStat(Stat.numberOfTorches);
    }

    private void PlaceTorch()
    {
        RaycastHit hit;

        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, placeDistance))
        {
            Debug.Log("No surface hit");
            return;
        }

        if (!hit.transform.CompareTag("Wall") && !hit.transform.CompareTag("Ground"))
        {
            Debug.Log("Invalid surface");
            return;
        }

        if (currentNumberOfTorches <= 0)
        {
            Debug.Log("No torches left");
            return;
        }

        Vector3 spawnPosition = hit.point + hit.normal * 0.05f;

        Quaternion spawnRotation;

        bool isGround = Vector3.Dot(hit.normal, Vector3.up) > 0.75f;

        if (isGround)
        {
            spawnRotation = Quaternion.identity;
        }
        else
        {
            Quaternion faceOut = Quaternion.LookRotation(hit.normal);

            spawnRotation = faceOut * Quaternion.Euler(20f, 0f, 0f);
        }

        Instantiate(torch, spawnPosition, spawnRotation);
        currentNumberOfTorches--;
    }

    public void OnPlaceTorch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PlaceTorch();
        }
    }
}
