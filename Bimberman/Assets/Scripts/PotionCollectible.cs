using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PotionCollectible : MonoBehaviour
{
    [SerializeField] private StationInteraction stationInteraction;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (stationInteraction == null) return;
        if (!stationInteraction.IsBrewingActive) return;
        if (!gameObject.activeInHierarchy) return;
        if (Mouse.current == null) return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null) return;

        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                stationInteraction.CollectPotion();
            }
        }
    }
}