using UnityEngine;

public class BaseScript : MonoBehaviour
{
    public GameObject InventoryText;
    public GameObject UI;

    void Awake()
    {
        UI.SetActive(false);
        //InventoryText.text += "";
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (UI.activeSelf)
            {
                UI.SetActive(false);
            }
            else
            {
                UI.SetActive(true);
            }
        }
    }

}
