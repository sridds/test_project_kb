using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Unit
{
    // keeps track of health and basic functionality


    public void SetTargetted(bool target)
    {

    }
}

public class EnemyUnit : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private UnitFlasher _flasher;

    [SerializeField]
    private Sprite _idleSprite;

    [SerializeField]
    private Sprite _hurtSprite;

    private IEnumerator ITakeHit()
    {
        _renderer.transform.DOKill(true);
        _renderer.transform.DOShakePosition(0.2f, new Vector3(0.5f, 0.0f), 45);
        _renderer.sprite = _hurtSprite;
        _flasher.DamageFlash(0.1f);
        yield return new WaitForSeconds(0.3f);
        _renderer.sprite = _idleSprite;
    }
}
