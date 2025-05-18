using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected bool isInteracting;
    public bool IsInteracting { get { return isInteracting; } }

    public void Interact()
    {
        if (isInteracting) return;

        isInteracting = true;

        HandleInteraction();
    }

    protected virtual void HandleInteraction() { }
}

public class SimpleInteractable : Interactable
{
    public DialogueData[] Data;

    private DialogueHandler handler;

    protected override void HandleInteraction()
    {
        if(handler == null) handler = FindFirstObjectByType<DialogueHandler>();

        GameManager.Instance.ChangeGameState(GameManager.EGameState.Cutscene);
        handler.HandleDialogue(Data);

        handler.OnQueueEmpty += FinishInteraction;
    }

    private void FinishInteraction()
    {
        isInteracting = false;
        handler.OnQueueEmpty -= FinishInteraction;
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Playing);
    }
}
