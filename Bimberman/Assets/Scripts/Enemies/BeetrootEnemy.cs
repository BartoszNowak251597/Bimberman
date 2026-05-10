using System.Collections;
using System.Collections.Generic;

//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class BeetrootEnemy : EnemyBase
{
    [Header("Beetroot Settings")]
    [SerializeField] private float attackCooldown = 7f; //cooldowne miêdzy atakami 7 sekund  
    [SerializeField] private int damage = 15; //segment zadaje 15 HP 

    [SerializeField] private float attackDuration = 2f; //ca³y atak trwa oko³o 2 sekundy
    [SerializeField] private float firstSegmentTime = 0.5f; //zaczynaj¹c od 0.5 sekundy. 


    private bool isAttacking = false;
    private bool hasHealedThisAttack = false;

    //public GameObject segment; //segmenty, kwadratowe (1x1 metr gdzie pierwszy pojawia siê jeden metr od buraka). 
    private List<GameObject> spawnedSegments = new List<GameObject>(); //1 metr szerokoœci na 8 metry wysokoœæi   ---> 8 x segement

    protected override void Awake()
    {
        base.Awake();
        //atakuje z odleg³osci oko³o 6 metrów  
        attackRange = 6f;
    }

    protected override void Start()
    {
        base.Start();
        if (playerTransform == null)
            Debug.LogWarning("EnemyBeetroot: playerTransform not assigned");
        else
        {
            //Prêdkoœæ - 85% prêdkoœci gracza
            m_Speed = playerTransform.GetComponent<PlayerController>().speed * 0.85f;
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
        hasHealedThisAttack = false;

        Vector3 startPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        float totalSpawnTime = attackDuration;
        float[] spawnDelays = new float[8];
        float firstDelay = firstSegmentTime;     // 0.5
        float remainingTime = totalSpawnTime - firstDelay; // 1.5 s dla pozosta³ych 7 segmentów
        float baseInterval = remainingTime / 7f;

        spawnDelays[0] = firstDelay;
        for (int i = 1; i < 8; i++)
        {
            float ratio = (float)i / 7f;
            float interval = Mathf.Lerp(baseInterval * 1.5f, baseInterval * 0.5f, ratio);
            spawnDelays[i] = spawnDelays[i - 1] + interval;
        }


        Vector3 dirToPlayer = (playerPos - startPos).normalized;

        for (int i = 0; i < 8; i++)
        {
            // odpowiedni czas od startu 
            float waitTime = (i == 0) ? spawnDelays[0] : spawnDelays[i] - spawnDelays[i - 1];
            yield return new WaitForSeconds(waitTime);

            // pozycja segmentu: 1 metr od buraka + i*1 metr w kierunku gracza
            Vector3 segmentPos = startPos + dirToPlayer * (1f + i);
            GameObject seg = Instantiate(m_ProjectilePrefab, segmentPos, Quaternion.identity);
            spawnedSegments.Add(seg);

            BeetrootSegment script = seg.GetComponent<BeetrootSegment>();
            if (script) script.Initialize(this);
        }

        // segmenty zostaj¹ przez 1 sekundê
        yield return new WaitForSeconds(1f);
        ClearSegments();

        agent.enabled = true;
        isAttacking = false;
        attackCooldown = 7f;
    }
    private void ClearSegments()
    {
        foreach (GameObject seg in spawnedSegments)
        {
            if (seg != null) Destroy(seg);
        }
        spawnedSegments.Clear();
    }
    public void OnSegmentHitPlayer()
    {
        if (!hasHealedThisAttack)
        {
            Debug.Log("BeetrootEnemy healed for hitting player!");
            if (m_hp < 100)
                m_hp += 15;   
            hasHealedThisAttack = true;
        }
    }

    private void OnDestroy()
    {
        ClearSegments();
    }
}