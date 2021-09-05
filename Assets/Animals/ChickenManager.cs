using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenManager : MonoBehaviour
{
    public GameObject chickenPrefab;
    public Transform chickenSpawnTransform;
    // public int maxChickens = 4;
    public int[] maxChickens;

    public float[] spawnDelays;
    public float spawnDelayVariance;

    private float currSpawnDelay;

    void Start()
    {
        currSpawnDelay = 1000;
    }

    void OnEnable()
    {
        GameManager.OnGameStart += OnGameStart;
    }

    void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStart;
    }

    void OnGameStart()
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
        currSpawnDelay = spawnDelays[GameManager.instance.currentDay - 1] + Random.Range(-spawnDelayVariance, spawnDelayVariance);
    }

    void SpawnChicken()
    {
        if (GameObject.FindObjectsOfType<Chicken>().Length >= maxChickens[GameManager.instance.currentDay - 1])
        {
            return;
        }
        Instantiate(chickenPrefab, chickenSpawnTransform.position, chickenSpawnTransform.rotation);
        ResetTimer();
    }
}
