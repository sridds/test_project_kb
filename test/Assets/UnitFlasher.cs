using UnityEngine;
using DG.Tweening;

public class UnitFlasher : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _unitRenderer;

    [SerializeField]
    private Material _flashMaterial;

    [SerializeField]
    private float _targetFlashSpeed = 1.0f;

    private Material defaultMaterial;
    private bool isFlashing;

    private float timer;

    private void Start()
    {
        defaultMaterial = _unitRenderer.material;
    }

    private void Update()
    {
        if (!isFlashing) return;

        timer += Time.deltaTime;
        _unitRenderer.material.SetFloat("_Multiplier", (Mathf.Cos(timer * _targetFlashSpeed) / 2f) + 0.5f);
    }

    public void EnableFlash()
    {
        isFlashing = true;

        timer = 0.0f;
        _unitRenderer.material = _flashMaterial;
        _unitRenderer.material.SetFloat("_Multiplier", 0.0f);
    }

    public void DisableFlashing()
    {
        isFlashing = false;

        _unitRenderer.material = defaultMaterial;
    }
}
