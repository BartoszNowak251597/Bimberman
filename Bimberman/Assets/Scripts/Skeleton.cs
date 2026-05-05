using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Skeleton : MonoBehaviour {
	public float radius;
    public float speed = 0.04f;
    private float originalSpeed=0.04f;
    private bool dead;
	private float lifetimeDead;
    //private float dt = 0;
    private bool isPetrified;
    private bool isBurning;
    private float petrifyRemainingTime;
    private float dotRemainingTime;
    private int takingDamage;
    public int hp = 100;
    private float damageInterval;
    private float lastDamageTime;
    private void OnPotionExplosion(PotionExplodeEvent e) {

        if (e.name == "Petrify" && Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
        {
            EventManager.Emit(new TargetHitEvent() { target = this });
            if (!isPetrified)
            {
                //Debug.Log("Skeleton hit by petrify potion! Ingredient count: " + e.ingredientCount);
                originalSpeed = speed;
                originalCooldown = cooldown;
                if (e.ingredientCount == 1)
                {
                    //spowalnia przeciwnika i zwiêksza cooldown
                    speed *= 0.5f;
                    cooldown *= modifier;
                }
                else
                {
                    //zamra¿a przeciwnika ca³kowicie
                    speed = 0;
                }
                isPetrified = true;
                petrifyRemainingTime = e.effectTime;
                
            }
        }
        if (e.name == "Fire" && Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
        {
            EventManager.Emit(new TargetHitEvent() { target = this });
            //TODO
            isBurning = true;
            dotRemainingTime = e.effectTime;
            takingDamage = e.damage;
            damageInterval = e.timeInterval;
            lastDamageTime = 0;
        }
        else if(e.name == "Explosion")
        {
            //TODO
            if (e.special1)
            {

            }
            if (e.special2)
            {

            }

            takingDamage = e.damage;

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
                    cooldown = originalCooldown;
                }
                //return;
            }

            if (isBurning)
            {
                //Debug.Log("Skeleton is burning! Remaining time: " + dotRemainingTime);
                dotRemainingTime -= Time.deltaTime;
                lastDamageTime += Time.deltaTime;
                if (dotRemainingTime <= 0)
                {
                    isBurning = false;
                    lastDamageTime = 0;
                }
                else
                {
                    if (Time.time >= lastDamageTime)
                    {
                        hp -= takingDamage;
                        lastDamageTime = Time.time + damageInterval;
                        if (hp <= 0) Die();
                    }
                }
            }
            
                Vector3 targetPos = FindFirstObjectByType<PlayerController>().transform.position;

			this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, speed);

			Vector3 relPos = targetPos - this.transform.position;
			relPos.y = 0;

			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(relPos, Vector3.up), 0.05f);

            AttackPlayer();
        }
		else {
			this.lifetimeDead += Time.deltaTime;

			if (this.lifetimeDead > 10) {
				Destroy(this.gameObject);
			}
		}
	}

    private float attackRange = 5f;
    private bool alreadyAttacked = false;
    public GameObject enemyShoot;
    public float cooldown = 5f;
    private float originalCooldown = 3f;
    public float modifier = 2f;
    private void AttackPlayer()
    {
        Vector3 targetPos = FindFirstObjectByType<PlayerController>().transform.position;
        float distance = Vector3.Distance(transform.position, targetPos);
        transform.LookAt(targetPos);
        if (distance < attackRange * 0.8f)
        {
            Vector3 directionAway = (transform.position - targetPos).normalized;
            Vector3 newPos = targetPos + directionAway * attackRange;
           
        }
        else
        {

            if (!alreadyAttacked)
            {
                Vector3 direction = (targetPos - transform.position).normalized;
                GameObject projectile = Instantiate(enemyShoot, transform.position + direction * 1f, Quaternion.LookRotation(direction));
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.linearVelocity = direction * 50f;

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), cooldown);
            }
        }
    }
    private void Die()
    {
       // Debug.Log("Skeleton died!");
        this.dead = true;
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void OnEnable()
	{
		EventManager.Subscribe(OnPotionExplosion);
	}

	public void OnDisable() {
		EventManager.Unsubscribe(OnPotionExplosion);
	}
}