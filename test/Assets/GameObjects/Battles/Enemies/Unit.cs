using UnityEngine;
public class Unit : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    protected UnitFlasher _flasher;

    [Header("Settings")]
    [SerializeField]
    protected Stats _myStats;

    protected Health myHealth;
    public Stats MyStats { get { return _myStats; } }
    public Health MyHealth { get { return myHealth; } }

    private void Awake()
    {
        // Initialize
        myHealth = new Health(_myStats.MaxHP);

        myHealth.OnHealthUpdated += HealthUpdate;
    }

    private void HealthUpdate(int oldHealth, int newHealth)
    {
        if (oldHealth > newHealth)
        {
            OnDamaged(oldHealth - newHealth);
        }
    }

    protected virtual void OnDamaged(int damage) { }
}