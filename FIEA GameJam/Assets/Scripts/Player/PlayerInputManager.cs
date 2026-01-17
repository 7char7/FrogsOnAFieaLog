using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private Gun gun;
    [SerializeField] private Pickaxe pickaxe;
    [SerializeField] private ToolManager toolManager;

    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        if (playerLook == null)
        {
            playerLook = GetComponent<PlayerLook>();
        }

        if (gun == null)
        {
            gun = GetComponentInChildren<Gun>();
        }

        if (pickaxe == null)
        {
            pickaxe = GetComponentInChildren<Pickaxe>();
        }

        if (toolManager == null)
        {
            toolManager = GetComponentInChildren<ToolManager>();
        }

    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        if (playerMovement != null)
        {
            inputActions.Player.Move.performed += playerMovement.OnMove;
            inputActions.Player.Move.canceled += playerMovement.OnMove;
            inputActions.Player.Sprint.performed += playerMovement.OnSprint;
            inputActions.Player.Sprint.canceled += playerMovement.OnSprint;
        }
        else
        {
            Debug.LogError("PlayerMovement null");
        }

        if (playerLook != null)
        {
            inputActions.Player.Look.performed += playerLook.OnLook;
            inputActions.Player.Look.canceled += playerLook.OnLook;
        }
        else
        {
            Debug.LogError("PlayerLook null");
        }

        if (gun != null)
        {
            inputActions.Player.Shoot.started += gun.StartFiring;
            inputActions.Player.Shoot.canceled += gun.StopFiring;
        }
        else
        {
            Debug.LogError("Gun null");
        }

        if (pickaxe != null)
        {
            inputActions.Player.Mine.started += pickaxe.StartMining;
            inputActions.Player.Mine.canceled += pickaxe.StopMining;
        }
        else
        {
            Debug.LogError("Pickaxe null");
        }

        if (toolManager != null)
        {
            inputActions.Player.SwitchTool.performed += toolManager.OnSwitchTool;
            inputActions.Player.SwitchTool.canceled += toolManager.OnSwitchTool;
        }
        else
        {
            Debug.LogError("ToolManager null");
        }
    }

    void OnDisable()
    {
        if (playerMovement != null)
        {
            inputActions.Player.Move.performed -= playerMovement.OnMove;
            inputActions.Player.Move.canceled -= playerMovement.OnMove;
            inputActions.Player.Sprint.performed -= playerMovement.OnSprint;
            inputActions.Player.Sprint.canceled -= playerMovement.OnSprint;
        }

        if (playerLook != null)
        {
            inputActions.Player.Look.performed -= playerLook.OnLook;
            inputActions.Player.Look.canceled -= playerLook.OnLook;
        }

        if (gun != null)
        {
            inputActions.Player.Shoot.started -= gun.StartFiring;
            inputActions.Player.Shoot.canceled -= gun.StopFiring;
        }

        if (pickaxe != null)
        {
            inputActions.Player.Mine.started -= pickaxe.StartMining;
            inputActions.Player.Mine.canceled -= pickaxe.StopMining;
        }

        if (toolManager != null)
        {
            inputActions.Player.SwitchTool.performed -= toolManager.OnSwitchTool;
            inputActions.Player.SwitchTool.canceled -= toolManager.OnSwitchTool;
        }

        inputActions.Player.Disable();
    }
}
