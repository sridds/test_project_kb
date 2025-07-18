using UnityEngine;
using DG.Tweening;

public class BattleTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _targetHolder;
    [SerializeField] private Transform _cursorHolder;
    [SerializeField] private SpriteRenderer _targetRenderer;
    [SerializeField] private SpriteRenderer _cursorRenderer;

    [Header("Modifiers")]
    [SerializeField] private float _targetFocusTime;
    [SerializeField] private float _targetFrequency;
    [SerializeField] private float _targetRotationSpeed;
    [SerializeField] private float _timeTillDeactive = 5.0f;
    [SerializeField] private float _increments = 0.1f;
    [SerializeField] private float _snapIncrement = 0.1f;
    [SerializeField] private float _cursorSpawnRadius = 3.0f;
    [SerializeField] private float _cursorFlickerInterval = 0.05f;
    [SerializeField] private float _preDeathFlickerInterval = 0.05f;
    [SerializeField] private int _flickerAmountBeforeDeath = 5;

    [Header("Audio")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioSource _cursorSource;
    [SerializeField] private AudioClip _tick;

    private float multiplier;
    private float blinkTimer;
    private float incrementTimer;
    private float cursorFlickerTimer;
    private float lifetime;
    private int cursorFlag;
    private int incrementFlag;
    private int flickers;
    float timeAccumulator;

    private void Start()
    {
        multiplier = _targetFocusTime;

        // Prepare target
        _targetRenderer.color = Color.red;
        _targetRenderer.color = new Color(_targetRenderer.color.r, _targetRenderer.color.g, _targetRenderer.color.b, 0.0f);
        _targetRenderer.DOFade(1.0f, _targetFocusTime);
        _targetRenderer.transform.localScale = Vector3.one * 2.0f;
        _targetRenderer.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutQuad);

        RandomizeCursor();
    }

    void Update()
    {
        UpdateLifetime();

        if (multiplier > 0.0f) multiplier -= Time.deltaTime;
        else multiplier = 0.0f;

        _source.pitch = Mathf.Lerp(3, 1, Mathf.Clamp(Vector2.Distance(_cursorHolder.transform.position, _targetRenderer.transform.localPosition) / _cursorSpawnRadius, 0.0f, _cursorSpawnRadius));

        // Use a separate time accumulator that continues even as frequency changes
        timeAccumulator += Time.deltaTime * Mathf.Lerp(0.0f, 35.0f, multiplier / _targetFocusTime);

        // circular motion
        float x = Mathf.Cos(timeAccumulator) * multiplier;
        float y = Mathf.Sin(timeAccumulator) * multiplier;

        // sway to add difficulty
        float xAmount = Mathf.Cos(Time.time * 1.5f) * 2.0f;
        float yAmount = Mathf.Sin(Time.time * 6.0f) * 0.5f;

        // shake when getting close
        Vector2 shake = Random.insideUnitCircle * Mathf.Lerp(0.2f, 0.0f, Mathf.Clamp(Vector2.Distance(_cursorHolder.transform.position, _targetRenderer.transform.localPosition) / 1.0f, 0.0f, 1.0f));

        _targetRenderer.transform.position = new Vector2(x + xAmount, y + yAmount) + shake;
        _targetRenderer.transform.eulerAngles = _targetRenderer.transform.eulerAngles + new Vector3(0, 0, _targetRotationSpeed * Time.deltaTime);

        UpdateCursorFlicker();
        UpdateCursorPosition();
    }

    /*
    public bool CheckForCompletion()
    {
        if (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f) return false;

        // check if cursor is at center
        if (Vector2.Distance(_cursorHolder.transform.position, Vector2.zero) > 0.01f) return false;

        return true;
    }*/

    private void UpdateLifetime()
    {
        lifetime += Time.deltaTime;

        // Blink before disappearing
        if (lifetime >= _timeTillDeactive && flickers < _flickerAmountBeforeDeath)
        {
            blinkTimer += Time.deltaTime;

            if (blinkTimer > _cursorFlickerInterval)
            {
                _targetRenderer.enabled = !_targetRenderer.enabled;
                blinkTimer = 0.0f;
                flickers++;
            }
        }

        // Die after all flickers have been completed
        else if (lifetime > _timeTillDeactive && flickers >= _flickerAmountBeforeDeath)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCursorFlicker()
    {
        cursorFlickerTimer += Time.deltaTime;

        if (cursorFlickerTimer < _cursorFlickerInterval) return;

        cursorFlickerTimer = 0.0f;
        cursorFlag++;

        if (cursorFlag % 2 == 0) _cursorRenderer.color = Color.red;
        else _cursorRenderer.color = Color.white;
    }

    public void UpdateCursorPosition()
    {
        incrementTimer += Time.deltaTime;
        if (incrementTimer < _increments) return;

        incrementTimer = 0.0f;
        float x = Input.GetAxisRaw("Horizontal") * _snapIncrement;
        float y = Input.GetAxisRaw("Vertical") * _snapIncrement;

        if (x != 0 || y != 0)
        {
            incrementFlag++;

            if (incrementFlag % 2 != 0) _cursorSource.PlayOneShot(_tick);
        }

        x += _cursorHolder.transform.localPosition.x;
        y += _cursorHolder.transform.localPosition.y;

        x = Mathf.Round(x / _snapIncrement) * _snapIncrement;
        y = Mathf.Round(y / _snapIncrement) * _snapIncrement;

        // don't go beyond reach
        if (x < -_cursorSpawnRadius || y < -_cursorSpawnRadius || x > _cursorSpawnRadius || y > _cursorSpawnRadius)
        {
            return;
        }

        _cursorHolder.transform.localPosition = new Vector3(x, y);
    }

    public void RandomizeCursor()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float x = _cursorSpawnRadius * Mathf.Cos(angle);
        float y = _cursorSpawnRadius * Mathf.Sin(angle);

        x = Mathf.Round(x / _snapIncrement) * _snapIncrement;
        y = Mathf.Round(y / _snapIncrement) * _snapIncrement;

        _cursorHolder.transform.localPosition = new Vector3(x, y);
    }
}
