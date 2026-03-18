using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestilatorScript : InteractiveItem {
	public Canvas uiCanvas;
	public Transform cameraPos;

	public TextMeshProUGUI waterUnitsText;
	public Button addWaterButton;

	public TextMeshProUGUI sugarUnitsText;
	public Button addSugarButton;

	[HideInInspector]
	public Stack<Ingredient> waterUnits = new Stack<Ingredient>();
	public Stack<Ingredient> sugarUnits = new Stack<Ingredient>();

	public Button makeMoonshineButton;

	public GameObject moonshinePrefab;
	public PotionCollectible moonshinePlacement;

	public void RefreshUI()
	{
		var ingredients = PlayerController.playerInstance.inventory.ingredients;

		addWaterButton.interactable = ingredients.Count( ingredient => ingredient.type == Ingredient.IngredientType.Water ) > 0;

		waterUnitsText.text = string.Format("Water Units: {0}", waterUnits.Count);

		addSugarButton.interactable = ingredients.Count( ingredient => ingredient.type == Ingredient.IngredientType.Sugar ) > 0;

		sugarUnitsText.text = string.Format("Sugar Units: {0}", sugarUnits.Count);

		makeMoonshineButton.gameObject.SetActive(waterUnits.Count > 0 && sugarUnits.Count > 0 && moonshinePlacement.potion == null);
	}

	public override void Interact() {
		uiCanvas.gameObject.SetActive(true);

		interactPrompt.gameObject.SetActive(false);

		var player = PlayerController.playerInstance;

		player.EnterStationMode();

		player.playerCamera.GetComponent<CameraController>().EnterStationMode(cameraPos);

		RefreshUI();
	}

	public void Exit()
	{
		uiCanvas.gameObject.SetActive(false);

		interactPrompt.gameObject.SetActive(true);

		var player = PlayerController.playerInstance;

		player.ExitStationMode();

		player.playerCamera.GetComponent<CameraController>().ExitStationMode();
	}

	public void AddWater()
	{
		var ingredients = PlayerController.playerInstance.inventory.ingredients;

		var waterBottle = ingredients.FirstOrDefault( i => i.type == Ingredient.IngredientType.Water );

		if (waterBottle != null)
		{
			Debug.Log("Does it vork?: " + ingredients.Remove(waterBottle));

			this.waterUnits.Push(waterBottle);
			waterBottle.transform.parent = this.transform;

			RefreshUI();
		}
	}

	public void AddSugar()
	{
		var ingredients = PlayerController.playerInstance.inventory.ingredients;

		var sugarCube = ingredients.FirstOrDefault( i => i.type == Ingredient.IngredientType.Sugar );

		if (sugarCube != null)
		{
			Debug.Log("Does that vork?: " + ingredients.Remove(sugarCube));

			ingredients.Remove(sugarCube);

			this.sugarUnits.Push(sugarCube);
			sugarCube.transform.parent = this.transform;

			RefreshUI();
		}
	}

	public void MakeMoonshine()
	{
		var moonshine = Instantiate(moonshinePrefab, moonshinePlacement.transform);

		moonshine.transform.localPosition = Vector3.zero;
		moonshine.transform.localRotation = Quaternion.identity;

		moonshine.SetActive(true);

		moonshinePlacement.potion = moonshine.GetComponent<Potion>();

		Destroy(sugarUnits.Pop());
		Destroy(waterUnits.Pop());

		RefreshUI();
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Exit();
		}
	}

	public override bool CanInteract()
	{
		return moonshinePlacement.potion == null;
	}
}