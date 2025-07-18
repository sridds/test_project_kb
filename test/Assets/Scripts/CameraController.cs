using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    private void LateUpdate()
    {
        Vector3 target = GameObject.FindWithTag("Leader").transform.position;

        transform.position = target + new Vector3(0, 0, -10);
    }

    public void PanCamera(Vector3 position, float duration, Ease easing)
    {
        transform.DOMove(position, duration).SetEase(easing);
    }

    public void PanCameraLocal(Vector3 amount, float duration, Ease easing)
    {
        transform.DOMove(transform.position + amount, duration).SetEase(easing);
    }
}
