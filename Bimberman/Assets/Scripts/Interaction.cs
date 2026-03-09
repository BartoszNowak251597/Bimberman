using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private List<InteractiveItem> availableItems = new List<InteractiveItem>();
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (availableItems.Count > 0)
        {
            availableItems[0].Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractiveItem item = other.GetComponent<InteractiveItem>();
        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractiveItem item = other.GetComponent<InteractiveItem>();
        if (item != null && availableItems.Contains(item))
        {
            availableItems.Remove(item);
        }
    }
}