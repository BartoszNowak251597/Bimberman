using UnityEngine;

public class CameraController : MonoBehaviour {
	public Vector3 target;

	public float height = 5f;
	public float angleY = 0f;
	public float angleX = 45f;

	public void LateUpdate() {
		Vector3 dir = Quaternion.AngleAxis(angleY, Vector3.up) * (Quaternion.AngleAxis(-angleX, Vector3.right) * Vector3.forward);

		Debug.Log(new Plane(Vector3.up, target + Vector3.up * height).Raycast(new Ray(target, dir), out float rayDist));

		Vector3 pos = target + dir * rayDist;

		this.transform.position = pos;

		this.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
	}
}