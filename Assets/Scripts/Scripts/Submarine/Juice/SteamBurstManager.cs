using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SteamBurstManager : MonoBehaviour
{
    [Header("Steam Burst Prefab")]
    [SerializeField] private GameObject steamBurstPrefab;
    [SerializeField] private float autoDestroyDelay = 3f;

    [Header("Burst Points")]
    [SerializeField] private List<Transform> burstPoints = new List<Transform>();

    [Header("Random Timing")]
    [SerializeField] private float minDelayBetweenWaves = 1f;
    [SerializeField] private float maxDelayBetweenWaves = 4f;

    [Header("Bursts Per Wave")]
    [SerializeField] private int minBurstsPerWave = 1;
    [SerializeField] private int maxBurstsPerWave = 3;
    [Range(0f, 1f)]
    [SerializeField] private float chancePerBurst = 1f;

    [Header("Sound")]
    [SerializeField] private EventReference steamBurstEvent;

    [Header("Control")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

    private Coroutine burstRoutine;

    private void Start()
    {
        if (playOnStart)
            StartBursting();
    }

    public void StartBursting()
    {
        if (burstRoutine == null && steamBurstPrefab != null && burstPoints.Count > 0)
            burstRoutine = StartCoroutine(BurstLoop());
    }

    public void StopBursting()
    {
        if (burstRoutine != null)
        {
            StopCoroutine(burstRoutine);
            burstRoutine = null;
        }
    }

    private IEnumerator BurstLoop()
    {
        do
        {
            float delay = Random.Range(minDelayBetweenWaves, maxDelayBetweenWaves);
            yield return new WaitForSeconds(delay);
            TriggerRandomWave();
        }
        while (loop);

        burstRoutine = null;
    }

    private void TriggerRandomWave()
    {
        if (steamBurstPrefab == null || burstPoints.Count == 0)
            return;

        int burstsThisWave = Random.Range(minBurstsPerWave, maxBurstsPerWave + 1);

        for (int i = 0; i < burstsThisWave; i++)
        {
            Transform point = burstPoints[Random.Range(0, burstPoints.Count)];
            if (point == null || !point.gameObject.activeInHierarchy)
                continue;

            if (Random.value <= chancePerBurst)
                SpawnBurstAt(point);
        }
    }

    private void SpawnBurstAt(Transform point)
    {
        GameObject instance = Instantiate(steamBurstPrefab, point.position, point.rotation);

        if (autoDestroyDelay > 0f)
            Destroy(instance, autoDestroyDelay);

        if (!steamBurstEvent.IsNull)
            RuntimeManager.PlayOneShot(steamBurstEvent, point.position);
    }

    [ContextMenu("Test One Burst Wave")]
    private void TestOneBurstWave()
    {
        TriggerRandomWave();
    }
}
