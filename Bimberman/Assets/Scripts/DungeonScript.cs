using TMPro;
using UnityEngine;

public class DungeonScript : GameModeScript
{
    void Awake()
    {
        LevelLoader.currentLoadedScene = "Dungeon";
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (PlayerController.playerInstance == null)
        {
            return;
        }
    }
}
