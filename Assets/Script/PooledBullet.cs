using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    private float _disableAt;

    private void OnEnable()
    {
        _disableAt = Time.time + lifeTime;
    }

    private void Update()
    {
        if (Time.time >= _disableAt)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
