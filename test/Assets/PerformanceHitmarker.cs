using UnityEngine;
using TMPro;
using DG.Tweening;

public class PerformanceHitmarker : MonoBehaviour
{
    [SerializeField]
    private float _lifetime = 1.0f;

    [SerializeField]
    private TextMeshPro _text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, _lifetime);

        float randomX = Random.Range(-0.5f, 0.5f);
        float y = 2.0f;
        _text.transform.localScale = new Vector3(1.6f, 1.6f);
        _text.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutCubic);

        _text.transform.DOLocalMove(new Vector3(randomX, y), 0.6f).SetEase(Ease.OutCubic);
        _text.transform.DOScaleY(1.2f, 0.2f).SetDelay(0.4f);
        _text.transform.DOScaleX(0.0f, 0.2f).SetDelay(0.4f);
    }

    public void SetText(string val)
    {
        _text.text = val;
    }
}
