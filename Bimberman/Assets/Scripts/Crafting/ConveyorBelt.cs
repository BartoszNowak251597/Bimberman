using System.Collections.Generic;
using UnityEngine;

namespace Crafting
{
    public class ConveyorBelt : MonoBehaviour
    {
        [Header("Movement")]
        public float speed = 1.5f;
        public Transform moveTarget;

        [Header("State")]
        public bool isRunning = false;

        private readonly List<ConveyorBottle> bottlesOnBelt = new List<ConveyorBottle>();

        private void Update()
        {
            if (!isRunning)
                return;

            MoveBottles();
        }

        private void MoveBottles()
        {
            Vector3 direction = GetMoveDirection();

            for (int i = bottlesOnBelt.Count - 1; i >= 0; i--)
            {
                ConveyorBottle bottle = bottlesOnBelt[i];

                if (bottle == null)
                {
                    bottlesOnBelt.RemoveAt(i);
                    continue;
                }

                bottle.transform.position += direction * speed * Time.deltaTime;
            }
        }

        private Vector3 GetMoveDirection()
        {
            if (moveTarget != null)
            {
                Vector3 direction = moveTarget.position - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                    return direction.normalized;
            }

            return transform.forward;
        }

        private void OnTriggerEnter(Collider other)
        {
            ConveyorBottle bottle = other.GetComponentInParent<ConveyorBottle>();

            if (bottle == null)
                return;

            if (!bottlesOnBelt.Contains(bottle))
                bottlesOnBelt.Add(bottle);
        }

        private void OnTriggerExit(Collider other)
        {
            ConveyorBottle bottle = other.GetComponentInParent<ConveyorBottle>();

            if (bottle == null)
                return;

            bottlesOnBelt.Remove(bottle);
        }

        public void StartBelt()
        {
            isRunning = true;
        }

        public void StopBelt()
        {
            isRunning = false;
        }
    }
}