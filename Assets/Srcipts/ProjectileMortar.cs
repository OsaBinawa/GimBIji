using UnityEngine;

public class ProjectileMortar : MonoBehaviour
{
    public float damage = 10f;
    public float explosionRadius = 0.5f;
    public LayerMask targetLayer;
    public GameObject impactEffect;

    private Rigidbody2D rb;
    private bool hasExploded = false;
    private Vector3 targetCenter;
    private bool falling = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        falling = false;
    }

    public void SetTargetCenter(Vector3 position)
    {
        targetCenter = position;
    }

    void Update()
    {
        // Detect when projectile is falling downward
        if (!falling && rb.linearVelocity.y < 0)
        {
            falling = true;
        }

        // Check proximity to target center (explode only in the center)
        if (falling && Vector2.Distance(transform.position, targetCenter) < 0.1f)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayer);
        foreach (var hit in hits)
        {
            IHealth h = hit.GetComponent<IHealth>();
            if (h != null) h.TakeDamage(damage);
        }

        if (impactEffect) Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
