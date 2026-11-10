using UnityEngine;

public enum FireType
{
    Automatic
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private float fireRate; //Bullets x min
    [SerializeField] private float range;
    
    [SerializeField] private Bullet projectilePrefab;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private FireType fireType;
    
    [SerializeField] private float magazineSize;
    [SerializeField] private float reloadTime;
    
    [SerializeField] private AK.Wwise.Event shootEvent;
    [SerializeField] private AK.Wwise.Event reloadEvent;

    public Bullet GetProjectile()
    {
        return projectilePrefab;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
    
    public float GetDamage()
    {
        return damage;
    }

    public float FireRate()
    {
        return fireRate;
    }

    public FireType GetFireType()
    {
        return fireType;
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }
    
    public float GetMagazineSize()
    {
        return magazineSize;
    }

    public AK.Wwise.Event GetShootEventSound()
    {
        return shootEvent;
    }
    
    public AK.Wwise.Event GetReloadEventSound()
    {
        return reloadEvent;
    }
}
