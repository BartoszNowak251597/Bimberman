using UnityEngine;

public class CameraController : MonoBehaviour {
	public Vector3 target;

	public float height = 5f;
	public float angleY = 0f;
	public float angleX = 45f;
	[Range(0, 1)]
	public float lerpAmount = 0.0f;

	public Vector3 outDebug;

	public void LateUpdate() {
		PlayerController player = FindFirstObjectByType<PlayerController>();

		Vector3 playerPos = player.transform.position;

		Vector3 dir = Quaternion.AngleAxis(angleY, Vector3.up) * (Quaternion.AngleAxis(-angleX, Vector3.right) * Vector3.forward);

		new Plane(Vector3.up, target + Vector3.up * height).Raycast(new Ray(target, dir), out float rayDist);

		Vector3 playerRelativePos = playerPos + dir * rayDist;
		Vector3 targetRelativePos = target + dir * rayDist;

		Vector3 pos = Vector3.Lerp(playerRelativePos, targetRelativePos, lerpAmount);
		Vector3 bep = Vector3.Lerp(playerPos, target, lerpAmount);

		outDebug = (target - pos).normalized;

		this.transform.position = pos;

		float wobble = Mathf.Sin(Time.time) + player.GetWoblinnessStrength() * 0.4f;
		wobble = 0;

		this.transform.rotation = Quaternion.LookRotation((bep - pos).normalized, Quaternion.AngleAxis(wobble * player.wobbliness, Vector3.forward) * Vector3.up);
	}
}