using DG.Tweening;
using UnityEngine;
using System.Collections;

public class OKPerformanceHelper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _letters;

    [SerializeField]
    private float _lifeTime = 1.0f;

    private void Start()
    {
        StartCoroutine(IPlayAnimation());
    }

    void Update()
    {
        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.localPosition = new Vector3(Mathf.Cos((Time.time * 4.0f) + i) * 0.05f, Mathf.Sin((Time.time * 4.0f) + i) * 0.05f);
        }
    }

    private IEnumerator IPlayAnimation()
    {
        foreach (SpriteRenderer renderer in _letters)
        {
            renderer.transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.parent.gameObject.SetActive(true);
            _letters[i].transform.localScale = Vector3.one * 1.5f;
            _letters[i].transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutQuad);
        }

        yield return new WaitForSeconds(_lifeTime);

        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.parent.DOScaleY(1.6f, 0.3f);
            _letters[i].transform.parent.DOScaleX(0.4f, 0.3f);
            _letters[i].transform.parent.DOLocalMoveY(1.5f, 0.5f);
            _letters[i].DOFade(0.0f, 0.3f);

            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
