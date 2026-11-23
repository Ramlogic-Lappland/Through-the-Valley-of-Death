using UnityEngine;

public class BasicMeleeEnemyWithKnockBack : MonoBehaviour, IEnemyAttack
{
    [Header("Melee")]
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;

    private float _cooldownTimer;

    private PlayerHealth _playerHealth;
    private PlayerController _playerController;

    public float AttackRange => attackRange;
    private Animator _animator;

    [SerializeField] private AK.Wwise.Event attackEvent;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    
    private void Update()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }

    public bool CanStartAttack(Transform player)
    {
        if (player == null)
        {
            return false;
        }

        float distance = Vector2.Distance
        (
            transform.position,
            player.position
        );

        return distance <= AttackRange;
    }

    public void Attack(Transform player)
    {
        if (player == null)
        {
            return;
        }

        if (_cooldownTimer > 0f)
        {
            return;
        }

        // Cache PlayerHealth.
        if (_playerHealth == null)
        {
            _playerHealth = player.GetComponentInParent<PlayerHealth>();
        }

        // Cache PlayerController.
        if (_playerController == null)
        {
            _playerController = player.GetComponentInParent<PlayerController>();
        }

        if (_playerHealth == null)
        {
            Debug.LogWarning("Player does not have PlayerHealth.");

            return;
        }

        if (_playerController == null)
        {
            Debug.LogWarning("Player does not have PlayerController.");

            return;
        }

        _cooldownTimer = attackCooldown;
        
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        Debug.Log
        (
            $"{name} attacked at {Time.time:F2}, " +
            $"Damage: {damage}, Cooldown: {attackCooldown}"
        );

        // Deal damag
        _playerHealth.TakeDamage(damage);

        // Calculate direction from enemy to player
        Vector2 knockbackDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;

        // Apply knockback
        _playerController.ApplyKnockback
        (
            knockbackDirection,
            knockbackForce
        );
        
        AkUnitySoundEngine.PostEvent(attackEvent.Id, gameObject);
    }
}