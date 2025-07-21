using System.Collections.Generic;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    private struct Waypoint
    {
        public EDirection direction;
        public Vector2 position;
    }

    [Header("Modifiers")]
    [SerializeField]
    private float _followerDistance = 1.0f;

    [SerializeField]
    private float _maxHistory = 10.0f;

    [SerializeField]
    private float _lerpCorrection = 40.0f;

    public UnitMovement leader;
    public List<Follower> followerList = new List<Follower>();

    private List<Waypoint> positionHistory = new List<Waypoint>();
    private List<float> timeHistory = new List<float>();
    private Waypoint lastLeaderPosition;
    private Waypoint previousFrameLeaderPosition;
    private float positionThreshold = 0.001f;
    private float timer;

    private void Start()
    {
        // initialize values
        lastLeaderPosition = new Waypoint() { position = leader.transform.position, direction = leader.Direction };
        previousFrameLeaderPosition = lastLeaderPosition;

        positionHistory.Add(lastLeaderPosition);
        timeHistory.Add(timer);        // Initialize path with a trail from each follower's current position to the leader
        InitializePathFromFollowerPositions();
    }

    public void Clear()
    {
        positionHistory.Clear();
        timeHistory.Clear();

        for(int i = 0; i < followerList.Count; i++)
        {
            followerList[i].direction = leader.Direction;
            followerList[i].transform.position = leader.transform.position;
        }
    }

    private void InitializePathFromFollowerPositions()
    {
        Vector2 leaderPos = leader.transform.position;
        float currentTime = Time.time;

        positionHistory.Clear();
        timeHistory.Clear();

        // calculate the path length
        float totalTimeSpan = followerList.Count * _followerDistance;

        // create a path that moves backwards
        float timeStep = 0.05f;
        int totalPoints = Mathf.CeilToInt(totalTimeSpan / timeStep) + 10;

        for (int i = 0; i < totalPoints; i++)
        {
            float timeOffset = i * timeStep;
            float pastTime = currentTime - timeOffset;

            // start with leaders position
            Vector2 interpolatedPos = leaderPos; 

            // go backwards thrrough follower list
            for (int f = followerList.Count - 1; f >= 0; f--)
            {
                float followerStartTime = currentTime - ((f + 1) * _followerDistance);

                if (pastTime <= followerStartTime)
                {
                    Vector2 followerStartPos = followerList[f].transform.position;
                    float lerpProgress = (followerStartTime - pastTime) / _followerDistance;
                    lerpProgress = Mathf.Clamp01(lerpProgress);

                    interpolatedPos = Vector2.Lerp(followerStartPos, leaderPos, lerpProgress);
                    break; // use the furthest follower
                }
            }

            // add newest follower waypoint to history
            positionHistory.Add(new Waypoint { position = interpolatedPos, direction = leader.Direction });
            timeHistory.Add(pastTime);
        }

        // reverse the lists so newest is first
        positionHistory.Reverse();
        timeHistory.Reverse();
    }

    private void Update()
    {
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

        // ensure position meets position threshold before recording
        if (Vector2.Distance(currentWaypoint.position, lastLeaderPosition.position) > positionThreshold)
        {
            // add timestamp
            positionHistory.Insert(0, currentWaypoint);
            timeHistory.Insert(0, timer);

            lastLeaderPosition = currentWaypoint;

            // clean up anything past max history
            while (positionHistory.Count > 0 && timeHistory[0] - timeHistory[positionHistory.Count - 1] > _maxHistory)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
                timeHistory.RemoveAt(timeHistory.Count - 1);
            }
        }
    }

    private void UpdateFollowerPositions()
    {
        if (positionHistory.Count < 2) return;

        Waypoint currentWaypoint = new Waypoint() { position = leader.transform.position, direction = leader.Direction };

        // leader has to be moving
        bool leaderIsMoving = Vector2.Distance(currentWaypoint.position, previousFrameLeaderPosition.position) > positionThreshold;
        if (!leaderIsMoving) return;

        timer += Time.deltaTime;
        float currentTime = timer;

        // update each follower
        for (int i = 0; i < followerList.Count; i++)
        {
            // get current waypoint at timestamp
            float targetTime = currentTime - ((i + 1) * _followerDistance);
            Waypoint targetPosition = GetPositionAtTime(targetTime);
            Vector2 currentFollowerPos = followerList[i].transform.position;

            // only move if the distance is significant enough
            if (Vector2.Distance(currentFollowerPos, targetPosition.position) > 0.001f)
            {
                // smoothing
                Vector2 smoothedPosition = Vector2.Lerp(currentFollowerPos, targetPosition.position, Time.deltaTime * _lerpCorrection);
                followerList[i].transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y);
                followerList[i].direction = targetPosition.direction;
            }
        }
    }

    private Waypoint GetPositionAtTime(float targetTime)
    {
        // if target time is newer than our newest record
        if (targetTime >= timeHistory[0]) return positionHistory[0];

        // if target time is older than our oldest record
        if (targetTime <= timeHistory[timeHistory.Count - 1]) return positionHistory[positionHistory.Count - 1];

        // interpolate between two positions
        for (int i = 0; i < timeHistory.Count - 1; i++)
        {
            if (targetTime <= timeHistory[i] && targetTime >= timeHistory[i + 1])
            {
                float timeDiff = timeHistory[i] - timeHistory[i + 1];
                if (timeDiff == 0) return positionHistory[i];

                float t = (timeHistory[i] - targetTime) / timeDiff;
                return new Waypoint() { position = Vector2.Lerp(positionHistory[i].position, positionHistory[i + 1].position, t), direction = positionHistory[i].direction };
            }
        }

        // fallback to last position
        return positionHistory[positionHistory.Count - 1];
    }

    public void AddFollower(Follower newFollower)
    {
        followerList.Add(newFollower);
    }

    public void RemoveFollower(Follower follower)
    {
        followerList.Remove(follower);
    }
}