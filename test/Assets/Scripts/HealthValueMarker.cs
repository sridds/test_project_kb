using DG.Tweening;
using TMPro;
using UnityEngine;

public class HealthValueMarker : MonoBehaviour
{
    [SerializeField]
    private Color _damageColor = Color.red;

    [SerializeField]
    private Color _healColor = Color.green;

    [SerializeField]
    private float _lifetime = 1.0f;

    [SerializeField]
    private TextMeshPro _text;

    void Start()
    {
        Destroy(gameObject, _lifetime);

        transform.position += new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f));
        float randomX = Random.Range(-0.5f, 0.5f);
        _text.transform.DOLocalMove(new Vector3(randomX, 0.0f), 0.6f).SetEase(Ease.OutCubic);
        _text.transform.DOScaleY(1.2f, 0.2f).SetDelay(_lifetime - 0.4f);
        _text.transform.DOScaleX(0.0f, 0.2f).SetDelay(_lifetime - 0.4f);
    }

    public void Setup(int value, bool isDamage)
    {
        _text.text = $"{value}";
        _text.color = isDamage ? _damageColor : _healColor;
    }
}
