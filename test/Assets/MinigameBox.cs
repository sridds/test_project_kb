using UnityEngine;

public class MinigameBox : MonoBehaviour
{
    [SerializeField] private SpriteMask _boxRenderer;
    [SerializeField] private SpriteRenderer _boxBackground;
    [SerializeField] private SpriteRenderer _boxOutlineRenderer;
    [SerializeField] private Vector2 _boxOutlinePadding = new Vector2(0.5f, 0.5f);

    private void Update()
    {
        // always automatically adjust size of renderers
        _boxOutlineRenderer.size = new Vector2(_boxRenderer.bounds.size.x, _boxRenderer.bounds.size.y) + _boxOutlinePadding;
        _boxBackground.size = new Vector2(_boxRenderer.bounds.size.x, _boxRenderer.bounds.size.y);
    }
}
