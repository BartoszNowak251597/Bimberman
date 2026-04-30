using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float radius = 1;
    public float speed = 5;
    public MeshRenderer explosionRenderer;
    private float lifetime;

    public void Awake() {
        EventManager.Emit(new PotionExplodeEvent() {
            position = this.transform.position,
            radius = this.radius
        });
    }

    public void Update() {
        float factor = lifetime / radius;

        factor = Mathf.Sin(factor * Mathf.PI / 2);

        explosionRenderer.material.SetFloat("_ExplosionTime", factor * radius);

        lifetime += Time.deltaTime * speed;

        if (lifetime > radius) {
            Destroy(this.gameObject);
        }
    }
}
