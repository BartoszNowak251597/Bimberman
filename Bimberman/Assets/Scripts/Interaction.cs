using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    private List<InteractiveItem> availableItems = new List<InteractiveItem>();
    private PlayerInputActions inputActions;

    public GameObject attackInstance;
    private InteractiveItem currentFocusedItem;

    public PlayerInventory playerInventory;
    public GameObject interactionPrompt;

    void Awake()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();
    }

    private void UpdateInteractionPrompt()
    {
        InteractiveItem targetItem = null;

        for (int i = 0; i < this.availableItems.Count; i++) {
            if (this.availableItems[i] != null && this.availableItems[i].isActiveAndEnabled && this.availableItems[i].CanInteract())
            {
                targetItem = this.availableItems[i];

                break;
            }
        }
        
        if (!targetItem)
        {
            this.interactionPrompt.SetActive(false);
            if (currentFocusedItem != null)
            {
                currentFocusedItem.SetFocused(false);
            }
            
            return;
        }

        this.interactionPrompt.SetActive(true);
        this.interactionPrompt.transform.position = targetItem.interactPrompt.transform.position;
        this.interactionPrompt.transform.rotation = Quaternion.LookRotation(
            this.interactionPrompt.transform.position - Camera.main.transform.position
        );

        currentFocusedItem = targetItem;

        currentFocusedItem.SetFocused(true);
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

                if (GameObject.FindAnyObjectByType<BaseScript>() != null)
                {
                    if (item != null)
                    {
                        item.Interact();
                    }
                }
                else
                {
                    if (
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("WhatIsGround")
                        ||
                        hit.collider.gameObject.CompareTag("Enemy")
                    )
                    {
                        Vector3 spawnPoint = hit.point;

                        spawnPoint.y = 0;

                        Instantiate(attackInstance, spawnPoint, Quaternion.identity).SetActive(true);
                    }
                }
            }
        }
        // if (Input.GetMouseButtonDown(1))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;

        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         if (hit.collider.CompareTag("Collectable"))
        //         {
        //             GameObject player = GameObject.FindGameObjectWithTag("Player");
        //             if (player != null)
        //             {
        //                 Renderer renderer = player.GetComponent<Renderer>();
        //                 if (renderer != null)
        //                 {
        //                     Color randomColor = hit.rigidbody.gameObject.GetComponent<Renderer>().material.color;
        //                     renderer.material.color = randomColor;
        //                 }
        //                 CollectableType type = CollectableType.SomeDeadBodyPart;
        //                 playerInventory.collectables.Add(type);

        //             }
        //             Destroy(hit.collider.gameObject);
        //         }
        //     }
        // }
    }

    private void LateUpdate()
    {
        UpdateInteractionPrompt();
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
        if (currentFocusedItem != null)
        {
            currentFocusedItem.Interact();
        }

        this.availableItems.RemoveAll(item => item == null || !item.isActiveAndEnabled);
    }

    public void ClearAvailable()
    {
        // this.availableItems.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractiveItem item = other.GetComponent<InteractiveItem>();

        if (item == null)
            item = other.GetComponentInParent<InteractiveItem>();

        if (item != null && !availableItems.Contains(item))
        {
            availableItems.Add(item);
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
        }
    }

    private void UpdateFocusedItem()
    {
        currentFocusedItem.SetFocused(false);
    }
}
