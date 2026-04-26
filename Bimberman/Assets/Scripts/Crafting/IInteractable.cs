namespace Crafting
{
    public interface IInteractable
    {
        void OnHoverEnter();
        void OnHoverExit();

        void OnClick(PlayerMouseInteractor interactor);

        void OnPressStart(PlayerMouseInteractor interactor);
        void OnPressEnd(PlayerMouseInteractor interactor);

        string GetInteractionText(PlayerMouseInteractor interactor);
    }
}