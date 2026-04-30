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
            Debug.Log("Klik");
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (isPouring)
                return "Rozlewanie...";

            BottleData mixture = heating.GetMixture();

            if (mixture == null)
                return "Brak alkoholu";

            return "Alkohol: " + mixture.stage;
        }

        public bool TryPourTest(FillableBottle bottle)
        {
            if (isPouring)
                return false;

            if (bottle == null)
                return false;

            if (bottle.isFilled)
                return false;

            StartCoroutine(PourTestRoutine(bottle));
            return true;
        }

        private IEnumerator PourTestRoutine(FillableBottle bottle)
        {
            isPouring = true;

            PlaceBottle(bottle);

            bottle.SetHeld(true);
            if (pouringStream != null)
                pouringStream.SetActive(true);

            yield return new WaitForSeconds(pourDuration);

            if (pouringStream != null)
                pouringStream.SetActive(false);

            bottle.Fill(null);
            bottle.SetHeld(false);

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