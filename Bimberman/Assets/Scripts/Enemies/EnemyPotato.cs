using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPotato : EnemyBase
{
    [Header("Potato Settings")]
    [SerializeField] private float attackCooldown = 10f;
    [SerializeField] private int damage = 30;
    //[SerializeField] private GameObject shadowPrefab;
    [SerializeField] private float attackDuration = 5f;
    [SerializeField] private float shadowChaseDuration = 3f;
    [SerializeField] private float shadowStayDuration = 2f;

    private GameObject currentShadow;
    //private Vector3 shadowTargetPos;

    private bool isAttacking = false;
    //private bool madeShadow = false;

    protected override void Awake()
    {
        base.Awake();
        attackRange = 3f;  
    }

    protected override void Start()
    {
        base.Start();
        if (playerTransform == null)
            Debug.LogWarning("EnemyPotato: playerTransform not assigned");
        else
        {
            //Prêdkoœæ na ziemi 70% gracza
            m_Speed = playerTransform.GetComponent<PlayerController>().speed * 0.7f;
            agent.speed = m_Speed;
        }
        //currentAttackCooldown = 0f;
        agent.enabled = true;

    }

    protected override void Update()
    {
        if (!agent.enabled) return;

        if (playerTransform)
            m_TargetPosition = playerTransform.position;
        else
            return;

        currentPos = transform.position;

        UpdateAttackAnimation();
        if (m_InAttackAnimation)
        {
            StopMoving();
            return;
        }

        if (isPlayerInRoom)
        {
            float dist = Vector3.Distance(currentPos, m_TargetPosition);
            if (m_hp <= 30)
                currentState = States.FLEEING;
            else if (dist <= attackRange)
                currentState = States.ATTACKING;
            else if (m_UsingAStar)
                currentState = States.AVOIDING_OBSTACLE;
            else
                currentState = States.CHASING;
        }
        else
        {
            currentState = States.PATROLLING;
        }

        if (currentState != States.AVOIDING_OBSTACLE)
        {
            m_UsingAStar = false;
        }

        switch (currentState)
        {
            case States.PATROLLING:
                m_StuckTimer = 0f;
                Patrol();
                break;
            case States.CHASING:
                DirectChase();
                break;
            case States.ATTACKING:
                if (!isAttacking && attackCooldown <= 0f)
                {
                    StartCoroutine(AttackSequence());
                }
                else
                {
                    StopMoving();
                    attackCooldown -= Time.deltaTime;
                }
                break;
            case States.FLEEING:
                Flee();
               // Attack();
                break;
            case States.AVOIDING_OBSTACLE:
                AstarChase();
                break;
        }
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        agent.enabled = false; 

        Vector3 startPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        Vector3 dirToPlayer = (playerPos - startPos).normalized;
        float horizontalDist = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(playerPos.x, 0, playerPos.z));
        Vector3 apex = startPos + dirToPlayer * (horizontalDist * 0.5f) + Vector3.up * 5f;

        if (m_ProjectilePrefab != null)
        {
            currentShadow = Instantiate(m_ProjectilePrefab, startPos, Quaternion.identity);
            currentShadow.transform.position = new Vector3(startPos.x, 0.01f, startPos.z);
        }

        float jumpUpDuration = 0.5f;
        float timer = 0f;
        while (timer < jumpUpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / jumpUpDuration;
            transform.position = Vector3.Lerp(startPos, apex, t);
            if (currentShadow)
                currentShadow.transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);
            yield return null;
        }

        float chaseTimer = 0f;
        while (chaseTimer < shadowChaseDuration)
        {
            chaseTimer += Time.deltaTime;
            Vector3 targetPos = playerTransform ? playerTransform.position : m_TargetPosition;
            targetPos.y = apex.y; 
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);

            if (currentShadow)
            {
                Vector3 shadowTarget = playerTransform ? playerTransform.position : m_TargetPosition;
                shadowTarget.y = 0.01f;
                currentShadow.transform.position = Vector3.Lerp(currentShadow.transform.position, shadowTarget, Time.deltaTime * 10f);
            }
            yield return null;
        }

        float stayTimer = 0f;
        Vector3 finalShadowPos = currentShadow ? currentShadow.transform.position : transform.position;
        while (stayTimer < shadowStayDuration)
        {
            stayTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, finalShadowPos + Vector3.up * 1f, Time.deltaTime);
            if (currentShadow)
                currentShadow.transform.position = finalShadowPos;
            yield return null;
        }

        float plungeDuration = 0.2f; 
        timer = 0f;
        Vector3 plungeStart = transform.position;
        while (timer < plungeDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(plungeStart, finalShadowPos, timer / plungeDuration);
            yield return null;
        }

        if (Vector3.Distance(playerTransform.position, finalShadowPos) <= 1.5f)
        {
            playerTransform.GetComponent<PlayerController>().TakeDamage(damage);
        }

        if (currentShadow) Destroy(currentShadow);
        agent.enabled = true;
        isAttacking = false;
        attackCooldown = 10f; 
    }
}