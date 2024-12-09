using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameStateManager.Instance.gameState == GameStateManager.GameState.LoseState) return;

        // Get the axis for movement
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Ensure movement is normalized. This is done because diagonal movement could be faster, so we normalize it to constrain the magnitude of the vector
        Vector2 movement = new Vector2(x, y);
        movement = movement.normalized * playerSpeed;

        // velocity is set to movement
        rb.velocity = movement;
    }
}
