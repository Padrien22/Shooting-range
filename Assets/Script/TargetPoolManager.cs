using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoolManager : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private GameObject targetPrefab; // option si tu veux instancier si besoin
    [SerializeField] private List<PooledTarget> initialTargets = new(); // tes Crates déjà dans la scène

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private bool respawnRandomPoint = true;

    private readonly List<PooledTarget> _pool = new();

    private void Awake()
    {

        foreach (var t in initialTargets)
        {
            if (t == null) continue;
            RegisterTarget(t);
            t.gameObject.SetActive(true);
        }
    }

    private void RegisterTarget(PooledTarget t)
    {
        t.pool = this;
        if (!_pool.Contains(t))
            _pool.Add(t);
    }

    public void ReturnTarget(PooledTarget target)
    {
        StartCoroutine(RespawnAfterDelay(target, respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(PooledTarget target, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Choix du point
        Transform spawn = ChooseSpawnPoint(target);

        // Reset + replacer + réactiver
        target.ResetState();
        target.transform.SetPositionAndRotation(spawn.position, spawn.rotation);
        target.gameObject.SetActive(true);
    }

    private Transform ChooseSpawnPoint(PooledTarget target)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return target.transform; // fallback

        if (!respawnRandomPoint && target.spawnIndex >= 0 && target.spawnIndex < spawnPoints.Length)
            return spawnPoints[target.spawnIndex];

        int idx = Random.Range(0, spawnPoints.Length);
        target.spawnIndex = idx;
        return spawnPoints[idx];
    }

    public PooledTarget GetAvailableOrCreate()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeInHierarchy)
                return _pool[i];
        }

        if (targetPrefab == null) return null;

        var go = Instantiate(targetPrefab);
        var pt = go.GetComponent<PooledTarget>();
        if (pt == null) pt = go.AddComponent<PooledTarget>();
        RegisterTarget(pt);
        go.SetActive(false);
        return pt;
    }
}
