using UnityEngine;
using System;

public abstract class UnitAttack : MonoBehaviour
{
    public Action OnAttackEnded;

    public abstract void Execute(BattleUnit unit, BattleUnit target);

    public virtual void RegisterInput() { }
}

public enum EAttackPerformance
{
    OK,
    GOOD,
    GREAT,
    EXCELLENT,
    PERFECT
}