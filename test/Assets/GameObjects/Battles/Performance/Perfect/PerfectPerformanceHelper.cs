using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PerfectPerformanceHelper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _letters;

    [SerializeField]
    private float _lifeTime;

    private void Start()
    {
        StartCoroutine(IPlayAnimation());
    }

    private void Update()
    {
        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.parent.localPosition = new Vector3(_letters[i].transform.parent.localPosition.x, Mathf.Sin((Time.time * 7.0f) + i) * 0.1f);
        }
    }

    private IEnumerator IPlayAnimation()
    {
        foreach(SpriteRenderer renderer in _letters)
        {
            renderer.gameObject.SetActive(false);
        }

        for(int i = 0; i < _letters.Length; i++)
        {
            _letters[i].gameObject.SetActive(true);
            _letters[i].transform.localScale = Vector3.one * 2;
            _letters[i].transform.localPosition += new Vector3(-0.5f, -0.2f);
            _letters[i].transform.localEulerAngles = new Vector3(0, 0, -15f);

            _letters[i].transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
            _letters[i].transform.DOLocalMove(_letters[i].transform.localPosition + new Vector3(0.5f, 0.2f), 0.4f).SetEase(Ease.OutBack);
            _letters[i].transform.DOLocalRotate(new Vector3(0, 0, 15f), 0.3f, RotateMode.LocalAxisAdd);

            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(_lifeTime);

        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.DOScaleY(1.6f, 0.3f);
            _letters[i].transform.DOScaleX(0.4f, 0.3f);
            _letters[i].transform.DOLocalMoveY(1.5f, 0.5f);
            _letters[i].DOFade(0.0f, 0.3f);

            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
