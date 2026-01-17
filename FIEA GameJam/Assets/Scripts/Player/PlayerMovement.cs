using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    [SerializeField] private Stats playerStatsScriptableObject;

    private Vector2 moveInput;

    private InputSystem_Actions inputActions;

    private bool isSprinting = false;
    public bool IsSprinting => isSprinting;

    void Awake()
    {
        playerStatsScriptableObject = GetComponent<PlayerManager>().playerStatsScriptableObject;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0f)
            return;

        HandleMovement();
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
    }
}
