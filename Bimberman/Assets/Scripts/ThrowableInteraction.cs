using UnityEngine;

public class ThrowableInteraction : MonoBehaviour
{
    public GameObject attackInstancePrefab;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WhatIsGround"))
        {
            Vector3 hitPoint = collision.contacts[0].point;
            hitPoint.y = 0; 
            Instantiate(attackInstancePrefab, hitPoint, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
