using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BrewingContraptionScript : InteractiveItem {
	public Canvas uiCanvas;
	public Transform cameraPos;
	
	public Transform moonshinePlace;
	public Button bottlePlaceButton;
	private Potion moonshineBottle;

	public TMP_Dropdown[] slotDropdowns;

	private void RefreshUI()
	{
		var potions = PlayerController.playerInstance.inventory.potions;

		bottlePlaceButton.interactable = potions.Count( p => p.effects.First() == Potion.PotionEffect.Moonshine ) > 0;
		bottlePlaceButton.gameObject.SetActive(moonshineBottle == null);

		var ingredients = PlayerController.playerInstance.inventory.ingredients;

		var ingredientTypes = new List<string>() { "None" };

		ingredientTypes.AddRange(ingredients.Where( i => i.type != Ingredient.IngredientType.Sugar && i.type != Ingredient.IngredientType.Water ).GroupBy( i => i.type ).Select( i => i.First().type.ToString() ));

		foreach (var slot in slotDropdowns)
		{
			slot.options.Clear();
			slot.SetValueWithoutNotify(0);

			for (int i = 0; i < ingredientTypes.Count; i++)
			{
				slot.options.Add(new TMP_Dropdown.OptionData()
				{
					text = ingredientTypes[i]
				});
			}
		}
	}

	public void OnDropdownSet(int index)
	{
		Debug.Log(index);
	}

	public override void Interact()
	{
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

	public void PlaceMoonshine()
	{
		var potions = PlayerController.playerInstance.inventory.potions;

		var moonshine = potions.FirstOrDefault( p => p.effects.First() == Potion.PotionEffect.Moonshine );

		if (moonshine == null)
		{
			return;
		}

		potions.Remove(moonshine);

		moonshine.transform.SetParent(moonshinePlace);
		moonshine.transform.localPosition = Vector3.zero;
		moonshine.transform.localRotation = Quaternion.identity;
		moonshine.gameObject.SetActive(true);

		moonshineBottle = moonshine;

		RefreshUI();
	}
}