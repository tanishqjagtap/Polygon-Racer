using UnityEngine;

public class EnemyCarAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waypointParent;

    [Header("Driving")]
    public float waypointReachDistance = 10f;
    public float steeringSensitivity = 0.8f;
    public int lookAheadPoints = 2;

    [Header("Stuck Recovery")]
    public float stuckTime = 5f;
    public float reverseTime = 1f;

    private Transform[] waypoints;
    private int currentWaypoint = 0;
    private Car car;
    private Rigidbody rb;

    private float stuckTimer = 0f;
    private float reverseTimer = 0f;
    private bool isReversing = false;
    private Vector3 lastPosition;
    private float positionCheckTimer = 0f;  // check every 1 sec not every frame

    void Start()
    {
        car = GetComponent<Car>();
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;

        int count = waypointParent.childCount;
        waypoints = new Transform[count];

        for (int i = 0; i < count; i++)
            waypoints[i] = waypointParent.GetChild(count - 1 - i);
    }

    void Update()
    {
        if (RaceManager.Instance == null) return;
        if (!RaceManager.Instance.raceStarted) return;
        if (RaceManager.Instance.raceFinished) return;
        if (waypoints.Length == 0) return;

        StuckCheck();

        if (isReversing)
        {
            Reverse();
            return;
        }

        Drive();
    }

    void Drive()
    {
        int targetIndex = (currentWaypoint + lookAheadPoints) % waypoints.Length;
        Vector3 localTarget = transform.InverseTransformPoint(waypoints[targetIndex].position);

        float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude * steeringSensitivity, -1f, 1f);
        car.steerInput = steer;

        float absSteer = Mathf.Abs(steer);
        float throttle = absSteer > 0.3f
            ? Mathf.Lerp(1f, 0.4f, absSteer)
            : 1f;

        car.throttleInput = throttle;
        car.brakeInput = 0f;

        float dist = Vector3.Distance(transform.position, waypoints[currentWaypoint].position);
        if (dist < waypointReachDistance)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void StuckCheck()
    {
        // Check position every 1 second instead of every frame
        // prevents slow corners from falsely triggering reverse
        positionCheckTimer += Time.deltaTime;

        if (positionCheckTimer >= 1f)
        {
            float moved = Vector3.Distance(transform.position, lastPosition);

            if (moved < 1f)  // moved less than 1 unit in 1 second = stuck
                stuckTimer += 1f;
            else
                stuckTimer = 0f;

            lastPosition = transform.position;
            positionCheckTimer = 0f;
        }

        if (stuckTimer >= stuckTime)
        {
            isReversing = true;
            reverseTimer = reverseTime;
            stuckTimer = 0f;
        }
    }

    void Reverse()
    {
        reverseTimer -= Time.deltaTime;
        car.throttleInput = -1f;
        car.brakeInput = 0f;
        car.steerInput = -car.steerInput;

        if (reverseTimer <= 0f)
            isReversing = false;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 1f);
            Gizmos.DrawLine(waypoints[i].position,
                waypoints[(i + 1) % waypoints.Length].position);
        }
        Gizmos.color = Color.green;
        if (currentWaypoint < waypoints.Length)
            Gizmos.DrawSphere(waypoints[currentWaypoint].position, 1.5f);
    }
}