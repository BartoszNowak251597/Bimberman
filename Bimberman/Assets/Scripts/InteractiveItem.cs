using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    [Header("Prompt nad obiektem")]
    public Transform interactPrompt;

    [Header("Outline")]
    public GameObject outlineObject;

    public virtual void Interact()
    {
        Debug.Log("Interakcja z: " + gameObject.name);
    }

    public virtual void SetFocused(bool focused)
    {
        if (outlineObject)
        {
            outlineObject.SetActive(focused);
        }
    }

    public virtual bool CanInteract() {
        return true;
    }
}