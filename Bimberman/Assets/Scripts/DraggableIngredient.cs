using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class DraggableIngredient : MonoBehaviour
{
    [SerializeField] private IngredientType ingredientType;
    [SerializeField] private Transform startPoint;

    private static DraggableIngredient currentlyDragged;

    private Camera mainCamera;
    private StationInteraction stationInteraction;

    private bool placedInSlot;
    private Plane dragPlane;
    private Vector3 dragOffset;

    public IngredientType IngredientType => ingredientType;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Init(StationInteraction station)
    {
        stationInteraction = station;
        placedInSlot = false;

        ResetToStartPoint();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (stationInteraction == null) return;
        if (!stationInteraction.IsBrewingActive) return;
        if (Mouse.current == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        HandleMousePress();
        HandleDragging();
        HandleMouseRelease();
    }

    private void HandleMousePress()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (currentlyDragged != null) return;
        if (placedInSlot) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                currentlyDragged = this;
                dragPlane = new Plane(Vector3.up, transform.position);

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    dragOffset = transform.position - hitPoint;
                }
            }
        }
    }

    private void HandleDragging()
    {
        if (currentlyDragged != this) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + dragOffset;
        }
    }

    private void HandleMouseRelease()
    {
        if (currentlyDragged != this) return;
        if (!Mouse.current.leftButton.wasReleasedThisFrame) return;

        currentlyDragged = null;

        Collider[] hits = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (Collider hit in hits)
        {
            MixSlot mixSlot = hit.GetComponent<MixSlot>();
            if (mixSlot != null && mixSlot.TryAddIngredient(ingredientType))
            {
                placedInSlot = true;
                transform.position = mixSlot.transform.position + stationInteraction.GetIngredientOffset(ingredientType);
                return;
            }
        }
        ResetToStartPoint();
    }

    public void ResetToStartPoint()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
    }
}