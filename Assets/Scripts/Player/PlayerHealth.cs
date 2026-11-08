using UnityEngine;
using System;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private PlayerController  _playerController;
    public event Action<float> OnHealthChanged;
    [SerializeField] private AK.Wwise.Event hitEvent;
    [SerializeField] private AK.Wwise.Event deathEvent;

    private float _health;
    private bool _isDead;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _health = maxHealth;
        OnHealthChanged?.Invoke(_health);
    }

    private void Update()
    {
        // Only For Test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(10f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Heal(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.LogWarning("Player HP is: " + _health);
        if (_isDead)
        {
            return;
        }

        _health = Mathf.Max(0f, _health - damage);

        OnHealthChanged?.Invoke(_health);

        AkUnitySoundEngine.PostEvent(hitEvent.Id, gameObject);
        
        if (_health <= 0f)
        {
            Die();
        }
    }

    public void Heal(float heal)
    {
        if (_isDead)
        {
            return;
        }

        _health = Mathf.Min(_health + heal, maxHealth);

        OnHealthChanged?.Invoke(_health);
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        WinConditionManager manager = FindFirstObjectByType<WinConditionManager>();

        if (manager != null)
        {
            manager.PlayerDefeated();
        }
        _playerController.SetIsAlive(false);
        AkUnitySoundEngine.PostEvent(deathEvent.Id, gameObject);
        
        Debug.Log("Player died!");

        // TODO:Disable movement, Play death animation, Show lose screen.
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return _health;
    }
}
