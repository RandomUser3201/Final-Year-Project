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

    public enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState;

    private Pathfinding pathfinding;
    public bool currentlyChasing = false;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        pathfinding = GetComponent<Pathfinding>();
    }

    void Start()
    {
        //pathfinding.enabled = false;
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
                    Debug.Log("");
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
                else if (playerInSightRange)
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
        agent.enabled = true;
        Debug.Log("Patrolling");

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            // normal value was 1, changed to 10, to solve problem of walkpoint positioning on obstacles.
            if (Vector3.Distance(transform.position, walkPoint) < 6f)
            {
                walkPointSet = false;
            }
        }
    }

    private void SearchWalkPoint()
    {
        for (int walkPointAttempts = 0; walkPointAttempts < 2; walkPointAttempts++)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                if (Physics.Raycast(hit.position, -transform.up, 2f, theGround))
                {
                    walkPoint = hit.position;
                    walkPointSet = true;
                    return;
                }
            }
        }

        walkPointSet = false;
        Debug.Log("Failed to find walk point");
    }


    private void TransitionToState(EnemyState newState)
    {
        if (newState == EnemyState.Patrol)
        {
            //pathfinding.ClearPath();
            walkPointSet = false;
        }

        currentState = newState;
    }

    private void Chase()
    {
        //agent.enabled = false;

        if (playerInSightRange)
        {
            currentlyChasing = true;

            if (currentlyChasing)
            {
                //pathfinding.enabled = true;
                //pathfinding.FindPath(transform.position, player.position);
                Debug.Log("Chase 5: A* Pathfinding activated");
            }
        }

        else
        {
            Debug.Log("player is not in sight");
            pathfinding.enabled = false;
            currentlyChasing = false;
            TransitionToState(EnemyState.Patrol);
        }
        //else
        //{
        //    agent.SetDestination(player.position);
        //    Debug.LogWarning("Pathfinding script missing");
        //}
    }

    public bool Something()
    {
        return currentlyChasing;
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

    public EnemyState GetCurrentState()
    {
        return currentState;
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

