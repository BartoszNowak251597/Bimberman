using UnityEngine;

namespace Crafting
{
    public class TapDispenser : MonoBehaviour, IInteractable
    {
        [Header("Source")]
        public Heating heating;

        [Header("Points")]
        public Transform bottleSnapPoint;
        public Transform bottlePlaceZone;

        [Header("Visual")]
        public GameObject pouringStream;

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
            Debug.Log("Klik");
        }

        public string GetInteractionText(PlayerMouseInteractor interactor)
        {
            BottleData mixture = heating.GetMixture();
            return "Alkohol" + mixture;
        }
    }
}