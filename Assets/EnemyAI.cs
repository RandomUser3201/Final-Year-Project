using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // [References]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask theGround, thePlayer;
    private Pathfinding pathfinding;

    // [Walk Point]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // [Attack]
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // [Range Detection]
    public float sightRange, attackRange, soundRange;
    public bool playerInSightRange, playerInAttackRange;

    // [Audio]
    public float detectionThreshold = 0.7f;
    private AudioSource playerAudioSource;

    // [State]
    public enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState;
    public bool currentlyChasing = false;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        pathfinding = GetComponent<Pathfinding>();
        playerAudioSource = player.GetComponent<AudioSource>();
    }

    void Start()
    {
        // Intial state set to Patrol
        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        // Check if the player is within sight, attack, or sound detection range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, thePlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, thePlayer);

        // Main state machine logic.
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (playerInSightRange && !playerInAttackRange)
                {
                    Debug.Log("Player detected during patrol - Transitioning to Chase.");
                    TransitionToState(EnemyState.Chase);
                }
                else if (playerInAttackRange && playerInSightRange)
                {
                    Debug.Log("Player within attack range - Transitioning to Attack.");
                    TransitionToState(EnemyState.Attack);
                }
                else if (IsPlayerMakingNoise())
                {
                    Debug.Log("Player making noise - Transitioning to Chase.");
                    TransitionToState(EnemyState.Chase);
                }
                else
                {
                    Debug.Log("Patrolling..");
                    Patrol();
                }
                break;

            case EnemyState.Chase:
                if (!playerInSightRange && !playerInAttackRange && !IsPlayerMakingNoise())
                {
                    Debug.Log("Player lost - Returning to Patrol.");
                    TransitionToState(EnemyState.Patrol);
                }
                else if (playerInAttackRange)
                {
                    Debug.Log("Chasing player - Now within attack range - Transitioning to Attack.");
                    TransitionToState(EnemyState.Attack);
                }
                else if (playerInSightRange)
                {
                    Debug.Log("Chasing..");
                    Chase();
                }
                break;

            case EnemyState.Attack:
                if (!playerInAttackRange)
                {
                    Debug.Log("Player out of attack range - Transitioning to Chase or Patrol.");
                    TransitionToState(playerInSightRange || IsPlayerMakingNoise() ? EnemyState.Chase : EnemyState.Patrol);
                }
                else
                {
                    Debug.Log("Attacking..");
                    Attack();
                }
                break;
        }
    }

    private void Patrol()
    {
        // Enable NavMeshAgent for movement
        agent.enabled = true;
        Debug.Log("Patrolling");

        // Find new walk point if not set
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            // Move towards walk point
            agent.SetDestination(walkPoint);

            // Check if the destination is reached
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
            // Randomly generate a walk point within range.
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Validate walk point on the NavMesh and ground.
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
        // Reset walk point when returning to Patrol
        if (newState == EnemyState.Patrol)
        {
            walkPointSet = false;
        }

        currentState = newState;
    }

    private bool IsPlayerMakingNoise()
    {
        if (playerAudioSource == null) 
        { 
            return false; 
        }

        // If noise is below threshold then 
        if (playerAudioSource.volume <= detectionThreshold)
        {
            return false;
        }

        // If player is not within the range
        if (Vector3.Distance(transform.position, player.position) > soundRange)
        {
            return false;
        }

        return true;
    }

    private void Chase()
    {
        if (playerInSightRange || IsPlayerMakingNoise())
        {
            currentlyChasing = true;

            if (currentlyChasing)
            {
                Debug.Log("Chase 5: A* Pathfinding activated");
            }
        }

        else
        {
            Debug.Log("player is not in sight or making noise");
            pathfinding.enabled = false;
            currentlyChasing = false;
            TransitionToState(EnemyState.Patrol);
        }
    }

    private void Attack()
    {
        // Stop moving to attack
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Debug.Log("Player being attacked!");
            alreadyAttacked = true;

            // Reset attack cooldown
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        // Allow another attack
        alreadyAttacked = false;
    }

    public EnemyState GetCurrentState()
    {
        // Return the current state of the enemy.
        return currentState;
    }

    private void OnDrawGizmos()
    {
        // Visualize detection ranges for debugging.

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(walkPoint, 3f);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, soundRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }
}

