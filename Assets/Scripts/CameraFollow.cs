using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = new Vector3(0f, 3f, -4.3f);

    public float positionSmoothTime = 0.08f;   // lower = tighter camera
    public float rotationSmooth = 8f;

    public float fov = 66f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) cam.fieldOfView = fov;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Desired camera position
        Vector3 desiredPos = target.position + target.rotation * offset;

        // ✅ SmoothDamp keeps distance stable (no zoom-out feeling)
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            positionSmoothTime
        );

        // Rotation
        Quaternion desiredRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, rotationSmooth * Time.deltaTime);
    }
}
