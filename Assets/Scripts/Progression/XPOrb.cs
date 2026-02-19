using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpValue = 10;
    public float attractRadius = 3f;
    public float attractSpeed = 8f;
    public float lifetime = 30f;

    private Transform player;

    void Start()
    {
        Destroy(gameObject, lifetime);

        if (GameManager.Instance?.Player != null)
            player = GameManager.Instance.Player.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < attractRadius)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            float speed = attractSpeed * (1f + (attractRadius - dist) / attractRadius);
            transform.Translate(dir * speed * Time.deltaTime);
        }
    }
}
