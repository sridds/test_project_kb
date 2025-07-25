using UnityEngine;
using UnityEngine.SceneManagement;

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
    private SaveData activeSaveData;

    #region - Accessors -
    public EGameState GameState { get { return state; } }
    public SaveData ActiveSaveData { get { return activeSaveData; } }
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

        // loading
        if(SaveManager.DoesSaveFileExist())
        {
            LoadGameData();
        }
        else
        {
            activeSaveData = new SaveData();
        }
    }

    public void LoadGameData()
    {
        activeSaveData = SaveManager.Load();

        UnitMovement movement = FindFirstObjectByType<UnitMovement>();
        movement.transform.position = new Vector3(activeSaveData.hankPositionX, activeSaveData.hankPositionY);
    }

    public void ActivateSavePoint()
    {
        UnitMovement movement = FindFirstObjectByType<UnitMovement>();

        // Record position of leader
        activeSaveData.hankPositionX = movement.transform.position.x;
        activeSaveData.hankPositionY = movement.transform.position.y;

        // Record the scene index and the room the player is currently in
        activeSaveData.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        activeSaveData.roomIndex = FindFirstObjectByType<RoomManager>().CurrentRoomIndex;

        // Record who is currently in the party, aswell as their positions in the world
        // Save flag list
        // Save value lists

        SaveManager.Save(activeSaveData);
    }

    public void ChangeGameState(EGameState state)
    {
        this.state = state;
        OnGameStateUpdated?.Invoke(state);
    }
}
