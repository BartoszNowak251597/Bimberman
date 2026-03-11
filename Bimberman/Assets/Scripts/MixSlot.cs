using System.Collections.Generic;
using UnityEngine;

public class MixSlot : MonoBehaviour
{
    [SerializeField] private GameObject potionObject;
    [SerializeField] private StationInteraction stationInteraction;

    private List<IngredientType> requiredIngredients = new List<IngredientType>();
    private List<IngredientType> placedIngredients = new List<IngredientType>();

    public void SetRecipe(List<IngredientType> ingredients)
    {
        requiredIngredients = new List<IngredientType>(ingredients);
        placedIngredients.Clear();

        if (potionObject != null)
            potionObject.SetActive(false);
    }

    public bool TryAddIngredient(IngredientType ingredientType)
    {
        if (requiredIngredients == null || requiredIngredients.Count == 0)
            return false;

        if (!requiredIngredients.Contains(ingredientType))
            return false;

        int requiredCount = CountOccurrences(requiredIngredients, ingredientType);
        int placedCount = CountOccurrences(placedIngredients, ingredientType);

        if (placedCount >= requiredCount)
            return false;

        placedIngredients.Add(ingredientType);
        CheckIfComplete();
        return true;
    }

    public bool IsComplete()
    {
        if (requiredIngredients == null || requiredIngredients.Count == 0)
            return false;

        foreach (IngredientType ingredient in requiredIngredients)
        {
            int requiredCount = CountOccurrences(requiredIngredients, ingredient);
            int placedCount = CountOccurrences(placedIngredients, ingredient);

            if (placedCount < requiredCount)
                return false;
        }

        return true;
    }

    public void ResetSlot()
    {
        placedIngredients.Clear();

        if (potionObject != null)
            potionObject.SetActive(false);
    }

    private void CheckIfComplete()
    {
        if (!IsComplete())
            return;

        if (potionObject != null)
            potionObject.SetActive(true);

        if (stationInteraction != null)
            stationInteraction.OnPotionCreated();
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
}