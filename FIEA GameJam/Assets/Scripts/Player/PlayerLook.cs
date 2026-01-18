using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{

    [Header("References")]
    public Transform head;
    public Camera camera;
    public Transform headlight;
    public Transform tool;

    [Header("Settings")]
    public float mouseSensitivity = .04f;
    public float maxLookX = 85f;
    public float minLookX = -85f;

    private Vector2 lookInput;
    private float rotX;

    private Vector3 headStartLocalPos;
    private Vector3 cameraStartLocalPos;

    private float targetCameraYOffset = 0f;
    private float currentCameraYOffset = 0f;
    private const float CAMERA_OFFSET_SMOOTH_SPEED = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        headStartLocalPos = head.localPosition;

        if (camera != null)
            cameraStartLocalPos = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        HandleLook();
        HandleHandRotation();
        UpdateCameraHeight();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
        {
            lookInput = Vector2.zero;
            return;
        }
        lookInput = context.ReadValue<Vector2>();
    }

    void HandleLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        rotX -= lookInput.y * mouseSensitivity;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        head.localRotation = Quaternion.Euler(rotX, 0f, 0f);
    }

    void HandleHandRotation()
    {
        if (headlight == null) return;

        Quaternion targetRot = Quaternion.LookRotation(camera.transform.forward, camera.transform.up);
        headlight.rotation = Quaternion.Slerp(headlight.rotation, targetRot, Time.deltaTime * 10f);
    }

    public void SetCameraOffset(float yOffset)
    {
        targetCameraYOffset = yOffset;
    }

    void UpdateCameraHeight()
    {
        if (camera == null) return;

        currentCameraYOffset = Mathf.Lerp(currentCameraYOffset, targetCameraYOffset, Time.deltaTime * CAMERA_OFFSET_SMOOTH_SPEED);
        camera.transform.localPosition = cameraStartLocalPos + new Vector3(0f, currentCameraYOffset, 0f);
    }
}
