using UnityEngine;

namespace Crafting
{
    public class CameraPointButton : MonoBehaviour, IInteractable
    {
        [Header("Camera")]
        public BaseViewController baseViewController;
        public Transform cameraPoint;

        [Header("Text")]
        public string interactionText = "Przejdź do widoku";

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
            if (baseViewController == null)
            {
                Debug.LogWarning("CameraPointButton: Brak BaseViewController.");
                return;
            }

            if (cameraPoint == null)
            {
                Debug.LogWarning("CameraPointButton: Brak Camera Point.");
                return;
            }

            baseViewController.EnterUiView(cameraPoint);

            Debug.Log("Przełączono kamerę na: " + cameraPoint.name);
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            return interactionText;
        }
    }
}