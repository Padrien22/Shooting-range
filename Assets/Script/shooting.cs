using UnityEngine;
using UnityEngine.InputSystem;   // pour InputActionProperty

public class VRGunShooter : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Action liée à la gâchette droite (Activate du contrôleur droit).")]
    public InputActionProperty triggerAction;

    [Header("Tir")]
    public GameObject bulletPrefab;
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
        if (triggerAction != null)
            triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        if (triggerAction != null)
            triggerAction.action.Disable();
    }

    private void Update()
    {
        if (triggerAction == null) return;

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
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = firePoint.up * bulletSpeed;
            // ou rb.velocity selon ta version de Unity
        }

        muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFlash.Play();

        muzzleFlash2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFlash2.Play();

        Destroy(bullet, 5f);
    }
}
