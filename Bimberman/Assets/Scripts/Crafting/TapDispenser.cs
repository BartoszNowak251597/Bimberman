using System.Collections;
using UnityEngine;

namespace Crafting
{
    public class TapDispenser : MonoBehaviour, IInteractable
    {
        [Header("Source")]
        public Heating heating;

        [Header("Points")]
        public Transform bottleSnapPoint;

        [Header("Pouring")]
        public float pourDuration = 1.5f;

        [Header("Visual")]
        public GameObject pouringStream;

        private bool isPouring = false;

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public void OnPressStart(PlayerMouseInteractor interactor)
        {
        }

        public void OnPressEnd(PlayerMouseInteractor interactor)
        {
        }

        public void OnClick(PlayerMouseInteractor interactor)
        {
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (isPouring)
                return "Rozlewanie...";

            if (heating == null)
                return "Brak Heating";

            BottleData mixture = heating.GetMixture();

            if (mixture == null)
                return "Brak alkoholu";

            if (mixture.stage != BrewStage.Finished)
                return "Alkohol nie jest gotowy";

            if (mixture.bottlesToFill <= 0)
                mixture.CalculateBottleYield();

            if (mixture.bottlesFilled >= mixture.bottlesToFill)
                return "Partia rozlana";

            return $"Postaw butelkę: {mixture.bottlesFilled}/{mixture.bottlesToFill}";
        }

        public bool TryFillPlacedBottle(FillableBottle bottle)
        {
            if (isPouring)
                return false;

            if (bottle == null)
                return false;

            if (bottle.isFilled)
                return false;

            if (heating == null)
                return false;

            BottleData alcohol = heating.GetMixture();

            if (alcohol == null)
                return false;

            if (alcohol.stage != BrewStage.Finished)
            {
                Debug.Log("Alkohol nie jest jeszcze gotowy.");
                return false;
            }

            if (alcohol.bottlesToFill <= 0)
                alcohol.CalculateBottleYield();

            if (alcohol.bottlesFilled >= alcohol.bottlesToFill)
            {
                Debug.Log("Cała partia została już rozlana.");
                return false;
            }

            StartCoroutine(FillRoutine(bottle, alcohol));
            return true;
        }

        private IEnumerator FillRoutine(FillableBottle bottle, BottleData alcohol)
        {
            isPouring = true;

            PlaceBottle(bottle);
            bottle.SetHeld(true);

            if (pouringStream != null)
                pouringStream.SetActive(true);

            yield return new WaitForSeconds(pourDuration);

            if (pouringStream != null)
                pouringStream.SetActive(false);

            alcohol.bottlesFilled++;

            BottleData filledData = alcohol.CreateCopyForFilledBottle();

            bottle.Fill(filledData);
            bottle.SetHeld(false);

            Debug.Log($"Napełniono butelkę {alcohol.bottlesFilled}/{alcohol.bottlesToFill}.");

            if (alcohol.bottlesFilled >= alcohol.bottlesToFill)
                Debug.Log("Cała partia alkoholu została rozlana.");

            isPouring = false;
        }

        private void PlaceBottle(FillableBottle bottle)
        {
            if (bottle == null)
                return;

            if (bottleSnapPoint == null)
                return;

            bottle.transform.position = bottleSnapPoint.position;
            bottle.transform.rotation = bottleSnapPoint.rotation;
        }
    }
}