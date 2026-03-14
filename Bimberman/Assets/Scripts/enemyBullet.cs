using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public int damage = 1;
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
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); 
            }
            else
            {
                player = collision.collider.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
        Destroy(gameObject); // This will destroy the bullet after it hits something
    }
}

