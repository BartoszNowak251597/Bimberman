using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour
{
    private List<InteractiveItem> availableItems = new List<InteractiveItem>();
    private PlayerInputActions inputActions;

    public GameObject attackInstance;

    void Awake()
    {
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

                
                if (item != null && currentScene == "base")
                {
                    item.Interact();
                }
                else
                {
                   
                    Instantiate(attackInstance, hit.point, Quaternion.identity);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
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