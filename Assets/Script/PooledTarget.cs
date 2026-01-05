using UnityEngine;

public class PooledTarget : MonoBehaviour
{
    [HideInInspector] public TargetPoolManager pool;
    [HideInInspector] public int spawnIndex = -1;

    private Rigidbody _rb;
    private Collider[] _cols;
    private Renderer[] _renderers;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cols = GetComponentsInChildren<Collider>(true);
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ReturnToPool()
    {
        if (pool == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // reset physique / collisions / visuel
        ResetState();

        // désactive puis rend au pool
        gameObject.SetActive(false);
        pool.ReturnTarget(this);
    }

    public void ResetState()
    {
        // collisions ON
        foreach (var c in _cols) c.enabled = true;

        // visuel ON (si tu changes des matériaux, tu peux rétablir ici)
        foreach (var r in _renderers) r.enabled = true;

        // rigidbody reset
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;     // ou _rb.velocity selon ton Unity
            _rb.angularVelocity = Vector3.zero;
            _rb.Sleep();
            _rb.WakeUp();
        }
    }
}
