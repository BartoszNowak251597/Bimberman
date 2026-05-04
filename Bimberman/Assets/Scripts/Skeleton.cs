using UnityEngine;

public class Skeleton : MonoBehaviour {
	public float radius;
    public float speed = 0.04f;
    private float originalSpeed=0.04f;
    private bool dead;
	private float lifetimeDead;
    private float dt = 0;
    private bool isPetrified;
    private float petrifyRemainingTime;
    private void OnPotionExplosion(PotionExplodeEvent e) {

        if (e.name == "Petrify" && Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
        {
            EventManager.Emit(new TargetHitEvent() { target = this });
            if (!isPetrified)
            {
                originalSpeed = speed; 
                speed = 0;
                isPetrified = true;
                petrifyRemainingTime = e.effectTime;
            }
        }
        else if(e.name == "Explosion")
        {
            if (Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
            {
                EventManager.Emit(new TargetHitEvent() { target = this });

                this.dead = true;

                foreach (var box in GetComponentsInChildren<BoxCollider>(true))
                {
                    box.enabled = true;
                }

                foreach (var body in GetComponentsInChildren<Rigidbody>(true))
                {
                    body.isKinematic = false;

                    body.AddExplosionForce(10, e.position, e.radius * 2, 1, ForceMode.Impulse);
                }
            }
        }
        
	}

	public void Update()
	{
		if (!dead) {
            if (isPetrified)
            {
                petrifyRemainingTime -= Time.deltaTime;
                if (petrifyRemainingTime <= 0)
                {
                    isPetrified = false;
                    speed = originalSpeed; 
                }
                return;
            }
            Vector3 targetPos = FindFirstObjectByType<PlayerController>().transform.position;

			this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed);

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