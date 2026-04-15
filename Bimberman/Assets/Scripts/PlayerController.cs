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

	public float throwStrengthAccumSpeed = 7;
	public float minThrowStrength = 1;
	public float maxThrowStrength = 5;
	public float throwStrengthMultiplier = 10;
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

		this.throwStrengthAccum = this.minThrowStrength;
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

	public void Update() {
		if (Camera.main) {
			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			new Plane(Vector3.up, Vector3.zero).Raycast(cameraRay, out float dist);

			Vector3 targetPos = cameraRay.GetPoint(dist);

			targetOffset = targetPos - this.transform.position;

			Camera.main.GetComponent<CameraController>().target = this.transform.position + targetOffset.normalized;

			aim.PointAt(targetPos);

			float throwInvLerpAmount = Mathf.InverseLerp(this.minThrowStrength, this.maxThrowStrength, this.throwStrengthAccum);

			if (Input.GetMouseButton(0) && this.throwStrengthCache == 0)
			{
				this.throwStrengthAccum += this.throwStrengthAccumSpeed * Time.deltaTime;

				if (this.throwStrengthAccum > this.maxThrowStrength)
				{
					this.throwStrengthAccum = this.maxThrowStrength;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(160 + 60 * throwInvLerpAmount), this.transform.right) * baseArmRotation;

				this.aim.SetStretch(throwInvLerpAmount);

				this.torso.localRotation = Quaternion.AngleAxis(throwInvLerpAmount * -10, Vector3.right);
			}
			else if (this.throwStrengthAccum > this.minThrowStrength)
			{
				if (this.throwStrengthCache == 0) {
					this.throwStrengthCache = this.throwStrengthAccum;
				}

				this.throwStrengthAccum = Mathf.MoveTowards(this.throwStrengthAccum, this.minThrowStrength, Time.deltaTime * 50);

				if (this.throwStrengthCache > 0 && throwInvLerpAmount < 0.6f) {
					GameObject thrownBottle = GameObject.Instantiate(this.throwablePrefab, this.throwPoint.position, Random.rotation, null);

					Vector3 throwDirection = targetOffset.normalized * this.throwStrengthCache;

					throwDirection.y = 1;

					thrownBottle.SetActive(true);

					thrownBottle.GetComponent<Rigidbody>().AddForce(throwDirection * this.throwStrengthMultiplier, ForceMode.Impulse);

					this.throwStrengthCache = -1;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(220 * throwInvLerpAmount), this.transform.right) * baseArmRotation;

				this.aim.SetStretch(-1);

				this.torso.localRotation = Quaternion.AngleAxis(10 + throwInvLerpAmount * -20, Vector3.right);
			}
			else {
				this.throwStrengthCache = 0;

				this.throwingArm.localRotation = baseArmRotation;

				this.torso.localRotation = Quaternion.identity;
			}
		}
	}
}