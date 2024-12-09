using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public enum GameState
    {
        CorrectState,
        QuestionPromptState,
        IncorrectState,
        ExitableState,
        LoseState
    }

    // Game state stuff
    private GameState lastGameState;
    public GameState gameState = GameState.CorrectState;

    private int score;
    private float questionTimer;

    // Public variables
    public float questionPromptThreshold = 2.0f;

    // Events
    public delegate void GameStateChanged(GameState newState);
    public GameStateChanged OnGameStateChanged;

    public delegate void PointsAdded(int newPointTotal);
    public PointsAdded OnPointsAdded;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateState(GameState gameState)
    {
        OnGameStateChanged?.Invoke(gameState);

        this.gameState = gameState;
    }

    private void Update()
    {
        if (gameState == GameState.CorrectState) HandleCorrectState();
        if (gameState == GameState.QuestionPromptState) HandleQuestionState();
        if (gameState == GameState.IncorrectState) HandleIncorrectState();
        if (gameState == GameState.ExitableState) HandleExitableState();
        if (gameState == GameState.LoseState) HandleLoseState();
    }

    private void HandleQuestionState()
    {
        questionTimer = 0.0f;
    }

    private void HandleIncorrectState()
    {
        lastGameState = gameState;

        questionTimer += Time.deltaTime;

        if (questionTimer > questionPromptThreshold)
        {
            UpdateState(GameState.QuestionPromptState);
            questionTimer = 0.0f;
        }
    }

    private void HandleCorrectState()
    {
        lastGameState = gameState;

        questionTimer += Time.deltaTime;

        if(questionTimer > questionPromptThreshold)
        {
            UpdateState(GameState.QuestionPromptState);
            questionTimer = 0.0f;
        }
    }

    private void HandleExitableState()
    {
        lastGameState = gameState;
    }

    private void HandleLoseState()
    {
        lastGameState = gameState;
    }

    public void IncreaseScore(int points)
    {
        score += points;

        // The ? operator is a shorthand way of checking if the OnPointsAdded delegate has any subscribers. It does not fire the event if there is noone subscribed
        OnPointsAdded?.Invoke(score);
    }
}
