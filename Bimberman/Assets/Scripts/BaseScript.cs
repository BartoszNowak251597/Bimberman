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
        TMP_Text textComponent = InventoryText.GetComponent<TMP_Text>();
        if (textComponent == null)
        {
            Debug.Log("text component is null");
            return;
        }

        //playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.Log("player inventory is null");
            return;
        }
        textComponent.text += "Ingredients: \n";
        foreach (IngredientType ingredient in playerInventory.ingredients)
        {
            textComponent.text += "- " + ingredient.ToString() + "\n";
        }
        textComponent.text += "Potions: \n";
        foreach (BimberType bimber in playerInventory.potions)
        {
            textComponent.text += "- " + bimber.ToString() + "\n";
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UI.SetActive(!UI.activeSelf);
        }
    }

}
