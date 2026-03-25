using UnityEngine;

public class DungeonSceneSetup : MonoBehaviour
{
    private void Start()
    {
        if (PlayerController.playerInstance == null)
            return;

        PlayerMode playerMode = PlayerController.playerInstance.GetComponent<PlayerMode>();
        if (playerMode != null)
        {
            playerMode.ExitBaseMode();
        }
    }
}