using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class PerfectPerformanceHelper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _letters;

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
    }
}
