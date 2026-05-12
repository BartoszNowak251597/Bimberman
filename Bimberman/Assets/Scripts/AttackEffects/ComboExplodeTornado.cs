using Unity.VisualScripting;
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
        base.Awake();
        this.transform.localScale = Vector3.one;

        tornadoRadius = effect2Strength * maxTornadoRadius;
        this.transform.localScale = new Vector3(tornadoRadius,1,tornadoRadius);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
                enemy.TakeDamage(GetDamage());

        }
        if (other.gameObject.TryGetComponent<EnemyBullet>(out EnemyBullet bullet))
        {
                bullet.BulletInTornadoAction(transform, tornadoRadius, rotationSpeed);
            
        }
    }

    private void Update()
    {
        base.Update();
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

}
