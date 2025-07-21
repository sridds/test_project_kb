using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _cameraFollowTarget;

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Vector2 _cameraBounds = new Vector2(320, 240);

    private Bounds roomBounds;


    private void LateUpdate()
    {
        Vector3 targetPos = new Vector3(_cameraFollowTarget.position.x, _cameraFollowTarget.position.y, -10);
        Vector2 camBounds = _cameraBounds / 16.0f;

        targetPos.x = Mathf.Clamp(targetPos.x, roomBounds.min.x + camBounds.x / 2.0f, roomBounds.max.x - camBounds.x / 2.0f);
        targetPos.y = Mathf.Clamp(targetPos.y, roomBounds.min.y + camBounds.y / 2.0f, roomBounds.max.y - camBounds.y / 2.0f);

        if(roomBounds.size.y < camBounds.y) targetPos.y = roomBounds.center.y;
        if(roomBounds.size.x < camBounds.x) targetPos.x = roomBounds.center.x;

        transform.position = targetPos;
    }

    public void SetBounds(Bounds bounds)
    {
        roomBounds = bounds;    
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
