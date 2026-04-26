using System.Collections;
using UnityEngine;

namespace Crafting
{
    public class Bellows : MonoBehaviour, IInteractable
    {
        [Header("References")]
        public Heating heating;

        [Header("Cooldown")]
        public float clickCooldown = 1f;

        [Header("Press")]
        public Vector3 pressedScaleMultiplier = new Vector3(1f, 0.55f, 1f);
        public float pressDuration = 0.15f;
        public float scaleSpeed = 12f;

        private Vector3 normalScale;
        private bool canClick = true;
        private bool isPressed;

        private void Awake()
        {
            normalScale = transform.localScale;
        }

        private void Update()
        {
            Vector3 targetScale = isPressed
                ? new Vector3(
                    normalScale.x * pressedScaleMultiplier.x,
                    normalScale.y * pressedScaleMultiplier.y,
                    normalScale.z * pressedScaleMultiplier.z
                )
                : normalScale;

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );
        }

        public void OnClick(PlayerMouseInteractor interactor)
        {
            if (!canClick)
                return;

            if (heating == null)
            {
                Debug.LogWarning("Bellows: Missing Heating reference.");
                return;
            }

            if (!heating.HasMixture())
            {
                Debug.Log("Move mixture to heating first.");
                return;
            }

            heating.Blow();

            StartCoroutine(ClickCooldownRoutine());
            StartCoroutine(PressAnimationRoutine());
        }

        public void OnHoverEnter() { }

        public void OnHoverExit() { }

        public void OnPressStart(PlayerMouseInteractor interactor) { }

        public void OnPressEnd(PlayerMouseInteractor interactor) { }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            if (heating == null)
                return "Missing heating";

            if (!heating.HasMixture())
                return "Move mixture first";

            if (!canClick)
                return "Bellows recharging...";

            return "Click to blow";
        }

        private IEnumerator ClickCooldownRoutine()
        {
            canClick = false;
            yield return new WaitForSeconds(clickCooldown);
            canClick = true;
        }

        private IEnumerator PressAnimationRoutine()
        {
            isPressed = true;
            yield return new WaitForSeconds(pressDuration);
            isPressed = false;
        }
    }
}