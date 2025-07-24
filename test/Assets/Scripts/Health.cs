using UnityEngine;

public class Health
{
    private int currentPercent;
    private float distance;

    #region Accessors
    public float Distance => distance;
    public int CurrentPercent => currentPercent;
    public bool IsDead { get { return Distance <= 0; } }
    #endregion

    #region Events
    public delegate void HealthUpdated(int oldHealth, int newHealth);
    public HealthUpdated OnHealthUpdated;

    public delegate void HealthDepleted();
    public HealthDepleted OnHealthDepleted;
    #endregion

    public Health()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;

        currentPercent += damageAmount;

        OnHealthUpdated?.Invoke(currentPercent - damageAmount, currentPercent);
    }

    public void Heal(int healAmount)
    {
        currentPercent -= healAmount;
        currentPercent = (int)Mathf.Clamp(currentPercent, 0, Mathf.Infinity);

        OnHealthUpdated?.Invoke(currentPercent + healAmount, currentPercent);
    }
}