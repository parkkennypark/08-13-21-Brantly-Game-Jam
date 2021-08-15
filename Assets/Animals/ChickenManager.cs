using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenManager : MonoBehaviour
{
    public GameObject chickenPrefab;
    public Transform chickenSpawnTransform;
    public int maxChickens = 4;

    public float spawnDelay;
    public float spawnDelayVariance;

    private float currSpawnDelay;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        currSpawnDelay -= Time.deltaTime;
        if (currSpawnDelay <= 0)
        {
            SpawnChicken();
        }
    }

    void ResetTimer()
    {
        currSpawnDelay = spawnDelay + Random.Range(-spawnDelayVariance, spawnDelayVariance);
    }

    void SpawnChicken()
    {
        if (GameObject.FindObjectsOfType<Chicken>().Length >= maxChickens)
        {
            return;
        }
        Instantiate(chickenPrefab, chickenSpawnTransform.position, chickenSpawnTransform.rotation);
        ResetTimer();
    }
}
