using UnityEngine;

namespace Crafting
{
    public class ConveyorBottle : MonoBehaviour
    {
        [Header("Fill")]
        [Range(0f, 1f)]
        public float fillAmount = 0f;

        public bool isFilled = false;

        [Header("Fail")]
        public bool destroyAtEnd = true;

        [Header("Optional Visual")]
        public Transform liquidVisual;
        public Renderer liquidRenderer;

        private bool wasMissed = false;

        public System.Action<ConveyorBottle> OnBottleFilled;
        public System.Action<ConveyorBottle> OnBottleMissed;

        public void Pour(float amount, BottleData alcoholData)
        {
            if (isFilled) return;
            if (alcoholData == null) return;

            fillAmount += amount;
            fillAmount = Mathf.Clamp01(fillAmount);

            UpdateLiquidVisual();

            if (fillAmount >= 1f)
            {
                Fill(alcoholData);
            }
        }

        private void Fill(BottleData alcoholData)
        {
            if (isFilled) return;

            isFilled = true;

            BottleData filledData = alcoholData.CreateCopyForFilledBottle();

            FillableBottle fillableBottle = GetComponent<FillableBottle>();
            if (fillableBottle != null)
            {
                fillableBottle.Fill(filledData);
            }

            if (liquidRenderer != null)
            {
                liquidRenderer.material.color = filledData.liquidColor;
            }

            OnBottleFilled?.Invoke(this);
        }

        public void ReachEnd()
        {
            if (!isFilled && !wasMissed)
            {
                wasMissed = true;
                OnBottleMissed?.Invoke(this);
            }

            if (destroyAtEnd)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateLiquidVisual()
        {
            if (liquidVisual == null) return;

            float yScale = Mathf.Lerp(0.05f, 1f, fillAmount);

            liquidVisual.localScale = new Vector3(
                liquidVisual.localScale.x,
                yScale,
                liquidVisual.localScale.z
            );
        }
    }
}