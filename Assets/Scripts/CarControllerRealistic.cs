using UnityEngine;

public class CarControllerWORKING : MonoBehaviour
{
    public float acceleration = 2000f;
    public float maxSpeed = 35f;
    public float turnSpeed = 120f;

    public float grip = 0.9f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // INPUT
        float throttle = 0f;
        if (Input.GetKey(KeyCode.W)) throttle = 1f;
        if (Input.GetKey(KeyCode.S)) throttle = -1f;

        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // SPEED
        float speed = rb.linearVelocity.magnitude;

        // ENGINE
        if (speed < maxSpeed)
        {
            rb.AddForce(transform.forward * throttle * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        // TURNING (only if moving)
        if (speed > 0.5f)
        {
            float turn = steer * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }

        // GRIP
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= grip;
        rb.linearVelocity = transform.TransformDirection(localVel);
    }
}
