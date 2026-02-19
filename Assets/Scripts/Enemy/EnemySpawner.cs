using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 1.5f;
    public int maxEnemies = 50;
    public float spawnRadius = 15f;
    public float minSpawnDistance = 8f;

    private float timer;
    private float difficultyTimer;
    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        EventManager.OnEnemyDied += HandleEnemyDied;
    }

    void OnDestroy()
    {
        EventManager.OnEnemyDied -= HandleEnemyDied;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        difficultyTimer += Time.deltaTime;
        enemies.RemoveAll(e => e == null);

        timer += Time.deltaTime;
        if (timer >= spawnInterval && enemies.Count < maxEnemies)
        {
            timer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (GameManager.Instance.Player == null) return;

        Vector2 playerPos = GameManager.Instance.Player.transform.position;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(minSpawnDistance, spawnRadius);
        Vector2 spawnPos = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        GameObject enemy = CircleFactory.CreateCircleWithPhysics(
            "Enemy", Color.red, 0.35f, spawnPos
        );

        float difficultyMultiplier = 1f + (difficultyTimer / 60f) * 0.5f;

        EnemyAI ai = enemy.AddComponent<EnemyAI>();
        ai.maxHealth = 30f * difficultyMultiplier;
        ai.currentHealth = ai.maxHealth;
        ai.moveSpeed = 2f + (difficultyTimer / 120f);
        ai.contactDamage = 10f * difficultyMultiplier;
        ai.xpValue = Mathf.RoundToInt(10 * difficultyMultiplier);

        enemies.Add(enemy);
    }

    void HandleEnemyDied(Vector2 position, int xpValue)
    {
        GameObject xpOrb = CircleFactory.CreateCircleWithPhysics(
            "XPOrb", Color.green, 0.15f, position, isTrigger: true, isKinematic: true
        );
        XPOrb orb = xpOrb.AddComponent<XPOrb>();
        orb.xpValue = xpValue;
    }
}
