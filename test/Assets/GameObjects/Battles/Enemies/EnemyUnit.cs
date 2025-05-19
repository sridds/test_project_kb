using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    protected UnitFlasher _flasher;

    [Header("Settings")]
    [SerializeField]
    protected Stats _myStats;

    protected Health myHealth;
    public Stats MyStats { get { return _myStats; } }
    public Health MyHealth { get { return myHealth; } }

    private void Awake()
    {
        // Initialize
        myHealth = new Health(_myStats.MaxHP);

        myHealth.OnHealthUpdated += HealthUpdate;
    }

    private void HealthUpdate(int oldHealth, int newHealth)
    {
        if(oldHealth > newHealth)
        {
            OnDamaged(oldHealth - newHealth);
        }
    }

    protected virtual void OnDamaged(int damage) { }
}

public class EnemyUnit : Unit
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [Header("Basic Animation")]
    [SerializeField]
    private Sprite _idleSprite;

    [SerializeField]
    private Sprite _hurtSprite;

    [SerializeField]
    private float _shakeFrequency = 6.0f;

    float timer;

    private void Start()
    {
        timer = Random.Range(0.0f, 10.0f);
    }

    protected override void OnDamaged(int damage)
    {
        StopAllCoroutines();
        StartCoroutine(ITakeHit());
    }

    private void Update()
    {
        // shake left and right like in off

        timer += Time.deltaTime;

        float val = Mathf.Sign(Mathf.Sin(timer * _shakeFrequency)) * 0.03f;
        val += 0.03f;

        _renderer.transform.localPosition = new Vector3(val, 0.0f);
    }

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
