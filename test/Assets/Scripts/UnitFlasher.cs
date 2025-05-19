using UnityEngine;
using DG.Tweening;
using System.Collections;

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

        // this makes it so it always starts at 0
        float frequency = (3 * Mathf.PI) / (_targetFlashSpeed * 2.0f);

        _unitRenderer.material.SetFloat("_Multiplier", (Mathf.Sin((timer + frequency) * _targetFlashSpeed) / 2.0f) + 0.5f);
    }

    public void DamageFlash(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(IFlash(duration));
    }

    private IEnumerator IFlash(float duration)
    {
        _unitRenderer.material.SetFloat("_Multiplier", 1.0f);
        yield return new WaitForSeconds(duration);
        _unitRenderer.material.SetFloat("_Multiplier", 0.0f);
    }

    public void EnableFlash()
    {
        isFlashing = true;

        timer = 0.0f;
        _unitRenderer.material = _flashMaterial;
        _unitRenderer.material.SetFloat("_Multiplier", timer);
    }

    public void DisableFlashing()
    {
        isFlashing = false;

        _unitRenderer.material = defaultMaterial;
    }
}
