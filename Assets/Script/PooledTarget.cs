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
        ResetState();
        gameObject.SetActive(false);

        if (pool != null)
            pool.ReturnTarget(this);
    }

    public void ResetState()
    {
        foreach (var c in _cols) c.enabled = true;
        foreach (var r in _renderers) r.enabled = true;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero; // sinon _rb.velocity selon ta version
            _rb.angularVelocity = Vector3.zero;
            _rb.Sleep();
            _rb.WakeUp();
        }
    }
}
