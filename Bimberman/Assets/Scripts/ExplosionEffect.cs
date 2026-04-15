using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    public float lifetime = 0.1f;

    public void Update() {
        lifetime -= Time.deltaTime;

		if (lifetime < 0) {
			Destroy(this.gameObject);
		}
    }
}
