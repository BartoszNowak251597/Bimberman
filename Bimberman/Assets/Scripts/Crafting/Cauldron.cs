using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    public class Cauldron : MonoBehaviour, IInteractable
    {
        public List<IngredientData> ingredients = new();
        public int maxIngredients = 3;
        public Renderer liquidRenderer;
        public Transform liquidTargetPoint;
        public float absorbDuration = 0.35f;

        public void OnHoverEnter() { }

        public void OnHoverExit() { }

        public void OnPressStart(PlayerMouseInteractor interactor) { }

        public void OnPressEnd(PlayerMouseInteractor interactor) { }

        public void OnClick(PlayerMouseInteractor interactor)
        {
            if (interactor == null) return;
            if (!interactor.HasIngredient()) return;

            AddHeldIngredientFromInteractor(interactor);
        }

        public void AddHeldIngredientFromInteractor(PlayerMouseInteractor interactor)
        {
            if (interactor == null) return;
            if (!interactor.HasIngredient()) return;
            if (ingredients.Count >= maxIngredients) return;

            IngredientData heldIngredient = interactor.GetHeldIngredient();
            IngredientWorldItem heldItem = interactor.GetHeldWorldIngredient();

            if (heldIngredient == null || heldItem == null)
                return;

            ingredients.Add(heldIngredient);
            interactor.ClearHands();

            StartCoroutine(AbsorbIngredientVisual(heldItem));
            UpdateLiquidVisual();
        }

        private IEnumerator AbsorbIngredientVisual(IngredientWorldItem item)
        {
            if (item == null)
                yield break;

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            Collider col = item.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            Vector3 startPos = item.transform.position;
            Vector3 startScale = item.transform.localScale;
            Vector3 targetPos = liquidTargetPoint != null ? liquidTargetPoint.position : transform.position;

            float time = 0f;

            while (time < absorbDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / absorbDuration);

                item.transform.position = Vector3.Lerp(startPos, targetPos, t);
                item.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

                yield return null;
            }

            Destroy(item.gameObject);
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (interactor != null && interactor.HasIngredient())
            {
                if (ingredients.Count >= maxIngredients)
                    return "Kocioł jest pełny";

                return "Wrzuć do kotła";
            }

            return $"Kocioł ({ingredients.Count}/{maxIngredients})";
        }

        public bool HasMixture()
        {
            return ingredients.Count > 0;
        }

        public BottleData PourToBottle()
        {
            if (ingredients.Count == 0)
                return null;

            BottleData bottle = new BottleData
            {
                usedIngredients = new List<IngredientData>(ingredients),
                liquidColor = CalculateColor(),
                stage = BrewStage.RawMixture,
                currentTemperature = 20f,
                qualityPercent = 100f
            };

            ingredients.Clear();
            UpdateLiquidVisual();

            return bottle;
        }

        private Color CalculateColor()
        {
            if (ingredients.Count == 0)
                return Color.clear;

            float r = 0f;
            float g = 0f;
            float b = 0f;

            foreach (var ing in ingredients)
            {
                r += ing.color.r;
                g += ing.color.g;
                b += ing.color.b;
            }

            return new Color(r / ingredients.Count, g / ingredients.Count, b / ingredients.Count, 1f);
        }

        private void UpdateLiquidVisual()
        {
            if (liquidRenderer == null)
                return;

            liquidRenderer.material.color = CalculateColor();
        }
    }
}