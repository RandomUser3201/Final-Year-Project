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
    public NavMeshAgent Agent;
    public Transform Player;
    public LayerMask TheGround, ThePlayer;
    private Pathfinding _pathfinding;
    private PulseRateManager _pulseRateManager;
    private Animator _animator;
    private PlayerSound _playerSound;


    // [Walk Point]
    public Vector3 WalkPoint;
    private bool _isWalkPointSet;
    public float WalkPointRange;

    // [Attack]
    public float TimeBetweenAttacks;
    private bool _hasAttacked;
    public int PlayerHealth = 3;

    // [Range Detection]
    public float SightRange;
    public float AttackRange; 
    public float SoundRange;
    public bool IsPlayerInSightRange; 
    public bool IsPlayerInAttackRange;

    // [Audio]
    public float DetectionThreshold = 7f;

    // [State]
    public enum EnemyState { Patrol, Chase, Attack }
    private EnemyState _currentState;

    void Awake()
    {
        Player = GameObject.Find("PlayerArmature").transform;
        Agent = GetComponent<NavMeshAgent>();
        _pathfinding = GetComponent<Pathfinding>();
        _animator = GetComponent<Animator>();
        _playerSound = Player.GetComponent<PlayerSound>();
    }

    void Start()
    {
        _pulseRateManager = FindObjectOfType<PulseRateManager>();

        if (_pulseRateManager == null)
        {
            Debug.LogWarning("Pulse Rate Manager Script not found");
        }
        // Intial state set to Patrol
        _currentState = EnemyState.Patrol;
    }

    void Update()
    {
        AnimationController();

        // Check if the player is within sight, attack, or sound detection range.
        IsPlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, ThePlayer);
        IsPlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, ThePlayer);

        // Main state machine logic.
        switch (_currentState)
        {
            case EnemyState.Patrol:
                if (IsPlayerInSightRange && !IsPlayerInAttackRange)
                {
                    Debug.LogWarning("Player within sight range, not in attack - Transitioning to Chase.");
                    TransitionToState(EnemyState.Chase);
                }
                else if (IsPlayerInAttackRange && IsPlayerInSightRange)
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
                if (!IsPlayerInSightRange && !IsPlayerMakingNoise())
                {
                    Debug.LogWarning("Player out of sight and not making noise - Returning to Patrol.");
                    TransitionToState(EnemyState.Patrol);
                }
                else if (IsPlayerInAttackRange)
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
                if (!IsPlayerInAttackRange)
                {
                    Debug.LogWarning("Player out of attack range - Transitioning to Chase or Patrol.");
                    TransitionToState(IsPlayerInSightRange ? EnemyState.Chase : EnemyState.Patrol);
                }
                else
                {
                    Debug.LogWarning("Attacking..");
                    Attack();
                }
                break;
        }

        if (_pulseRateManager.IsVisible == false)
        {
            Debug.LogWarning($"Pulse Rate Manager: {_pulseRateManager.IsVisible}");
        }
    }

    private void Patrol()
    {
        // Enable NavMeshAgent for movement
        Agent.enabled = true;
        Debug.Log("Patrolling");

        // Find new walk point if not set
        if (!_isWalkPointSet)
        {
            SearchWalkPoint();
 
        }

        if (_isWalkPointSet)
        {
            // Move towards walk point
            Agent.SetDestination(WalkPoint);

            Vector3 direction = WalkPoint - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); 
            }

            // Check if the destination is reached
            if (Vector3.Distance(transform.position, WalkPoint) < 9f)
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
            float randomZ = Random.Range(-WalkPointRange, WalkPointRange);
            float randomX = Random.Range(-WalkPointRange, WalkPointRange);

            WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // Validate walk point on the NavMesh and ground.
            if (NavMesh.SamplePosition(WalkPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                if (Physics.Raycast(hit.position, -transform.up, 2f, TheGround))
                {
                    WalkPoint = hit.position;
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

        _currentState = newState;
    }

    private bool IsPlayerMakingNoise()
    {
        if (IsPlayerInAttackRange || IsPlayerInSightRange)
        {
            return false;
        }

        // Check if the player is within the sound range
        if (Vector3.Distance(transform.position, Player.position) <= SoundRange)
        {
            Debug.LogWarning($"Current volume if distance {_playerSound.CurrentVolume}");

            // Return true if the player is making noise above the threshold OR is within the sound range
            if (_playerSound.CurrentVolume > DetectionThreshold)
            {
                Debug.LogWarning($"Player making noise, detection threshold exceeded. {_playerSound.CurrentVolume}");
                return true;
            }
        }

        return false;
    }

    private void Chase()
    {

        Debug.LogWarning("Enter chase()");

        if (!IsPlayerInSightRange && !IsPlayerMakingNoise())
        {
            Debug.LogWarning("Player out of chase range - stopping FollowPath");
            return;
        }

        if (IsPlayerInSightRange)
        {
            Debug.Log("Chasing Player");
            _pathfinding.FollowPath();
            RotateToPlayer();
            //Debug.LogWarning("Destination set to: " + player.position);
        }
        else
        {
            Debug.LogWarning("Else chase");
        }
    }

    private void Attack()
    {
        Debug.LogWarning($"Attacking! Cooldown: {_hasAttacked}");

        // Stop moving to attack
        Agent.SetDestination(transform.position);

        if (!_hasAttacked)
        {
            Debug.LogWarning("Attacking the player");

            // Reduce player health
            PlayerHealth--;

            Debug.LogWarning($"Player Health: {PlayerHealth}");

            // Destroy the player if health reaches 0
            if (PlayerHealth <= 0)
            {
                Debug.Log("Player destroyed");
                Destroy(Player.gameObject);
            }

            _hasAttacked = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        // Allow another attack
        _hasAttacked = false;
    }

    public EnemyState GetCurrentState()
    {
        // Return the current state of the enemy.
        return _currentState;
    }

    private void OnDrawGizmos()
    {
        // Visualize detection ranges for debugging.

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(WalkPoint, 3f);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, SoundRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, WalkPointRange);
    }

    private void AnimationController()
    {
        if (_currentState == EnemyState.Patrol)
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
        Vector3 playerDirection = Player.position - transform.position;
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