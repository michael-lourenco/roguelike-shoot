using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float maxHealth = 30f;
    public float currentHealth;
    public float contactDamage = 10f;
    public int xpValue = 10;

    private Transform target;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 2f;
        rb.mass = 0.5f;

        if (GameManager.Instance?.Player != null)
            target = GameManager.Instance.Player.transform;
    }

    void FixedUpdate()
    {
        if (target == null || GameManager.Instance.State != GameState.Playing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (GameManager.Instance?.Player != null)
                GameManager.Instance.Player.killCount++;

            EventManager.EnemyDied(transform.position, xpValue);
            Destroy(gameObject);
        }
    }
}
