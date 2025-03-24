using System.Collections;
using UnityEngine;

namespace Hank.Battles
{
    public class ConsumeItem : UnitAction
    {
        private BattleUnit unit;
        private BattleUnit target;
        private Consumable consumable;

        [SerializeField]
        private SpriteRenderer _renderer;

        public void Initialize(BattleUnit consumer, BattleUnit target, Consumable consumable)
        {
            this.unit = consumer;
            this.consumable = consumable;
            this.target = target;
        }

        public override IEnumerator IExecuteAction()
        {
            yield return null;

            BattleHandler.Instance.RegisterUnitHeal(target, consumable.HealAmount);

            yield return new WaitForSeconds(1);
            OnActionComplete?.Invoke();
        }
    }
}
