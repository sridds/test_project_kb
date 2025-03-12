using UnityEngine;
using System;

public abstract class UnitAttack : MonoBehaviour
{
    public Action OnAttackEnded;

    public abstract void Execute(EnemyUnit target);
}