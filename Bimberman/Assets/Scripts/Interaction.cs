using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private readonly List<InteractiveItem> availableItems = new List<InteractiveItem>();
    private PlayerInputActions inputActions;

    private InteractiveItem currentFocusedItem;

    void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        inputActions.Enable();
        inputActions.Player.Interact.performed += OnInteractPerformed;
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Interact.performed -= OnInteractPerformed;
            inputActions.Disable();
        }
    }

    private void Update()
    {
        UpdateFocusedItem();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentFocusedItem == null)
            return;

        currentFocusedItem.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractiveItem item = other.GetComponent<InteractiveItem>();

        if (item == null)
            item = other.GetComponentInParent<InteractiveItem>();

        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
            UpdateFocusedItem();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractiveItem item = other.GetComponent<InteractiveItem>();

        if (item == null)
            item = other.GetComponentInParent<InteractiveItem>();

        if (item != null && availableItems.Contains(item))
        {
            availableItems.Remove(item);

            if (currentFocusedItem == item)
                currentFocusedItem.SetFocused(false);

            UpdateFocusedItem();
        }
    }

    private void UpdateFocusedItem()
    {
        availableItems.RemoveAll(item => item == null);

        if (availableItems.Count == 0)
        {
            if (currentFocusedItem != null)
            {
                currentFocusedItem.SetFocused(false);
                currentFocusedItem = null;
            }
            return;
        }

        InteractiveItem closest = null;
        float closestDistance = float.MaxValue;

        foreach (InteractiveItem item in availableItems)
        {
            if (item == null)
                continue;

            float distance = (transform.position - item.transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = item;
            }
        }

        if (closest == null)
        {
            if (currentFocusedItem != null)
            {
                currentFocusedItem.SetFocused(false);
                currentFocusedItem = null;
            }
            return;
        }

        if (currentFocusedItem == closest)
            return;

        if (currentFocusedItem != null)
            currentFocusedItem.SetFocused(false);

        currentFocusedItem = closest;
        currentFocusedItem.SetFocused(true);
    }
}