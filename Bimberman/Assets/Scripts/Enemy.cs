using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Serializable]
    public struct DroppedItem {
        public float chance;
        public GameObject item;
    }

    public NavMeshAgent agent;
   //public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public GameObject enemyShoot;

    public List<DroppedItem> enemyDrop;

    public float health = 10;
    public int m_Locked = 0;
    public bool Locked => m_Locked > 0;


    private void Awake()
    {
        //player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

      

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if(!playerInSightRange && !playerInAttackRange && !Locked) Patroling();
        if (playerInSightRange && !playerInAttackRange && !Locked) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();

    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet) {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(PlayerController.playerInstance.transform.position);
    }

    private void AttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, PlayerController.playerInstance.transform.position);

        if (distance < attackRange * 0.8f && !Locked)
        {
            Vector3 directionAway = (transform.position - PlayerController.playerInstance.transform.position).normalized;
            Vector3 newPos = PlayerController.playerInstance.transform.position + directionAway * attackRange; 
            transform.LookAt(PlayerController.playerInstance.transform);
            agent.SetDestination(newPos);
        }
        else 
        {
            agent.SetDestination(transform.position); 
            transform.LookAt(PlayerController.playerInstance.transform);

            if (!alreadyAttacked)
            {
                Vector3 direction = (PlayerController.playerInstance.transform.position + Vector3.up - transform.position).normalized;
                GameObject projectile = Instantiate(enemyShoot, transform.position + direction * 1f, Quaternion.LookRotation(direction));
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.linearVelocity = direction * 50f;

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    public void DestroyEnemy() {
        Destroy(gameObject);
        DropItem();
    }

    private GameObject GetRandomDrop() {
        float totalChance = enemyDrop.Sum( i => i.chance );

        float choice = Random.Range(0, totalChance);

        foreach (var drop in enemyDrop) {
            if (drop.chance >= choice) {
                return drop.item;
            }

            choice -= drop.chance;
        }

        return enemyDrop.Last().item;
    }

    public void DropItem()
    {
        int count = Random.Range(3, 6); 
        for (int i = 0; i < count; i++)
        {
            
            GameObject drop = Instantiate(GetRandomDrop(), transform.position, Quaternion.identity);

            
            Renderer renderer = drop.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color randomColor = new Color(Random.value, Random.value, Random.value);
                renderer.material.color = randomColor; 
            }

           
            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomForce = Random.insideUnitSphere * 3f; 
                rb.AddForce(randomForce, ForceMode.Impulse);
            }
        }
    }
}
