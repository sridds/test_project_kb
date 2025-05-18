using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GreatPerformanceHelper : MonoBehaviour
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

        yield return null;
    }
}
