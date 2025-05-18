using UnityEngine;
using TMPro;
using DG.Tweening;

public class PerformanceHitmarker : MonoBehaviour
{
    [SerializeField]
    private float _lifetime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, _lifetime);
    }
}
