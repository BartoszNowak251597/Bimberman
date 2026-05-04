using UnityEngine;

namespace Crafting
{
    public class Bottle : MonoBehaviour, IInteractable
    {
        [Header("References")]
        public Cauldron cauldron;
        public Heating heating;

        [Header("Camera")]
        public BaseViewController baseViewController;
        public Transform heatingCameraPoint;

        [Header("Optional Visual")]
        public Renderer liquidRenderer;

        private BottleData currentData;

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
            if (cauldron == null)
                return;

            if (heating == null)
                return;

            if (!cauldron.HasMixture())
                return;

            if (heating.HasMixture())
                return;

            currentData = cauldron.PourToBottle();

            if (currentData == null)
                return;

            UpdateVisual();

            heating.SetMixture(currentData);

            currentData = null;
            UpdateVisual();

            if (baseViewController != null && heatingCameraPoint != null)
            {
                baseViewController.EnterUiView(heatingCameraPoint);
            }
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (cauldron == null)
                return "Missing cauldron";

            if (!cauldron.HasMixture())
                return "Add ingredients first";

            if (heating != null && heating.HasMixture())
                return "Heating station occupied";

            return "Move to heating";
        }

        private void UpdateVisual()
        {
            if (liquidRenderer == null)
                return;

            liquidRenderer.material.color = currentData != null
                ? currentData.liquidColor
                : Color.clear;
        }
    }
}