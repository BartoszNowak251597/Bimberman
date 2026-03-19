using UnityEngine;

public class ThrowableInteraction : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (
            collision.gameObject.layer == LayerMask.NameToLayer("WhatIsGround")
            ||
            collision.gameObject.tag == "Enemy"
        ) {
            Destroy(gameObject);

            AttackGenerator.GenerateAttack(GetComponent<Potion>());
        }
    }
}
