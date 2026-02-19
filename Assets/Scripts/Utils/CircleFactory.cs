using UnityEngine;

public static class CircleFactory
{
    private static Sprite _circleSprite;

    public static Sprite GetCircleSprite()
    {
        if (_circleSprite != null) return _circleSprite;

        int res = 64;
        Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;

        float center = res / 2f;
        float radius = center - 1f;
        Color[] pixels = new Color[res * res];

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);

                if (dist <= radius)
                {
                    float edge = Mathf.Clamp01((radius - dist) * 2f);
                    pixels[y * res + x] = new Color(1, 1, 1, edge);
                }
                else
                {
                    pixels[y * res + x] = Color.clear;
                }
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        _circleSprite = Sprite.Create(
            tex,
            new Rect(0, 0, res, res),
            Vector2.one * 0.5f,
            res
        );

        return _circleSprite;
    }

    public static GameObject CreateCircle(string name, Color color, float radius, Vector2 position)
    {
        GameObject go = new GameObject(name);
        go.transform.position = new Vector3(position.x, position.y, 0);
        go.transform.localScale = Vector3.one * radius * 2f;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite();
        sr.color = color;

        return go;
    }

    public static GameObject CreateCircleWithPhysics(
        string name, Color color, float radius, Vector2 position,
        bool isTrigger = false, bool isKinematic = false)
    {
        GameObject go = CreateCircle(name, color, radius, position);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = isTrigger;
        col.radius = 0.5f;

        return go;
    }
}
