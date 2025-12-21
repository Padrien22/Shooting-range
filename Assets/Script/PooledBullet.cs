using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        BulletPool.Instance.ReturnBullet(gameObject);
    }
}
