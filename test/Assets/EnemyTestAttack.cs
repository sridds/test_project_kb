using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTestAttack : EnemyAttack
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
    private string _swingHash = "Swing";

    [SerializeField]
    private float _attackWindow = 0.5f;

    [SerializeField]
    private float _walkUpTime = 0.8f;

    public override void Execute()
    {
        StartCoroutine(IAttack());
    }

    private IEnumerator IAttack()
    {
        // Walk
        _animator.Play(_walkHash);

        // Setup start and end pos
        Vector2 startPos = transform.position;
        Vector2 targetPos = new Vector2(target.transform.position.x + (target.GetSpriteSize().x / 2.0f), target.transform.position.y - (target.GetFeetPos()));

        // Move to position
        _renderer.flipX = true;
        yield return transform.DOMove(targetPos, _walkUpTime).SetEase(Ease.Linear).WaitForCompletion();

        // Open attack window
        _animator.Play(_windupHash);
        yield return new WaitForSeconds(_attackWindow);

        _animator.Play(_swingHash);
        yield return new WaitForSeconds(0.5f);

        _animator.Play(_walkHash);
        _renderer.flipX = false;
        yield return transform.DOMove(startPos, _walkUpTime).SetEase(Ease.Linear).WaitForCompletion();

        EndAttack();
    }
}
