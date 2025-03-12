using UnityEngine;
using System;

public abstract class UnitAttack : MonoBehaviour
{
    protected BattleUnit unit;
    protected BattleUnit target;

    public Action OnAttackEnded;

    public void Init(BattleUnit unit, BattleUnit target)
    {
        this.unit = unit;
        this.target = target;

        Execute();
    }

    public abstract void Execute();
}

public abstract class PartyMemberAttack : UnitAttack
{
    public abstract void RegisterInput();
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