// EnemyBase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum States
{
    ATTACKING,
    FLEEING,
    CHASING,
    PATROLLING,
    AVOIDING_OBSTACLE
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{

    [Header("Movement")]
    protected NavMeshAgent agent;
    [SerializeField] protected float m_Speed = 5.0f;
    [SerializeField] protected float m_RotationSpeed = 2.0f;
    protected Vector3 m_TargetPosition;
    //protected Vector3 currentPos => transform.position;
    public Vector3 currentPos { get; set; }
    protected Vector3 walkPoint;
    protected bool walkPointSet;
    protected List<Vector3> patrolPoints = new List<Vector3>();  
    protected int posIndex;

    [Header("Combat")]
    [SerializeField] protected float attackRange = 5.0f;
    [SerializeField] protected int m_hp = 100;
    [SerializeField] protected float m_AttackCooldown = 1.5f;
    protected float m_AttackTimer;
    [SerializeField] protected float m_ProjectileSpeed = 15.0f;
    [SerializeField] protected GameObject m_ProjectilePrefab;    
    [SerializeField] protected Transform m_ProjectileSpawnPoint;  
    [SerializeField] protected Animator animator;

    [Header("States")]
    public States currentState = States.PATROLLING;
    public bool isPlayerInRoom = false;
    protected bool m_UsingAStar = false;

    [Header("Target")]
    [SerializeField] protected Transform playerTransform;

    public Animator Animator => animator;
    protected float m_AttackAnimationDuration = 1.0f;
    protected float m_AttackAnimationElapsed;
    protected bool m_InAttackAnimation;
    protected string m_CurrentAnimation;

    protected Vector3 m_LastChasePosition;
    protected float m_StuckTimer = 0.0f;
    protected float m_StuckThreshold = 1.5f;
    protected float m_MinMovementThreshold = 0.2f;

    [SerializeField] protected LayerMask obstacleMask = -1;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = m_Speed;
        agent.angularSpeed = m_RotationSpeed * 100f;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log($"{gameObject.name} warped to NavMesh at {hit.position}");
            }
            else
            {
                Debug.LogError($"{gameObject.name} is not near any NavMesh! Disabling.");
                gameObject.SetActive(false); 
            }
        }
    }

    protected virtual void Update()
    {
        currentPos = transform.position;
    }

    protected virtual void Start() { }

    public void SetTarget(Vector3 target) { m_TargetPosition = target; }

    public void SetTargetNode(Transform node) { if (node) m_TargetPosition = node.position; }

    protected void MoveInDirection(Vector3 direction)
    {
        if (direction.magnitude < 0.001f)
        {
            StopMoving();
            return;
        }
        Vector3 dir = direction.normalized;
        agent.velocity = dir * m_Speed;
        RotateNode(dir);
    }

    protected void StopMoving()
    {
        agent.velocity = Vector3.zero;
        if (!m_InAttackAnimation && animator)
            SetAnimation("stop_001");
    }

    protected void RotateNode(Vector3 dir)
    {
        if (dir.magnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, agent.angularSpeed * Time.deltaTime);
        }
    }

    protected void Patrol()
    {
        if (!walkPointSet)
            SearchWalkPoint();
        else
        {
            Vector3 dir = walkPoint - currentPos;
            float distance = dir.magnitude;
            if (distance > 0.5f)
                agent.SetDestination(walkPoint);
            else
            {
                StopMoving();
                walkPointSet = false;
            }
        }
    }

    protected void SearchWalkPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += currentPos;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
        else
        {
            walkPointSet = false;
        }
        if (patrolPoints.Count > 0)
            LookForNextPoint();
    }

    protected void LookForNextPoint()
    {
        posIndex = (posIndex + 1) % patrolPoints.Count;
        walkPoint = patrolPoints[posIndex];
        walkPointSet = true;
    }

    public void SetPatrolPoints(List<Vector2> points)
    {
        foreach (var p in points)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(new Vector3(p.x, 0, p.y), out hit, 5f, NavMesh.AllAreas))
                patrolPoints.Add(hit.position);
        }
    }

    protected Vector3 GetValidNavMeshPoint(Vector3 point, float maxDistance = 5f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, maxDistance, NavMesh.AllAreas))
            return hit.position;
        return point; // fallback – mo¿e byæ nadal poza NavMesh
    }

    protected Vector3 ClampToNavMesh(Vector3 point, float maxDistance = 5f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, maxDistance, NavMesh.AllAreas))
            return hit.position;
        return transform.position; // awaryjnie – zostañ w miejscu
    }
    protected void DirectChase()
    {
        if (m_TargetPosition == Vector3.zero)
        {
            StopMoving();
            return;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning("DirectChase: agent not ready");
            return;
        }
        Vector3 target = ClampToNavMesh(m_TargetPosition, 5f);
        float distance = Vector3.Distance(transform.position, target);
        if (distance > 0.2f)
        {
            agent.SetDestination(target);
        }
        else
        {
            StopMoving();
        }
    }

    protected void AstarChase()
    {
        DirectChase();
    }

    protected void Flee()
    {
        Vector3 dirAway = currentPos - m_TargetPosition;
        if (dirAway.magnitude < 0.1f)
        {
            StopMoving();
            return;
        }
        Vector3 fleeTarget = currentPos + dirAway.normalized * 10f;
        agent.SetDestination(fleeTarget);
    }

    protected virtual void Attack()
    {
        if (m_InAttackAnimation) return;

        Vector3 dirTo = m_TargetPosition - currentPos;
        if (dirTo.magnitude > 0.01f)
            RotateNode(dirTo);

        m_AttackTimer += Time.deltaTime;
        if (m_AttackTimer >= m_AttackCooldown)
        {
            m_AttackTimer = 0f;
            SetAnimation("attack_001");
            m_InAttackAnimation = true;
            m_AttackAnimationElapsed = 0f;
            PlayAttackAnimation("attack_001");
            SpawnProjectile(m_TargetPosition);
        }
    }

    protected void SpawnProjectile(Vector3 targetPos)
    {
        if (!m_ProjectilePrefab)
        {
            Debug.LogWarning("Projectile prefab not set");
            return;
        }
        Vector3 startPos = (m_ProjectileSpawnPoint ? m_ProjectileSpawnPoint.position : currentPos + Vector3.up);
        GameObject proj = Instantiate(m_ProjectilePrefab, startPos, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 dir = (targetPos - startPos).normalized;
            rb.linearVelocity = dir * m_ProjectileSpeed;
        }
    }

    public void SetAttackAnimation(Animator anim) { } 

    public void PlayAttackAnimation(string name)
    {
        if (animator)
            animator.Play(name);
    }

    public void UpdateAttackAnimation()
    {
        if (!m_InAttackAnimation) return;
        m_AttackAnimationElapsed += Time.deltaTime;
        if (m_AttackAnimationElapsed >= m_AttackAnimationDuration)
        {
            m_InAttackAnimation = false;
            if (agent.velocity.magnitude > 0.1f)
                SetAnimation("idle_001");
            else
                SetAnimation("stop_001");
        }
    }

    protected void SetAnimation(string name)
    {
        Debug.Log($"EnemyBase: Setting animation to {name}");
        if (!animator) return;
        if (m_CurrentAnimation == name) return;
        animator.Play(name);
        m_CurrentAnimation = name;
    }

    public virtual void OnPlayerEnteredRoom() => isPlayerInRoom = true;
    public virtual void OnPlayerExitedRoom() => isPlayerInRoom = false;

    public void TakeDamage(int damage)
    {
        Debug.Log($"EnemyBase: Took {damage} damage");
        m_hp -= damage;
        if (m_hp <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected bool CanSeePlayer()
    {
        Vector3 origin = currentPos + Vector3.up;
        Vector3 dir = m_TargetPosition - origin;
        float dist = dir.magnitude;
        if (dist < 0.1f) return true;

        if (Physics.Raycast(origin, dir.normalized, dist, obstacleMask))
            return false;
        return true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentPos, 15f); // sightRange
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentPos, attackRange);

        if (walkPointSet)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(walkPoint, 0.2f);
        }

        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var p in patrolPoints) Gizmos.DrawSphere(p, 0.1f);
        }
    }
}

