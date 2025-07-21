using UnityEngine;

public class SavePoint : Interactable
{
    public DialogueData[] flavorText;

    private DialogueHandler handler;

    protected override void HandleInteraction()
    {
        // save the game
    }
}
