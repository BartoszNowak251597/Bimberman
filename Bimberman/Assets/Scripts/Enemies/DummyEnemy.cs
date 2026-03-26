using UnityEngine;

public class DummyEnemy : Enemy {
	void Update()
    {}

	public override void TakeDamage(int damage) {
		health -= damage;

		transform.Find("Mouth").localScale = new Vector3(-100, 100, 100);

        if (health <= 0) Invoke(nameof(DestroyEnemy), 1.0f / damage);
	}
}