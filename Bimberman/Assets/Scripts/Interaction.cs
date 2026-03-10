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
            //Debug.Log("Klikniêto obiekt:");
            //Instantiate(attackInstance, new Vector3( Input.mousePosition.x, 0, Input.mousePosition.z),Quaternion.identity);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                InteractiveItem item = hit.collider.GetComponent<InteractiveItem>();
                string currentScene = SceneManager.GetActiveScene().name;

                // Jeœli jesteœmy w scenie "base" i obiekt jest interaktywny – wykonaj interakcjê
                if (item != null && currentScene == "base")
                {
                    item.Interact();
                }
                else
                {
                    // W przeciwnym razie (inna scena lub obiekt nieinteraktywny) – instancjonuj atak
                    Instantiate(attackInstance, hit.point, Quaternion.identity);
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