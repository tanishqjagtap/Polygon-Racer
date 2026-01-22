using UnityEngine;

public class CarCameraFollowNFS : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // CarRoot

    [Header("Offsets")]
    public float height = 3f;
    public float distance = 4.3f;

    [Header("Follow")]
    public float positionSmooth = 0.10f;

    [Header("Rotation Lag (important)")]
    public float yawFollowSpeed = 3.5f;     // lower = more lag (good)
    public float minSpeedToFollowYaw = 2f;  // don't rotate camera yaw when car barely moving

    [Header("Look")]
    public Vector3 lookOffset = new Vector3(0f, 1.2f, 0.6f);

    [Header("FOV")]
    public float fov = 66f;

    private Vector3 posVel;
    private Camera cam;

    // our own camera yaw that follows car slowly
    private float camYaw;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) cam.fieldOfView = fov;

        if (target != null)
            camYaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // detect car speed (using rigidbody if present)
        float speed = 0f;
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null) speed = rb.linearVelocity.magnitude;

        // car yaw (Y rotation)
        float targetYaw = target.eulerAngles.y;

        // ✅ only follow yaw when moving (prevents micro jitter)
        if (speed > minSpeedToFollowYaw)
        {
            camYaw = Mathf.LerpAngle(camYaw, targetYaw, yawFollowSpeed * Time.deltaTime);
        }

        // ✅ build camera rotation using our smoothed yaw
        Quaternion camRot = Quaternion.Euler(0f, camYaw, 0f);

        // ✅ behind the car using smoothed yaw (NOT instant car yaw)
        Vector3 desiredPos = target.position + camRot * new Vector3(0f, height, -distance);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref posVel, positionSmooth);

        // smooth look at
        Vector3 lookPoint = target.position + lookOffset;
        Quaternion lookRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 8f * Time.deltaTime);
    }
}
