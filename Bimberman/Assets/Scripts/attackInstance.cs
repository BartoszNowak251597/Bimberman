using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class attackInstance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    float t = 0;
    // Update is called once per frame
    void Update()
    {
        t += 1;
        if (t>180)
        {
            //Timer = new Timer(DestroySelf, null, 2000, Timeout.Infinite);
            Destroy(gameObject);
        }
    }

    private bool canAttack()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "Base" ||sceneName=="base")
        {
            return false;
        }
        return true;
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
                //Debug.Log("Hit: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    //Debug.Log("Enemy hit!");
                    //Destroy(hit.collider.gameObject);
                    Enemy enemyComponent = hit.collider.gameObject.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        enemyComponent.TakeDamage(5);
                    }
                }
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision detected with: " + collision.gameObject.name);
    //    //if (collision.collider.TryGetComponent<Enemy>(out var enemy))
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Enemy hit!");
    //        Destroy(collision.gameObject);
    //    }

    //}

}
