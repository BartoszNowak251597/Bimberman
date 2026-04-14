namespace Crafting
{
    public interface IInteractable
    {
        void OnHoverEnter();
        void OnHoverExit();
        void OnClick(PlayerMouseInteractor interactor);
        string GetInteractionText(PlayerMouseInteractor interactor);
    }
}