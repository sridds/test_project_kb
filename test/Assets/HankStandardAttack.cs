using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Rev up car, honk twice, heavy bash into foe!
/// if you miss it just drives off screen and hits something. hank very quickly gets sad at you
/// </summary>
public class HankStandardAttack : MonoBehaviour
{
    // foreach unit helper, set unit helper at position of unit.
    [Header("Modifiers")]
    [SerializeField] private float _attackWindow;

    [Header("References")]
    [SerializeField] private Animator _hankAnimator;
    [SerializeField] private Transform _hankTransform;
    [SerializeField] private Transform _carTransform;
    [SerializeField] private HankCar _car;
    [SerializeField] private GhostTrail _trail;
    [SerializeField] private BattleAnimationHelper _battleAnimationHelper;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _carSprite;
    [SerializeField] private Sprite _hankJumpSprite;
    [SerializeField] private Sprite _hankInCarSprite;
    [SerializeField] private Sprite _hankIdleSprite;
    [SerializeField] private Sprite _hankDisgustedSprite;
    [SerializeField] private Sprite _hankDisgustedSpriteAlt;

    [Header("Audio")]
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private AudioClip _tireScreechClip;
    [SerializeField] private AudioClip _carDoorClip;
    [SerializeField] private AudioClip _carCrashClip;
    [SerializeField] private AudioClip _missCarClip;
    [SerializeField] private AudioClip _carStartClip;
    [SerializeField] private AudioClip _stepSoundClip;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioSource _revvingSource;

    private float attackWindowTimer = 0.0f;
    private float attackWindowTimestamp;
    private bool isAttackWindowOpen;

    private void Update()
    {
        // tick attack window
        if (isAttackWindowOpen)
        {
            attackWindowTimer -= Time.deltaTime;

            // Record timestamp
            if(Input.GetKeyDown(KeyCode.Z))
            {
                attackWindowTimestamp = Time.time;
                CloseAttackWindow();
            }
        }
    }

    // make abstract
    private DamageHelper.EDamagePerformance LerpAttackPerformance(float t)
    {
        int initialValue = 0;
        int targetValue = Enum.GetNames(typeof(DamageHelper.EDamagePerformance)).Length - 1;
        float performanceValueRaw = Mathf.Lerp(targetValue, initialValue, t);

        // Round to nearest attack performane
        DamageHelper.EDamagePerformance attackPerformance = (DamageHelper.EDamagePerformance)Mathf.RoundToInt(performanceValueRaw);

        return attackPerformance;
    }

    private DamageHelper.EDamagePerformance GetPerformance() => LerpAttackPerformance((Time.time - attackWindowTimestamp) / _attackWindow);

    public void Setup(EnemyUnit[] targets)
    {
        StartCoroutine(IExecuteAction(targets));
    }

    private IEnumerator IExecuteAction(EnemyUnit[] targets)
    {
        // Hank turns around and it's super juiced broh
        _renderer.transform.parent.DORotate(new Vector3(0, 0, -5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad);
        yield return _renderer.transform.parent.DOLocalMoveX(0.3f, 0.1f).SetEase(Ease.InQuad).WaitForCompletion();
        _renderer.flipX = true;
        _renderer.transform.parent.DOLocalMoveX(0.0f, 0.1f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.05f);
        _renderer.transform.parent.DORotate(new Vector3(0, 0, 5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);

        // Car slides in
        _trail.enabled = false;
        _source.PlayOneShot(_tireScreechClip);
        yield return _carTransform.DOLocalMoveX(_hankTransform.localPosition.x, 0.4f).SetEase(Ease.OutQuad).WaitForCompletion();

        // Prepare to jump into car
        _source.PlayOneShot(_jumpClip);
        _renderer.flipX = false;
        _renderer.sprite = _hankJumpSprite;

        // Jump into car
        yield return _battleAnimationHelper.StartCoroutine(_battleAnimationHelper.JumpToPosition(new Vector3(_battleAnimationHelper.transform.position.x, _carTransform.position.y), 2.4f));
        yield return null;
        _renderer.sprite = _hankInCarSprite;
        _renderer.sortingOrder = 1;
        _hankTransform.parent = _carTransform;
        _hankTransform.transform.localPosition = new Vector3(0.0f, _hankTransform.transform.localPosition.y);
       
        // Start car and drive towards enemy
        _source.PlayOneShot(_carStartClip);
        _revvingSource.pitch = -3f;
        _revvingSource.DOFade(1.0f, 0.1f);
        _revvingSource.DOPitch(0.7f, 0.3f);

        // Get target
        Vector3 target = targets[0].transform.position;

        // Start revving and drive
        yield return new WaitForSeconds(0.3f);
        _trail.enabled = true;
        _revvingSource.DOPitch(1.5f, 0.7f).SetEase(Ease.InQuad);
        yield return _car.StartCoroutine(_car.INormalDrive(target));
        
        yield return new WaitForSeconds(0.4f);
        OpenAttackWindow();

        yield return new WaitForSeconds(_attackWindow);

        // Hit something
        if(!isAttackWindowOpen)
        {
            _trail.enabled = false;
            _carTransform.DOShakePosition(0.5f, 1.0f, 35);

            FindObjectOfType<DamageHelper>().SpawnPerformanceHitmarker(GetPerformance(), _carTransform.position + new Vector3(0.0f, 3.0f));
            FindObjectOfType<DamageHelper>().DamageChain(FindFirstObjectByType<EnemyUnit>(), 15, target);
            targets[0].MyHealth.TakeDamage(15);

            // Jump out of car
            yield return new WaitForSeconds(0.3f);
        }

        Vector2 jumpOffPoint = target - new Vector3(4.0f, 3.0f);
        _source.PlayOneShot(_jumpClip);
        _renderer.sortingOrder = 3;
        _renderer.sprite = _hankJumpSprite;
        _hankTransform.parent = transform;
        yield return _battleAnimationHelper.StartCoroutine(_battleAnimationHelper.JumpToPosition(jumpOffPoint, 1.5f));
        _renderer.sprite = _hankIdleSprite;

        // Miss
        if (isAttackWindowOpen)
        {
            yield return new WaitForSeconds(0.2f);
            _source.PlayOneShot(_missCarClip);

            // Hank turns around and looks at what he just did
            _renderer.transform.parent.DORotate(new Vector3(0, 0, 5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad);
            yield return _renderer.transform.parent.DOLocalMoveX(-0.3f, 0.1f).SetEase(Ease.InQuad).WaitForCompletion();
            _renderer.sprite = _hankDisgustedSprite;
            _renderer.transform.parent.DOLocalMoveX(0.0f, 0.1f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.05f);
            _renderer.transform.parent.DORotate(new Vector3(0, 0, -5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);

            // Hank moves his eyes back and forth
            yield return new WaitForSeconds(0.4f);
            for (int i = 0; i < 4; i++)
            {
                if(i % 2 == 0)
                {
                    _renderer.sprite = _hankDisgustedSpriteAlt;
                }
                else
                {
                    _renderer.sprite = _hankDisgustedSprite;
                }
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.2f);

            // Hank turns back around
            _renderer.transform.parent.DORotate(new Vector3(0, 0, -5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad);
            yield return _renderer.transform.parent.DOLocalMoveX(0.3f, 0.1f).SetEase(Ease.InQuad).WaitForCompletion();
            _renderer.sprite = _hankIdleSprite;
            _renderer.flipX = true;
            _renderer.transform.parent.DOLocalMoveX(0.0f, 0.1f).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(0.05f);
            _renderer.transform.parent.DORotate(new Vector3(0, 0, 5.0f), 0.15f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);
        }

        _renderer.flipX = true;
        yield return new WaitForSeconds(0.3f);

        // Walk back to start point
        _hankAnimator.Play("Car_Hank_Walk");
        yield return _hankTransform.DOMove(transform.position, 0.7f).SetEase(Ease.Linear).WaitForCompletion();
        _hankAnimator.Play("Car_Hank_Idle");
        _renderer.flipX = false;
    }

    public void Footstep()
    {
        _source.PlayOneShot(_stepSoundClip);
    }

    public void CarShake()
    {
        _source.PlayOneShot(_carDoorClip);
        _carTransform.DOShakePosition(0.4f, new Vector3(0.4f, 0.0f), 35);
    }

    public void OpenAttackWindow()
    {
        attackWindowTimer = _attackWindow;
        isAttackWindowOpen = true;
    }

    public void CloseAttackWindow()
    {
        isAttackWindowOpen = false;
    }
}
