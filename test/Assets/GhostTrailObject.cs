using UnityEngine;
using DG.Tweening;

public class GhostTrailObject : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime = 0.5f;

    public SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Fade out
        spriteRenderer.DOFade(0.0f, _lifeTime).OnComplete(() => Destroy(gameObject));
    }
}
