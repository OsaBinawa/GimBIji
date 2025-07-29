using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed = 5f;
    private Vector3 direction;
    public float damage = 100f;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}");
        Enemy enemy = collision.GetComponent<Enemy>(); // <- collision, not this
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
