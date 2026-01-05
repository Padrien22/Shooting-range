using UnityEngine;
using UnityEngine.InputSystem;

public class VRGunShooter : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty triggerAction;

    [Header("Tir")]
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 0.1f;

    [Header("Effets visuels")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem muzzleFlash2;

    private bool _wasPressed = false;
    private float _lastShotTime = 0f;

    private void OnEnable()
    {
        if (triggerAction.action != null)
            triggerAction.action.Enable();
    }


    private void OnDisable()
    {
        if (triggerAction.action != null)
            triggerAction.action.Disable();
    }


    private void Update()
    {
        if (triggerAction == null || triggerAction.action == null) return;

        float value = triggerAction.action.ReadValue<float>();
        bool isPressed = value > 0.9f;

        if (isPressed && !_wasPressed && Time.time - _lastShotTime >= fireCooldown)
        {
            Shoot();
            _lastShotTime = Time.time;
        }

        _wasPressed = isPressed;
    }

    private void Shoot()
    {
        if (firePoint == null) return;
        if (BulletPool.Instance == null)
        {
            Debug.LogError("BulletPool.Instance est null : ajoute BulletPool dans la scène.");
            return;
        }

        GameObject bullet = BulletPool.Instance.GetBullet(firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash.Play();
        }

        if (muzzleFlash2 != null)
        {
            muzzleFlash2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash2.Play();
        }
    }
}
