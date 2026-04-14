using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    [System.Serializable]
    public class BottleData
    {
        public List<IngredientData> usedIngredients = new();
        public Color liquidColor = Color.clear;
    }
}