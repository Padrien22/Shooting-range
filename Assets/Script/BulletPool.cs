using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [Header("Pool")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int prewarmCount = 20;

    private readonly List<GameObject> _pool = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Prewarm(prewarmCount);
    }

    private void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var bullet = CreateNewBullet();
            bullet.SetActive(false);
            _pool.Add(bullet);
        }
    }

    private GameObject CreateNewBullet()
    {
        var bullet = Instantiate(bulletPrefab, transform);
        return bullet;
    }

    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        // 1) priorité aux inactifs
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                var b = _pool[i];
                ResetBullet(b, position, rotation);
                b.SetActive(true);
                return b;
            }
        }

        // 2) sinon, on crée (uniquement si pool vide côté dispo)
        var newBullet = CreateNewBullet();
        _pool.Add(newBullet);
        ResetBullet(newBullet, position, rotation);
        newBullet.SetActive(true);
        return newBullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null) return;
        bullet.SetActive(false);
    }

    private static void ResetBullet(GameObject bullet, Vector3 position, Quaternion rotation)
    {
        bullet.transform.SetPositionAndRotation(position, rotation);
    }
}
