using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    [Header("Prompt nad obiektem")]
    [SerializeField] private GameObject interactPrompt;

    [Header("Outline")]
    [SerializeField] private GameObject outlineObject;

    public virtual void Interact()
    {
        Debug.Log("Interakcja z: " + gameObject.name);
    }

    public virtual void SetFocused(bool focused)
    {
        interactPrompt.SetActive(focused);
        outlineObject.SetActive(focused);
    }
}