using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public GameObject InventoryText;
    public InventoryUI inventoryUI;
    public KeyCode inventoryKey;

    public LevelLoader loader;

    void Awake()
    {
        inventoryUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
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

    void TryAgain()
    {
        //SceneManager.LoadScene("Base");
        loader.LoadLevel();
    }
    void Looser()
    {
        Application.Quit();
    }
}
