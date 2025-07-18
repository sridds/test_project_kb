using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    private struct Waypoint
    {
        public EDirectionFacing direction;
        public Vector2 position;
    }

    [SerializeField]
    private float _followerDistance = 1.0f; // Distance in time (seconds)

    [SerializeField]
    private float _lerpCorrection = 40.0f;

    public UnitMovement leader;
    public List<Follower> followerList = new List<Follower>();

    private List<Waypoint> positionHistory = new List<Waypoint>();
    private List<float> timeHistory = new List<float>();
    private Waypoint lastLeaderPosition;
    private Waypoint previousFrameLeaderPosition;
    private float positionThreshold = 0.001f; // Minimum movement to record
    private float timer;

    private void Start()
    {
        lastLeaderPosition = new Waypoint() { position = leader.transform.position, direction = leader.Direction };
        previousFrameLeaderPosition = lastLeaderPosition;

        // Initialize with current position
        positionHistory.Add(lastLeaderPosition);
        timeHistory.Add(timer);
    }

    private void Update()
    {
        // Update previous frame position before recording new position
        previousFrameLeaderPosition = lastLeaderPosition;
        RecordLeaderPosition();
    }

    private void LateUpdate()
    {
        UpdateFollowerPositions();
    }

    private void RecordLeaderPosition()
    {
        Waypoint currentWaypoint = new Waypoint() { position = leader.transform.position, direction = leader.Direction };

        // Only record if the leader has moved significantly
        if (Vector2.Distance(currentWaypoint.position, lastLeaderPosition.position) > positionThreshold)
        {
            positionHistory.Insert(0, currentWaypoint);
            timeHistory.Insert(0, timer);

            lastLeaderPosition = currentWaypoint;

            // Clean up old positions (keep about 10 seconds of history)
            while (positionHistory.Count > 0 && timeHistory[0] - timeHistory[positionHistory.Count - 1] > 10f)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
                timeHistory.RemoveAt(timeHistory.Count - 1);
            }
        }
    }

    private void UpdateFollowerPositions()
    {
        if (positionHistory.Count < 2) return;

        // Check if leader is currently moving by comparing with previous frame
        Waypoint currentWaypoint = new Waypoint() { position = leader.transform.position, direction = leader.Direction };
        bool leaderIsMoving = Vector2.Distance(currentWaypoint.position, previousFrameLeaderPosition.position) > positionThreshold;

        // If leader is NOT moving, don't update followers - they should pause
        if (!leaderIsMoving) return;

        timer += Time.deltaTime;

        // Only move followers when leader is actively moving
        float currentTime = timer;

        for (int i = 0; i < followerList.Count; i++)
        {
            // Calculate target time for this follower
            float targetTime = currentTime - ((i + 1) * _followerDistance);

            // Find the position at that time
            Waypoint targetPosition = GetPositionAtTime(targetTime);

            // Smooth the movement using lerp to reduce micro-jitter
            Vector3 currentFollowerPos = followerList[i].transform.position;
            Vector2 currentFollower2D = new Vector2(currentFollowerPos.x, currentFollowerPos.y);

            // Only move if the distance is significant enough
            if (Vector2.Distance(currentFollower2D, targetPosition.position) > 0.001f)
            {
                // Use a small amount of smoothing to eliminate micro-jitter
                Vector2 smoothedPosition = Vector2.Lerp(currentFollower2D, targetPosition.position, Time.deltaTime * _lerpCorrection);
                followerList[i].transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, currentFollowerPos.z);
                followerList[i].UpdateDirection(targetPosition.direction);
            }
        }
    }

    private Waypoint GetPositionAtTime(float targetTime)
    {
        // If target time is newer than our newest record, return the newest position
        if (targetTime >= timeHistory[0])
        {
            return positionHistory[0];
        }

        // If target time is older than our oldest record, return the oldest position
        if (targetTime <= timeHistory[timeHistory.Count - 1])
        {
            return positionHistory[positionHistory.Count - 1];
        }

        // Find the two positions to interpolate between
        for (int i = 0; i < timeHistory.Count - 1; i++)
        {
            if (targetTime <= timeHistory[i] && targetTime >= timeHistory[i + 1])
            {
                // Interpolate between these two positions
                float timeDiff = timeHistory[i] - timeHistory[i + 1];
                if (timeDiff == 0) return positionHistory[i];

                float t = (timeHistory[i] - targetTime) / timeDiff;
                return new Waypoint() { position = Vector2.Lerp(positionHistory[i].position, positionHistory[i + 1].position, t), direction = positionHistory[i].direction };
            }
        }

        // Fallback to the last position
        return positionHistory[positionHistory.Count - 1];
    }

    // Helper methods for adding/removing followers
    public void AddFollower(Follower newFollower)
    {
        followerList.Add(newFollower);
    }

    public void RemoveFollower(Follower follower)
    {
        followerList.Remove(follower);
    }
}