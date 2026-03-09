using System.Collections.Generic;

public class BimberRecipe
{
    public BimberType bimberType;
    public string displayName;
    public List<IngredientType> ingredients;

    public BimberRecipe(BimberType bimberType, string displayName, List<IngredientType> ingredients)
    {
        this.bimberType = bimberType;
        this.displayName = displayName;
        this.ingredients = ingredients;
    }
}

public static class BimberRecipeBook
{
    private static readonly List<BimberRecipe> recipes = new List<BimberRecipe>()
    {
        new BimberRecipe(BimberType.ClassicBimber, "Klasyczny Bimber", 
            new List<IngredientType>
            {
                IngredientType.Sugar,
                IngredientType.Water
            }
        )
    };

    public static List<BimberRecipe> GetRecipes()
    {
        return recipes;
    }
}