using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public float fireInterval = 0.8f;
    public float projectileSpeed = 10f;
    public float damage = 10f;
    public float projectileSize = 0.15f;
    public int projectileCount = 1;
    public float projectileLifetime = 2f;

    private float timer;

    void Start()
    {
        EventManager.OnUpgradeSelected += HandleUpgrade;
    }

    void OnDestroy()
    {
        EventManager.OnUpgradeSelected -= HandleUpgrade;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            timer = 0;
            Fire();
        }
    }

    void Fire()
    {
        EnemyAI nearest = FindNearestEnemy();
        if (nearest == null) return;

        Vector2 origin = transform.position;
        Vector2 targetPos = nearest.transform.position;
        Vector2 dir = (targetPos - origin).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector2 fireDir = dir;

            if (projectileCount > 1)
            {
                float spreadAngle = 15f;
                float totalSpread = spreadAngle * (projectileCount - 1);
                float startAngle = -totalSpread / 2f;
                float currentAngle = startAngle + spreadAngle * i;

                float rad = currentAngle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                fireDir = new Vector2(
                    dir.x * cos - dir.y * sin,
                    dir.x * sin + dir.y * cos
                );
            }

            SpawnProjectile(origin, fireDir);
        }
    }

    void SpawnProjectile(Vector2 origin, Vector2 direction)
    {
        GameObject proj = CircleFactory.CreateCircleWithPhysics(
            "Projectile", Color.yellow, projectileSize, origin,
            isTrigger: true, isKinematic: true
        );

        Projectile p = proj.AddComponent<Projectile>();
        p.damage = damage;
        p.speed = projectileSpeed;
        p.direction = direction;
        p.lifetime = projectileLifetime;
    }

    EnemyAI FindNearestEnemy()
    {
        EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        EnemyAI nearest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    void HandleUpgrade(UpgradeType type, float value)
    {
        switch (type)
        {
            case UpgradeType.Damage:
                damage += value;
                break;
            case UpgradeType.AttackSpeed:
                fireInterval = Mathf.Max(0.1f, fireInterval - value);
                break;
            case UpgradeType.ProjectileSize:
                projectileSize += value;
                break;
            case UpgradeType.ExtraProjectile:
                projectileCount += (int)value;
                break;
        }
    }
}
