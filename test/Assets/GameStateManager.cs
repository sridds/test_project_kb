using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public enum GameState
    {
        RegularState, // The state entered at the very beginning of the game
        CorrectState, // The state entered when the player gets an answer correct
        SafeState, // The state entered when no enemies can spawn and the player is selecting something
        QuestionPromptState, // The state entered when the player is prompted a question
        IncorrectState, // The state entered when the player answers an incorrect question
        ExitableState,
        LoseState
    }

    // Game state stuff
    private GameState lastGameState;
    public GameState gameState = GameState.CorrectState;

    // Private fields
    private int score;

    // public values
    public int scoreWinThreshold = 10000;

    // Events
    public delegate void GameStateChanged(GameState newState);
    public GameStateChanged OnGameStateChanged;

    public delegate void PointsAdded(int newPointTotal);
    public PointsAdded OnPointsAdded;

    public QuestionPrompt.Question lastQuestion;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Updates the current game state and calls out an event to any script that is listening to it.
    /// </summary>
    /// <param name="gameState"></param>
    public void UpdateState(GameState gameState)
    {
        // The reason why we do this is because if we are already in the exit state we don't want anything to change that
        if(this.gameState == GameState.ExitableState)
        {
            return;
        }

        OnGameStateChanged?.Invoke(gameState);

        this.gameState = gameState;
    }

    private void Update()
    {
        // check if the score has exceeded the win threshold and we aren't already in the exitable state
        if (score >= scoreWinThreshold && gameState != GameState.ExitableState)
        {
            // Update the state to exitable now that we know we've won
            UpdateState(GameState.ExitableState);
        }
    }

    /// <summary>
    /// Calls the score to be increased
    /// </summary>
    /// <param name="points"></param>
    public void IncreaseScore(int points)
    {
        score += points;

        // The ? operator is a shorthand way of checking if the OnPointsAdded delegate has any subscribers. It does not fire the event if there is noone subscribed
        OnPointsAdded?.Invoke(score);
    }
}
