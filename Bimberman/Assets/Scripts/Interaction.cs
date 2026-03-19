using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public float throwSpeed = 10f;      
    public float throwCooldown = 1;

    private float lastThrowTime;

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

    private void ThrowProjectile(GameObject projectile, Vector3 target) {
        Vector3 startPos = transform.position + transform.forward * 1f;
        startPos.y = transform.position.y+0.5f; 

        Vector3 horizontalStart = new Vector3(startPos.x, 0, startPos.z);
        Vector3 horizontalTarget = new Vector3(target.x, 0, target.z);
        float distance = Vector3.Distance(horizontalStart, horizontalTarget);
        float heightDiff = target.y - startPos.y; 
        float timeToTarget = distance / this.throwSpeed;

        float gravity = Physics.gravity.magnitude; 
        float v_y = (heightDiff + 0.5f * gravity * timeToTarget * timeToTarget) / timeToTarget;

        Vector3 horizontalDir = (horizontalTarget - horizontalStart).normalized;
        Vector3 velocity = horizontalDir * this.throwSpeed + Vector3.up * v_y;

        projectile.transform.position = startPos;
        projectile.transform.rotation = Quaternion.LookRotation(velocity);

        projectile.AddComponent<ThrowableInteraction>();

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody>();
        }

        rb.linearVelocity = velocity;
        
        projectile.SetActive(true);

        lastThrowTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool isInBase = GameObject.FindAnyObjectByType<BaseScript>() != null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (isInBase) {
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) {
                    InteractiveItem item = hit.collider.GetComponent<InteractiveItem>();
                    if (item != null) item.Interact();
                }
            }
            else if (PlayerController.playerInstance.inventory.equippedPotion != null && Time.time > this.lastThrowTime + this.throwCooldown) {
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                groundPlane.Raycast(ray, out float dist);

                Vector3 targetPos = ray.GetPoint(dist);
                GameObject projectile = Instantiate(PlayerController.playerInstance.inventory.equippedPotion.gameObject);

                ThrowProjectile(projectile, targetPos);
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
