using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float wobbliness;
	public float speed = 100;
	public float velocityThrowBoost = 1;
	public float sidewaysWobbleFrequency;

	public Rigidbody rb;

	public Vector3 targetOffset;
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
	private Vector3 desiredMovement;

	public TMPro.TMP_Text debugText;

	private float wobblinessAccum = 0;

	public void Awake()
	{
		this.aim.SetStretch(-1);

		this.baseArmRotation = this.throwingArm.localRotation;

		this.throwStrengthAccum = 0;
	}
	
	private float GetWoblinnessStrength()
	{
		return (
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency)
			+
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 2.3f)
			+
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 7.35f)
			+
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 17)
		) / 4;
	}

	private Vector3 ApplyWobblyness() {
		if (this.desiredMovement.sqrMagnitude < 0.1f) {
			return Vector3.zero;
		}

		Vector3 left = Vector3.Cross(this.desiredMovement, Vector3.up).normalized;

		Vector3 forward = Vector3.Cross(Vector3.up, left).normalized;

		Vector3 deflection = left * GetWoblinnessStrength() + forward * Mathf.Sin(Time.time * 0);

		deflection *= wobbliness;
		
		float wobblinessRamp = Mathf.Pow(
			Mathf.Clamp01(this.desiredMovement.magnitude / this.speed),
			3
		);

		wobblinessAccum += wobblinessRamp * Time.fixedDeltaTime * Random.Range(0.7f, 3.3f);

		this.transform.Find("visual_pivot").localRotation = Quaternion.AngleAxis(-GetWoblinnessStrength() * 10 * wobblinessRamp, Vector3.forward);

		deflection *= wobblinessRamp;

		return this.desiredMovement + deflection;
	}

	public void FixedUpdate() {
		if (Camera.main) {
			Vector3 left = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

			Vector3 forward = Vector3.Cross(Vector3.up, left).normalized;

			Vector3 movement = Vector3.zero;
			Vector3 keyboardMovement = Vector3.zero;
			Vector3 gamepadMovement = Vector3.zero;

			if (Input.GetKey("w"))
			{
				keyboardMovement += forward;
			}
			if (Input.GetKey("s")) {
				keyboardMovement -= forward;
			}
			if (Input.GetKey("a")) {
				keyboardMovement += left;
			}
			if (Input.GetKey("d"))
			{
				keyboardMovement -= left;
			}

			if (Vector3.Magnitude(keyboardMovement) > Mathf.Epsilon)
			{
				keyboardMovement = Vector3.Normalize(keyboardMovement);
			}

			gamepadMovement = -left * Input.GetAxis("Horizontal") + forward * Input.GetAxis("Vertical");

			movement = (Vector3.Magnitude(gamepadMovement) > Vector3.Magnitude(keyboardMovement)) ? gamepadMovement : keyboardMovement;

			movement *= this.speed;

			float velocityLerpFactor = Vector3.Dot(this.desiredMovement, movement) / (this.speed * this.speed);
			velocityLerpFactor *= velocityLerpFactor;

			// debugText.text = velocityLerpFactor.ToString();

			float minVelocityLerpFactor = this.desiredMovement.sqrMagnitude < movement.sqrMagnitude ? 0.03f : 0.3f;
			float maxVelocityLerpFactor = this.desiredMovement.sqrMagnitude < movement.sqrMagnitude ? 0.2f : 0.25f;

			this.desiredMovement = Vector3.Lerp(this.desiredMovement, movement, Mathf.Lerp(minVelocityLerpFactor, maxVelocityLerpFactor, velocityLerpFactor));

			// rb.AddForce(movement * speed, ForceMode.Force);
			rb.linearVelocity = ApplyWobblyness();
		}
	}

	private Vector3 GetStrengthFromVelocity() {
		return this.rb.linearVelocity * velocityThrowBoost;
	}

	private float ThrowStrengthEasing(float strength) {
		return -(Mathf.Cos(Mathf.PI * strength) - 1) / 2;
	}

	public void Update() {
		if (Camera.main) {
			Vector3 left = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

			Vector3 forward = Vector3.Cross(Vector3.up, left).normalized;

			Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			new Plane(Vector3.up, Vector3.zero).Raycast(cameraRay, out float dist);

			Vector3 targetPos = cameraRay.GetPoint(dist);

			Vector3 gamepadLook = -(left * Input.GetAxis("Look Horizontal") + forward * Input.GetAxis("Look Vertical"));

			if (Vector3.Magnitude(gamepadLook) > Mathf.Epsilon)
			{
				targetPos = gamepadLook + this.transform.position;
			}

			targetOffset = targetPos - this.transform.position;

			Camera.main.GetComponent<CameraController>().target = this.transform.position + targetOffset.normalized;

			aim.PointAt(targetPos);

			this.transform.rotation = Quaternion.LookRotation(targetOffset.normalized, Vector3.up);

			float throwOomph = 0;

			if (Input.GetMouseButton(0))
			{
				throwOomph = 1;
			}
			else if (Input.GetAxis("Throw Thing Gamepad") > 0)
			{
				throwOomph = Input.GetAxis("Throw Thing Gamepad");
			}

			if (throwOomph > 0 && this.throwStrengthCache == 0)
			{
				this.throwStrengthAccum += Time.deltaTime * throwOomph / this.throwSpeedTime;

				if (this.throwStrengthAccum > 1)
				{
					this.throwStrengthAccum = 1;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(160 + 60 * ThrowStrengthEasing(this.throwStrengthAccum)), Vector3.right) * baseArmRotation;

				this.aim.SetStretch(ThrowStrengthEasing(this.throwStrengthAccum));

				Vector3 hitPoint = targetOffset.normalized * Mathf.LerpUnclamped(minThrowDistance, maxThrowDistance, ThrowStrengthEasing(this.throwStrengthAccum)) + GetStrengthFromVelocity();

				this.aim.crosshair.SetActive(true);
				this.aim.SetCrosshairPosition(this.transform.position + hitPoint);

				this.torso.localRotation = Quaternion.AngleAxis(ThrowStrengthEasing(this.throwStrengthAccum) * -10, Vector3.right);
			}
			else if (this.throwStrengthAccum > 0)
			{
				if (this.throwStrengthCache == 0)
				{
					this.throwStrengthCache = this.throwStrengthAccum;
				}

				this.throwStrengthAccum = Mathf.MoveTowards(this.throwStrengthAccum, 0, Time.deltaTime * 10);

				if (this.throwStrengthCache > 0 && this.throwStrengthAccum < 0.7f)
				{
					GameObject thrownBottle = GameObject.Instantiate(this.throwablePrefab, this.throwPoint.position, Random.rotation, null);

					Vector3 hitPoint = targetOffset.normalized * Mathf.LerpUnclamped(minThrowDistance, maxThrowDistance, ThrowStrengthEasing(this.throwStrengthCache)) + GetStrengthFromVelocity();

					float speedX = hitPoint.magnitude / this.flightTime;

					float speedY = ((Vector3.Dot(Physics.gravity, Vector3.down) / 2) * this.flightTime * this.flightTime - Vector3.Dot(this.throwPoint.position, Vector3.up)) / this.flightTime;

					Vector3 throwDirection = this.transform.position + hitPoint - this.throwPoint.position;
					throwDirection.y = 0;

					Vector3 throwForce = throwDirection.normalized * speedX + Vector3.up * speedY;

					Debug.Log(hitPoint.magnitude);

					thrownBottle.SetActive(true);

					// thrownBottle.GetComponent<Rigidbody>().AddForce(throwDirection, ForceMode.VelocityChange);
					thrownBottle.GetComponent<Rigidbody>().linearVelocity = throwForce;

					this.throwStrengthCache = -1;
				}

				this.throwingArm.localRotation = Quaternion.AngleAxis(-(220 * ThrowStrengthEasing(this.throwStrengthAccum)), this.transform.right) * baseArmRotation;

				this.aim.SetStretch(-1);

				this.aim.crosshair.SetActive(false);

				this.torso.localRotation = Quaternion.AngleAxis(10 + ThrowStrengthEasing(this.throwStrengthAccum) * -20, Vector3.right);
			}
			else
			{
				this.throwStrengthCache = 0;

				this.throwingArm.localRotation = baseArmRotation;

				this.torso.localRotation = Quaternion.identity;
			}
		}
	}
}