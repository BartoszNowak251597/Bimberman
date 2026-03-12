using UnityEngine;
using TMPro;              
using System.Collections.Generic;

public class BaseScript : MonoBehaviour
{
    public GameObject InventoryText;
    public GameObject UI;
    public PlayerInventory playerInventory;

    void Awake()
    {
        UI.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.I))
        {
            UI.SetActive(!UI.activeSelf);
            if(UI.activeSelf)
            {
                TMP_Text textComponent = InventoryText.GetComponent<TMP_Text>();
                if (textComponent == null)
                {
                    Debug.Log("text component is null");
                    return;
                }
                textComponent.text = "Inventory: \n";
                textComponent.text += "Ingredients: \n";
                foreach (IngredientType ingredient in PlayerController.playerInstance.inventory.ingredients)
                {
                    textComponent.text += "- " + ingredient.ToString() + "\n";
                }
                textComponent.text += "Potions: \n";
                foreach (BimberType bimber in PlayerController.playerInstance.inventory.potions)
                {
                    textComponent.text += "- " + bimber.ToString() + "\n";
                }
                textComponent.text += "Collectables: \n";
                foreach (Collectible collectable in PlayerController.playerInstance.inventory.collectables)
                {
                    textComponent.text += "- " + collectable.ToString() + "\n";
                }
            }
        }
    }

}
