using UnityEngine;

public class PlayerMode : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private BaseViewController baseViewController;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private bool inBaseMode;

    public void EnterBaseMode(Transform playerOrigin, Transform[] lookPoints, Vector3 cameraLocalOffset, int startIndex = 0)
    {
        inBaseMode = true;

        if (playerController != null && playerOrigin != null)
        {
            playerController.transform.position = playerOrigin.position;
            playerController.transform.rotation = playerOrigin.rotation;
            playerController.SetMode(PlayerController.PlayerMode.StationaryPOV);
        }

        if (cameraController != null)
            cameraController.enabled = false;

        if (baseViewController != null)
        {
            baseViewController.enabled = true;
            baseViewController.Initialize(playerOrigin, lookPoints, cameraLocalOffset, startIndex);
        }
    }

    public void ExitBaseMode()
    {
        inBaseMode = false;

        if (baseViewController != null)
            baseViewController.enabled = false;

        if (cameraController != null)
        {
            cameraController.enabled = true;
            cameraController.ExitStationMode();
            cameraController.target = playerController.transform;
        }

        if (playerController != null)
        {
            playerController.SetMode(PlayerController.PlayerMode.Normal);
        }
    }

    public bool IsInBaseMode()
    {
        return inBaseMode;
    }
}