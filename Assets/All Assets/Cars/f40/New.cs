using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float speed = 30f;
    public float reverseSpeed = 15f;
    public float turnSpeed = 40f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");   // W/S
        float h = Input.GetAxis("Horizontal"); // A/D

        Vector3 forwardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        // movement
        if (v > 0f)
            rb.AddForce(forwardFlat * v * speed, ForceMode.Acceleration);
        else if (v < 0f)
            rb.AddForce(-forwardFlat * (-v) * reverseSpeed, ForceMode.Acceleration);

        // ✅ steering only while moving
        if (Mathf.Abs(v) > 0.1f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, h * turnSpeed * Time.fixedDeltaTime, 0));
        }
    }
}
