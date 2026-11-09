using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class BasicEnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [Header("Death")]
    [SerializeField] private GameObject deadEnemyPrefab ;
    [Header("Vision Layer items will block vision (must add player to it too)")]
    [SerializeField] private LayerMask visionLayer;
    [SerializeField] private HealtBarBehaviour healtBar;
    
    [Header("Basic Attributes")]
    [SerializeField] private float movementSpeed = 2f;
    //[SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float maxHitPoints = 10f;
    [SerializeField] private float hitPoints = 0f;
    private Vector2 _facingDirection = Vector2.right;
    
    
    [Header("Detection Attributes")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float detectionAngle = 90f;
    [SerializeField] private float searchDuration = 3f;
    
    [Header("Patrol Attributes")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    private int _currentPatrolIndex;
    private bool _isWaitingAtPatrolPoint;
    private float _patrolWaitTimer;
    
    [Header("Search Attributes")]
    [SerializeField] private float searchPointRadius = 2f;
    [SerializeField] private int searchPointCount = 3;
    [SerializeField] private float searchArrivalDistance = 0.2f;
    [SerializeField] private float searchWaitTime = 0.5f;
    
    
    private Vector2 _lastKnownPosition;
    private Vector2[] _searchPoints;
    private int _currentSearchIndex;
    private float _searchTimer; 
    private float _searchWaitTimer;
    private float _dmg = 1f; // ONLY FOR TEST
    
    [Header("Damage Attributes")]
    [SerializeField] private MonoBehaviour attackBehaviour;
    private IEnemyAttack _attack;
    private Animator _animator;
    
    [Header("Sounds")]
    [SerializeField] private AK.Wwise.Event deathEvent;
    [SerializeField] private AK.Wwise.Event hitEvent;
    [SerializeField] private AK.Wwise.Event walkEvent;
    [SerializeField] private AK.Wwise.Event idleEvent;

    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Search,
        Attack
    }
    
    private Rigidbody2D _rb;
    private NavMeshAgent _agent;
    private EnemyState _currentState;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        
        if (player == null)
        {
            GameObject playerObject = GameObject.Find("PlayerVIsual");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError(
                    "Could not find a GameObject named PlayerVIsual!",
                    this
                );
            }
        }
        
        _rb = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _agent.speed = movementSpeed;
        _agent.stoppingDistance = searchArrivalDistance;
        _facingDirection = transform.right;
        
        _currentState = EnemyState.Idle;
        hitPoints = maxHitPoints;

        _attack = attackBehaviour as IEnemyAttack;

        if (_attack == null)
        {
            Debug.LogError($"{gameObject.name} has no valid IEnemyAttack assigned.");
        }
    }

    private void Start()
    {
        healtBar.SetHealth(hitPoints, maxHitPoints);
        
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            _currentState = EnemyState.Patrol;
            GoToPatrolPoint();
        }
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.J))// ONLY FOR TEST
        {
            Vector2 sourcePosition = player != null ? (Vector2)player.position : (Vector2)transform.position - _facingDirection;

            TakeDamage(_dmg, sourcePosition);
        }
    }
    
    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Patrol:
                UpdatePatrol();
                break;

            case EnemyState.Chase:
                UpdateChase();
                break;

            case EnemyState.Search:
                UpdateSearch();
                break;

            case EnemyState.Attack:
                UpdateAttack();
                break;
        }
        
        UpdateFacingDirection();
    }

    private void UpdateIdle()
    {
        if (CanSeePlayer())
        {
            DetectPlayer();
        }
    }

    #region PATROL CODE =============================================================================================================================

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            DetectPlayer();
            return;
        }
        
        if (patrolPoints == null || patrolPoints.Length == 0)       // No patrol points.
        {
            return;
        }
        
        if (_isWaitingAtPatrolPoint)
        {
            _patrolWaitTimer -= Time.deltaTime;

            if (_patrolWaitTimer <= 0f)
            {
                _isWaitingAtPatrolPoint = false;
                GoToNextPatrolPoint();
            }

            return;
        }
        
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance) // Got to point
        {
            _isWaitingAtPatrolPoint = true;
            _patrolWaitTimer = patrolWaitTime;

            _agent.ResetPath();
        }
        
    }
    
    private void GoToPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }
        
        Transform target = patrolPoints[_currentPatrolIndex];

        _agent.isStopped = false;
        _agent.SetDestination(target.position);
    }

    private void GoToNextPatrolPoint()
    {
        _currentPatrolIndex++;

        if (_currentPatrolIndex >= patrolPoints.Length)
        {
            _currentPatrolIndex = 0;
        }

        GoToPatrolPoint();
    }
    
    private void ReturnToPatrol()
    {
        _agent.ResetPath();

        _currentState = EnemyState.Patrol;

        _isWaitingAtPatrolPoint = false;

        GoToPatrolPoint();
    }
    #endregion PATROL CODE =============================================================================================================================
    
    private void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            StartSearch();
            return;
        }

        _lastKnownPosition = player.position;

        if (_attack != null &&
            _attack.CanStartAttack(player))
        {
            _agent.ResetPath();
            _currentState = EnemyState.Attack;
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(player.position);
    }

    #region Chase Code =============================================================================================================================
    private void StartSearch()
    {
        _searchTimer = searchDuration;
        _searchWaitTimer = 0f;
        _currentSearchIndex = 0;

        GenerateSearchPoints();

        _agent.isStopped = false;
        
        _agent.SetDestination(_lastKnownPosition); //goes to last seen point

        _currentState = EnemyState.Search;
    }
    
    private void GenerateSearchPoints()
    {
        _searchPoints = new Vector2[searchPointCount];

        for (int i = 0; i < searchPointCount; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * searchPointRadius;

            Vector2 candidate =
                _lastKnownPosition + randomOffset;

            if (NavMesh.SamplePosition(
                    candidate,
                    out NavMeshHit hit,
                    searchPointRadius,
                    NavMesh.AllAreas))
            {
                _searchPoints[i] = hit.position;
            }
            else
            {
                _searchPoints[i] = _lastKnownPosition;
            }
        }
    }
    
    private void UpdateSearch()
    {
        if (CanSeePlayer())
        {
            DetectPlayer();
            return;
        }

        _searchTimer -= Time.deltaTime;
        
        if (_searchTimer <= 0f) // searched for max time
        {
            ReturnToPatrol();
            return;
        }
        
        if (_agent.pathPending)
        {
            return;
        }
        
        if (_agent.remainingDistance <= _agent.stoppingDistance) // Got to point
        {
            _agent.ResetPath();

            _searchWaitTimer -= Time.deltaTime;

            if (_searchWaitTimer > 0f)
            {
                LookAround();
                return;
            }

            _searchWaitTimer = searchWaitTime;

           
            if (_currentSearchIndex < _searchPoints.Length) // Move to search point till none left.
            {
                Vector2 nextPoint = _searchPoints[_currentSearchIndex];

                _currentSearchIndex++;

                _agent.isStopped = false;
                _agent.SetDestination(nextPoint);
            }
            else
            {
                ReturnToPatrol();
            }
        }
    }
    
    private void LookAround()
    {
        float angle =
            Mathf.Sin(Time.time * 2f) * 60f;

        _facingDirection =
            Quaternion.Euler(0f, 0f, angle) *
            Vector2.right;
    }
    #endregion =======================================================================================================================================

    #region Player Detection code ==========================================================================================================
    private void DetectPlayer()
    {
        _lastKnownPosition = player.position;
        
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position; //TURN TO PLAYER ON DETECTION

        if (toPlayer.sqrMagnitude > 0.001f)
        {
            _facingDirection = toPlayer.normalized;
        }
        
        _agent.isStopped = false;
        _agent.SetDestination(player.position);

        _currentState = EnemyState.Chase;
    }


    private bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector2 toPlayer =
            player.position - transform.position;

      
        if (toPlayer.sqrMagnitude > detectionRange * detectionRange)
        {
            return false;
        }
        
        float angle = Vector2.Angle(_facingDirection, toPlayer); // Cone check.

        if (angle > detectionAngle / 2f)
        {
            return false;
        }

        
        RaycastHit2D hit = Physics2D.Raycast // Line of sight.
        (
            transform.position,
            toPlayer.normalized,
            toPlayer.magnitude,
            visionLayer
        );

        if (hit.collider == null)
        {
            return false;
        }

        return hit.collider.CompareTag("Player");
    }
    #endregion =============================================================================================================================

    private void UpdateFacingDirection()
    {
        Vector2 velocity = _agent.velocity;
        
        bool isWalking = velocity.sqrMagnitude > 0.01f;
        
        if (_animator != null)
        {
            _animator.SetBool("IsWalking", isWalking);
        }
        
        if (isWalking)
        {
            _facingDirection = velocity.normalized;

            float angle = Mathf.Atan2(
                _facingDirection.y,
                _facingDirection.x
            ) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                angle
            );
        }
    }

    
    private void UpdateAttack()
    {
        if (!CanSeePlayer())
        {
            StartSearch();
            return;
        }

        if (_attack == null)
        {
            _currentState = EnemyState.Chase;
            return;
        }

        if (!_attack.CanStartAttack(player))
        {
            _currentState = EnemyState.Chase;
            return;
        }

        _agent.ResetPath();

        _attack.Attack(player);
    }

    public void TakeDamage(float damage, Vector2 sourcePosition)
    {
        hitPoints -= damage;

        healtBar.SetHealth(hitPoints, maxHitPoints);

        if (hitPoints <= 0)
        {
            AkUnitySoundEngine.PostEvent(deathEvent.Id, gameObject);
            if (deadEnemyPrefab != null) // sprite of dead enemy
            {
                Instantiate
                (
                    deadEnemyPrefab,
                    transform.position,
                    transform.rotation
                );
            }

       
            WinConditionManager winManager = FindFirstObjectByType<WinConditionManager>();

            if (winManager != null)
            {
                winManager.EnemyDefeated();
            }

            // Destroy the living enemy.
            Destroy(gameObject);
            return;
        }

        ReactToHit(sourcePosition);
        AkUnitySoundEngine.PostEvent(hitEvent.Id, gameObject);
    }
    
    private void ReactToHit(Vector2 sourcePosition)
    {
        _lastKnownPosition = sourcePosition;

        Vector2 direction = (sourcePosition - (Vector2)transform.position).normalized;

        if (direction.sqrMagnitude > 0.01f)
        {
            _facingDirection = direction;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler
            (
                0f,
                0f,
                angle
            );
        }

        StartSearch();
    }
    
    #region GIZMOS =========================================================================================
    private void OnDrawGizmos()
    {
        Color gizmoColor = Color.green;

        if (_currentState == EnemyState.Search)
        {
            gizmoColor = Color.yellow;
        }

        if (Application.isPlaying && CanSeePlayer())
        {
            gizmoColor = Color.red;
        }

        Gizmos.color = gizmoColor;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        Vector3 leftBoundary =
            Quaternion.Euler
            (
                0f,
                0f,
                detectionAngle / 2f
            ) * _facingDirection;

        Vector3 rightBoundary = Quaternion.Euler
        (
            0f,
            0f,
            -detectionAngle / 2f
        ) * _facingDirection;

        Gizmos.DrawLine
        (
            transform.position,
            transform.position +
            leftBoundary * detectionRange
        );

        Gizmos.DrawLine
        (
            transform.position,
            transform.position +
            rightBoundary * detectionRange
        );
        
        if (Application.isPlaying && _currentState == EnemyState.Search) // Last known position.
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere
            (
                _lastKnownPosition,
                0.25f
            );

            Gizmos.DrawLine
            (
                transform.position,
                _lastKnownPosition
            );
        }
        
        if (patrolPoints != null) // Patrol points.
        {
            Gizmos.color = Color.cyan;

            foreach (Transform point in patrolPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(
                        point.position,
                        0.15f
                    );
                }
            }
        }
    }
    #endregion GIZMOS ======================================================================================
    
}
