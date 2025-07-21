using UnityEngine;

public static class DirectionExtensions
{
    public static Vector2 ToVector(this EDirection inDir)
    {
        switch (inDir)
        {
            case EDirection.Left:
                return Vector2.left;
            case EDirection.Right:
                return Vector2.right;
            case EDirection.Up:
                return Vector2.up;
            case EDirection.Down:
                return Vector2.down;
            default:
                return Vector2.zero;
        }
    }
}
