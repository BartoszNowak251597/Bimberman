using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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

		addWaterButton.interactable = ingredients.Count(ingredient => ingredient.type == Ingredient.IngredientType.Water) > 0;

		waterUnitsText.text = string.Format("Water Units: {0}", waterUnits.Count);

		addSugarButton.interactable = ingredients.Count(ingredient => ingredient.type == Ingredient.IngredientType.Sugar) > 0;

		sugarUnitsText.text = string.Format("Sugar Units: {0}", sugarUnits.Count);

		makeMoonshineButton.gameObject.SetActive(
			waterUnits.Count > 0 &&
			sugarUnits.Count > 0 &&
			moonshinePlacement.potion == null
		);
	}

	public override void Interact() {
		uiCanvas.gameObject.SetActive(true);
		interactPrompt.gameObject.SetActive(false);

		var player = PlayerController.playerInstance;
		player.EnterStationMode();

		BaseViewController baseViewController = player.playerCamera.GetComponent<BaseViewController>();
		if (baseViewController != null && baseViewController.enabled)
		{
			baseViewController.EnterUiView(cameraPos);
		}
		else
		{
			CameraController cameraController = player.playerCamera.GetComponent<CameraController>();
			if (cameraController != null)
				cameraController.EnterStationMode(cameraPos);
		}

		RefreshUI();
	}

	public void Exit()
	{
		uiCanvas.gameObject.SetActive(false);
		interactPrompt.gameObject.SetActive(false);

		var player = PlayerController.playerInstance;

		BaseViewController baseViewController = player.playerCamera.GetComponent<BaseViewController>();
		if (baseViewController != null && baseViewController.enabled)
		{
			baseViewController.ExitUiView();
			StartCoroutine(FinishExitAfterCameraReturns(player, baseViewController));
		}
		else
		{
			CameraController cameraController = player.playerCamera.GetComponent<CameraController>();
			if (cameraController != null)
				cameraController.ExitStationMode();

			player.ExitStationMode();
		}
	}

	private IEnumerator FinishExitAfterCameraReturns(PlayerController player, BaseViewController baseViewController)
	{
		while (baseViewController.IsBusy())
			yield return null;

		baseViewController.ForceBaseView();
		player.ExitStationMode();
	}

	public void AddWater()
	{
		var ingredients = PlayerController.playerInstance.inventory.ingredients;

		var waterBottle = ingredients.FirstOrDefault(i => i.type == Ingredient.IngredientType.Water);

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

		var sugarCube = ingredients.FirstOrDefault(i => i.type == Ingredient.IngredientType.Sugar);

		if (sugarCube != null)
		{
			Debug.Log("Does that vork?: " + ingredients.Remove(sugarCube));

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

	public void CollectMoonshine()
	{
		if (moonshinePlacement == null || moonshinePlacement.potion == null)
			return;

		PlayerController.playerInstance.inventory.Collect(moonshinePlacement.potion);
		moonshinePlacement.potion = null;

		RefreshUI();
	}

	public void Update()
	{
		if (uiCanvas == null || !uiCanvas.gameObject.activeSelf)
			return;

		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			Exit();
		}
	}

	public override bool CanInteract()
	{
		return moonshinePlacement.potion == null;
	}
}