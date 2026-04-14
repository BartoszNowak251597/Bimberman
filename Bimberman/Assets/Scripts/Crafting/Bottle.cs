using UnityEngine;

namespace Crafting
{
    public class Bottle : MonoBehaviour, IInteractable
    {
        public BottleData currentData;
        public Renderer liquidRenderer;
        public Cauldron cauldron;

        public void OnHoverEnter() { }
        public void OnHoverExit() { }

        public void OnClick(PlayerMouseInteractor interactor)
        {
            if (currentData == null)
            {
                BottleData newBottle = cauldron.PourToBottle();
                if (newBottle != null)
                {
                    currentData = newBottle;
                    UpdateVisual();
                    Debug.Log("Przelano do butelki");
                }

                return;
            }

            if (!interactor.HasIngredient() && !interactor.HasBottle())
            {
                interactor.HoldBottle(currentData);
                currentData = null;
                UpdateVisual();
                Debug.Log("Podniesiono butelkę");
            }
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (currentData == null)
                return "Przelej z kotła do butelki";

            return "Podnieś butelkę";
        }

        private void UpdateVisual()
        {
            if (liquidRenderer == null) return;

            liquidRenderer.material.color = currentData != null
                ? currentData.liquidColor
                : Color.clear;
        }
    }
}