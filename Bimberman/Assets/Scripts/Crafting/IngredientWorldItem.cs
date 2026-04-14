using UnityEngine;

namespace Crafting
{
    public class IngredientWorldItem : MonoBehaviour, IInteractable
    {
        public IngredientData data;
        public IngredientBowl sourceBowl;

        private Renderer highlightRenderer;
        private Color originalColor = Color.white;
        private bool isHeld = false;
        private bool initialized = false;

        private void Awake()
        {
            highlightRenderer = GetComponent<Renderer>();

            if (highlightRenderer != null && highlightRenderer.sharedMaterial != null)
            {
                originalColor = highlightRenderer.sharedMaterial.color;
            }
        }

        private void Start()
        {
            ApplyColorFromData();
        }

        public void OnHoverEnter()
        {
            if (!initialized) return;
            if (highlightRenderer == null) return;
            if (isHeld) return;

            highlightRenderer.material.color = Color.yellow;
        }

        public void OnHoverExit()
        {
            if (!initialized) return;
            if (highlightRenderer == null) return;
            if (isHeld) return;

            highlightRenderer.material.color = originalColor;
        }

        public void OnClick(PlayerMouseInteractor interactor)
        {
            if (isHeld) return;
            if (data == null) return;
            if (interactor == null) return;
            if (interactor.HasIngredient()) return;
            if (interactor.HasBottle()) return;

            interactor.PickupWorldIngredient(this);
            isHeld = true;

            if (sourceBowl != null)
            {
                sourceBowl.NotifyIngredientTaken(this);
            }
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            return data != null ? $"Weź: {data.ingredientName}" : "Weź składnik";
        }

        public void SetHeld(bool held)
        {
            isHeld = held;
        }

        public void ApplyColorFromData()
        {
            if (data == null) return;

            if (highlightRenderer == null)
            {
                highlightRenderer = GetComponent<Renderer>();
            }

            if (highlightRenderer == null) return;

            originalColor = data.color;
            highlightRenderer.material.color = originalColor;
            initialized = true;
        }
    }
}