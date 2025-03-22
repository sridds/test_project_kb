using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.UIElements;

namespace Hank.Battles
{
    public class BaseAttack : UnitAttack
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private SpriteRenderer _renderer;

        [SerializeField]
        private string _walkHash = "Walk";

        [SerializeField]
        private string _windupHash = "Windup";

        [SerializeField]
        private string _missHash = "Miss";

        [SerializeField]
        private string _swingHash = "Swing";

        [SerializeField]
        private float _attackWindow = 0.5f;

        [SerializeField]
        private float _walkUpTime = 0.8f;

        private float attackWindowTimestamp;
        private bool attackWindowOpen;
        private bool swingFlag;
        private Vector2 startPos;

        public override EAttackPerformance GetPerformance() => LerpAttackPerformance((Time.time - attackWindowTimestamp) / _attackWindow);

        public override IEnumerator IExecuteAction()
        {
            // Walk
            _animator.Play(_walkHash);

            // Setup start and end pos
            startPos = transform.position;
            Vector2 targetPos = new Vector2(target.transform.position.x - (target.GetSpriteSize().x / 2.0f), target.transform.position.y);

            // Move to position
            _renderer.flipX = false;
            yield return transform.DOMove(targetPos, _walkUpTime).SetEase(Ease.Linear).WaitForCompletion();

            // Open attack window
            attackWindowOpen = true;
            attackWindowTimestamp = Time.time;
            _animator.Play(_windupHash);

            // Wait and close attack window
            yield return new WaitForSeconds(_attackWindow);
            attackWindowOpen = false;

            // Miss attack
            //BattleHandler.Instance.RegisterMiss(target as EnemyUnit);
            _animator.Play(_missHash);
            currentActionCoroutine = null;

            StartCoroutine(IEndAttack());
        }

        private IEnumerator IEndAttack()
        {
            yield return new WaitForSeconds(0.5f);

            // Move back to starting position
            transform.DOMove(startPos, _walkUpTime).SetEase(Ease.Linear);
            _animator.Play(_walkHash);
            _renderer.flipX = true;
            yield return new WaitForSeconds(_walkUpTime);

            OnActionComplete?.Invoke();
        }

        public override void RegisterInput()
        {
            if (!attackWindowOpen) return;

            // Register hit
            attackWindowOpen = false;
            BattleHandler.Instance.RegisterEnemyHit(unit as PartyMemberUnit, target as EnemyUnit, _attackDamageBonus, GetPerformance());

            _animator.Play(_swingHash);
            StopCoroutine(currentActionCoroutine);
            currentActionCoroutine = null;

            StartCoroutine(IEndAttack());
        }
    }
}
