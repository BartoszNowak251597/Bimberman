using NUnit.Framework.Constraints;
using UnityEngine;

public class Ingredient : Collectible
{
	public enum IngredientType
	{
		None,
		Sugar,
		Water,
		Bones,
		Roots
	}

	public IngredientType type;

	public override string DisplayName()
	{
		return this.type.ToString();
	}
}