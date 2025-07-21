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

    #region - Accessors -
    public EGameState GameState { get { return state; } }
    #endregion

    #region - Delegates -
    public delegate void GameStateUpdated(EGameState state);
    public GameStateUpdated OnGameStateUpdated;
    #endregion

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

    public void RoomChanged(Room newRoom)
    {
        
    }
}
