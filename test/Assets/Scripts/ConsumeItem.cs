using System.Collections;
using UnityEngine;

namespace Hank.Battles
{
    public class ConsumeItem : UnitAction
    {
        private BattleUnit unit;
        private Consumable consumable;

        [SerializeField]
        private SpriteRenderer _renderer;

        public void Initialize(BattleUnit consumer, Consumable consumable)
        {
            this.unit = consumer;
            this.consumable = consumable;
        }

        public override IEnumerator IExecuteAction()
        {
            yield return null;

            BattleHandler.Instance.RegisterUnitHeal(unit, consumable.HealAmount);

            yield return new WaitForSeconds(1);
            OnActionComplete?.Invoke();
        }
    }
}
