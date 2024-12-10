using UnityEngine;

public class SafeZoneHandler : MonoBehaviour
{
    public GameObject holder;
    public QuestionPrompt promptReference;

    private void Start()
    {
        holder.SetActive(false);

        GameStateManager.Instance.OnGameStateChanged += GameStateUpdate;
    }

    private void GameStateUpdate(GameStateManager.GameState gameState)
    {
        // Only if we are in the safe state -- set the holder active
        if(gameState == GameStateManager.GameState.SafeState)
        {
            holder.SetActive(true);
        }
    }

    public void EasySelected()
    {
        // Tells the QuestionPrompt script to prompt a easy question to the player
        promptReference.PromptEasyQuestion();

        // Hide the holder
        holder.SetActive(false);

        // Enter the question prompt state
        GameStateManager.Instance.UpdateState(GameStateManager.GameState.QuestionPromptState);
    }

    public void MediumSelected()
    {
        // Tells the QuestionPrompt script to prompt a medium question to the player
        promptReference.PromptMediumQuestion();

        // Hide the holder
        holder.SetActive(false);

        // Enter the question prompt state
        GameStateManager.Instance.UpdateState(GameStateManager.GameState.QuestionPromptState);
    }

    public void HardSelected()
    {
        // Tells the QuestionPrompt script to prompt a hard question to the player
        promptReference.PromptHardQuestion();

        // Hide the holder
        holder.SetActive(false);

        // Enter the question prompt state
        GameStateManager.Instance.UpdateState(GameStateManager.GameState.QuestionPromptState);
    }
}
