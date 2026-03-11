using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    private List<InteractiveItem> availableItems = new List<InteractiveItem>();
    private PlayerInputActions inputActions;

    public GameObject attackInstance;
    private InteractiveItem currentFocusedItem;

    void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                InteractiveItem item = hit.collider.GetComponent<InteractiveItem>();
                string currentScene = SceneManager.GetActiveScene().name;

                
                if (item != null && currentScene == "Base")
                {
                    item.Interact();
                }
                else if (currentScene != "Base")
                {
                    Instantiate(attackInstance, hit.point, Quaternion.identity);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Collectable"))
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        Renderer renderer = player.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            Color randomColor = hit.rigidbody.gameObject.GetComponent<Renderer>().material.color;
                            renderer.material.color = randomColor;
                        }
                    }
                    Destroy(hit.collider.gameObject);
                }
            }
        }
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
