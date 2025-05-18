using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GoodPerformanceHelper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] _letters;

    private void Start()
    {
        StartCoroutine(IPlayAnimation());
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
            _letters[i].color = new Color(1, 1, 1, 0);
            _letters[i].transform.eulerAngles = new Vector3(0, 0, 20);
            _letters[i].transform.localScale = Vector3.zero;

            _letters[i].DOFade(1.0f, 0.1f);
            _letters[i].transform.localPosition -= new Vector3(1, 0.0f);
            _letters[i].transform.DOLocalRotate(new Vector3(0, 0, -20), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);
            _letters[i].transform.DOLocalMoveX(0.0f, 0.2f).SetEase(Ease.OutQuad);
            _letters[i].transform.DOScale(1.0f, 0.1f).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(0.03f);
        }

        yield return null;
    }
}
