using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<GameObject> bars;
    public UnityEngine.UI.RawImage circleRenderer;

    public void OpenUI()
    {
        this.gameObject.SetActive(true);

        PlayerInventory inventory = PlayerController.playerInstance.inventory;

        int circlesCount = Mathf.Max(inventory.potions.Count, 4);

        if (circlesCount != bars.Count)
        {
            if (circlesCount < bars.Count)
            {
                for (int i = circlesCount; i < bars.Count; i++)
                {
                    Destroy(bars[i]);
                }
            }
            else
            {
                for (int i = bars.Count; i < circlesCount; i++)
                {
                    bars.Add(Instantiate<GameObject>(bars[0], bars[0].transform.parent));
                }
            }
        }

        for (int i = 0; i < bars.Count; i++)
        {
            bars[i].transform.rotation = Quaternion.Euler(0, 0, i * (360 / bars.Count));
        }

        circleRenderer.material.SetFloat("_SplitCount", bars.Count);
        
        var potions = PlayerController.playerInstance.inventory.potions;

        float angleStep = Mathf.PI * 2 / Mathf.Max(potions.Count, 4);
        for (int i = 0; i < potions.Count; i++)
        {
            float angle = angleStep * (i + 0.5f);

            float distFromCenter = 0.5f;
            float aspectRatio = (float) Screen.width / Screen.height;

            Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            float x = Mathf.Sin(angle) * distFromCenter / aspectRatio;
            float y = Mathf.Cos(angle) * distFromCenter;

            Ray ray = Camera.main.ScreenPointToRay(center + center * new Vector2(x, y));

            Vector3 pos = ray.GetPoint(3);

            potions[i].transform.position = pos;
            potions[i].transform.parent = Camera.main.transform;
            potions[i].gameObject.SetActive(true);
        }

        Time.timeScale = 0.2f;
    }

    public void HideUI()
    {
        this.gameObject.SetActive(false);

        var potions = PlayerController.playerInstance.inventory.potions;

        for (int i = 0; i < potions.Count; i++)
        {
            potions[i].gameObject.SetActive(false);
        }

        Time.timeScale = 1;
    }

    public void Update()
    {
        Vector2 mousePos = Input.mousePosition / new Vector2(Screen.width, Screen.height);

        mousePos -= new Vector2(0.5f, 0.5f);

        float tan = ((Mathf.Atan2(mousePos.x, mousePos.y) + Mathf.PI) * bars.Count) / (Mathf.PI * 2);

        circleRenderer.material.SetFloat("_Highlight", (int) tan);

        var potions = PlayerController.playerInstance.inventory.potions;

        for (int i = 0; i < potions.Count; i++)
        {
            potions[i].transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            potions[i].transform.rotation *= Quaternion.AngleAxis(60 * Time.time, Vector3.up);
            potions[i].transform.rotation *= Quaternion.AngleAxis(20, Camera.main.transform.right);
        }

        if (Input.GetMouseButtonDown(0))
        {
            HideUI();
        }
    }
}