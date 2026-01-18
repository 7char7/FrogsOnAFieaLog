using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tools;
    [SerializeField] private int startingToolIndex = 0;
    [SerializeField] private int currentToolIndex = 0;
    public event Action<GameObject> OnToolSwitched;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < tools.Length; i++)
        {
            if (tools[i] != null)
            {
                if (i == startingToolIndex)
                {
                    tools[i].SetActive(true);
                }
                else
                {
                    tools[i].SetActive(false);
                }
            }
        }

        OnToolSwitched?.Invoke(tools[startingToolIndex]);
    }

    private void SwitchTool()
    {
        tools[currentToolIndex].SetActive(false);
        currentToolIndex = (currentToolIndex + 1) % tools.Length;
        tools[currentToolIndex].SetActive(true);
        OnToolSwitched?.Invoke(tools[currentToolIndex]);
    }

    public void OnSwitchTool(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SwitchTool();
        }
    }
}
