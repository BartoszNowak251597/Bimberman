// EnemySkeleton.cs
using UnityEngine;


public class EnemySkeleton : EnemyBase
{
    [SerializeField] private Transform playerTransform; 

    void Update()
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
                StopMoving();
                Attack();
                break;
            case States.FLEEING:
                Flee();
                Attack();
                break;
            case States.AVOIDING_OBSTACLE:
                AstarChase();
                break;
        }
    }
}