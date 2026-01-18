using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System;

public class Torch : MonoBehaviour
{
    public Stats torchStatsScriptableObject;
    public event Action OnTorchCountChanged;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject torchHandModel;
    [SerializeField] private float currentNumberOfTorches;
    [SerializeField] private float placeDistance = 1.5f;

    void Awake()
    {
        torchStatsScriptableObject = Instantiate(torchStatsScriptableObject);
        currentNumberOfTorches = torchStatsScriptableObject.GetStat(Stat.numberOfTorches);
        
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("Torch: Could not find main camera!");
            }
        }
    }

    private void PlaceTorch()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Torch: Camera transform is not assigned!");
            return;
        }

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
            torchHandModel.SetActive(false);
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
        OnTorchCountChanged?.Invoke();
    }

    public void OnPlaceTorch(InputAction.CallbackContext context)
    {
        if (gameObject.activeSelf == false)
            return;

        if (context.performed)
            PlaceTorch();
    }

    public int GetTorchCount()
    {
        return Mathf.FloorToInt(currentNumberOfTorches);
    }
}
