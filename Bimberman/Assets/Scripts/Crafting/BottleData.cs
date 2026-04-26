using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    public enum BrewStage
    {
        RawMixture,
        Heating,
        HeatedCorrectly,
        Burned,
        Finished
    }

    [System.Serializable]
    public class BottleData
    {
        public List<IngredientData> usedIngredients = new();
        public Color liquidColor = Color.clear;

        public BrewStage stage = BrewStage.RawMixture;

        public float currentTemperature = 20f;
        public float targetTemperature = 75f;
        public float temperatureTolerance = 5f;

        [Header("Quality")]
        public float qualityPercent = 100f;

        public bool IsInCorrectTemperatureRange()
        {
            return currentTemperature >= targetTemperature - temperatureTolerance
                   && currentTemperature <= targetTemperature + temperatureTolerance;
        }

        public bool IsOverheated()
        {
            return currentTemperature > targetTemperature + temperatureTolerance;
        }
    }
}