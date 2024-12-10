using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damageAmount = 25;

    private Rigidbody2D rb;

    private void Awake()
    {
        // get the rigidbody
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if the collsiion tag is the enemy, take damage
        if (collision.GetComponent<Enemy>())
        {
            // take damage
            collision.GetComponent<Enemy>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
