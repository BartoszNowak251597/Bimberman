using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public GameObject InventoryText;
    public InventoryUI inventoryUI;
    public KeyCode inventoryKey;


    void Awake()
    {
        inventoryUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            inventoryUI.OpenUI();
        }

        if (Input.GetKeyUp(inventoryKey))
        {
            inventoryUI.HideUI();
        }
    }
}
