using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawnerZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetPoolManager pool;

    [Header("Spawn settings")]
    [SerializeField] private int targetCount = 4;
    [SerializeField] private float checkEverySeconds = 0.2f;

    [Header("Zone (local)")]
    [SerializeField] private Vector2 sizeXZ = new Vector2(10f, 10f);

    [Header("Grounding")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rayStartHeight = 10f;
    [SerializeField] private float rayLength = 50f;
    [SerializeField] private float yOffset = 0.02f;

    [Header("Anti-overlap")]
    [SerializeField] private float minDistanceBetweenTargets = 1.0f;
    [SerializeField] private int maxAttemptsPerSpawn = 20;

    private readonly List<PooledTarget> _active = new();
    private Coroutine _loop;

    private void OnEnable()
    {
        if (pool == null)
        {
            Debug.LogError("[TargetSpawnerZone] Pool is null", this);
            return;
        }

        _loop = StartCoroutine(Loop());
    }

    private void OnDisable()
    {
        if (_loop != null) StopCoroutine(_loop);
        _loop = null;
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            CleanupActiveList();
            EnsureTargets();
            yield return new WaitForSeconds(checkEverySeconds);
        }
    }

    private void CleanupActiveList()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            if (_active[i] == null || !_active[i].gameObject.activeInHierarchy)
                _active.RemoveAt(i);
        }
    }

    private void EnsureTargets()
    {
        int missing = targetCount - _active.Count;
        for (int i = 0; i < missing; i++)
        {
            TrySpawnOne();
        }
    }

    private void TrySpawnOne()
    {
        var target = pool.GetInactive();
        if (target == null)
        {
            Debug.LogWarning("[TargetSpawnerZone] No inactive target available in pool.", this);
            return;
        }

        if (!TryFindValidSpawn(out Vector3 pos, out Quaternion rot))
        {
            Debug.LogWarning("[TargetSpawnerZone] No valid spawn position found.", this);
            return;
        }

        // prépare l’objet
        target.pool = pool;         // IMPORTANT : maintenant PooledTarget peut ReturnToPool() correctement
        target.ResetState();

        var go = target.gameObject;
        go.transform.SetParent(null);           // optionnel : sinon laisse poolRoot
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        _active.Add(target);
    }

    private bool TryFindValidSpawn(out Vector3 pos, out Quaternion rot)
    {
        rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        for (int attempt = 0; attempt < maxAttemptsPerSpawn; attempt++)
        {
            Vector3 pLocal = new Vector3(
                Random.Range(-sizeXZ.x * 0.5f, sizeXZ.x * 0.5f),
                0f,
                Random.Range(-sizeXZ.y * 0.5f, sizeXZ.y * 0.5f)
            );

            Vector3 origin = transform.TransformPoint(pLocal) + Vector3.up * rayStartHeight;

            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, groundMask, QueryTriggerInteraction.Ignore))
                continue;

            Vector3 candidate = hit.point + Vector3.up * yOffset;

            // anti-overlap (distance minimale entre cibles actives)
            if (minDistanceBetweenTargets > 0f)
            {
                bool tooClose = false;
                for (int i = 0; i < _active.Count; i++)
                {
                    if (_active[i] == null) continue;
                    float d = Vector3.Distance(_active[i].transform.position, candidate);
                    if (d < minDistanceBetweenTargets) { tooClose = true; break; }
                }
                if (tooClose) continue;
            }

            pos = candidate;
            return true;
        }

        pos = default;
        rot = default;
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(sizeXZ.x, 0.1f, sizeXZ.y));
    }
#endif
}
