using UnityEngine;

namespace Crafting
{
    public class ConveyorEndZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            ConveyorBottle bottle = other.GetComponentInParent<ConveyorBottle>();

            if (bottle == null) return;

            bottle.ReachEnd();
        }
    }
}