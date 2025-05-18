using UnityEngine;
using DG.Tweening;

public class FieldEnemy : MonoBehaviour
{
    private const float OFFSET = 0.1f;
    private const float TRAIL_FREQUENCY = 2f;
    private const float TRAIL_AMPLITUDE = 0.1f;

    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private Sprite _shockedSprite;

    private SpriteRenderer[] _hostileRenderers;
    private Color hostileColor = new Color(1, 0, 0, 0.4f);

    private void Start()
    {
        _hostileRenderers = new SpriteRenderer[4];

        for(int i = 0; i < 4; i++)
        {
            float xOffset = ((i % 2) * 2 - 1) * OFFSET;
            float yOffset = ((i / 2) * 2 - 1) * OFFSET;

            _hostileRenderers[i] = CreateTrail(new Vector3(xOffset, yOffset));
        }
    }

    private SpriteRenderer CreateTrail(Vector3 offset)
    {
        GameObject go = new GameObject("Trail");
        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        go.transform.position = transform.position + offset;
        go.transform.SetParent(transform);
        renderer.color = hostileColor;

        return renderer;
    }

    private void Update()
    {

        for(int i = 0; i < _hostileRenderers.Length; i++)
        {
            int mult = 1;

            if (i % 2 == 0) mult *= -1;

            float xOffset = ((i % 2) * 2 - 1) * OFFSET;
            float yOffset = ((i / 2) * 2 - 1) * OFFSET;

            _hostileRenderers[i].sprite = _renderer.sprite;
            _hostileRenderers[i].transform.localPosition = new Vector3(Mathf.Cos((Time.time * TRAIL_FREQUENCY + i)) * TRAIL_AMPLITUDE * mult, Mathf.Sin((Time.time * TRAIL_FREQUENCY) + i) * TRAIL_AMPLITUDE) + new Vector3(xOffset, yOffset);
        }
    }

    private void Encounter()
    {
        _renderer.sprite = _shockedSprite;
        _renderer.transform.DOShakePosition(0.3f, new Vector3(0.4f, 0.0f), 40).SetEase(Ease.OutQuad);

        FindFirstObjectByType<BattleManager>().StartBattle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Leader") return;

        Encounter();
    }
}
