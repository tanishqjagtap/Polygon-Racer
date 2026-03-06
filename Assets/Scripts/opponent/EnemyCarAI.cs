using UnityEngine;

public class EnemyCarAI : MonoBehaviour
{
    public Transform waypointParent;
    public float waypointReachDistance = 8f;
    public float throttle = 1f;

    private Transform[] waypoints;
    private int currentWaypoint = 0;

    private Car car;

    void Start()
    {
        car = GetComponent<Car>();

        int count = waypointParent.childCount;
        waypoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (!RaceManager.Instance.raceStarted) return;

        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        Vector3 localTarget = transform.InverseTransformPoint(target.position);

        float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f);

        car.steerInput = steer;
        car.throttleInput = throttle;
        car.brakeInput = 0f;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < waypointReachDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
                currentWaypoint = 0;
        }
    }
}
