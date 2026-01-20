using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public float acceleration = 6000f;
    public float maxSpeed = 28f;

    public float turnSpeed = 120f;
    public float grip = 0.88f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // stability
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // lower COM so no shake / flip
        rb.centerOfMass = new Vector3(0f, -0.6f, 0f);
    }

    void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        // input
        float throttle = 0f;
        if (Input.GetKey(KeyCode.W)) throttle = 1f;
        if (Input.GetKey(KeyCode.S)) throttle = -1f;

        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // gaming reverse steering
        if (throttle < 0f) steer *= -1f;

        // engine
        if (speed < maxSpeed)
            rb.AddForce(transform.forward * throttle * acceleration, ForceMode.Force);

        // steering only if moving
        if (speed > 0.5f)
        {
            float steerStrength = Mathf.Lerp(turnSpeed, 50f, speed / maxSpeed);
            float turn = steer * steerStrength * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }

        // grip
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= grip;
        rb.linearVelocity = transform.TransformDirection(localVel);
    }
}
