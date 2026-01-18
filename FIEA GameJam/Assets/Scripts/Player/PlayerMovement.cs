using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    [SerializeField] private Stats playerStatsScriptableObject;
    [SerializeField] private PlayerLook playerLook;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideDecay = 5f;
    [SerializeField] private float minSlideSpeed = 2f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float slideCameraHeight = 0.5f;
    
    [Header("Footstep Settings")]
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float sprintFootstepInterval = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Vector2 moveInput;

    private InputSystem_Actions inputActions;

    private bool isSprinting = false;
    public bool IsSprinting => isSprinting;

    private bool isSliding = false;
    public bool IsSliding => isSliding;
    private float slideTimer = 0f;
    private Vector3 slideDirection;
    
    private float footstepTimer = 0f;

    void Awake()
    {
        playerStatsScriptableObject = GetComponent<PlayerManager>().playerStatsScriptableObject;
        
        if (playerLook == null)
        {
            playerLook = GetComponent<PlayerLook>();
        }
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        if (isSliding)
        {
            HandleSlide();
        }
        else
        {
            HandleMovement();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
            return;

        isSprinting = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
            return;

        if (context.performed)
        {
            if (isSprinting && moveInput.magnitude > 0.1f && !isSliding)
            {
                StartSlide();
            }
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        
        slideDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        
        isSprinting = false;

        if (playerLook != null)
        {
            playerLook.SetCameraOffset(-slideCameraHeight);
        }
    }

    void HandleSlide()
    {
        slideTimer += Time.fixedDeltaTime;

        float currentSlideSpeed = Mathf.Lerp(slideSpeed, minSlideSpeed, slideTimer / slideDuration);

        Vector3 targetVel = slideDirection * currentSlideSpeed;

#if UNITY_6000_0_OR_NEWER
        targetVel.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVel;
#else
        targetVel.y = rb.velocity.y;
        rb.velocity = targetVel;
#endif

        if (slideTimer >= slideDuration || currentSlideSpeed <= minSlideSpeed)
        {
            isSliding = false;
            slideTimer = 0f;

            if (playerLook != null)
            {
                playerLook.SetCameraOffset(0f);
            }
        }
    }

    void HandleMovement()
    {
        Vector3 moveDir = transform.forward * moveInput.y + transform.right * moveInput.x;
        float baseSpeed = isSprinting ? playerStatsScriptableObject.GetStat(Stat.movementSpeed) * 1.5f : playerStatsScriptableObject.GetStat(Stat.movementSpeed);
        float currentSpeed = baseSpeed;
        Vector3 targetVel = moveDir * currentSpeed;

#if UNITY_6000_0_OR_NEWER
        targetVel.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVel;
#else
        targetVel.y = rb.velocity.y;
        rb.velocity = targetVel;
#endif

        HandleFootsteps();
    }
    
    void HandleFootsteps()
    {
        if (moveInput.magnitude > 0.1f && IsGrounded())
        {
            footstepTimer += Time.fixedDeltaTime;
            float currentInterval = isSprinting ? sprintFootstepInterval : footstepInterval;
            
            if (footstepTimer >= currentInterval)
            {
                footstepTimer = 0f;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstepSound();
                }
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    
    bool IsGrounded()
    {
        float rayDistance = 1.1f;
        return Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);
    }
}
