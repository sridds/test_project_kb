using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public interface IRoomBehavior
{
    void OnRoomValidate(); // Before the room is entered, but the transition has started
    void OnRoomEnter(); // Called as soon as the room is entered
    void OnRoomExit(); // Called as soon as the room is exited
}

[System.Serializable]
public class RoomConnection
{
    public RoomTrigger From;
    public RoomTrigger To;
}

public class Room : MonoBehaviour
{
    public enum ERoomMusicType
    {
        UseDefault,
        ExclusiveTrack,
        None
    }

    #region Inspector Fields
    [Header("Modifiers")]
    [SerializeField]
    private RoomConnection[] _connections;
    [SerializeField]
    private BoxCollider2D _collider;

    [Header("Music")]
    [SerializeField]
    private ERoomMusicType _musicPlayType;
    [SerializeField, ShowIf(nameof(_musicPlayType), ERoomMusicType.ExclusiveTrack)]
    private MusicStream _roomExclusiveMusic;

    [Header("Sound")]
    [SerializeField]
    private bool _hasEnteranceSound;
    [SerializeField]
    private bool _hasExitSound;
    [SerializeField, ShowIf(nameof(_hasExitSound))]
    private AudioStream _roomEnterStream;
    [SerializeField, ShowIf(nameof(_hasEnteranceSound))]
    private AudioStream _roomExitStream;
    #endregion

    #region Accessors
    public Bounds RoomBounds { get { return _collider.bounds; } }
    public ERoomMusicType MusicPlayType { get { return _musicPlayType; } }
    #endregion

    private List<IRoomBehavior> roomBehaviors = new List<IRoomBehavior>();
    private RoomManager roomManager;
    private CameraController camera;

    private void Awake()
    {
        // Cache component
        if (_collider == null) _collider = GetComponent<BoxCollider2D>();
        if (roomManager == null) roomManager = FindObjectOfType<RoomManager>();
        if (camera == null) camera = FindObjectOfType<CameraController>();

        InitalizeRoomBehaviors();
    }

    private void InitalizeRoomBehaviors()
    {
        var behaviors = GetComponentsInChildren<IRoomBehavior>();
        roomBehaviors.AddRange(behaviors);
    }

    public void HandleRoomTransition(RoomTrigger myTrigger)
    {
        RoomConnection connection = _connections.FirstOrDefault(x => x.From == myTrigger);

        // failed to find matching room connection
        if (connection == null)
        {
            Debug.Log($"No room connection found! [{myTrigger.name}]");
            return;
        }

        roomManager.HandleTransition(connection);
    }

    public void ExitRoom()
    {
        Debug.Log($"Exited room: [{gameObject.name}]");

        HandleAudioOnExit();
    }

    public void EnterRoom()
    {
        Debug.Log($"Entered room: [{gameObject.name}]");

        HandleAudioOnEnter();
        UpdateCameraBounds();
        UpdateMusic();
    }

    private void UpdateCameraBounds()
    {
        camera.SetBounds(RoomBounds);
    }

    private void HandleAudioOnExit()
    {
        if(_hasExitSound && _roomExitStream != null)
        {
            AudioManager.Instance.PlaySound(_roomExitStream);
        }
    }

    private void HandleAudioOnEnter()
    {
        if (_hasEnteranceSound && _roomEnterStream != null)
        {
            AudioManager.Instance.PlaySound(_roomEnterStream);
        }
    }

    private void UpdateMusic()
    {
        if (_musicPlayType == ERoomMusicType.ExclusiveTrack) AudioManager.Instance.PlayTrack(_roomExclusiveMusic);
        else if (_musicPlayType == ERoomMusicType.UseDefault && AudioManager.Instance.CurrentMusicTrack != AudioManager.Instance.DefaultAreaMusic) AudioManager.Instance.PlayTrack(AudioManager.Instance.DefaultAreaMusic);
        else if (_musicPlayType == ERoomMusicType.None) AudioManager.Instance.PauseMusic();
    }

    private void OnDrawGizmosSelected()
    {
        if (_collider != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }

        // Draw connection gizmos
        if (_connections != null)
        {
            Gizmos.color = Color.blue;
            foreach (var connection in _connections)
            {
                if (connection.From != null && connection.To != null)
                {
                    Gizmos.DrawLine(connection.From.transform.position, connection.To.transform.position);
                }
            }
        }
    }
}
