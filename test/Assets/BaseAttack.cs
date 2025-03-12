using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BaseAttack : UnitAttack
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

    private bool attackWindowOpen;
    private EnemyUnit target;

    public override void Execute(EnemyUnit target)
    {
        this.target = target;
        StartCoroutine(IAttack());
    }

    private IEnumerator IAttack()
    {
        _animator.Play(_walkHash);
        Vector2 currentPos = transform.position;
        Vector2 targetPos = new Vector2(target.transform.position.x - (target.GetSpriteSize().x / 2.0f), target.transform.position.y - (target.GetSpriteSize().y / 2.0f));

        transform.DOMove(targetPos, _walkUpTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(_walkUpTime);

        _animator.Play(_windupHash);
        yield return new WaitForSeconds(_attackWindow);

        _animator.Play(_swingHash);
        yield return new WaitForSeconds(0.5f);

        transform.DOMove(currentPos, _walkUpTime).SetEase(Ease.Linear);
        _animator.Play(_walkHash);
        yield return new WaitForSeconds(_walkUpTime);

        OnAttackEnded?.Invoke();
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
