using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int MaxHealth = 30;
    private int currentHealth;

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    /// <summary>
    /// Tells the enemy to take some damage
    /// </summary>
    /// <param name="damageAmount"></param>
    public void TakeDamage(int damageAmount)
    {
        // Subtract from health
        currentHealth -= damageAmount;

        /// INSERT SOME DAMAGE SOUND

        // If health is depleted, call the death function
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Tells the EnemySpawner to take itself out of the list and destroy it.
    /// </summary>
    private void Death()
    {
        // Deregister the enemy first
        FindObjectOfType<EnemySpawner>().DeregisterEnemy(this);

        /// INSERT SOME DEATH SOUND HERE
    }

    /// <summary>
    /// All this does is return the current health
    /// </summary>
    /// <returns></returns>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
