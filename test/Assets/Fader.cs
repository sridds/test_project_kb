using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    public void FadeIn(float time)
    {
        _image.DOKill(false);
        _image.DOFade(1.0f, time).SetEase(Ease.Linear);
    }

    public void FadeOut(float time)
    {
        _image.DOKill(false);
        _image.DOFade(0.0f, time).SetEase(Ease.Linear);
    }
}
