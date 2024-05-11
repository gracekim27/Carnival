using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; // The enemy prefab to spawn
    [SerializeField] private float baseSpawnInterval; // Base time between each spawn
    [SerializeField] private int baseMaxEnemies; // Base maximum number of enemies alive at the same time

    private float spawnInterval; // Dynamic spawn interval
    private int maxEnemies; // Dynamic maximum number of enemies
    private float timer; // To keep track of time since last spawn
    private int currentEnemyCount; // To keep track of the number of enemies currently spawned

    void Start()
    {
        spawnInterval = baseSpawnInterval;
        maxEnemies = baseMaxEnemies;
        UpdateDifficultySettings();
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentEnemyCount < maxEnemies)
        {
            if (ShopManager.Instance.GetDifficultyScore() > 5)
            {
                SpawnEnemyAtRandomLocation();
            }
            else
            {
                SpawnEnemyAtFixedLocation();
            }
            timer = 0;
        }
    }

    void SpawnEnemy(Vector3 position)
    {
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
        if (enemyScript != null)
        {
            enemyScript.spawner = this;  // Set this spawner as the enemy's spawner
        }
        currentEnemyCount++;
    }
    void SpawnEnemyAtFixedLocation()
    {
        SpawnEnemy(transform.position);
    }

    void SpawnEnemyAtRandomLocation()
    {
        Vector3 spawnPosition = ChooseRandomLocation();
        SpawnEnemy(spawnPosition);
    }
    Vector3 ChooseRandomLocation()
    {
        float rand = Random.value;
        if (rand > 0.7f) // 30% chance to spawn at the top edge
        {
            float randomX = Random.Range(0f, Screen.width-200);
            Vector3 screenPosition = new Vector3(randomX, Screen.height, 0);
            return Camera.main.ScreenToWorldPoint(screenPosition) + new Vector3(0, 0, 10); // Adjust Z to match your game setup
        }
        else if (rand < 0.3f) // 30% chance to spawn at the right edge
        {
            float randomY = Random.Range(0f, Screen.height-200);
            Vector3 screenPosition = new Vector3(Screen.width, randomY, 0);
            return Camera.main.ScreenToWorldPoint(screenPosition) + new Vector3(0, 0, 10); // Adjust Z to match your game setup
        }
        else
        {
            return transform.position;
        }
    }

    public void EnemyDestroyed()
    {
        currentEnemyCount--;  // Decrease the count when an enemy is destroyed
        currentEnemyCount = Mathf.Max(0, currentEnemyCount);  // Ensure count doesn't go negative
    }

    private void UpdateDifficultySettings()
    {
        if (ShopManager.Instance != null)
        {
            int difficultyScore = ShopManager.Instance.GetDifficultyScore(); // Assume this method calculates and returns the difficulty score

            // Scale maxEnemies based on difficulty score
            maxEnemies = baseMaxEnemies + (difficultyScore * 2); // Increase max enemies as difficulty increases

            // Decrease spawn interval based on difficulty score
            spawnInterval = Mathf.Max(0.5f, baseSpawnInterval - (difficultyScore - 3) * 0.1f); // Faster spawns with higher difficulty
        }
    }
}
