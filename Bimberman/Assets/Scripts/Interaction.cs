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

    public GameObject throwablePrefab;  
    public float throwSpeed = 10f;      

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (GameObject.FindAnyObjectByType<BaseScript>() != null)
                {
                    InteractiveItem item = hit.collider.GetComponent<InteractiveItem>();
                    if (item != null) item.Interact();
                }
                else
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("WhatIsGround") ||
                        hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        Vector3 targetPos = hit.point;
                        Vector3 startPos = transform.position + transform.forward * 1f;
                        startPos.y = transform.position.y+0.5f; 

                        float speed = 20f; 
                        Vector3 horizontalStart = new Vector3(startPos.x, 0, startPos.z);
                        Vector3 horizontalTarget = new Vector3(targetPos.x, 0, targetPos.z);
                        float distance = Vector3.Distance(horizontalStart, horizontalTarget);
                        float heightDiff = targetPos.y - startPos.y; 
                        float timeToTarget = distance / speed;

                        float gravity = Physics.gravity.magnitude; 
                        float v_y = (heightDiff + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

                        Vector3 horizontalDir = (horizontalTarget - horizontalStart).normalized;
                        Vector3 velocity = horizontalDir * speed + Vector3.up * v_y;

                        GameObject projectile = Instantiate(throwablePrefab, startPos, Quaternion.LookRotation(velocity));
                        Rigidbody rb = projectile.GetComponent<Rigidbody>();
                        rb.linearVelocity = velocity;

                    }
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
