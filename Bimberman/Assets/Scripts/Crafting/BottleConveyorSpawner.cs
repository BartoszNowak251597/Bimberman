using System.Collections;
using UnityEngine;

namespace Crafting
{
    public class BottleConveyorSpawner : MonoBehaviour
    {
        [Header("References")]
        public Heating heating;
        public ConveyorBelt conveyorBelt;
        public GameObject bottlePrefab;
        public Transform spawnPoint;

        [Header("Spawn Settings")]
        public float spawnInterval = 1.5f;

        [Header("Penalty")]
        public float missedBottleQualityPenalty = 10f;

        private bool isRunning = false;
        private int spawnedBottleCount = 0;
        private int resolvedBottleCount = 0;
        private int bottlesToSpawnThisRun = 0;

        public bool IsRunning => isRunning;

        public void StartConveyor()
        {
            BottleData alcohol = GetReadyAlcohol();

            if (alcohol.bottlesToFill <= 0)
            {
                alcohol.CalculateBottleYield();
            }

            isRunning = true;
            spawnedBottleCount = 0;
            resolvedBottleCount = 0;
            bottlesToSpawnThisRun = alcohol.bottlesToFill - alcohol.bottlesFilled;

            if (conveyorBelt != null)
            {
                conveyorBelt.StartBelt();
            }

            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (spawnedBottleCount < bottlesToSpawnThisRun)
            {
                SpawnBottle();
                spawnedBottleCount++;

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnBottle()
        {

            GameObject bottleObject = Instantiate(
                bottlePrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            ConveyorBottle bottle = bottleObject.GetComponent<ConveyorBottle>();

            if (bottle == null)
            {
                bottle = bottleObject.AddComponent<ConveyorBottle>();
            }

            bottle.OnBottleFilled += HandleBottleFilled;
            bottle.OnBottleMissed += HandleBottleMissed;
        }

        private void HandleBottleFilled(ConveyorBottle bottle)
        {
            BottleData alcohol = GetReadyAlcohol();

            if (alcohol == null) return;

            alcohol.bottlesFilled++;
            resolvedBottleCount++;

            CheckIfFinished();
        }

        private void HandleBottleMissed(ConveyorBottle bottle)
        {
            BottleData alcohol = heating != null ? heating.GetMixture() : null;

            if (alcohol != null)
            {
                alcohol.qualityPercent -= missedBottleQualityPenalty;
                alcohol.qualityPercent = Mathf.Clamp(alcohol.qualityPercent, 0f, 100f);
            }

            resolvedBottleCount++;
            CheckIfFinished();
        }

        private void CheckIfFinished()
        {
            if (resolvedBottleCount < bottlesToSpawnThisRun) return;

            isRunning = false;

            if (conveyorBelt != null)
            {
                conveyorBelt.StopBelt();
            }
        }

        private BottleData GetReadyAlcohol()
        {
            if (heating == null) return null;

            BottleData alcohol = heating.GetMixture();

            if (alcohol == null) return null;
            if (alcohol.stage != BrewStage.Finished) return null;

            return alcohol;
        }
    }
}