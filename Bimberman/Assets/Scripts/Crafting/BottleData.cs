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
        public float qualityPercent = 100f;
        public int maxBottleCount = 4;
        public int bottlesToFill = 0;
        public int bottlesFilled = 0;
        public float powerStat = 0f;
        public float tasteStat = 0f;
        public float valueStat = 0f;

        public bool IsInCorrectTemperatureRange()
        {
            return currentTemperature >= targetTemperature - temperatureTolerance &&
                   currentTemperature <= targetTemperature + temperatureTolerance;
        }

        public bool IsOverheated()
        {
            return currentTemperature > targetTemperature + temperatureTolerance;
        }

        public void CalculateBottleYield()
        {
            maxBottleCount = 4;

            if (qualityPercent >= 90f)
            {
                bottlesToFill = 4;
            }
            else if (qualityPercent >= 70f)
            {
                bottlesToFill = 3;
            }
            else if (qualityPercent >= 50f)
            {
                bottlesToFill = 2;
            }
            else if (qualityPercent >= 30f)
            {
                bottlesToFill = 1;
            }
            else
            {
                bottlesToFill = 0;
            }

            bottlesFilled = 0;
        }

        public bool HasBottleServingsLeft()
        {
            return bottlesFilled < bottlesToFill;
        }

        public float GetStatMultiplier()
        {
            if (bottlesToFill >= 4)
                return 1f;

            if (bottlesToFill == 3)
                return 0.9f;

            if (bottlesToFill == 2)
                return 0.75f;

            if (bottlesToFill == 1)
                return 0.55f;

            return 0f;
        }

        public BottleData CreateCopyForFilledBottle()
        {
            return new BottleData
            {
                usedIngredients = new List<IngredientData>(usedIngredients),

                liquidColor = liquidColor,

                stage = BrewStage.Finished,

                currentTemperature = currentTemperature,
                targetTemperature = targetTemperature,
                temperatureTolerance = temperatureTolerance,

                qualityPercent = qualityPercent,

                maxBottleCount = maxBottleCount,
                bottlesToFill = bottlesToFill,
                bottlesFilled = bottlesFilled,

                powerStat = powerStat,
                tasteStat = tasteStat,
                valueStat = valueStat
            };
        }
    }
}