using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;

public class BaseAttack : PartyMemberAttack
{
    [SerializeField]
    private Animator _animator;

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
    private Coroutine attackCoroutine;

    public override void Execute()
    {
        attackCoroutine = StartCoroutine(IAttack());
    }

    private IEnumerator IAttack()
    {
        _animator.Play(_walkHash);
        startPos = transform.position;
        Vector2 targetPos = new Vector2(target.transform.position.x - (target.GetSpriteSize().x / 2.0f), target.transform.position.y - (target.GetSpriteSize().y / 2.0f));

        transform.DOMove(targetPos, _walkUpTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(_walkUpTime);

        OpenAttackWindow();
        attackWindowTimestamp = Time.time;
        _animator.Play(_windupHash);
        yield return new WaitForSeconds(_attackWindow);
        CloseAttackWindow();

        _animator.Play(_missHash);
        attackCoroutine = null;

        StartCoroutine(IEndAttack());
    }

    private IEnumerator IEndAttack()
    {
        yield return new WaitForSeconds(0.5f);

        transform.DOMove(startPos, _walkUpTime).SetEase(Ease.Linear);
        _animator.Play(_walkHash);
        yield return new WaitForSeconds(_walkUpTime);

        OnAttackEnded?.Invoke();
    }

    public override void RegisterInput()
    {
        if (attackWindowOpen)
        {
            attackWindowOpen = false;

            float performanceValueRaw = Mathf.Lerp(0, Enum.GetNames(typeof(EAttackPerformance)).Length - 1, (Time.time - attackWindowTimestamp) / _attackWindow);
            EAttackPerformance attackPerformance = (EAttackPerformance)Mathf.RoundToInt(performanceValueRaw);
            BattleHandler.Instance.RegisterEnemyHit(unit as PartyMemberUnit, target as EnemyUnit, attackPerformance);

            _animator.Play(_swingHash);

            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            StartCoroutine(IEndAttack());
        }
    }

    public void OpenAttackWindow()
    {
        attackWindowOpen = true;
    }

    public void CloseAttackWindow()
    {
        attackWindowOpen = false;
    }
}
