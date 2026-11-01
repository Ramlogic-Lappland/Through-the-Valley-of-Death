using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;
    [SerializeField] private UIPause uiPause;

    [Header("Movement")]
    [SerializeField] private float speed = 7.0f;

    [Header("Knockback")]
    [SerializeField] private float knockbackRecovery = 10f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D _rb;
    private bool _gamePasued;

    private Vector2 _movementInput;
    private Vector2 _knockbackVelocity;
    
    private bool _isDashing;
    private Vector2 _dashDirection;
    private Vector2 _lastMoveDirection;

    private float _dashTimeRemaining;
    private float _dashCooldownRemaining;

    private bool _isAlive;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _isAlive = true;
        _gamePasued = false;

        if (uiPause != null)
        {
            uiPause.GameTogglePouse += toggleFreezeInput;
        }
    }

    private void Update()
    {
        if (_gamePasued || !_isAlive)
        {
            _movementInput = Vector2.zero;

            if (animator != null) 
                animator.SetBool("IsWalking", false);

            return;
        }

        HandleInput();
        HandleDashInput();
        HandleRotation();
        HandleShooting();
        HandleReloading();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (_gamePasued || !_isAlive)
        {
            _rb.linearVelocity = Vector2.zero;
            _knockbackVelocity = Vector2.zero;
            return;
        }

        HandleMovement();
        HandleKnockback();
        UpdateDashTimers();
    }

    private void OnDisable()
    {
        if (uiPause != null)
        {
            uiPause.GameTogglePouse -= toggleFreezeInput;
        }
    }

    private void HandleInput()
    {
        _movementInput = new Vector2
        (
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // last movement direction.
        if (_movementInput.sqrMagnitude > 0.01f)
        {
            _lastMoveDirection = _movementInput;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isWalking = !_isDashing && _movementInput.sqrMagnitude > 0.01f;

        animator.SetBool("IsWalking", isWalking);
    }

    private void HandleMovement()
    {
        // Override normal movement during a dash.
        if (_isDashing)
        {
            _rb.linearVelocity = _dashDirection * dashSpeed;
            return;
        }

        Vector2 movementVelocity = _movementInput * speed;

        _rb.linearVelocity = movementVelocity + _knockbackVelocity;
    }

    private void HandleKnockback()
    {
        _knockbackVelocity = Vector2.MoveTowards
        (
            _knockbackVelocity,
            Vector2.zero,
            knockbackRecovery * Time.fixedDeltaTime
        );
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (direction.sqrMagnitude <= 0.01f) return;

        _knockbackVelocity += direction.normalized * force;
    }
    
    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        
        if (_isDashing || _dashCooldownRemaining > 0f) return;

        // use current direction if there is none, use last
        Vector2 direction = _movementInput.sqrMagnitude > 0.01f ? _movementInput : _lastMoveDirection;

    
        if (direction.sqrMagnitude <= 0.01f) return;

        _dashDirection = direction.normalized;

        _isDashing = true;
        _dashTimeRemaining = dashDuration;
        _dashCooldownRemaining = dashCooldown;
    }

    private void UpdateDashTimers()
    {
        if (_dashCooldownRemaining > 0f)
        {
            _dashCooldownRemaining -= Time.fixedDeltaTime;
        }

        if (!_isDashing)
            return;

        _dashTimeRemaining -= Time.fixedDeltaTime;

        if (_dashTimeRemaining <= 0f)
        {
            _isDashing = false;
        }
    }


    private void HandleRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 distance = mousePosition - transform.position;

        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler
        (
            0f,
            0f,
            angle - 90f
        );
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            weapon.Shoot();
        }
    }

    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            weapon.StartReloading();
        }
    }

    private void toggleFreezeInput()
    {
        _gamePasued = !_gamePasued;
    }

    public void SetIsAlive(bool isAlive)
    {
        _isAlive = isAlive;
    }
}