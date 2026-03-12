using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationInteraction : InteractiveItem
{
    [Header("Widok stacji")]
    public Transform stationCameraPoint;

    [Header("Obiekt gotowej mikstury")]
    public GameObject bimberDisplayObject;

    [Header("UI")]
    public BrewStationUI brewStationUI;

    [Header("Mieszanie")]
    public MixSlot mixSlot;
    public List<DraggableIngredient> sceneIngredients = new List<DraggableIngredient>();

    private bool inStationView = false;
    private bool isBrewingActive = false;

    private CameraController cameraController;
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    private List<BimberRecipe> recipes;
    private int currentRecipeIndex;
    private BimberRecipe currentRecipe;

    public bool IsBrewingActive => isBrewingActive;

    public override void Interact()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        playerController = player.GetComponent<PlayerController>();
        playerInventory = player.GetComponent<PlayerInventory>();

        if (Camera.main != null)
            cameraController = Camera.main.GetComponent<CameraController>();

        recipes = BimberRecipeBook.GetRecipes();
        currentRecipeIndex = 0;
        currentRecipe = null;

        if (playerController != null)
            playerController.SetMovementEnabled(false);

        if (cameraController != null && stationCameraPoint != null)
            cameraController.EnterStationMode(stationCameraPoint);

        if (bimberDisplayObject != null)
            bimberDisplayObject.SetActive(false);

        HideAllSceneIngredients();
        mixSlot?.ResetSlot();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (brewStationUI != null)
        {
            brewStationUI.gameObject.SetActive(true);
            brewStationUI.Show();

            if (brewStationUI.previousButton != null)
            {
                brewStationUI.previousButton.onClick.RemoveListener(SelectPreviousRecipe);
                brewStationUI.previousButton.onClick.AddListener(SelectPreviousRecipe);
            }

            if (brewStationUI.nextButton != null)
            {
                brewStationUI.nextButton.onClick.RemoveListener(SelectNextRecipe);
                brewStationUI.nextButton.onClick.AddListener(SelectNextRecipe);
            }

            if (brewStationUI.distillButton != null)
            {
                brewStationUI.distillButton.onClick.RemoveListener(BeginBrewing);
                brewStationUI.distillButton.onClick.AddListener(BeginBrewing);
            }

            if (brewStationUI.leaveButton != null)
            {
                brewStationUI.leaveButton.onClick.RemoveListener(ExitStationView);
                brewStationUI.leaveButton.onClick.AddListener(ExitStationView);
            }
        }

        RefreshUI();
        inStationView = true;
        isBrewingActive = false;
    }

    private void Update()
    {
        if (!inStationView) return;

        if (!isBrewingActive)
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.qKey.wasPressedThisFrame)
                    SelectPreviousRecipe();

                if (Keyboard.current.eKey.wasPressedThisFrame)
                    SelectNextRecipe();

                if (Keyboard.current.fKey.wasPressedThisFrame)
                    BeginBrewing();
            }
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            ExitStationView();
    }

    private void SelectPreviousRecipe()
    {
        if (recipes == null || recipes.Count == 0 || isBrewingActive) return;

        currentRecipeIndex--;
        if (currentRecipeIndex < 0)
            currentRecipeIndex = recipes.Count - 1;

        RefreshUI();
    }

    private void SelectNextRecipe()
    {
        if (recipes == null || recipes.Count == 0 || isBrewingActive) return;

        currentRecipeIndex++;
        if (currentRecipeIndex >= recipes.Count)
            currentRecipeIndex = 0;

        RefreshUI();
    }

    private void BeginBrewing()
    {
        if (recipes == null || recipes.Count == 0 || playerInventory == null || mixSlot == null)
            return;

        currentRecipe = recipes[currentRecipeIndex];

        if (!playerInventory.HasIngredients(currentRecipe.ingredients))
        {
            if (brewStationUI != null)
                brewStationUI.SetStatus("Brakuje składników");

            return;
        }

        if (brewStationUI != null)
            brewStationUI.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isBrewingActive = true;

        mixSlot.SetRecipe(currentRecipe.ingredients);

        if (bimberDisplayObject != null)
            bimberDisplayObject.SetActive(false);

        ShowOnlyNeededIngredients(currentRecipe.ingredients);
    }

    public void CollectPotion()
    {
        if (!isBrewingActive || currentRecipe == null || playerInventory == null || mixSlot == null)
            return;

        if (!mixSlot.IsComplete())
            return;

        bool removed = playerInventory.RemoveIngredients(currentRecipe.ingredients);
        if (!removed)
        {
            if (brewStationUI != null)
            {
                brewStationUI.gameObject.SetActive(true);
                brewStationUI.Show();
                brewStationUI.SetStatus("Nie udało się zużyć składników");
            }

            isBrewingActive = false;
            HideAllSceneIngredients();
            mixSlot.ResetSlot();

            if (bimberDisplayObject != null)
                bimberDisplayObject.SetActive(false);

            return;
        }

        playerInventory.EquipBimber(currentRecipe.bimberType);
        ExitStationView();
    }

    public void OnPotionCreated()
    {
        HideAllSceneIngredients();
    }

    private void RefreshUI()
    {
        if (brewStationUI == null || recipes == null || recipes.Count == 0 || playerInventory == null)
            return;

        BimberRecipe recipe = recipes[currentRecipeIndex];
        bool canDistill = playerInventory.HasIngredients(recipe.ingredients);

        brewStationUI.UpdateRecipeView(recipe, canDistill);
    }

    private void HideAllSceneIngredients()
    {
        foreach (DraggableIngredient ingredient in sceneIngredients)
        {
            if (ingredient != null)
                ingredient.gameObject.SetActive(false);
        }
    }

    private void ShowOnlyNeededIngredients(List<IngredientType> requiredIngredients)
    {
        HideAllSceneIngredients();

        List<IngredientType> alreadyActivated = new List<IngredientType>();

        foreach (IngredientType required in requiredIngredients)
        {
            int neededCount = CountOccurrences(requiredIngredients, required);
            int currentCount = CountOccurrences(alreadyActivated, required);

            if (currentCount >= neededCount)
                continue;

            foreach (DraggableIngredient ingredient in sceneIngredients)
            {
                if (ingredient == null) continue;
                if (ingredient.IngredientType != required) continue;
                if (ingredient.gameObject.activeSelf) continue;

                ingredient.gameObject.SetActive(true);
                ingredient.Init(this);
                alreadyActivated.Add(required);
                break;
            }
        }
    }

    private int CountOccurrences(List<IngredientType> list, IngredientType type)
    {
        int count = 0;

        foreach (IngredientType item in list)
        {
            if (item == type)
                count++;
        }

        return count;
    }

    public Vector3 GetIngredientOffset(IngredientType type)
    {
        return type switch
        {
            IngredientType.Water => new Vector3(-0.15f, 0f, 0f),
            IngredientType.Sugar => new Vector3(0.15f, 0f, 0f),
            _ => Vector3.zero
        };
    }

    private void ExitStationView()
    {
        isBrewingActive = false;
        currentRecipe = null;

        HideAllSceneIngredients();
        mixSlot?.ResetSlot();

        if (playerController != null)
            playerController.SetMovementEnabled(true);

        if (cameraController != null)
            cameraController.ExitStationMode();

        if (bimberDisplayObject != null)
            bimberDisplayObject.SetActive(false);

        if (brewStationUI != null)
        {
            if (brewStationUI.previousButton != null)
                brewStationUI.previousButton.onClick.RemoveListener(SelectPreviousRecipe);

            if (brewStationUI.nextButton != null)
                brewStationUI.nextButton.onClick.RemoveListener(SelectNextRecipe);

            if (brewStationUI.distillButton != null)
                brewStationUI.distillButton.onClick.RemoveListener(BeginBrewing);

            if (brewStationUI.leaveButton != null)
                brewStationUI.leaveButton.onClick.RemoveListener(ExitStationView);

            brewStationUI.gameObject.SetActive(true);
            brewStationUI.Hide();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        inStationView = false;
    }

	public override bool CanInteract()
	{
		return !inStationView;
	}
}