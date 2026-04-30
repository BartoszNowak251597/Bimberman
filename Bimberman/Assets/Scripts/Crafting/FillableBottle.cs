using UnityEngine;

namespace Crafting
{
    public class FillableBottle : MonoBehaviour, IInteractable
    {
        [Header("State")]
        public bool isFilled = false;

        [Header("Data")]
        public BottleData data;

        private bool isHeld = false;

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
            if (interactor == null)
                return;

            if (isHeld)
                return;

            if (interactor.HasIngredient())
                return;

            if (interactor.HasBottle())
                return;

            interactor.PickupFillableBottle(this);
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            return isFilled ? "Pełna butelka" : "Weź pustą butelkę";
        }

        public void SetHeld(bool held)
        {
            isHeld = held;

            Rigidbody rb = GetComponent<Rigidbody>();

            if (rb == null)
                return;

            rb.isKinematic = held;
            rb.useGravity = !held;
        }

        public void Fill(BottleData bottleData)
        {
            data = bottleData;
            isFilled = true;
        }
    }
}