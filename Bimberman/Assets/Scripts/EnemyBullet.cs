using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 200;
    private float lifetime = 0f;
    public float maxLifetime = 5f;
    private bool isInTornado = false;
    private Transform tornadoCenter;
    private float orbitRadius;
    private float orbitSpeed; 
    private float currentAngle;
    private float orbitHeightOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInTornado && tornadoCenter != null)
        {
            // Oblicz now¹ pozycjê na orbicie
            currentAngle += orbitSpeed * Time.deltaTime;
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
            transform.position = new Vector3(
         tornadoCenter.position.x + offset.x,
         //tornadoCenter.position.y + orbitHeightOffset,
         tornadoCenter.position.y + 0.5f,
         tornadoCenter.position.z + offset.z
     );
        }
        else
        {
            lifetime += Time.deltaTime;
            if (lifetime >= maxLifetime)
                Destroy(gameObject);
        }
    }

    public void BulletInTornadoAction(Transform tornadoCenter, float orbitRadius, float orbitSpeedDegrees)
    {
        this.tornadoCenter = tornadoCenter;
        this.orbitRadius = orbitRadius + this.transform.localScale.x;
        this.orbitSpeed = orbitSpeedDegrees;
        isInTornado = true;
        orbitHeightOffset = transform.position.y - tornadoCenter.position.y;
        Vector3 dir = transform.position - tornadoCenter.position;
        dir.y = 0;
        currentAngle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && !isInTornado)
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                // player.TakeDamage(damage);
            }
            else
            {
                player = collision.collider.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    //  player.TakeDamage(damage);
                }
            }
        }
        else if (collision.collider.CompareTag("Enemy") && isInTornado)
        {
            Skeleton enemy = collision.collider.GetComponent<Skeleton>();
            if (enemy == null) enemy = collision.collider.GetComponentInParent<Skeleton>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Bullet hit enemy! Damage: " + damage);
            }
            Destroy(gameObject); // This will destroy the bullet after it hits something
        }
    }
    private void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Enemy") && isInTornado)
        {
            Skeleton enemy = other.GetComponent<Skeleton>();
            if (enemy == null) enemy = other.GetComponentInParent<Skeleton>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Bullet hit enemy! Damage: " + damage);
            }
            Destroy(gameObject);
        }
    }
}
