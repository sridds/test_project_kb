using DG.Tweening;
using UnityEngine;

public class DamageTotal : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _backdrop;

    [SerializeField]
    private SpriteNumbers _numbers;

    [SerializeField]
    private GameObject[] _totalLetters;

    [SerializeField]
    private Transform _numberHolder;

    public SpriteNumbers Numbers { get { return _numbers; } }

    private void Update()
    {
        // Add some sin to the TOTAL letters
        for(int i = 0; i < _totalLetters.Length; i++)
        {
            float val = Mathf.Sin((Time.time * 7.0f) + i) * 0.05f;
            _totalLetters[i].transform.localPosition = new Vector3(_totalLetters[i].transform.localPosition.x, val);
        }

        _backdrop.transform.localPosition = new Vector3(Mathf.Cos(Time.time * 3.0f) * 0.1f, Mathf.Sin(Time.time * 3.0f) * 0.05f);
    }
}
