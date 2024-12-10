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
        if(gameState == GameStateManager.GameState.QuestionPromptState)
        {
            holder.SetActive(true);
        }
    }

    public void EasySelected()
    {
        promptReference.PromptEasyQuestion();

        holder.SetActive(false);
    }

    public void MediumSelected()
    {
        promptReference.PromptMediumQuestion();

        holder.SetActive(false);
    }

    public void HardSelected()
    {
        promptReference.PromptHardQuestion();

        holder.SetActive(false);
    }
}
