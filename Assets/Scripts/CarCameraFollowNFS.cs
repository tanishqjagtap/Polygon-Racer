using UnityEngine;

public class CarCameraFollowNFS : MonoBehaviour
{
    public Transform target;

    public float height = 2.2f;
    public float distance = 6.5f;
    public float sideOffset = 0f;   // ← NEW (controls left/right centering)

    public float positionSmooth = 0.10f;

    public float yawFollowSpeed = 2.5f;
    public float minSpeedToFollowYaw = 0f;

    public Vector3 lookOffset = new Vector3(0f, 1.0f, 0.5f);

    public float fov = 66f;

    private Vector3 posVel;
    private Camera cam;
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

        float targetYaw = target.eulerAngles.y;
        camYaw = Mathf.LerpAngle(camYaw, targetYaw, yawFollowSpeed * Time.deltaTime);

        Quaternion camRot = Quaternion.Euler(0f, camYaw, 0f);

        // 👇 SIDE OFFSET ADDED HERE
        Vector3 desiredPos = target.position + camRot * new Vector3(sideOffset, height, -distance);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref posVel, positionSmooth);

        Vector3 lookPoint = target.position + lookOffset;
        Quaternion lookRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 8f * Time.deltaTime);
    }
}
