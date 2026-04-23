using UnityEngine;

public class ThrowTarget : MonoBehaviour {
	public float radius;

	private void OnPotionExplosion(PotionExplodeEvent e) {
		if (Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius) {
			EventManager.Emit(new TargetHitEvent() { target = this });
		}
	}

	public void Update()
	{
		Vector3 targetPos = FindFirstObjectByType<PlayerController>().transform.position;

		this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, Time.deltaTime);
	}

	public void OnEnable()
	{
		EventManager.Subscribe(OnPotionExplosion);
	}

	public void OnDisable() {
		EventManager.Unsubscribe(OnPotionExplosion);
	}
}