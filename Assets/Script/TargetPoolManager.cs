using System.Collections.Generic;
using UnityEngine;

public class TargetPoolManager : MonoBehaviour
{
    [Header("Prefabs (Project)")]
    [SerializeField] private List<GameObject> targetPrefabs = new();

    [Header("Pool size")]
    [SerializeField] private int instancesPerPrefab = 1;

    private readonly List<PooledTarget> _all = new();

    private void Awake()
    {

        BuildPool();
    }

    private void BuildPool()
    {
        _all.Clear();

        if (targetPrefabs == null || targetPrefabs.Count == 0)
        {
            Debug.LogError("[TargetPoolManager] No prefabs assigned!", this);
            return;
        }

        int index = 0;
        foreach (var prefab in targetPrefabs)
        {
            if (prefab == null) continue;

            for (int i = 0; i < Mathf.Max(1, instancesPerPrefab); i++)
            {
                var go = Instantiate(prefab);
                go.name = $"{prefab.name}_Pooled_{index}";
                go.SetActive(false);

                var pt = go.GetComponent<PooledTarget>();
                if (pt == null) pt = go.AddComponent<PooledTarget>();

                pt.pool = null;            // on le set juste après
                pt.spawnIndex = index;
                pt.pool = null;            // double sécurité

                // lien pool
                pt.pool = null; // sera set juste en dessous
                pt.pool = null;

                pt.pool = null; // ok on clean, puis:
                pt.pool = null;

                // (vraie assignation)
                pt.pool = this.GetComponentInChildren<TargetSpawnerZone>(true) != null
                    ? null
                    : null;

                // => on met pool = null et le spawner la set quand il active une cible

                _all.Add(pt);
                index++;
            }
        }

        Debug.Log($"[TargetPoolManager] Pool built: {_all.Count} instances.", this);
    }

    /// <summary>Retourne une instance inactive disponible, ou null.</summary>
    public PooledTarget GetInactive()
    {
        for (int i = 0; i < _all.Count; i++)
        {
            if (_all[i] != null && !_all[i].gameObject.activeInHierarchy)
                return _all[i];
        }
        return null;
    }

    /// <summary>Appelé par PooledTarget.ReturnToPool()</summary>
    public void ReturnTarget(PooledTarget target)
    {
        if (target == null) return;
        // Ici on ne respawn pas directement: c’est le spawner qui s’occupe de maintenir 4 actives.
        // On s’assure juste qu’elle est bien inactive.
        target.gameObject.SetActive(false);
    }

    public IReadOnlyList<PooledTarget> All => _all;
}
