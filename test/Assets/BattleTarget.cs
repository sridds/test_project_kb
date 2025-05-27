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

    [Header("Audio")]
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _tick;

    private float multiplier;
    private float incrementTimer;
    private float cursorFlickerTimer;
    private int cursorFlag;
    private int incrementFlag;
    float timeAccumulator;

    private void Start()
    {
        multiplier = _targetFocusTime;

        // Prepare target
        _targetRenderer.color = new Color(_targetRenderer.color.r, _targetRenderer.color.g, _targetRenderer.color.b, 0.0f);
        _targetRenderer.DOFade(1.0f, _targetFocusTime);

        RandomizeCursor();
    }

    void Update()
    {
        if (multiplier > 0.0f) multiplier -= Time.deltaTime;
        else multiplier = 0.0f;

        // Use a separate time accumulator that continues even as frequency changes
        timeAccumulator += Time.deltaTime * Mathf.Lerp(0.0f, 25.0f, multiplier / _targetFocusTime);

        float x = Mathf.Cos(timeAccumulator) * multiplier;
        float y = Mathf.Sin(timeAccumulator) * multiplier;
        _targetRenderer.transform.position = new Vector3(x, y);
        _targetRenderer.transform.eulerAngles = _targetRenderer.transform.eulerAngles + new Vector3(0, 0, _targetRotationSpeed * Time.deltaTime);

        UpdateCursorFlicker();
        UpdateCursorPosition();
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

            if(incrementFlag % 2 != 0) _source.PlayOneShot(_tick);
        }

        x += _cursorHolder.transform.localPosition.x;
        y += _cursorHolder.transform.localPosition.y;

        x = Mathf.Round(x / _snapIncrement) * _snapIncrement;
        y = Mathf.Round(y / _snapIncrement) * _snapIncrement;

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
