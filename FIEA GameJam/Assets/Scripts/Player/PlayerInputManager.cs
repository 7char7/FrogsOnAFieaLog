using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private Gun gun;
    private InputSystem_Actions inputActions;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
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

        if (gun != null && inputActions.Player.Shoot != null)
        {
            inputActions.Player.Shoot.started -= gun.StartFiring;
            inputActions.Player.Shoot.canceled -= gun.StopFiring;
        }

        inputActions.Player.Disable();
    }
}
