using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask theGround, thePlayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState;


    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, thePlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, thePlayer);

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (playerInSightRange && !playerInAttackRange)
                {
                    Debug.Log("Patrol 1");
                    TransitionToState(EnemyState.Chase);
                }
                else if (playerInAttackRange && playerInSightRange)
                {
                    Debug.Log("Patrol 2");
                    TransitionToState(EnemyState.Attack);
                }
                else
                {
                    Debug.Log("Patrol 3");
                    Patrol();
                }
                break;

            case EnemyState.Chase:
                if (!playerInSightRange && !playerInAttackRange)
                {
                    Debug.Log("Chase 1");
                    TransitionToState(EnemyState.Patrol);
                }
                else if (playerInAttackRange)
                {
                    Debug.Log("Chase 2");
                    TransitionToState(EnemyState.Attack);
                }
                else
                {
                    Debug.Log("Chase 3");
                    Chase();
                }
            break;

            case EnemyState.Attack:
                if (!playerInAttackRange)
                {
                    Debug.Log("Attack 1");
                    TransitionToState(playerInSightRange ? EnemyState.Chase : EnemyState.Patrol);
                }
                else
                {
                    Debug.Log("Attack 2");
                    Attack();
                }
            break;
        }
    }


    private void Patrol()
    {
        Debug.Log("Patrolling");

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 1f)
            {
                walkPointSet = false;
            }
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, theGround))
        {
            walkPointSet = true;
        }
    }

    private void TransitionToState(EnemyState newState)
    {
        currentState = newState;
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
        Debug.Log("Chase 5");
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Debug.Log("Player being attacked!");

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(walkPoint, 3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }
}

