using System.Collections;
using UnityEngine;

public class PlayerOrigin : MonoBehaviour
{
    [Header("Look Targets")]
    [SerializeField] private Transform[] lookPoints;
    [SerializeField] private int startViewIndex = 0;

    [Header("Camera Offset")]
    [SerializeField] private Vector3 cameraLocalOffset = new Vector3(0f, 1.6f, 0f);

    private IEnumerator Start()
    {
        yield return null;

        if (PlayerController.playerInstance == null)
            yield break;

        PlayerMode modeController = PlayerController.playerInstance.GetComponent<PlayerMode>();
        if (modeController == null)
            yield break;

        modeController.EnterBaseMode(transform, lookPoints, cameraLocalOffset, startViewIndex);
    }
}