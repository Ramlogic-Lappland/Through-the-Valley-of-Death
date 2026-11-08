using UnityEngine;


public abstract class FireBehaviour : MonoBehaviour
{
    private float _fireCooldown;
    
    protected bool CanFire()
    {
        return _fireCooldown <= 0f;
    }

    protected void UpdateCooldown()
    {
        _fireCooldown = Mathf.Max
        (
            0f,
            _fireCooldown - Time.deltaTime
        );
    }

    protected void SetCooldown(float fireRate)
    {
        _fireCooldown = 60f / fireRate;
    }

    protected float GetFireCooldown()
    {
        return _fireCooldown;
    }
    
    public abstract void Shoot(Weapon weapon);
}
