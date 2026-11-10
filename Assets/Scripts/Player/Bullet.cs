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

    [SerializeField] private AK.Wwise.Event missEvent;

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
    BasicEnemyBehaviour basicEnemy =
        other.GetComponentInParent<BasicEnemyBehaviour>();

    ChargerEnemyBehaviour chargerEnemy =
        other.GetComponentInParent<ChargerEnemyBehaviour>();

    if (basicEnemy != null)
    {
        basicEnemy.TakeDamage(damage, _sourcePosition);
    }
    else if (chargerEnemy != null)
    {
        chargerEnemy.TakeDamage(damage, _sourcePosition);
    }
    else
    {
        AkUnitySoundEngine.PostEvent(missEvent.Id, gameObject);
    }

    Destroy(gameObject);
}
    
}