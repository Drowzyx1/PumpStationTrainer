using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class SparkEffects : MonoBehaviour
{
    [SerializeField] private MistakeTracker mistakes;
    [SerializeField] private ParticleSystem sparkPrefab;
    [SerializeField] private Transform spawnPoint;

    private ObjectPool<ParticleSystem> _pool;

    void Awake()
    {
        _pool = new ObjectPool<ParticleSystem>(
            createFunc: () => Instantiate(sparkPrefab),
            actionOnGet: p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            defaultCapacity: 5);
    }

    void OnEnable() => mistakes.OnMistake += PlaySparks;
    void OnDisable() => mistakes.OnMistake -= PlaySparks;

    private void PlaySparks(string _)
    {
        ParticleSystem p = _pool.Get();
        p.transform.position = spawnPoint.position;
        p.Play();
        StartCoroutine(ReleaseWhenDone(p));
    }

    private IEnumerator ReleaseWhenDone(ParticleSystem p)
    {
        yield return new WaitWhile(() => p.IsAlive(true));
        _pool.Release(p);
    }
}
