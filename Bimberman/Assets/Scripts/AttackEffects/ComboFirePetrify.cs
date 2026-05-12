using UnityEngine;

public class ComboFirePetrify : ComboEffectBase
{
    /// <summary>
    /// Zadaje obra¿enia DOT w zale¿noœci od si³y ognia i spowalnia w zale¿noœci od si³y petrify 
    /// </summary>
    /// 

    private void Start()
    {
        //Debug.Log($"ComboFirePetrify Start, effect1Strength={effect1Strength}, effect2Strength={effect2Strength}");
        base.Awake();

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"ComboFirePetrify trigger with {other.name}");
        if (other.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
            enemy.TakeDamage(GetDamage());
            Burn(enemy, effect2Strength);
            Petrify(enemy, effect2Strength);
            // }
        }
    }
    private void Burn(Skeleton enemy, float strenght)
    {
        enemy.SetEffectTime(EffectType.FIRE, duration * (1 - strenght));
        enemy.SetEffect(EffectType.FIRE, GetDamage());
    }

    private void Petrify(Skeleton enemy, float strength)
    {
        enemy.SetSpeed(enemy.speed * (0.5f + 0.5f * (1 - strength)));
        enemy.SetEffect(EffectType.PETRIFY, 0);
        enemy.SetEffectTime(EffectType.PETRIFY, duration * (1 - strength));
    }

    void Update()
    {
        base.Update();
    }

}
