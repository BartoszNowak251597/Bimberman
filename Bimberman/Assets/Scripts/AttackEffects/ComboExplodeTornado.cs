using UnityEngine;

public class ComboExplodeTornado : ComboEffectBase
{
    /// <summary>
    ///Explode + Tornado - 
    ///zadaje obra¿enia w zale¿noœci od si³y explode dla wrogów w pobli¿u tornada 
    ///i przechwyca pociski w pobli¿u tornada, tornado ma radius w zale¿noœci od si³y tornada  
    ///</summary>
    ///

    [Header("Tornado")]
    public float maxTornadoDuration = 3f;
    public float maxTornadoRadius = 5f;
    private float rotationSpeed = 90f;
    private float tornadoRadius;

    private void Awake()
    {

         tornadoRadius = effect2Strength * maxTornadoRadius;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= GetRange())
            {
                enemy.TakeDamage(GetDamage());

            }
        }
        if (other.gameObject.TryGetComponent<EnemyBullet>(out EnemyBullet bullet))
        {
            if (Vector3.Distance(transform.position, bullet.transform.position) <= GetRange())
            {
                bullet.BulletInTornadoAction(transform, tornadoRadius, rotationSpeed);
            }
        }
    }

    private void Update()
    {

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

}
