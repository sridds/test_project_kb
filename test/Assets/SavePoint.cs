using UnityEngine;

public class SavePoint : Interactable
{
    public DialogueData[] flavorText;

    private DialogueHandler handler;

    protected override void HandleInteraction()
    {
        if (handler == null) handler = FindFirstObjectByType<DialogueHandler>();

        GameManager.Instance.ChangeGameState(GameManager.EGameState.Cutscene);
        handler.HandleDialogue(flavorText);

        handler.OnQueueEmpty += FinishInteraction;
    }

    private void FinishInteraction()
    {
        GameManager.Instance.ActivateSavePoint();

        isInteracting = false;
        handler.OnQueueEmpty -= FinishInteraction;
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Playing);
    }
}
