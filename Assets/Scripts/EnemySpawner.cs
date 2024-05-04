using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // The enemy prefab to spawn
    [SerializeField] private float spawnInterval = 2f; // Time between each spawn
    [SerializeField] private int maxEnemies = 5; // Maximum number of enemies alive at the same time

    private float timer; // To keep track of time since last spawn
    private int currentEnemyCount; // To keep track of the number of enemies currently spawned

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime; // Update the timer

        // Check if it's time to spawn a new enemy and if we haven't exceeded the maximum
        if (timer >= spawnInterval && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            timer = 0; // Reset the timer after spawning an enemy
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        currentEnemyCount++; // Increase the count of current enemies
    }
}