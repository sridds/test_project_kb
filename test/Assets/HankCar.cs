using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HankCar : MonoBehaviour
{
    [SerializeField] private Transform _carHolder;
    [SerializeField] private ParticleSystem _burst;

    public IEnumerator INormalDrive(Vector2 point)
    {
        Vector2 pos = transform.position;
        Vector2 backDir = (point - pos).normalized * 0.8f;
        Vector2 backPoint = pos - backDir;
        Vector2 forwardDir = (point - pos).normalized * 25.0f;
        Vector2 forwardPoint = pos + forwardDir;

        yield return transform.DOLocalJump(transform.localPosition, 0.5f, 1, 0.1f).SetEase(Ease.OutQuad).WaitForCompletion();
        _carHolder.DORotate(new Vector3(0, 0, 8.0f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad);
        yield return transform.DOMove(backPoint, 0.2f).SetEase(Ease.OutQuad).WaitForCompletion();
        transform.DOMove(forwardPoint, 0.4f).SetEase(Ease.InQuad);
        _carHolder.DOScale(new Vector3(1.5f, 0.5f), 0.4f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.1f);
        _burst.Play();
        _carHolder.DORotate(new Vector3(0, 0, -8.0f), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuad);
    }
}