using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float wobbliness;
	public float speed = 100;
	public float cursorMovement = 2;

	public Rigidbody rb;
	public Vector3 targetOffset;
	public GameObject targetCrosshair;
	public GameObject visualBody;
	public AimingAid aim;

	public float throwSpeedTime = 0.6f;
	public float minThrowDistance = 1;
	public float maxThrowDistance = 5;
	public float flightTime = 1;
	float throwStrengthAccum;
	float throwStrengthCache;

	public Transform throwingArm;
	public Transform torso;
	public Transform throwPoint;

	public GameObject throwablePrefab;

	private Quaternion baseArmRotation;

	public void Awake()
	{
		this.aim.SetStretch(-1);

		this.baseArmRotation = this.throwingArm.localRotation;

		this.throwStrengthAccum = 0;
	}

	public void FixedUpdate() {
		return;

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

	private float ThrowStrengthEasing(float strength) {
		return strength * strength;
	}

	public void Update() {
		if (Camera.main) {
			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			new Plane(Vector3.up, Vector3.zero).Raycast(cameraRay, out float dist);

			Vector3 targetPos = cameraRay.GetPoint(dist);

			targetOffset = targetPos - this.transform.position;

			Camera.main.GetComponent<CameraController>().target = this.transform.position + targetOffset.normalized;

			aim.PointAt(targetPos);

			if (Input.GetMouseButton(0) && this.throwStrengthCache == 0)
			{
				this.throwStrengthAccum += Time.deltaTime / this.throwSpeedTime;

				if (this.throwStrengthAccum > 1)
				{
					this.throwStrengthAccum = 1;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(160 + 60 * ThrowStrengthEasing(this.throwStrengthAccum)), this.transform.right) * baseArmRotation;

				this.aim.SetStretch(ThrowStrengthEasing(this.throwStrengthAccum));

				this.aim.crosshair.SetActive(true);
				this.aim.SetCrosshairPosition(this.transform.position + targetOffset.normalized * Mathf.Lerp(minThrowDistance, maxThrowDistance, ThrowStrengthEasing(this.throwStrengthAccum)));

				this.torso.localRotation = Quaternion.AngleAxis(ThrowStrengthEasing(this.throwStrengthAccum) * -10, Vector3.right);
			}
			else if (this.throwStrengthAccum > 0)
			{
				if (this.throwStrengthCache == 0) {
					this.throwStrengthCache = this.throwStrengthAccum;
				}

				this.throwStrengthAccum = Mathf.MoveTowards(this.throwStrengthAccum, 0, Time.deltaTime * 10);

				if (this.throwStrengthCache > 0 && this.throwStrengthAccum < 0.7f) {
					GameObject thrownBottle = GameObject.Instantiate(this.throwablePrefab, this.throwPoint.position, Random.rotation, null);

					float speedX = Mathf.Lerp(minThrowDistance, maxThrowDistance, ThrowStrengthEasing(this.throwStrengthCache)) / this.flightTime;

					float speedY = ((Vector3.Dot(Physics.gravity, Vector3.down) / 2) * this.flightTime * this.flightTime - Vector3.Dot(this.throwPoint.position, Vector3.up)) / this.flightTime;

					Vector3 throwDirection = targetOffset.normalized * speedX + Vector3.up * speedY;

					Debug.Log(speedX + " " + speedY);

					thrownBottle.SetActive(true);

					// thrownBottle.GetComponent<Rigidbody>().AddForce(throwDirection, ForceMode.VelocityChange);
					thrownBottle.GetComponent<Rigidbody>().linearVelocity = throwDirection;

					this.throwStrengthCache = -1;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(220 * ThrowStrengthEasing(this.throwStrengthAccum)), this.transform.right) * baseArmRotation;

				this.aim.SetStretch(-1);

				this.aim.crosshair.SetActive(false);

				this.torso.localRotation = Quaternion.AngleAxis(10 + ThrowStrengthEasing(this.throwStrengthAccum) * -20, Vector3.right);
			}
			else {
				this.throwStrengthCache = 0;

				this.throwingArm.localRotation = baseArmRotation;

				this.torso.localRotation = Quaternion.identity;
			}
		}
	}
}