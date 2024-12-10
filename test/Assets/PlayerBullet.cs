using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damageAmount = 25;
    public float lifeTime = 5.0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        // get the rigidbody
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // destroys the object after a certain amount of time
        Destroy(gameObject, lifeTime);
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
