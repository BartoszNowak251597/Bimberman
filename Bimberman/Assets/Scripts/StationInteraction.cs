using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StationInteraction : InteractiveItem
{
    [Header("Widok stacji")]
    public Transform stationCameraPoint;

    [Header("Podgląd bimbru")]
    public GameObject bimberDisplayObject;

    [Header("UI")]
    public BrewStationUI brewStationUI;

    private bool inStationView = false;

    private CameraController cameraController;
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    private List<BimberRecipe> recipes;
    private int currentRecipeIndex;

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

        if (playerController != null)
            playerController.SetMovementEnabled(false);

        if (cameraController != null && stationCameraPoint != null)
            cameraController.EnterStationMode(stationCameraPoint);

        if (bimberDisplayObject != null)
            bimberDisplayObject.SetActive(true);

        if (brewStationUI != null)
        {
            brewStationUI.Show();

            if (brewStationUI.previousButton != null)
                brewStationUI.previousButton.onClick.AddListener(SelectPreviousRecipe);

            if (brewStationUI.nextButton != null)
                brewStationUI.nextButton.onClick.AddListener(SelectNextRecipe);

            if (brewStationUI.distillButton != null)
                brewStationUI.distillButton.onClick.AddListener(DistillCurrentRecipe);

            if (brewStationUI.leaveButton != null)
                brewStationUI.leaveButton.onClick.AddListener(ExitStationView);
        }

        RefreshUI();
        inStationView = true;
    }

    void Update()
    {
        if (!inStationView) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            SelectPreviousRecipe();

        if (Keyboard.current.eKey.wasPressedThisFrame)
            SelectNextRecipe();

        if (Keyboard.current.fKey.wasPressedThisFrame)
            DistillCurrentRecipe();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            ExitStationView();
    }

    void SelectPreviousRecipe()
    {
        if (recipes == null || recipes.Count == 0) return;

        currentRecipeIndex--;
        if (currentRecipeIndex < 0)
            currentRecipeIndex = recipes.Count - 1;

        RefreshUI();
    }

    void SelectNextRecipe()
    {
        if (recipes == null || recipes.Count == 0) return;

        currentRecipeIndex++;
        if (currentRecipeIndex >= recipes.Count)
            currentRecipeIndex = 0;

        RefreshUI();
    }

    void DistillCurrentRecipe()
    {
        if (recipes == null || recipes.Count == 0 || playerInventory == null)
            return;

        BimberRecipe currentRecipe = recipes[currentRecipeIndex];

        if (!playerInventory.HasIngredients(currentRecipe.ingredients))
        {
            if (brewStationUI != null)
                brewStationUI.SetStatus("Brakuje składników");

            return;
        }

        bool removed = playerInventory.RemoveIngredients(currentRecipe.ingredients);
        if (!removed)
        {
            if (brewStationUI != null)
                brewStationUI.SetStatus("Nie udało się zużyć składników");

            return;
        }

        playerInventory.EquipBimber(currentRecipe.bimberType);

        if (brewStationUI != null)
            brewStationUI.SetStatus("Uwarzono: " + currentRecipe.displayName);

        ExitStationView();
    }

    void RefreshUI()
    {
        if (brewStationUI == null || recipes == null || recipes.Count == 0 || playerInventory == null)
            return;

        BimberRecipe currentRecipe = recipes[currentRecipeIndex];
        bool canDistill = playerInventory.HasIngredients(currentRecipe.ingredients);

        brewStationUI.UpdateRecipeView(currentRecipe, canDistill);
    }

    void ExitStationView()
    {
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
                brewStationUI.distillButton.onClick.RemoveListener(DistillCurrentRecipe);

            if (brewStationUI.leaveButton != null)
                brewStationUI.leaveButton.onClick.RemoveListener(ExitStationView);

            brewStationUI.Hide();
        }

        inStationView = false;
    }
}