using UnityEngine;

public class Health
{
    private int currentHealth;
    private int maxHealth;

    #region Accessors
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead { get { return CurrentHealth <= 0; } }
    #endregion

    #region Events
    public delegate void HealthUpdated(int oldHealth, int newHealth);
    public HealthUpdated OnHealthUpdated;

    public delegate void HealthDepleted();
    public HealthDepleted OnHealthDepleted;
    #endregion

    public Health(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            OnHealthDepleted?.Invoke();
        }

        OnHealthUpdated?.Invoke(currentHealth + damageAmount, currentHealth);
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth %= maxHealth;

        OnHealthUpdated?.Invoke(currentHealth - healAmount, currentHealth);
    }
}