using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptionInteractor : MonoBehaviour
{
    private Option currentHoveredOption;

    public void Update()
    {
        // We have to be in the correct state, first!
        if(GameStateManager.Instance.gameState == GameStateManager.GameState.QuestionPromptState)
        {
            // Ensure that the option the player is currently hovering over exists
            if (Input.GetKeyDown(KeyCode.Z) && currentHoveredOption != null)
            {
                currentHoveredOption.SelectOption();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this is a guard clause that ensures the following code is not executed if this condition is not met
        if (collision.tag != "Option") return;

        currentHoveredOption = collision.gameObject.GetComponent<Option>();
        currentHoveredOption.Hover();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // this is a guard clause that ensures the following code is not executed if this condition is not met
        if (collision.tag != "Option") return;

        currentHoveredOption.Unhover();
        currentHoveredOption = null;
    }
}
