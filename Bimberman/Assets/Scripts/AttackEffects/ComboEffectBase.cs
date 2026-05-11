using UnityEngine;

public enum EffectType
{
    EXPLODE,
    FIRE,
    PETRIFY,
    TORNADO
}

public class ComboEffectBase : MonoBehaviour
{
    // Wartoœæ od 0 do 1 w zale¿noœci od proporcji sk³adniku eksplozji w potce
    //private float lifetime;
    public float duration = 2f;
    private float elapsed = 0f;
    [Header(" Effect1 ")]
    [Range(0, 1)]
    public float effect1Strength;
    public float maxEffect1Range;
    public float maxEffect1Damage;
    [Header(" Effect 2")]
   // [Range(0, 1)]
    protected float effect2Strength;
    public float GetRange()
    {
        return effect1Strength * maxEffect1Range;
    }

    public float GetDamage()
    {
        return effect1Strength * maxEffect1Damage;
    }

    protected void Awake()
    {
        effect2Strength = 1- effect1Strength;
    }

    public void Update()
    {
        //float factor = lifetime / GetRange();

        //factor = Mathf.Sin(factor * Mathf.PI / 2);

        //explosionRenderer.material.SetFloat("_ExplosionTime", factor * GetRange());

        //lifetime += Time.deltaTime * 30;

        //if (lifetime > GetRange())
        //{
        //    Destroy(this.gameObject);
        //}
        elapsed += Time.deltaTime;
        //Debug.Log($"ComboEffectBase Update, elapsed={elapsed}, duration={duration}");
        float factor = elapsed / duration;
        if (elapsed >= duration) Destroy(gameObject);
    }
}
