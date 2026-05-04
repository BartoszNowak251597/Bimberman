using UnityEngine;

public class PlayerController : MonoBehaviour {
	public float wobbliness;
	public float throwWobbliness;
	public float speed = 100;
	public float velocityThrowBoost = 1;
	public float sidewaysWobbleFrequency;
	public float throwWobblinessFrequency;

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

	public float legMovement;
	public float legMovementSpeed = 1;
	public float woblinessBodyDeflection = 15f;
	public Transform leftLeg;
	public Transform rightLeg;
	public Vector3 gamepadAim;

	private float wobblinessAccum = 0;
	public bool gamepadControl = true;

	public void Awake()
	{
		this.aim.SetStretch(-1);

		this.baseArmRotation = this.throwingArm.localRotation;

		this.throwStrengthAccum = 0;
	}
	
	public float GetWoblinnessStrength()
	{
		return (
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency)
			+
			Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 2.3f)
			// +
			// Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 7.35f)
			// +
			// Mathf.Sin(wobblinessAccum * sidewaysWobbleFrequency * 17)
		) / 2;
	}

	private void ApplyWobblyness() {
		float wobblinessRamp = Mathf.Pow(
			Mathf.Clamp01(this.desiredMovement.magnitude / this.speed),
			3
		);

		wobblinessRamp /= 3;

		wobblinessRamp += 0.4f;

		wobblinessAccum += wobblinessRamp * Time.fixedDeltaTime * Random.Range(0.7f, 1.3f);

		this.transform.Find("visual_pivot").localRotation = Quaternion.AngleAxis(-GetWoblinnessStrength() * woblinessBodyDeflection * wobblinessRamp, Vector3.forward);
	}

	private float LinearDampingFunc() {
		float top = Mathf.Pow(Mathf.Sin(Mathf.Cos(Time.time) * 1.42f), 100);
		float bottom = Mathf.Pow(Mathf.Cos(Time.time * 0.27f), 2) + 0.1f;

		return (top / bottom);
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

			float minVelocityLerpFactor = this.desiredMovement.sqrMagnitude < movement.sqrMagnitude ? 0.03f : 0.1f;
			float maxVelocityLerpFactor = this.desiredMovement.sqrMagnitude < movement.sqrMagnitude ? 0.2f : 0.25f;

			this.desiredMovement = Vector3.Lerp(this.desiredMovement, movement, Mathf.Lerp(minVelocityLerpFactor, maxVelocityLerpFactor, velocityLerpFactor));

			movement += left * GetWoblinnessStrength() * this.wobbliness * (movement.magnitude / speed);

			rb.AddForce(movement * speed, ForceMode.Force);

			ApplyWobblyness();

			rb.linearDamping = 3 + Mathf.Clamp01(LinearDampingFunc()) * 4;
		}
	}

	public Vector3 GetThrowWobble()
	{
		return new Vector3(Mathf.Sin(Time.time * throwWobblinessFrequency), 0, Mathf.Cos(Time.time * (throwWobblinessFrequency + 0.71f))) * (0.1f + this.throwStrengthAccum) * throwWobbliness;
	}

	private Vector3 GetStrengthFromVelocity()
	{
		return this.rb.linearVelocity * velocityThrowBoost;
	}

	private float ThrowStrengthEasing(float strength) {
		return -(Mathf.Cos(Mathf.PI * strength) - 1) / 2;
	}

	public void Update() {
        // Prevent crash if prefab is missing or destroyed
        if (this.throwablePrefab == null)
        {
            Debug.LogError("Throwable prefab is missing or destroyed! Cannot throw bottle.");
            this.throwStrengthCache = -1;  // reset throw state
            return;
        }

        // Optional: also check throwPoint (if it's destroyed, instantiation position fails)
        if (this.throwPoint == null)
        {
            Debug.LogError("Throw point transform is missing!");
            this.throwStrengthCache = -1;
            return;
        }
        if (Camera.main) {
			Vector3 left = Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized;

			Vector3 forward = Vector3.Cross(Vector3.up, left).normalized;

			Vector3 targetPos;

			if (!gamepadControl)
			{
				Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				new Plane(Vector3.up, Vector3.zero).Raycast(cameraRay, out float dist);

				targetPos = cameraRay.GetPoint(dist);
			}
			else
			{
				Vector3 gamepadLook = -(left * Input.GetAxis("Look Horizontal") + forward * Input.GetAxis("Look Vertical"));

				gamepadAim = Vector3.MoveTowards(gamepadAim, gamepadLook, Time.deltaTime * 3);

				if (Vector3.Magnitude(gamepadAim) > Mathf.Epsilon)
				{
					targetPos = gamepadAim + this.transform.position;
				}
				else
				{
					targetPos = targetOffset + this.transform.position;
				}
			}

			targetOffset = targetPos - this.transform.position;

			Camera.main.GetComponent<CameraController>().target = this.transform.position + targetOffset.normalized;

			aim.PointAt(targetPos);

			if (targetOffset.sqrMagnitude > Mathf.Epsilon) {
				this.transform.rotation = Quaternion.LookRotation(targetOffset.normalized, Vector3.up);
			}

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

				hitPoint += GetThrowWobble();

				this.aim.crosshair.SetActive(true);
				this.aim.SetCrosshairPosition(this.transform.position + hitPoint + Vector3.up * 0.001f);

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

					hitPoint += GetThrowWobble();

					float speedX = hitPoint.magnitude / this.flightTime;

					float speedY = ((Vector3.Dot(Physics.gravity, Vector3.down) / 2) * this.flightTime * this.flightTime - Vector3.Dot(this.throwPoint.position, Vector3.up)) / this.flightTime;

					Vector3 throwDirection = this.transform.position + hitPoint - this.throwPoint.position;
					throwDirection.y = 0;

					Vector3 throwForce = throwDirection.normalized * speedX + Vector3.up * speedY;

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

			this.torso.rotation *= Quaternion.AngleAxis(Vector3.Dot(GetThrowWobble(), this.transform.right) * 20, this.transform.up);

			legMovement += Vector3.Magnitude(this.rb.linearVelocity) * Time.deltaTime * legMovementSpeed;

			if (Vector3.Magnitude(this.rb.linearVelocity) < Mathf.Epsilon)
			{
				if (legMovement < Mathf.PI)
				{
					legMovement = Mathf.MoveTowards(legMovement, 0, Time.deltaTime * Mathf.PI);
				}
				else
				{
					legMovement = Mathf.MoveTowards(legMovement, Mathf.PI * 2, Time.deltaTime * Mathf.PI);
				}
			}

			if (legMovement > Mathf.PI * 2)
			{
				legMovement = legMovement - Mathf.PI * 2;
			}

			leftLeg.localEulerAngles = new Vector3(Mathf.Sin(legMovement) * 50, 0, 0);
			rightLeg.localEulerAngles = new Vector3(-Mathf.Sin(legMovement) * 50, 0, 0);
		}
	}
}