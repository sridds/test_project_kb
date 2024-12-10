using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptionInteractor : MonoBehaviour
{
    private Option currentHoveredOption;
    private SafeZoneOption currentHoveredSafeOption;

    public void Update()
    {
        // We have to be in the correct state, first!
        if (GameStateManager.Instance.gameState == GameStateManager.GameState.QuestionPromptState)
        {
            // Ensure that the option the player is currently hovering over exists
            if (Input.GetKeyDown(KeyCode.Z) && currentHoveredOption != null)
            {
                currentHoveredOption.SelectOption();
            }
        }

        // Must be in the safe state to make any selection
        else if(GameStateManager.Instance.gameState == GameStateManager.GameState.SafeState)
        {
            if (Input.GetKeyDown(KeyCode.Z) && currentHoveredSafeOption != null)
            {
                currentHoveredSafeOption.Select();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // this is a guard clause that ensures the following code is not executed if this condition is not met
        if (collision.tag == "Option")
        {
            currentHoveredOption = collision.gameObject.GetComponent<Option>();
            currentHoveredOption.Hover();
        }

        else if (collision.GetComponent<SafeZoneOption>())
        {
            currentHoveredSafeOption = collision.gameObject.GetComponent<SafeZoneOption>();
            currentHoveredSafeOption.Hover();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // this is a guard clause that ensures the following code is not executed if this condition is not met
        if (collision.tag == "Option" && currentHoveredOption != null)
        {
            currentHoveredOption.Unhover();
            currentHoveredOption = null;
        }

        else if (collision.GetComponent<SafeZoneOption>() && currentHoveredSafeOption != null)
        {
            currentHoveredSafeOption.Unhover();
            currentHoveredSafeOption = null;
        }
    }
}
