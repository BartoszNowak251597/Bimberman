using UnityEngine;

public class Skeleton : MonoBehaviour {
	public float radius;

	private bool dead;
	private float lifetimeDead;

	private void OnPotionExplosion(PotionExplodeEvent e) {
		if (Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius) {
			EventManager.Emit(new TargetHitEvent() { target = this });

			this.dead = true;

			foreach (var box in GetComponentsInChildren<BoxCollider>(true)) {
				box.enabled = true;
			}

			foreach (var body in GetComponentsInChildren<Rigidbody>(true)) {
				body.isKinematic = false;

				body.AddExplosionForce(10, e.position, e.radius * 2, 1, ForceMode.Impulse);
			}
		}
	}

	public void Update()
	{
		if (!dead) {
			Vector3 targetPos = FindFirstObjectByType<PlayerController>().transform.position;

			this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, Time.deltaTime);

			Vector3 relPos = targetPos - this.transform.position;
			relPos.y = 0;

			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(relPos, Vector3.up), 0.05f);
		}
		else {
			this.lifetimeDead += Time.deltaTime;

			if (this.lifetimeDead > 10) {
				Destroy(this.gameObject);
			}
		}
	}

	public void OnEnable()
	{
		EventManager.Subscribe(OnPotionExplosion);
	}

	public void OnDisable() {
		EventManager.Unsubscribe(OnPotionExplosion);
	}
}