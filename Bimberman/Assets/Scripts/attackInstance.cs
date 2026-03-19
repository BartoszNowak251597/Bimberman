using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class attackInstance : MonoBehaviour
{
    float t = 0;
    void Start()
    {
        
    }
    void Update()
    {
        t += 1;
        if (t>180)
        {
            Destroy(gameObject);
        }
    }

    private bool canAttack()
    {
        return GameObject.FindAnyObjectByType<BaseScript>() == null;
    }


    private void Awake()
    {
        if (!canAttack())
        {
            Destroy(gameObject);
        }
        else
        {
            var hits = Physics.CapsuleCastAll(transform.position, transform.position + Vector3.up, 0.5f, Vector3.up);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemyComponent = hit.collider.gameObject.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(5);
                    }
                }
            }
        }
    }
}
