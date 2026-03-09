using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log("Interakcja z: " + gameObject.name);
    }
}