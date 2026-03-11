using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public BimberType equippedBimber = BimberType.None;

    [Header("Testowe składniki")]
    public List<IngredientType> ingredients = new List<IngredientType>()
    {
        IngredientType.Sugar,
        IngredientType.Water
    };

    public List<BimberType> potions = new List<BimberType>()
    {
        BimberType.ClassicBimber
    };

    public void EquipBimber(BimberType bimber)
    {
        equippedBimber = bimber;
    }

    public bool HasBimber()
    {
        return equippedBimber != BimberType.None;
    }

    public bool HasIngredients(List<IngredientType> required)
    {
        List<IngredientType> temp = new List<IngredientType>(ingredients);

        foreach (IngredientType ingredient in required)
        {
            if (!temp.Contains(ingredient))
                return false;

            temp.Remove(ingredient);
        }

        return true;
    }

    public bool RemoveIngredients(List<IngredientType> required)
    {
        if (!HasIngredients(required))
            return false;

        foreach (IngredientType ingredient in required)
        {
            ingredients.Remove(ingredient);
        }

        return true;
    }
}