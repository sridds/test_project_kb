using UnityEngine;

public class Follower : MonoBehaviour
{
    private EDirectionFacing direction;

    public void UpdateDirection(EDirectionFacing dir)
    {
        direction = dir;
    }
}
