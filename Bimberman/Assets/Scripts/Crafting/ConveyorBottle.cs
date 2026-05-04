using UnityEngine;

namespace Crafting
{
    public class ConveyorBottle : MonoBehaviour
    {
        [Header("Fill")]
        [Range(0f, 1f)]
        public float fillAmount = 0f;

        public bool isFilled = false;

        [Header("Data")]
        [SerializeField] private BottleData filledData;

        [Header("Fail")]
        public bool destroyAtEnd = true;

        [Header("Bottle Visual")]
        public Renderer bottleRenderer;

        private bool wasMissed = false;

        public System.Action<ConveyorBottle> OnBottleFilled;
        public System.Action<ConveyorBottle> OnBottleMissed;

        public BottleData GetFilledData()
        {
            return filledData;
        }

        public void Pour(float amount, BottleData alcoholData)
        {
            if (isFilled)
                return;

            if (alcoholData == null)
                return;

            fillAmount += amount;
            fillAmount = Mathf.Clamp01(fillAmount);

            ApplyBottleColor(alcoholData.liquidColor);

            if (fillAmount >= 1f)
            {
                Fill(alcoholData);
            }
        }

        private void Fill(BottleData alcoholData)
        {
            if (isFilled)
                return;

            isFilled = true;

            filledData = alcoholData.CreateCopyForFilledBottle();

            FillableBottle fillableBottle = GetComponent<FillableBottle>();

            if (fillableBottle != null)
            {
                fillableBottle.Fill(filledData);
            }

            ApplyBottleColor(filledData.liquidColor);

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

        private void ApplyBottleColor(Color color)
        {
            if (bottleRenderer == null)
                return;

            bottleRenderer.material.color = color;
        }
    }
}