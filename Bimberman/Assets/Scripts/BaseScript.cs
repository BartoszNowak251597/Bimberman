using UnityEngine;
using TMPro;              
using System.Collections.Generic;

public class BaseScript : GameModeScript
{
    void Awake()
    {
        LevelLoader.currentLoadedScene = "Base";
    }

    void Start()
    {

    }

    void Update()
    {
        if (PlayerController.playerInstance == null)
		{
            return;
		}
    }

}
