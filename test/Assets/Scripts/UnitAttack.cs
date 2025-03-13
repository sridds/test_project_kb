using UnityEngine;
using System;

public abstract class UnitAttack : MonoBehaviour
{
    protected BattleUnit unit;
    protected BattleUnit target;

    public void Init(BattleUnit unit, BattleUnit target)
    {
        this.unit = unit;
        this.target = target;

        Execute();
    }

    public virtual void EndAttack()
    {
        unit.EndAttack();
    }

    public abstract void Execute();
}

public abstract class PartyMemberAttack : UnitAttack
{
    public int AttackDamageBonus => _attackDamageBonus;

    [SerializeField]
    protected int _attackDamageBonus;

    public abstract void RegisterInput();

    // Just in case, this can be overrided
    public abstract EAttackPerformance GetPerformance();

    // This is a standard function that will probably be used a lot
    protected EAttackPerformance LerpAttackPerformance(float t)
    {
        int initialValue = 0;
        int targetValue = Enum.GetNames(typeof(EAttackPerformance)).Length - 1;
        float performanceValueRaw = Mathf.Lerp(initialValue, targetValue, t);

        // Round to nearest attack performane
        EAttackPerformance attackPerformance = (EAttackPerformance)Mathf.RoundToInt(performanceValueRaw);

        return attackPerformance;
    }
}

public abstract class EnemyAttack : UnitAttack
{
}

public enum EAttackPerformance
{
    OK,
    GOOD,
    GREAT,
    EXCELLENT,
    PERFECT
}