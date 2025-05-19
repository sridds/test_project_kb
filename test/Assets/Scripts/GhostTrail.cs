using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private GhostTrailObject _ghostTrailObjectPrefab;

    [SerializeField]
    private float _spawnInterval = 0.2f;

    private bool ghostTrailEnabled = false;
    private float timer;
    private Vector3 lastPos;

    public void SetGhostTrailActive(bool active) => ghostTrailEnabled = active;

    void Update()
    {
        timer += Time.deltaTime;

        if (lastPos != transform.position && timer > _spawnInterval)
        {
            // Create new trail
            GhostTrailObject trail = Instantiate(_ghostTrailObjectPrefab, _renderer.transform.position, Quaternion.identity);
            trail.gameObject.name = gameObject.name + "(Trail)";
            trail.spriteRenderer.sprite = _renderer.sprite;
            trail.spriteRenderer.color = _renderer.color;

            // always behind
            trail.spriteRenderer.sortingLayerID = _renderer.sortingLayerID;
            trail.spriteRenderer.sortingOrder = _renderer.sortingOrder - 1;

            timer = 0.0f;
        }


        lastPos = transform.position;
    }
}
