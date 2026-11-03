using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 1f;

    private Rigidbody2D _rb;
    private float _speed;
    private Vector2 _sourcePosition;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Check where bullet was fired
        _sourcePosition = transform.position;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        _rb.MovePosition(_rb.position + (Vector2)transform.up * _speed * Time.fixedDeltaTime);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BasicEnemyBehaviour enemy =
            other.GetComponentInParent<BasicEnemyBehaviour>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage, _sourcePosition);
        }

        Destroy(gameObject);
    }
    
}