using UnityEngine;

public class ComboExplodePetrify : ComboEffectBase
{
    /// <summary>
    /// Effect1 = explosion, 
    /// Effect2 = petrify
    /// 
    /// zadaje obra¿enia w zale¿noœci od si³y explode i spowalnia w zale¿noœci od si³y petrify
    /// 
    /// spowolnienie ma bazowy maksymalny czas trwania i jest zmniejszany w zale¿noœci od si³y petrify 
    /// </summary>
    /// 

    [Header("Petrify")]
    public float maxPetrifyDuration = 3f;




    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log($"ExplodePetrify trigger with {other.name}, range={GetRange()}");
        if (other.TryGetComponent<Skeleton>(out Skeleton enemy))
        {
           // float dist = Vector3.Distance(transform.position, enemy.transform.position);
           // Debug.Log($"Distance to skeleton: {dist}, range: {GetRange()}");
           // if (dist <= GetRange())
           // {
                enemy.TakeDamage(GetDamage());
                Petrify(enemy, effect2Strength);
                enemy.SetEffect(EffectType.PETRIFY);
          // }
        }
    }

    public void Petrify(Skeleton enemy, float strength)
    {
       enemy.SetSpeed(enemy.speed * (0.5f + 0.5f * (1 - strength)));
        enemy.SetEffect(EffectType.PETRIFY);
        enemy.SetEffectTime(EffectType.PETRIFY, maxPetrifyDuration * (1 - strength));
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3( Time.deltaTime , 0, Time.deltaTime );
        //.Log($"ExplodePetrify Update, scale={transform.localScale}, elapsed={elapsed}");
        base.Update();
    }
}
