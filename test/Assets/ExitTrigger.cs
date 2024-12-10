using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public SpriteRenderer sprite;

    private bool canUse;

    private void Start()
    {
        GameStateManager.Instance.OnGameStateChanged += GameStateUpdated;

        sprite.enabled = false;
    }

    private void GameStateUpdated(GameStateManager.GameState gameState)
    {
        if(gameState == GameStateManager.GameState.ExitableState)
        {
            sprite.enabled = true;
            canUse = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && canUse)
        {
            // win!
            Debug.Log("Epic win!");

            // Do what needs to happen for winning here. New scene?
        }
    }
}
