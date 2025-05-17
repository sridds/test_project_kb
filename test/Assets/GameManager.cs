using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum EGameState
    {
        Playing,    // can enter cutscene, can enter battle, can pause
        Cutscene,   // cannot pause
        Paused,     // cannot enter battle or cutscene or play
        Battle,     // cannot pause or cutscene
    }

    private EGameState state;
    public EGameState GameState { get { return state; } }
    
    public delegate void GameStateUpdated(EGameState state);
    public GameStateUpdated OnGameStateUpdated;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeGameState(EGameState state)
    {
        this.state = state;
        OnGameStateUpdated?.Invoke(state);
    }
}
