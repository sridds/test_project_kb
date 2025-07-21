using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private float _roomFadeInTime = 0.3f;

    [SerializeField]
    private float _roomFadeOutTime = 0.3f;

    [SerializeField]
    private Room _defaultRoom;

    private void Start()
    {
        // get room from save data or resort to default
        _defaultRoom.EnterRoom();
    }

    public void HandleTransition(RoomConnection connection)
    {
        StartCoroutine(HandleRoomTransition(connection));
    }

    // Make this entire thing beter imo
    private IEnumerator HandleRoomTransition(RoomConnection connection)
    {
        connection.From.MyRoom.ExitRoom();

        // Fade into black
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Cutscene);
        FindFirstObjectByType<Fader>().FadeIn(_roomFadeInTime);
        if (connection.To.MyRoom.MusicPlayType == Room.ERoomMusicType.ExclusiveTrack || connection.To.MyRoom.MusicPlayType == Room.ERoomMusicType.None || (connection.To.MyRoom.MusicPlayType == Room.ERoomMusicType.UseDefault && AudioManager.Instance.CurrentMusicTrack != AudioManager.Instance.DefaultAreaMusic)) AudioManager.Instance.FadeOutMusic(_roomFadeInTime);

        yield return new WaitForSecondsRealtime(_roomFadeInTime);

        // Move party to appropriate location
        Transform leader = GameObject.FindWithTag("Leader").transform;
        Vector2 connectionPos = connection.To.transform.position;
        leader.transform.position = connectionPos + connection.To.EnterDirection.ToVector();
        FindFirstObjectByType<FollowerManager>().Clear();

        yield return new WaitForSecondsRealtime(0.1f);

        // Fade back out to the game, allow the party to move again
        connection.To.MyRoom.EnterRoom();
        FindFirstObjectByType<Fader>().FadeOut(_roomFadeOutTime);
        GameManager.Instance.ChangeGameState(GameManager.EGameState.Playing);
    }
}
