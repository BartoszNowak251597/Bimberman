using UnityEngine;
using TMPro;              
using System.Collections.Generic;

public class BaseScript : GameModeScript
{
    void Awake()
    {
        LevelLoader.currentLoadedScene = "Base";
        if (PlayerController.playerInstance != null)
        {
            PlayerController.playerInstance.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (PlayerController.playerInstance == null)
		{
            return;
		}
    }

}
