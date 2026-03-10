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
    public float stuckTime = 2f;
    public float reverseTime = 1.5f;

    private Transform[] waypoints;
    private int currentWaypoint = 0;
    private Car car;
    private Rigidbody rb;

    private float stuckTimer = 0f;
    private float reverseTimer = 0f;
    private bool isReversing = false;
    private Vector3 lastPosition;

    void Start()
    {
        car = GetComponent<Car>();
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;

        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypoints.Length; i++)
            waypoints[i] = waypointParent.GetChild(i);
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
        if (Vector3.Distance(transform.position, lastPosition) < 0.3f)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        lastPosition = transform.position;

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
}