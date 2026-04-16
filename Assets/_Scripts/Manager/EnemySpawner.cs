using System.Collections.Generic;
using PurrNet;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private List<EnemyHealth> enemies = new();
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private int maxEnemies = 20;
    [SerializeField] private float spawnInterval = 2f;

    private float actualSpawnInterval => spawnInterval / networkManager.players.Count;
    private List<EnemyHealth> spawnedEnemies = new();
    private float lastSpawnTime;

    override protected void OnSpawned()
    {
        base.OnSpawned();
        enabled = isServer;

        if (isServer)
        {
            EnemyHealth.OnEnemyKilled += OnEnemyKilled;
        }
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();
        EnemyHealth.OnEnemyKilled -= OnEnemyKilled;
    }

    private void Update()
    {
        if (!isServer) { return; }
        if (spawnedEnemies.Count >= maxEnemies) { return; }
        if (lastSpawnTime + actualSpawnInterval > Time.time) { return; }

        lastSpawnTime = Time.time;
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemies.Count <= 0 || spawnPoints.Count <= 0)
        {
            Debug.LogWarning("No enemies or spawn points available for spawning.");
            return;
        }

        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        EnemyHealth enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(enemy);
    }

    private void OnEnemyKilled(EnemyHealth enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
}
