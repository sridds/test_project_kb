using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float _readyTime = 1.0f;
    [SerializeField] private float _minigameTimer = 5.0f;

    public delegate void GameFinished(bool playerWon);
    public GameFinished OnGameFinished;

    protected float timer;
    protected bool isPlaying; 
    protected bool isWon; // determines whether the player successfully completed the minigame or not

    protected Unit target;
    protected Unit attacker;

    public void UpdateMinigame()
    {
        // Tick timer
        timer += Time.deltaTime;

        if(timer > _minigameTimer)
        {
            EndMinigame();
            return;
        }

        OnUpdateMinigame();
    }

    public void StartMinigame(Unit target, Unit attacker)
    {
        this.target = target;
        this.attacker = attacker;

        isPlaying = true;
        timer = 0.0f;
    }

    protected virtual void OnUpdateMinigame()
    {

    }

    protected virtual void EndMinigame()
    {
        OnGameFinished?.Invoke(isWon);
    }
}