using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BrewingContraptionScript : InteractiveItem {
	[Serializable]
	public struct Recipe
	{
		[Serializable]
		public struct RecipeIngredient
		{
			public Ingredient.IngredientType ingredient;
			public int count;
		}

		public RecipeIngredient[] ingredients;
		public GameObject resultPrefab;
	};

	public Canvas uiCanvas;
	public Transform cameraPos;
	
	public Transform moonshinePlace;
	public Button bottlePlaceButton;
	private Potion moonshineBottle;

	public TMP_Dropdown[] slotDropdowns;

	public Recipe[] recipes;
	private Recipe? workingRecipe;

	public Button makeRecipeButton;
	public PotionCollectible potionPlacement;
	
	private void Update()
	{
		if (uiCanvas == null || !uiCanvas.gameObject.activeSelf)
			return;

		if (UnityEngine.InputSystem.Keyboard.current != null &&
		    UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			Exit();
		}
	}
	
	private void RefreshUI()
	{
		var potions = PlayerController.playerInstance.inventory.potions;

		bottlePlaceButton.interactable = potions.Count(p => p.effects.First() == Potion.PotionEffect.Moonshine) > 0;
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

		RefreshWorkingRecipe();
	}

	public void RefreshWorkingRecipe()
	{
		if (this.moonshineBottle == null)
		{
			makeRecipeButton.gameObject.SetActive(false);
			
			return;
		}

		Dictionary<Ingredient.IngredientType, int> ingredientCounts = new Dictionary<Ingredient.IngredientType, int>();

		foreach (var slot in slotDropdowns)
		{
			string ingredientName = slot.options[slot.value].text;

			var ingredient = Enum.Parse<Ingredient.IngredientType>(ingredientName);

			if (ingredientCounts.ContainsKey(ingredient))
			{
				ingredientCounts[ingredient]++;
			}
			else
			{
				ingredientCounts[ingredient] = 1;
			}
		}

		workingRecipe = null;

		foreach (var recipe in recipes)
		{
			bool matches = true;

			foreach (var part in recipe.ingredients)
			{
				if (!ingredientCounts.ContainsKey(part.ingredient) || ingredientCounts[part.ingredient] < part.count)
				{
					matches = false;
					
					break;
				}
			}

			if (matches)
			{
				workingRecipe = recipe;

				break;
			}
		}

		Debug.Log(workingRecipe.HasValue);
		makeRecipeButton.gameObject.SetActive(workingRecipe.HasValue);
	}

	public void OnDropdownSet(int index)
	{
		RefreshWorkingRecipe();
	}

	public void MakeRecipe()
	{
		if (workingRecipe.HasValue)
		{
			var bottleResult = Instantiate(workingRecipe.Value.resultPrefab, this.potionPlacement.transform);

			bottleResult.transform.localPosition = Vector3.zero;
			bottleResult.transform.localRotation = Quaternion.identity;

			bottleResult.SetActive(true);

			potionPlacement.potion = bottleResult.GetComponent<Potion>();

			for (int index = 0; index < workingRecipe.Value.ingredients.Length; index++)
			{
				var ingredient = workingRecipe.Value.ingredients[index];

				while (ingredient.count > 0)
				{
					PlayerController.playerInstance.inventory.ingredients.Remove(PlayerController.playerInstance.inventory.ingredients.Find( i => i.type == ingredient.ingredient ));

					ingredient.count--;
				}
			}

			foreach (var slot in slotDropdowns)
			{
				slot.SetValueWithoutNotify(0);
			}

			Destroy(moonshineBottle.gameObject);
			moonshineBottle = null;

			workingRecipe = null;
			RefreshUI();
		}
	}

	public void CollectResultPotion()
	{
		if (potionPlacement == null || potionPlacement.potion == null)
			return;

		PlayerController.playerInstance.inventory.Collect(potionPlacement.potion);
		potionPlacement.potion = null;

		RefreshUI();
	}

	public override void Interact()
	{
		uiCanvas.gameObject.SetActive(true);
		interactPrompt.gameObject.SetActive(false);

		var player = PlayerController.playerInstance;
		player.EnterStationMode();

		BaseViewController baseViewController = player.playerCamera.GetComponent<BaseViewController>();
		if (baseViewController != null && baseViewController.enabled)
			baseViewController.EnterUiView(cameraPos);
		else
			player.playerCamera.GetComponent<CameraController>().EnterStationMode(cameraPos);

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
			player.playerCamera.GetComponent<CameraController>().ExitStationMode();
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

	public void PlaceMoonshine()
	{
		var potions = PlayerController.playerInstance.inventory.potions;

		var moonshine = potions.FirstOrDefault(p => p.effects.First() == Potion.PotionEffect.Moonshine);

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

	public override bool CanInteract()
	{
		return potionPlacement.potion == null;
	}
}