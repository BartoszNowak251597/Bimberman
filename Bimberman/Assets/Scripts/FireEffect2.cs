using UnityEngine;

public class FireEffect2 : MonoBehaviour
{
    // Wartość od 0 do 1 w zależności od proporcji składniku ognia w potce
    [Range(0, 1)]
    public float strength;
    public float maxRange;
    public float maxDamage;
    public GameObject visual;
    public float lifetime;

    private float GetRange() {
        return strength * maxRange;
    }

    private float GetDamage() {
        return strength * maxDamage;
    }

    public void Awake() {
        this.visual.transform.localScale = Vector3.one * GetRange();
    }

    public void Update() {
        foreach (var enemy in GameObject.FindObjectsByType<Skeleton>(FindObjectsSortMode.None)) {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= GetRange()) {
                enemy.TakeDamage(GetDamage() * Time.deltaTime);
            }
        }

        if (this.lifetime < 0) {
            Destroy(this.gameObject);
        }

        this.lifetime -= Time.deltaTime;
    }
}
