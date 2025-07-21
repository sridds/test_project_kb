using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField]
    private EDirection _enterDirection;

    [SerializeField]
    private Room _myRoom;

    public Room MyRoom { get { return _myRoom; } }
    public EDirection EnterDirection { get { return _enterDirection; } }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Leader") return;

        _myRoom.HandleRoomTransition(this);
    }
}
