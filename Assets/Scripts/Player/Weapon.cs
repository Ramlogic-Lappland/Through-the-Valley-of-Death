using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData data;
    [SerializeField] private Transform muzzle;
    private FireBehaviour _fireBehaviour;
    private float _reloadTimer;
    private float _roundsInMag;
    private bool _canShoot;
    private bool _isReloading;
    
    private void Awake()
    {
        switch (data.GetFireType())
        {
            case FireType.Automatic:
                _fireBehaviour = gameObject.AddComponent<AutomaticFire>();
                break;
        }
    }

    private void Start()
    {
        _roundsInMag = data.GetMagazineSize();
        _canShoot = true;
        _isReloading = false;
        EventManager.Instance?.RestartUIAmmo();
    }

    private void Update()
    {
        HandleReload();
    }

    public WeaponData GetData()
    {
        return data;
    }

    
    public Transform GetMuzzle()
    {
        return muzzle;
    }

    public void Shoot()
    {
        if (_canShoot)
        {
            _fireBehaviour.Shoot(this);
        }

        if ((int)_roundsInMag == 0)
        {
            _isReloading = true;
        }
    }

    private void HandleReload()
    {
        if ((int)_roundsInMag >= (int)data.GetMagazineSize())
        {
            _isReloading = false;
            _reloadTimer = 0f;
            EventManager.Instance?.ResetReload();
            return;
        }
            
            
        if (!_isReloading)
            return;
        
        _canShoot = false;
        _reloadTimer += Time.deltaTime;
        EventManager.Instance?.StartReload(_reloadTimer, data.GetReloadTime());
        
        if (_reloadTimer >= data.GetReloadTime())
        {
            Reload();
            EventManager.Instance?.ResetReload();
        }
        
    }

    public void StartReloading()
    {
        if (_isReloading || _roundsInMag >= data.GetMagazineSize())
            return;

        _reloadTimer = 0f;
        _isReloading = true;
    }

    private void Reload()
    {
        _roundsInMag = data.GetMagazineSize();
        _reloadTimer = 0f;
        _canShoot = true;
        _isReloading = false;
        EventManager.Instance?.RestartUIAmmo();
        AkUnitySoundEngine.PostEvent(data.GetReloadEventSound().Id, gameObject);
    }
    
    public void DecreaseRounds()
    {
        _roundsInMag--;
    }

    public int GetRoundsInMag()
    {
        return (int)_roundsInMag;
    }
    
}
