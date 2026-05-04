using UnityEngine;

namespace Crafting
{
    public class TapDispenser : MonoBehaviour, IInteractable
    {
        [Header("Source")]
        public Heating heating;

        [Header("Bottle Detection")]
        public Collider bottleTrigger;

        [Header("Pouring")]
        public float fillPerSecond = 0.8f;

        [Header("Visual")]
        public GameObject pouringStream;

        private bool isTapHeld = false;
        private ConveyorBottle currentConveyorBottle;

        private void Reset()
        {
            SetupBottleTrigger();
        }

        private void OnValidate()
        {
            SetupBottleTrigger();
        }

        private void Start()
        {
            SetupBottleTrigger();

            if (pouringStream != null)
                pouringStream.SetActive(false);
        }

        private void Update()
        {
            bool shouldShowStream = isTapHeld
                && currentConveyorBottle != null
                && !currentConveyorBottle.isFilled
                && GetReadyAlcohol() != null;

            if (pouringStream != null)
                pouringStream.SetActive(shouldShowStream);

            if (!shouldShowStream)
                return;

            BottleData alcohol = GetReadyAlcohol();

            currentConveyorBottle.Pour(
                fillPerSecond * Time.deltaTime,
                alcohol
            );
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsBottleTriggerActive())
                return;

            ConveyorBottle bottle = other.GetComponentInParent<ConveyorBottle>();

            if (bottle == null)
                return;

            currentConveyorBottle = bottle;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsBottleTriggerActive())
                return;

            ConveyorBottle bottle = other.GetComponentInParent<ConveyorBottle>();

            if (bottle == null)
                return;

            if (bottle == currentConveyorBottle)
                currentConveyorBottle = null;
        }

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public void OnPressStart(PlayerMouseInteractor interactor)
        {
            isTapHeld = true;
        }

        public void OnPressEnd(PlayerMouseInteractor interactor)
        {
            isTapHeld = false;

            if (pouringStream != null)
                pouringStream.SetActive(false);
        }

        public void OnClick(PlayerMouseInteractor interactor)
        {
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            return "Przytrzymaj LPM, żeby nalać";
        }

        private BottleData GetReadyAlcohol()
        {
            if (heating == null)
                return null;

            BottleData alcohol = heating.GetMixture();

            if (alcohol == null)
                return null;

            if (alcohol.stage != BrewStage.Finished)
                return null;

            return alcohol;
        }

        private void SetupBottleTrigger()
        {
            if (bottleTrigger != null)
                bottleTrigger.isTrigger = true;
        }

        private bool IsBottleTriggerActive()
        {
            return bottleTrigger == null || bottleTrigger.isTrigger;
        }
    }
}