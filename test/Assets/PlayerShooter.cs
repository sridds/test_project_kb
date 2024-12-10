using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public PlayerBullet playerBulletPrefab;
    public float shootInterval;
    public float shootSpeed = 7.0f;
    public Vector2 shootPos = new Vector2(0.0f, 0.6f);

    private float shootTimer;

    private void Update()
    {
        // Can we shoot?
        if(shootTimer >= shootInterval)
        {
            // Are we holding the key?
            if (Input.GetKey(KeyCode.X))
            {
                // FIRE IN THE HOLE!!
                Shoot();

                // make sure the timer resets or else we'll be firing every second and your game will crash... derp...
                shootTimer = 0.0f;
            }
        }

        shootTimer += Time.deltaTime;
    }

    /// <summary>
    /// Handle the shoot functionality
    /// </summary>
    private void Shoot()
    {
        // create bullet
        PlayerBullet bullet = Instantiate(playerBulletPrefab, new Vector3(transform.position.x + shootPos.x, transform.position.y + shootPos.y), Quaternion.identity);

        /// HEY LOOK HERE this might cause an error. replace linear velocity with velocity its a unity 6 thing derp...
        bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.0f, shootSpeed);
    }
}
