using NUnit.Framework.Constraints;
using UnityEngine;

public class Ingredient : Collectible
{
	public enum IngredientType
	{
		None,
		Sugar,
		Water,
		SomeDeadBodyPart,
		Cactus
	}

	public IngredientType type;

	public override string DisplayName()
	{
		return this.type.ToString();
	}
}