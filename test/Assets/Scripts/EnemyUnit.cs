using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Hank.Battles
{
    public class EnemyUnit : BattleUnit
    {
        [SerializeField]
        private Transform _holder;

        [SerializeField]
        private Sprite _damageSprite;

        [SerializeField]
        private Sprite _defaultSprite;

        [SerializeField]
        private List<UnitAttack> _myAttacks = new List<UnitAttack>();

        private UnitAttack currentAttack;

        public override void StartBash(BattleUnit target)
        {
            // Create attack animation and hide the battle sprite
            SetRendererEnabled(false);

            // Initalize attack
            //UnitAttack attack = Instantiate(_myAttacks[attackIndex], transform.position, Quaternion.identity);
            //attack.Init(this, target);

            //currentAttack = attack;
        }

        protected override void HandleActionEnding()
        {
            Destroy(currentAttack.gameObject);

            // Advance to the next turn
            base.HandleActionEnding();
        }

        public override void HandleHealthUpdate(int oldHealth, int newHealth)
        {
            // Damage Taken
            if (newHealth < oldHealth)
            {
                StartCoroutine(IDamageEffects());
            }
        }

        private IEnumerator IDamageEffects()
        {
            _holder.DOShakePosition(0.4f, new Vector3(0.8f, 0.0f), 45);
            _unitRenderer.sprite = _damageSprite;

            yield return new WaitForSeconds(0.4f);
            _unitRenderer.sprite = _defaultSprite;
        }
    }
}