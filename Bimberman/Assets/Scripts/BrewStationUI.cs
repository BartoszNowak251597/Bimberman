using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BrewStationUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Texty")]
    public TMP_Text titleText;
    public TMP_Text recipeNameText;
    public TMP_Text ingredientsText;
    public TMP_Text statusText;
    public TMP_Text hintText;

    [Header("Przyciski")]
    public Button previousButton;
    public Button nextButton;
    public Button distillButton;
    public Button leaveButton;

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void UpdateRecipeView(BimberRecipe recipe, bool canDistill)
    {
        if (recipeNameText != null)
            recipeNameText.text = recipe.displayName;

        if (ingredientsText != null)
            ingredientsText.text = "Składniki: " + FormatIngredients(recipe.ingredients);

        if (statusText != null)
            statusText.text = canDistill ? "Gotowe do pędzenia" : "Brakuje składników";
    }

    public void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private string FormatIngredients(List<IngredientType> ingredients)
    {
        if (ingredients == null || ingredients.Count == 0)
            return "Brak";

        return string.Join(" + ", ingredients);
    }
}