using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public List<Ingredient> ingredients = new List<Ingredient>();
    public List<Potion> potions = new List<Potion>();
    public List<CollectibleRecipe> recipes = new List<CollectibleRecipe>();

    public void CollectRecipe(CollectibleRecipe recipe)
    {
        if (this.recipes.Contains(recipe))
        {
            return;
        }

        this.recipes.Add(recipe);
        recipe.gameObject.SetActive(false);

        SceneManager.MoveGameObjectToScene(recipe.gameObject, this.gameObject.scene);
    }

    public void CollectIngredient(Ingredient ingredient)
    {
        if (this.ingredients.Contains(ingredient))
        {
            return;
        }

        this.ingredients.Add(ingredient);
        ingredient.gameObject.SetActive(false);

        SceneManager.MoveGameObjectToScene(ingredient.gameObject, this.gameObject.scene);
    }

    public void Collect(Collectible item)
    {
        if (item == null)
        {
            return;
        }

        if (item is Ingredient)
        {
            CollectIngredient(item as Ingredient);
        }
        else if (item is CollectibleRecipe)
        {
            CollectRecipe(item as CollectibleRecipe);
        }
    }

    public void Collect(Potion potion)
    {
        if (potion == null)
        {
            return;
        }

        this.potions.Add(potion);
        potion.gameObject.SetActive(false);
        potion.transform.parent = null;

        SceneManager.MoveGameObjectToScene(potion.gameObject, this.gameObject.scene);
    }
}