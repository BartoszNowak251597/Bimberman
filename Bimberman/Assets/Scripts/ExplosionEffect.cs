using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float lifetime = 0.1f;
    public float radius = 1;

    public void Awake() {
        EventManager.Emit(new PotionExplodeEvent() {
            position = this.transform.position,
            radius = this.radius
        });
    }

    public void Update() {
        lifetime -= Time.deltaTime;

		if (lifetime < 0) {
			Destroy(this.gameObject);
		}
    }
}
