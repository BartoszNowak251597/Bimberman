using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float wobbliness;
	public float speed = 100;
	public float cursorMovement = 2;

	public Rigidbody rb;
	public Vector3 targetOffset;
	public GameObject targetCrosshair;
	public GameObject visualBody;

	public void Awake() {
		
	}

	public void FixedUpdate() {
		if (Camera.main) {
			Vector3 left = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

			Vector3 forward = Vector3.Cross(Vector3.up, left).normalized;

			Vector3 movement = Vector3.zero;

			if (Input.GetKey("w")) {
				movement += forward * speed;
			}
			if (Input.GetKey("s")) {
				movement -= forward * speed;
			}
			if (Input.GetKey("a")) {
				movement += left * speed;
			}
			if (Input.GetKey("d")) {
				movement -= left * speed;
			}

			rb.AddForce(movement, ForceMode.Force);

			Camera.main.GetComponent<CameraController>().target = this.transform.position;

			visualBody.GetComponent<ConfigurableJoint>().axis = Vector3.Cross(
				targetOffset,
				Vector3.up
			).normalized;

			visualBody.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Euler(
				0,
				-Quaternion.LookRotation(targetOffset, Vector3.right).eulerAngles.y,
				0
			);
		}
	}

	public void Update() {
		if (Camera.main) {
			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			new Plane(Vector3.up, Vector3.zero).Raycast(cameraRay, out float dist);

			Vector3 targetPos = cameraRay.GetPoint(dist);

			targetOffset = targetPos - this.transform.position;

			targetCrosshair.transform.position = this.transform.position + targetOffset;
		}
	}
}