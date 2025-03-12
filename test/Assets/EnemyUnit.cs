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

    public override void HandleAttack(int attackIndex, BattleUnit target)
    {

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
