using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    public void PanCamera(Vector3 position, float duration, Ease easing)
    {
        transform.DOMove(position, duration).SetEase(easing);
    }

    public void PanCameraLocal(Vector3 amount, float duration, Ease easing)
    {
        transform.DOMove(transform.position + amount, duration).SetEase(easing);
    }
}
