using UnityEngine;
using TMPro;              
using System.Collections.Generic;

public class BaseScript : GameModeScript
{
    void Awake()
    {
        LevelLoader.currentLoadedScene = "Base_Pov";
        if (PlayerController.playerInstance != null)
        {
            PlayerController.playerInstance.gameObject.SetActive(true);
        }
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (PlayerController.playerInstance == null)
		{
            return;
		}
    }

}
