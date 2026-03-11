using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            // Here you can add code to reduce the player's health or trigger a death animation
            //Destroy(collision.gameObject); // This will destroy the player object
        }
        Destroy(gameObject); // This will destroy the bullet after it hits something
    }
}

