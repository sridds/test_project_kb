using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GreatPerformanceHelper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _letters;

    [SerializeField]
    private float _lifeTime = 1.0f;

    private bool flag;

    private void Start()
    {
        StartCoroutine(IPlayAnimation());
    }

    private void Update()
    {
        if (flag) return;

        for (int i = 0; i < _letters.Length; i++)
        {
            _letters[i].transform.parent.localPosition = new Vector3(_letters[i].transform.parent.localPosition.x, Mathf.Sin((Time.time * 4.0f) + i) * 0.05f);
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

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(_lifeTime);
        flag = true;

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
