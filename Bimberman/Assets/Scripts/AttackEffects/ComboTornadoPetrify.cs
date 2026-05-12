using UnityEngine;

public class ComboTornadoPetrify : ComboEffectBase
{
    /// <summary>
    /// Tornado + Petrify – 
    /// tornado przechwytuje pociski w pobli¿u i ma radius w zale¿noœci od si³y tornada 
    /// Spowalnia przeciwników w poblizu tornada w zale¿noœci od si³y petrify,
    /// </summary>
         [Header("Tornado")]
    public float maxTornadoDuration = 3f;
    public float maxTornadoRadius = 5f;
    private float rotationSpeed = 90f;
    private float tornadoRadius;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
            Petrify(enemy, effect2Strength);
        }
        if (other.gameObject.TryGetComponent<EnemyBullet>(out EnemyBullet bullet))
        {
            bullet.BulletInTornadoAction(transform, tornadoRadius, rotationSpeed);
        }
    }

    private void Petrify(Skeleton enemy, float strength)
    {
        enemy.SetSpeed(enemy.speed * (0.5f + 0.5f * (1 - strength)));
        enemy.SetEffect(EffectType.PETRIFY, 0);
        enemy.SetEffectTime(EffectType.PETRIFY, duration * (1 - strength));
    }


    void Start()
    {
        base.Awake();
        this.transform.localScale = Vector3.one;

        tornadoRadius = effect2Strength * maxTornadoRadius;
        this.transform.localScale = new Vector3(tornadoRadius, 1, tornadoRadius);
    }

    void Update()
    {
        base.Update();

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
