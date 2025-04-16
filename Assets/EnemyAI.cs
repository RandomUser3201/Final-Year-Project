using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    // [References]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask theGround, thePlayer;
    private Pathfinding _pathfinding;
    private PulseRateManager psm;
    private AudioSource _audioSource;
    private Animator _animator;


    // [Walk Point]
    public Vector3 walkPoint;
    private bool _isWalkPointSet;
    public float walkPointRange;

    // [Attack]
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public int playerHealth = 3;

    // [Range Detection]
    public float sightRange, attackRange, soundRange;
    public bool playerInSightRange, playerInAttackRange;

    // [Audio]
    public float detectionThreshold = 0.7f;

    // [State]
    public enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState;
    public bool currentlyChasing = false;

    void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
        _pathfinding = GetComponent<Pathfinding>();
        _audioSource = player.GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        psm = FindObjectOfType<PulseRateManager>();

        if (psm == null)
        {
            Debug.LogWarning("psm null");
        }
        // Intial state set to Patrol
        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        AnimationController();

        // Check if the player is within sight, attack, or sound detection range.
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, thePlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, thePlayer);

        // Main state machine logic.
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (playerInSightRange && !playerInAttackRange)
                {
                    Debug.LogWarning("Player within sight range, not in attack - Transitioning to Chase.");
                    TransitionToState(EnemyState.Chase);
                }
                else if (playerInAttackRange && playerInSightRange)
                {
                    Debug.LogWarning("Player within sight range and attack range - Transitioning to Attack.");
                    TransitionToState(EnemyState.Attack);
                }
                else if (IsPlayerMakingNoise())
                {
                    Debug.LogWarning("Player detected by noise - Transitioning to Chase.");
                    TransitionToState(EnemyState.Chase);
                }
                else
                {
                    Debug.LogWarning("Patrolling..");
                    Patrol();
                }
                break;

            case EnemyState.Chase:
                if (!playerInSightRange && !IsPlayerMakingNoise())
                {
                    Debug.LogWarning("Player out of sight and not making noise - Returning to Patrol.");
                    TransitionToState(EnemyState.Patrol);
                }
                else if (playerInAttackRange)
                {
                    Debug.LogWarning("Player in attack range - Transitioning to Attack.");
                    TransitionToState(EnemyState.Attack);
                }
                else
                {
                    Debug.LogWarning("Chasing Player.");
                    Chase();
                }
                break;

            case EnemyState.Attack:
                if (!playerInAttackRange)
                {
                    Debug.LogWarning("Player out of attack range - Transitioning to Chase or Patrol.");
                    TransitionToState(playerInSightRange ? EnemyState.Chase : EnemyState.Patrol);
                }
                else
                {
                    Debug.LogWarning("Attacking..");
                    Attack();
                }
                break;
        }

        if (psm.visibility == false)
        {
            Debug.LogWarning("psm visiblity");
        }
    }

    private void Patrol()
    {
        // Enable NavMeshAgent for movement
        agent.enabled = true;
        Debug.Log("Patrolling");

        // Find new walk point if not set
        if (!_isWalkPointSet)
        {
            SearchWalkPoint();
 
        }

        if (_isWalkPointSet)
        {
            // Move towards walk point
            agent.SetDestination(walkPoint);

            Vector3 direction = walkPoint - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
            }

            // Check if the destination is reached
            if (Vector3.Distance(transform.position, walkPoint) < 9f)
            {
                _isWalkPointSet = false;
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
                    _isWalkPointSet = true;
                    return;
                }
            }
        }

        _isWalkPointSet = false;
        Debug.Log("Failed to find walk point");
    }

    private void TransitionToState(EnemyState newState)
    {
        // Reset walk point when returning to Patrol
        if (newState == EnemyState.Patrol)
        {
            _isWalkPointSet = false;
        }

        currentState = newState;
    }

    private bool IsPlayerMakingNoise()
    {
        if (_audioSource == null)
        {
            return false;
        }

        if (playerInAttackRange || playerInSightRange)
        {
            return false;
        }

        // Check if the player is within the sound range
        if (Vector3.Distance(transform.position, player.position) <= soundRange)
        {
            // Return true if the player is making noise above the threshold OR is within the sound range
            if (_audioSource.volume > detectionThreshold)
            {
                Debug.LogWarning("Player making noise, detection threshold exceeded.");
                return true;
            }
        }

        return false;
    }

    private void Chase()
    {

        Debug.LogWarning("Enter chase()");

        if (!playerInSightRange && !IsPlayerMakingNoise())
        {
            Debug.LogWarning("Player out of chase range - stopping FollowPath");
            return;
        }

        if (playerInSightRange)
        {
            Debug.Log("Chasing Player");
            _pathfinding.FollowPath();
            RotateToPlayer();
            Debug.LogWarning("Destination set to: " + player.position);
        }
        else
        {
            Debug.LogWarning("Else chase");
        }
    }

    private void Attack()
    {
        Debug.LogWarning($"Attacking! Cooldown: {alreadyAttacked}");

        // Stop moving to attack
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Debug.LogWarning("Attacking the player");

            // Reduce player health
            playerHealth--;

            Debug.LogWarning($"Player Health: {playerHealth}");

            // Destroy the player if health reaches 0
            if (playerHealth <= 0)
            {
                Debug.Log("Player destroyed");
                Destroy(player.gameObject);
            }

            alreadyAttacked = true;
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

    private void AnimationController()
    {
        if (currentState == EnemyState.Patrol)
        {
            _animator.SetBool("isRunning", false);
        }
        else 
        {
            _animator.SetBool("isRunning", true);
        }
    }

    private void RotateToPlayer()
    {
        Vector3 playerDirection = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(playerDirection, Vector3.up);
        transform.rotation = rotation;
        if (playerDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
        }
    }
}

/* References:
[Enemy AI & State Machines]
Code Monkey (2021). Simple Enemy AI in Unity (State Machine, Find Target, Chase, Attack). [online] Available at: https://www.youtube.com/watch?v=db0KWYaWfeM [Accessed 3 Jan 2025].

Dave / Game Development (2020). FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial. [online] Available at: https://youtu.be/UjkSFoLxesw. [Accessed 3 Jan 2025].

git-amend (2023). EASY Unity Enemy AI using a State Machine. [online] YouTube. Available at: https://www.youtube.com/watch?v=eR-AGr5nKEU [Accessed 3 Jan. 2025].

This is GameDev (2023). How to code SMARTER A.I. enemies | Unity Tutorial. [online] YouTube. Available at: https://www.youtube.com/watch?v=rs7xUi9BqjE [Accessed 3 Jan 2025]. */