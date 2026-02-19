using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public float currentHealth;
    public int killCount = 0;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 5f;

        EventManager.OnUpgradeSelected += HandleUpgrade;
    }

    void OnDestroy()
    {
        EventManager.OnUpgradeSelected -= HandleUpgrade;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;
        rb.linearVelocity = input * moveSpeed;
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 pos = transform.position;
            pos.z = -10f;
            cam.transform.position = pos;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EventManager.PlayerDied();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            TakeDamage(enemy.contactDamage * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        XPOrb xp = other.GetComponent<XPOrb>();
        if (xp != null)
        {
            EventManager.XPCollected(xp.xpValue);
            Destroy(other.gameObject);
        }
    }

    void HandleUpgrade(UpgradeType type, float value)
    {
        if (type == UpgradeType.MoveSpeed)
        {
            moveSpeed += value;
        }
    }
}
