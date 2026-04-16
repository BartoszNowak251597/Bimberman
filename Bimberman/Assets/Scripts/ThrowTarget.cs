using UnityEngine;

public class ThrowTarget : MonoBehaviour {
	public float radius;

	private void OnPotionExplosion(PotionExplodeEvent e) {
		if (Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius) {
			EventManager.Emit(new TargetHitEvent() { target = this });
		}
	}

	public void OnEnable() {
		EventManager.Subscribe(OnPotionExplosion);
	}

	public void OnDisable() {
		EventManager.Unsubscribe(OnPotionExplosion);
	}
}