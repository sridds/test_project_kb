using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class EnemyUnit : BattleUnit
{
    [SerializeField]
    private Transform _holder;

    [SerializeField]
    private Sprite _damageSprite;

    [SerializeField]
    private Sprite _defaultSprite;

    [SerializeField]
    private UnitFlasher _flasher;

    [SerializeField]
    private List<EnemyAttack> _myAttacks = new List<EnemyAttack>();

    private EnemyAttack currentAttack;

    public override void StartAttack(int attackIndex, BattleUnit target)
    {
        // Create attack animation and hide the battle sprite
        SetRendererEnabled(false);

        // Initalize attack
        EnemyAttack attack = Instantiate(_myAttacks[attackIndex], transform.position, Quaternion.identity);
        attack.Init(this, target);

        currentAttack = attack;
    }

    public override void EndAttack()
    {
        SetRendererEnabled(true);
        Destroy(currentAttack.gameObject);

        // Advance to the next turn
        base.EndAttack();
    }

    public override void HandleHealthUpdate(int oldHealth, int newHealth)
    {
        // Damage Taken
        if (newHealth < oldHealth)
        {
            StartCoroutine(IDamageEffects());
        }
    }

    public void SetTarget(bool isTarget)
    {
        if(isTarget) _flasher.EnableFlash();
        else _flasher.DisableFlashing();
    }

    private IEnumerator IDamageEffects()
    {
        _holder.DOShakePosition(0.4f, new Vector3(0.8f, 0.0f), 45);
        _unitRenderer.sprite = _damageSprite;

        yield return new WaitForSeconds(0.4f);
        _unitRenderer.sprite = _defaultSprite;
    }
}
