using UnityEngine;

public class AICarController : MonoBehaviour
{
    public Transform waypointParent;
    public float waypointReachDistance = 8f;

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

        car.canDrive = true;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        Vector3 localTarget = transform.InverseTransformPoint(target.position);

        float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f);

        car.steerInput = steer;
        car.throttleInput = 1f;
        car.brakeInput = 0f;

        if (Vector3.Distance(transform.position, target.position) < waypointReachDistance)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }
}