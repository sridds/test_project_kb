using DG.Tweening;
using UnityEngine;
using System.Collections;

public class HankGameIntro : MonoBehaviour
{
    [SerializeField] private AnnoyingRock _fuckAssRockThatIhate;
    [SerializeField] private GameObject _bulldozerHolder;
    [SerializeField] private GameObject _spotlightHolder;
    [SerializeField] private Transform _bulldozer;
    [SerializeField] private SpriteRenderer _topHalf;
    [SerializeField] private SpriteRenderer _spotlight;
    [SerializeField] private SpriteRenderer _hankSleep;
    [SerializeField] private Sprite _snoreSprite;
    [SerializeField] private Sprite[] _hankOpenEyesSprites;
    [SerializeField] private float _moveAmount = 27.0f;
    [SerializeField] private float _moveDuration = 3.0f;
    [SerializeField] private float _spotlightMoveAmplitude = 0.15f;
    [SerializeField] private float _spotlightMoveFrequency = 3.0f;

    [Header("Audio")]
    [SerializeField] private AudioClip _liftCompleteClip;
    [SerializeField] private AudioClip _crashClip;
    [SerializeField] private AudioClip _introMusic;
    [SerializeField] private AudioClip _snoreClip;
    [SerializeField] private AudioSource _backingUpSource;
    [SerializeField] private AudioSource _carMotorSource;
    [SerializeField] private AudioSource _liftSource;
    [SerializeField] private AudioSource _rumbleSource;
    [SerializeField] private AudioSource _source;

    private bool moveFlag;
    private float timer;
    private Vector3 originPoint;
    private bool spotlightOpen;
    private Sprite normalSprite;

    private void Start()
    {
        spotlightOpen = false;
        originPoint = _spotlight.transform.localPosition;
        _spotlight.transform.localScale = Vector3.zero;
    }


    private void Update()
    {
        if (spotlightOpen) UpdateSpotlight();

        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(IStartIntro());
        }

        timer += Time.deltaTime;

        if(timer > 0.2f)
        {
            timer = 0.0f;

            // me when i use magic numbers for FUN!!!

            _topHalf.transform.localPosition = moveFlag ? new Vector3(_topHalf.transform.localPosition.x, _topHalf.transform.localPosition.y + 0.062f) : new Vector3(_topHalf.transform.localPosition.x, _topHalf.transform.localPosition.y - 0.062f);

            moveFlag = !moveFlag;
        }
    }

    private void UpdateSpotlight()
    {
        float moveX = Mathf.Sin(Time.time * _spotlightMoveFrequency) * _spotlightMoveAmplitude;
        float moveY = Mathf.Cos(Time.time * _spotlightMoveFrequency) * _spotlightMoveAmplitude;

        _spotlight.transform.localPosition = originPoint + new Vector3(moveX, moveY);
    }

    private IEnumerator IStartIntro()
    {
        // start music
        AudioManager.Instance.PlayTrack(_introMusic);

        // open spotlight
        _spotlight.transform.DOScale(1.0f, 1.2f).SetEase(Ease.OutQuad);
        spotlightOpen = true;
        yield return new WaitForSeconds(3.0f);
        normalSprite = _hankSleep.sprite;

        // snore x 1
        _source.PlayOneShot(_snoreClip);
        yield return new WaitForSeconds(0.2f);
        _hankSleep.sprite = _snoreSprite;
        _hankSleep.transform.DOShakePosition(0.7f, new Vector3(0.02f, 0.0f), 45, 90, false, false);
        yield return new WaitForSeconds(0.7f);
        _source.Stop();
        _hankSleep.sprite = normalSprite;

        yield return new WaitForSeconds(3.0f);

        // snore x 2
        _source.PlayOneShot(_snoreClip);
        yield return new WaitForSeconds(0.2f);
        _hankSleep.sprite = _snoreSprite;

        Vector3 formerScale = _hankSleep.transform.localScale;
        _hankSleep.transform.DOScaleX(1.3f, 0.3f);
        _hankSleep.transform.DOShakePosition(0.7f, new Vector3(0.02f, 0.0f), 45, 90, false, false);
        yield return new WaitForSeconds(0.3f);
        _hankSleep.transform.localScale = formerScale;
        _source.Stop();
        _hankSleep.sprite = normalSprite;

        // shake spotlight
        spotlightOpen = false;
        AudioManager.Instance.PauseMusic();
        _source.PlayOneShot(_crashClip);
        _spotlight.transform.DOShakePosition(0.5f, new Vector3(0.2f, 0.0f), 45).SetEase(Ease.OutQuad);

        // lift and rotate (shake slightly)
        yield return new WaitForSeconds(2.0f);

        _rumbleSource.Play();
        _spotlight.transform.DOShakePosition(1.0f, new Vector3(0.15f, 0.0f), 45, 90, false, false);
        yield return new WaitForSeconds(1.0f);

        _rumbleSource.Stop();
        _liftSource.Play();
        _spotlightHolder.transform.parent = _bulldozerHolder.transform;

        float liftTime = 1.0f;
        _bulldozerHolder.transform.DOMoveY(_bulldozerHolder.transform.position.y + 1.0f, liftTime);
        _spotlightHolder.transform.DOLocalRotate(new Vector3(0, 0, 1.5f), liftTime, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(liftTime);
        _liftSource.Stop();
        _bulldozerHolder.transform.DOShakePosition(0.5f, new Vector3(0.3f, 0.0f), 45).SetEase(Ease.OutQuad);
        _source.PlayOneShot(_liftCompleteClip);

        // move da bulldozer
        float startPosX = _bulldozerHolder.transform.position.x;
        _backingUpSource.Play();
        _carMotorSource.Play();
        _carMotorSource.DOPitch(0.8f, 0.5f);
        _carMotorSource.DOFade(0.0f, 2.5f).SetDelay(_moveDuration - 1.0f);
        _backingUpSource.DOFade(0.0f, 2.5f).SetDelay(_moveDuration - 1.0f);
        yield return _bulldozerHolder.transform.DOMoveX(startPosX + _moveAmount, _moveDuration).SetEase(Ease.Linear).WaitForCompletion();

        yield return new WaitForSeconds(1.0f);

        // snore x 1
        _source.PlayOneShot(_snoreClip);
        yield return new WaitForSeconds(0.2f);
        _hankSleep.sprite = _snoreSprite;
        _hankSleep.transform.DOShakePosition(0.7f, new Vector3(0.02f, 0.0f), 45, 90, false, false);
        yield return new WaitForSeconds(0.7f);
        _source.Stop();
        _hankSleep.sprite = normalSprite;

        yield return new WaitForSeconds(3.0f);

        AnnoyingRock fuckAssRock = Instantiate(_fuckAssRockThatIhate);
        fuckAssRock.OnRockHit += StopSnore;
        yield return null;
        fuckAssRock.Fall(_hankSleep.transform, -2.5f, 3.0f);

        yield return new WaitForSeconds(0.3f);
        _source.PlayOneShot(_snoreClip);
        yield return new WaitForSeconds(0.1f);
        _hankSleep.sprite = _snoreSprite;
        _hankSleep.transform.DOShakePosition(0.3f, new Vector3(0.02f, 0.0f), 45, 90, false, false);
        yield return new WaitForSeconds(0.3f);


        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < _hankOpenEyesSprites.Length; i++)
        {
            _hankSleep.sprite = _hankOpenEyesSprites[i];
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void StopSnore()
    {
        _source.Stop();
        _hankSleep.sprite = normalSprite;
    }
}
