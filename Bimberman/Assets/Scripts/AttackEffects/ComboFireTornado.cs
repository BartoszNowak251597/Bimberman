using UnityEngine;

public class ComboFireTornado : ComboEffectBase
{
    /// <summary>
    /// Fire + Tornado - 
    /// Zadaje obra¿enia DOT w zale¿noœci od si³y ognia, 
    /// tornado przechwytuje pociski w pobli¿u i ma radius w zale¿noœci od si³y tornada 
    /// </summary>

    [Header("Tornado")]
    public float maxTornadoRadius = 5f;
    private float rotationSpeed = 90f;
    private float tornadoRadius;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
            enemy.TakeDamage(GetDamage());
            Burn(enemy, effect2Strength);
        }
        if (other.gameObject.TryGetComponent<EnemyBullet>(out EnemyBullet bullet))
        {
            bullet.BulletInTornadoAction(transform, tornadoRadius, rotationSpeed);
        }
    }

    private void Burn(Skeleton enemy, float strenght)
    {
        enemy.SetEffectTime(EffectType.FIRE, duration * (1 - strenght));
        enemy.SetEffect(EffectType.FIRE, GetDamage());
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
