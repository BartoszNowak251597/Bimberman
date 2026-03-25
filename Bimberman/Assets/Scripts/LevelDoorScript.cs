using UnityEngine;
using UnityEngine.InputSystem;

public class LevelDoorScript : InteractiveItem
{
    public LevelLoader levelLoader;

    public override void Interact() {
        if (levelLoader != null)
        {
            levelLoader.LoadLevel();
        }
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (PlayerController.playerInstance == null || PlayerController.playerInstance.playerCamera == null)
            return;

        Ray ray = PlayerController.playerInstance.playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Interact();
            }
        }
    }
}