using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Hank.Battles
{
    public abstract class UnitAttack : UnitAction
    {
        protected BattleUnit unit;
        protected BattleUnit target;

        public int AttackDamageBonus => _attackDamageBonus;

        [SerializeField]
        protected int _attackDamageBonus;

        public virtual void Initialize(BattleUnit unit, BattleUnit target)
        {
            this.unit = unit;
            this.target = target;
        }

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

    public enum EAttackPerformance { OK, GOOD, GREAT, EXCELLENT, PERFECT }
}