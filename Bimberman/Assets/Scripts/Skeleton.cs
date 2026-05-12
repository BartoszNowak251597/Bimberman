using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Skeleton : MonoBehaviour
{
    public float radius;
    public float speed = 0.04f;
    private float originalSpeed = 0.04f;
    private bool dead;
    private float lifetimeDead;
    private bool isPetrified;
    private bool isBurning;
    private float petrifyRemainingTime;
    private float dotRemainingTime;
    private float takingDamage;
    public float hp = 100;
    private float damageInterval;
    private float lastDamageTime;
    private float attackRange = 5f;
    private bool alreadyAttacked = false;
    public GameObject enemyShoot;
    public float cooldown = 5f;
    private float originalCooldown = 3f;
    public float modifier = 2f;
    Vector3 targetPos;
    public bool IsConfused;
    private float confuseRemainingTime;
    private bool attackPrecisely = false;
    private Vector3 confusedMoveTarget; 
    private float confusionMoveCooldown;

    public bool IsAlive()
    {
        return !IsDead();
    }

    public bool IsDead()
    {
        return dead;
    }

    void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        originalSpeed = speed;

        originalCooldown = cooldown;
        this.speed = newSpeed;
    }

    public void SetEffectTime(EffectType effectName, float time)
    {
        if (effectName == EffectType.PETRIFY)
        {
            petrifyRemainingTime = time;
        }
        else if (effectName == EffectType.FIRE)
        {
            dotRemainingTime = time;
        }
    }
    public float SetEffect(EffectType effectName, float damage)
    {
        if (effectName == EffectType.PETRIFY)
        {
            if (!isPetrified)
            {
                // originalSpeed = speed;
                //originalCooldown = cooldown;
                //speed *= 0.5f + 0.5f * (1 - strength);
                // cooldown *= modifier;
                isPetrified = true;
                // petrifyRemainingTime = strength * modifier;
            }
            return petrifyRemainingTime;
        }
        else if (effectName == EffectType.FIRE)
        {
            isBurning = true;
            //dotRemainingTime = strength * modifier;
            takingDamage = damage; // Przykładowa formuła na obrażenia
            damageInterval = 1f; // Przykładowy interwał obrażeń
            lastDamageTime = 0;
            return dotRemainingTime;
        }
        return 0;
    }

    private void OnPotionExplosion(PotionExplodeEvent e)
    {

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
                    //spowalnia przeciwnika i zwi�ksza cooldown
                    speed *= 0.5f;
                    cooldown *= modifier;
                }
                else
                {
                    //zamra�a przeciwnika ca�kowicie
                    speed = 0;
                }
                isPetrified = true;
                petrifyRemainingTime = e.effectTime;

            }
        }
        if (e.name == "Fire" && Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
        {
            EventManager.Emit(new TargetHitEvent() { target = this });
            isBurning = true;
            dotRemainingTime = e.effectTime;
            takingDamage = e.damage;
            damageInterval = e.timeInterval;
            lastDamageTime = 0;
        }
        //if (e.name == "Tornado" && Vector3.Distance(e.position, this.transform.position) < e.radius + this.radius)
        //{
        //    EventManager.Emit(new TargetHitEvent() { target = this });
        //    if (e.ingredientCount == 1)
        //    {
        //        //1 sk�adnik: Tornado kr�ci si� i despawnuje pociski wrog�w lec�ce przez nie. 

        //    }
        //    else
        //    {
        //        //2 sk�adniki: Tornado kr�ci si� i przechwytuje pociski wrog�w lec�ce przez nie. Pociski kr�c� si� po obwodzie tornada i mog� trafi� innych przeciwnik�w

        //    }
        //}
        else if (e.name == "Explosion")
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

                Die();


            }
        }

        else if (e.name == "Confuse")
        {
            if (Vector3.Distance(e.position, transform.position) < e.radius + radius)
            {
                IsConfused = true;
                confuseRemainingTime = e.effectTime;
                IsConfused = true;
                if (e.ingredientCount == 1)
                {
                    attackPrecisely = false;      
                    confusedMoveTarget = GetRandomPointAround(); // losowy punkt
                }
                else
                {
                    attackPrecisely = true;       
                                                 
                }
                EventManager.Emit(new TargetHitEvent() { target = this });
            }
        }
    }

    public void Update()
    {
        if (!dead)
        {
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
                Debug.Log("Skeleton is burning! Remaining time: " + dotRemainingTime);
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
            
            if (IsConfused)
            {
                confuseRemainingTime -= Time.deltaTime;
                if (confuseRemainingTime <= 0)
                {
                    IsConfused = false;
                    attackPrecisely = false; 
                }
            }

            Vector3 moveTargetPos;
            if (!IsConfused)
            {
                moveTargetPos = FindFirstObjectByType<PlayerController>().transform.position;
            }
            else
            {
                if (!attackPrecisely)
                {
                    if (confusionMoveCooldown <= 0f)
                    {
                        confusedMoveTarget = GetRandomPointAround();
                        confusionMoveCooldown = 0.8f;
                    }
                    else
                    {
                        confusionMoveCooldown -= Time.deltaTime;
                    }
                    moveTargetPos = confusedMoveTarget;
                }
                else
                {
                    Skeleton nearest = FindNearestOtherSkeleton();
                    moveTargetPos = nearest != null ? nearest.transform.position : transform.position;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, moveTargetPos, speed * Time.deltaTime);

            Vector3 relPos = moveTargetPos - transform.position;
            if (relPos.magnitude > 0.01f)
            {
                relPos.y = 0;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relPos, Vector3.up), 0.05f);
            }


            AttackPlayer();
        }
        else
        {
            this.lifetimeDead += Time.deltaTime;

            if (this.lifetimeDead > 10)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void AttackPlayer()
    {
        Vector3 attackTarget;
        if (!IsConfused)
        {
            attackTarget = FindFirstObjectByType<PlayerController>().transform.position;
        }
        else if (!attackPrecisely)
        {
            attackTarget = GetRandomPointAround();
        }
        else
        {
            Skeleton other = FindNearestOtherSkeleton();
            attackTarget = other != null ? other.transform.position : transform.position;
        }

        float distance = Vector3.Distance(transform.position, attackTarget);
        transform.LookAt(attackTarget); 

        if (distance >= attackRange * 0.8f) 
        {
            if (!alreadyAttacked)
            {
                Vector3 direction = (attackTarget - transform.position).normalized;
                GameObject projectile = Instantiate(enemyShoot, transform.position + direction * 1f, Quaternion.LookRotation(direction));
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.linearVelocity = direction * 50f;

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), cooldown);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("Skeleton took " + damage + " damage! Remaining HP: " + hp);
        if (hp <= 0) Die();
    }
    private void Die()
    {
        Debug.Log("Skeleton died!");
        this.dead = true;

        foreach (var box in GetComponentsInChildren<BoxCollider>(true))
        {
            box.enabled = true;
        }

        foreach (var body in GetComponentsInChildren<Rigidbody>(true))
        {
            body.isKinematic = false;
        }
    }

    private Vector3 GetRandomPointAround(float radius = 10f)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        randomDir.y = 0; 
        return transform.position + randomDir;
    }

    private Skeleton FindNearestOtherSkeleton()
    {
        Skeleton[] all = FindObjectsByType<Skeleton>(FindObjectsSortMode.None);
        Skeleton nearest = null;
        float minDist = float.MaxValue;
        foreach (var sk in all)
        {
            if (sk == this || sk.IsDead()) continue;
            float d = Vector3.Distance(transform.position, sk.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = sk;
            }
        }
        return nearest;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void OnEnable()
    {
        EventManager.Subscribe(OnPotionExplosion);
    }

    public void OnDisable()
    {
        EventManager.Unsubscribe(OnPotionExplosion);
    }
}