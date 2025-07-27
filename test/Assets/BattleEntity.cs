using NaughtyAttributes;
using UnityEngine;


public class BattleEntity : MonoBehaviour
{
    protected Unit unit;

    #region -Selector Variables-
    [Foldout("Selection Settings")]
    [SerializeField] public Vector2 selectorSize;
    [Foldout("Selection Settings")]
    [SerializeField] public Vector2 selectorCenter;
    #endregion

    #region -Accessors-
    public Unit Unit { get { return unit; } }
    #endregion

    //When instatianted, must be initted
    public void Init(Unit unit)
    {
        this.unit = unit;
    }

    public void TakeDamage(int amount)
    {
        unit.Health.TakeDamage(amount);
    }

    public void HealDamage(int amount)
    {
        unit.Health.Heal(amount);
    }
}
