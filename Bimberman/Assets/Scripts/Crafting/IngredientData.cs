using UnityEngine;

namespace Crafting
{
    public enum IngredientRole
    {
        MainEffect,
        Modifier
    }

    public enum MainEffectType
    {
        None,
        Burn,
        Freeze,
        Lighting
    }

    public enum ModifierType
    {
        None,
        Duration,
        Radius
    }

    [CreateAssetMenu(fileName = "Ingredient_", menuName = "Brewing/Ingredient")]
    public class IngredientData : ScriptableObject
    {
        public string ingredientName;
        public IngredientRole role;

        [Header("Main Effect")]
        public MainEffectType mainEffectType = MainEffectType.None;

        [Header("Modifier")]
        public ModifierType modifierType = ModifierType.None;

        [Header("Value")]
        public float value = 1f;

        public Color color = Color.white;
    }
}