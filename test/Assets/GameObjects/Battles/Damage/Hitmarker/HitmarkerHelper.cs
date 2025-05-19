using DG.Tweening;
using UnityEngine;

public class HitmarkerHelper : MonoBehaviour
{
    [SerializeField]
    private NumberSprite _myNumberSprite;

    [SerializeField]
    private Transform _holder;

    private void Start()
    {
        _holder.DOShakePosition(0.3f, 0.2f, 20).SetEase(Ease.OutQuad);
    }

    private void Update()
    {
        float val = (Mathf.Sin((Time.time * 16) + _myNumberSprite.MyIndex) / 20.0f) + 1.0f;

        _holder.localScale = new Vector3(val, val, 1);
    }
}
