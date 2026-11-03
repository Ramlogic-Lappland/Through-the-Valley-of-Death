using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D))]
public class ChargingEnemyAttack : MonoBehaviour, IEnemyAttack, IEnemyAttackState
{
    [Header("When to Charge")]
    [SerializeField] private float attackRange = 6f;
    [SerializeField] private float minChargeDistance = 1.5f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Timing and Movement")]
    [SerializeField] private float windupTime = 2f;
    [SerializeField] private float chargeSpeed = 14f;
    [SerializeField] private float maxChargeDistance = 8f;
    [SerializeField] private float targetOvershoot = 0.6f;
    [SerializeField] private float recoveryTime = 0.7f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Impact")]
    [SerializeField] private float damage = 15f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float chargeRadius = 0.35f;
    [SerializeField] private float wallPadding = 0.05f;
    [SerializeField] private float navMeshRecoveryRadius = 1f;

    [Header("Optional Visuals / Debugging")]
    [SerializeField] private LineRenderer warningLine;
    [SerializeField] private bool useAnimationTriggers;
    [SerializeField] private bool logChargeEvents = true;

    private enum ChargePhase { Ready, Windup, Charging, Recovery }
    private ChargePhase _phase = ChargePhase.Ready;

    private Rigidbody2D _rb;
    private NavMeshAgent _agent;
    private Animator _animator;
    private PlayerHealth _targetHealth;

    private Vector2 _chargeDirection;
    private Vector2 _chargeStartPosition;
    private float _plannedChargeDistance;
    private float _phaseTimer;
    private float _cooldownTimer;
    private bool _hasHitPlayer;

    public float AttackRange => attackRange;
    public bool IsAttackInProgress => _phase != ChargePhase.Ready;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (warningLine != null)
        {
            warningLine.useWorldSpace = true;
            warningLine.enabled = false;
        }
    }

    public bool CanStartAttack(Transform player)
    {
        if (player == null || _phase != ChargePhase.Ready ||
            _cooldownTimer > 0f || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
            return false;

        Vector2 toPlayer = (Vector2)player.position - _rb.position;
        float distance = toPlayer.magnitude;

        // Require enough range to reach the target, including a little overshoot.
        if (distance < minChargeDistance || distance > attackRange ||
            distance + targetOvershoot > maxChargeDistance)
            return false;

        // Check that the charger's width can fit in a straight line.
        RaycastHit2D wall = FindNearestHit(
            _rb.position, toPlayer.normalized, distance, wallLayer, false);

        return wall.collider == null;
    }

    public void Attack(Transform player)
    {
        if (!CanStartAttack(player))
            return;

        _targetHealth = player.GetComponentInParent<PlayerHealth>();
        if (_targetHealth == null)
        {
            Debug.LogWarning($"{name}: PlayerHealth not found on charge target.", this);
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - _rb.position;
        _chargeDirection = toPlayer.normalized;
        _chargeStartPosition = _rb.position;
        _plannedChargeDistance = Mathf.Min(
            maxChargeDistance, toPlayer.magnitude + targetOvershoot);
        _phase = ChargePhase.Windup;
        _phaseTimer = Mathf.Max(0f, windupTime);
        _hasHitPlayer = false;

        _agent.ResetPath();
        _agent.isStopped = true;
        _rb.linearVelocity = Vector2.zero;
        FaceChargeDirection();

        if (useAnimationTriggers && _animator != null)
            _animator.SetTrigger("ChargeWindup");

        UpdateWarningLine();
        Log($"Windup: planned distance {_plannedChargeDistance:F2} units.");
    }

    private void FixedUpdate()
    {
        _cooldownTimer = Mathf.Max(0f, _cooldownTimer - Time.fixedDeltaTime);

        switch (_phase)
        {
            case ChargePhase.Windup:
                UpdateWarningLine();
                _phaseTimer -= Time.fixedDeltaTime;
                if (_phaseTimer <= 0f)
                    BeginCharge();
                break;

            case ChargePhase.Charging:
                MoveCharge();
                break;

            case ChargePhase.Recovery:
                _phaseTimer -= Time.fixedDeltaTime;
                if (_phaseTimer <= 0f)
                    _phase = ChargePhase.Ready;
                break;
        }
    }

    private void BeginCharge()
    {
        if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name}: Cannot charge: agent is not on NavMesh.", this);
            BeginRecovery();
            return;
        }

        if (warningLine != null)
            warningLine.enabled = false;

        _phase = ChargePhase.Charging;
        _chargeStartPosition = _rb.position;
        _agent.ResetPath();
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _rb.linearVelocity = Vector2.zero;

        // Safety timeout: finish if a physical collider prevents movement.
        _phaseTimer = _plannedChargeDistance / Mathf.Max(0.01f, chargeSpeed) + 0.5f;

        if (useAnimationTriggers && _animator != null)
            _animator.SetTrigger("Charge");

        Log("Charge movement started.");
    }

    private void MoveCharge()
    {
        _phaseTimer -= Time.fixedDeltaTime;

        float traveled = Vector2.Distance(_chargeStartPosition, _rb.position);
        float remaining = _plannedChargeDistance - traveled;
        if (_phaseTimer <= 0f || remaining <= 0.01f)
        {
            FinishCharge(_phaseTimer <= 0f ? "movement timeout" : "reached distance");
            return;
        }

        float step = Mathf.Min(Mathf.Max(0.01f, chargeSpeed) * Time.fixedDeltaTime, remaining);
        float castLength = step + wallPadding;

        RaycastHit2D wall = FindNearestHit(
            _rb.position, _chargeDirection, castLength, wallLayer, false);
        RaycastHit2D playerHit = FindNearestHit(
            _rb.position, _chargeDirection, castLength, playerLayer, true);

        if (wall.collider != null &&
            (playerHit.collider == null || wall.distance <= playerHit.distance))
        {
            FinishCharge($"wall: {wall.collider.name}, cast distance {wall.distance:F2}");
            return;
        }

        if (playerHit.collider != null)
        {
            DamagePlayerOnce(playerHit.collider);
            FinishCharge($"player: {playerHit.collider.name}");
            return;
        }

        // Physically move in a straight line; let the Rigidbody2D respect walls.
        // Do not check NavMesh.SamplePosition on every tiny step
        _rb.MovePosition(_rb.position + _chargeDirection * step);
    }

    // Only return colliders that really are a wall or the target player.
    private RaycastHit2D FindNearestHit(
        Vector2 origin, Vector2 direction, float distance,
        LayerMask mask, bool requireTargetPlayer)
    {
        RaycastHit2D nearest = default;
        float bestDistance = float.PositiveInfinity;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin, chargeRadius, direction, distance, mask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                continue;

            // Ignore all colliders on this enemy or any of its children.
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            if (requireTargetPlayer &&
                hit.collider.GetComponentInParent<PlayerHealth>() != _targetHealth)
                continue;

            if (hit.distance < bestDistance)
            {
                bestDistance = hit.distance;
                nearest = hit;
            }
        }

        return nearest;
    }

    private void DamagePlayerOnce(Collider2D hitCollider)
    {
        if (_hasHitPlayer)
            return;

        PlayerHealth health = hitCollider.GetComponentInParent<PlayerHealth>();
        if (health == null || health != _targetHealth)
            return;

        _hasHitPlayer = true;
        health.TakeDamage(damage);

        PlayerController controller = hitCollider.GetComponentInParent<PlayerController>();
        if (controller != null)
            controller.ApplyKnockback(_chargeDirection, knockbackForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_phase != ChargePhase.Charging)
            return;

        Collider2D other = collision.collider;
        if (other.GetComponentInParent<PlayerHealth>() == _targetHealth && _targetHealth != null)
        {
            DamagePlayerOnce(other);
            FinishCharge("physical collision with player");
        }
        else if ((wallLayer.value & (1 << other.gameObject.layer)) != 0 &&
                 !other.transform.IsChildOf(transform))
        {
            FinishCharge($"physical collision with wall: {other.name}");
        }
    }

    private void FinishCharge(string reason)
    {
        if (_phase != ChargePhase.Charging)
            return;

        _rb.linearVelocity = Vector2.zero;
        Log($"Charge finished: {reason}. Moved {Vector2.Distance(_chargeStartPosition, _rb.position):F2} units.");

        // Restore the NavMeshAgent only after moving its internal position Re-enabling updatePosition without this can snap the enemy backward.
        if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
        {
            Vector3 currentPosition = new Vector3(
                _rb.position.x, _rb.position.y, transform.position.z);

            bool warped = false;
            if (NavMesh.SamplePosition(currentPosition, out NavMeshHit closest,
                    navMeshRecoveryRadius, _agent.areaMask))
            {
                warped = _agent.Warp(closest.position);
            }

            if (!warped)
            {
                Debug.LogWarning($"{name}: No NavMesh near charge end; returning to start.", this);
                warped = _agent.Warp(new Vector3(
                    _chargeStartPosition.x, _chargeStartPosition.y,
                    transform.position.z));
            }

            if (warped)
                _agent.updatePosition = true;
            else
                Debug.LogError($"{name}: Could not restore NavMeshAgent after charging.", this);

            _agent.isStopped = true;
        }

        BeginRecovery();
    }

    private void BeginRecovery()
    {
        _phase = ChargePhase.Recovery;
        _phaseTimer = Mathf.Max(0f, recoveryTime);
        _cooldownTimer = Mathf.Max(0f, attackCooldown);
        _rb.linearVelocity = Vector2.zero;
        if (warningLine != null)
            warningLine.enabled = false;
    }

    private void FaceChargeDirection()
    {
        float angle = Mathf.Atan2(
            _chargeDirection.y, _chargeDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void UpdateWarningLine()
    {
        if (warningLine == null)
            return;

        
        float length = _plannedChargeDistance;
        RaycastHit2D wall = FindNearestHit(
            _rb.position, _chargeDirection, length, wallLayer, false);
        if (wall.collider != null)
            length = Mathf.Max(0f, wall.distance - wallPadding);

        Vector3 start = transform.position;
        Vector2 end = _rb.position + _chargeDirection * length;
        warningLine.positionCount = 2;
        warningLine.SetPosition(0, start);
        warningLine.SetPosition(1, new Vector3(end.x, end.y, start.z));
        warningLine.enabled = true;
    }

    private void Log(string message)
    {
        if (logChargeEvents)
            Debug.Log($"{name} | ChargingEnemyAttack: {message}", this);
    }

    private void OnDisable()
    {
        if (warningLine != null)
            warningLine.enabled = false;

        if (_rb != null && _phase == ChargePhase.Charging)
            _rb.linearVelocity = Vector2.zero;
    }
}
